using EasyUI.PickerWheelUI;
using ilasm.WordBomb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Request;
using WordBombServer.Common.Packets.Response;
using DG.Tweening;


public class RouletteBehaviour : MonoBehaviour
{
    public Button StartRouletteButton;

    public Button BackButton;

    public List<WheelOfFortuneBoxScriptable> Boxes = new List<WheelOfFortuneBoxScriptable>();

    public InstantiateTemplate<WheelOfFortuneBoxView> BoxViewTemplate;

    public TMP_Text PriceText;


    private short selectedId = 0;
    public int Price;

    public Transform BoxContent;
    public Image BoxContentTemplate;

    private List<Image> _createdElements = new List<Image>();

    public void OnBackClicked()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnUnlockedAvatar += OnUnlockedAvatar;
    }



    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnUnlockedAvatar -= OnUnlockedAvatar;
    }

    private void Start()
    {
        StartRouletteButton.gameObject.SetActive(false);
        for (int i = 0; i < Boxes.Count; i++)
        {
            var view = BoxViewTemplate.Instantiate();
            view.Selection.gameObject.SetActive(false);
            view.Text.text = Boxes[i].Price + "";
            view.Image.sprite = Boxes[i].Icon;
            view.transform.localScale = Vector3.zero;
            view.transform.DOScale(0.95f, 0.3f).SetDelay(i*0.1f);
            view.CanvasGroup.alpha = 0.4f;
            var index = i;
            view.Button.onClick.AddListener(() =>
            {
                SelectBox(index, view);
            });

            if (i == 0)
            {
                SelectBox(i, view);
            }
        }
    }

    private WheelOfFortuneBoxView selected;
    private void SelectBox(int index, WheelOfFortuneBoxView view)
    {
        if (selected != null)
        {
            selected.transform.DOScale(0.95f, 0.3f);
            selected.Selection.gameObject.SetActive(false);
            selected.CanvasGroup.DOFade(0.4f,0.3f);
        }

        selected = view;
        selected.transform.DOScale(1f, 0.3f);
        selected.CanvasGroup.DOFade(1f,0.3f);
        selected.Selection.gameObject.SetActive(true);
        selectedId = Boxes[index].Id;
        Price = Boxes[index].Price;
        PriceText.text = Price.ToString();



        for (int i = 0; i < _createdElements.Count; i++)
        {
            Destroy(_createdElements[i].gameObject);
        }
        _createdElements.Clear();

        for (int i = 0; i < Boxes[index].BoxContainer.Count; i++)
        {
            Sprite p = Boxes[index].BoxContainer[i];
            var element = Instantiate(BoxContentTemplate, BoxContent);
            element.gameObject.SetActive(true);
            element.sprite = p;
            element.color = Color.black;
            if (UserData.User.UnlockedAvatars.Contains(element.sprite.name))
            {
                element.color = Color.white;
            }
            _createdElements.Add(element);
        }
        StartRouletteButton.gameObject.SetActive(true);
    }


    private void OnUnlockedAvatar(UnlockAvatarResponse obj)
    {
        var sprite = AvatarManager.GetAvatarByName(obj.UnlockedAvatar);
        CanvasUtilities.Instance.ShowNewAvatarUnlocked(sprite);
        StartRouletteButton.interactable = true;
        BackButton.gameObject.SetActive(true);
        UserData.User.UnlockedAvatars.Add(obj.UnlockedAvatar);

        StartCoroutine(UnlockAvatarImage(obj));
    }

    private IEnumerator UnlockAvatarImage(UnlockAvatarResponse obj)
    {
        yield return new WaitForSeconds(0.6f);
        if (_createdElements.Count >= 0)
        {
            var unlockedAvatarImage = _createdElements.First(t => t.sprite.name == obj.UnlockedAvatar);
            unlockedAvatarImage.color = Color.white;
        }
    }

    public void RouletteSkin()
    {
        if (UserData.User.EmeraldCount >= Price)
        {
            WordBombNetworkManager.Instance.SendPacket(new UnlockAvatarRequest()
            {
                Id = selectedId
            });
        }
        else
        {
            PopupManager.Instance.Show(Language.Get("BOX_DONTHAVEENOUGHCOINS"));
        }
    }
}
