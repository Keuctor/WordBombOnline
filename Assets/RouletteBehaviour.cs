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
            view.Text.text = Language.Get(Boxes[i].Name);
            view.Image.sprite = Boxes[i].Icon;
            var index = i;
            view.Button.onClick.AddListener(() =>
            {
                SelectBox(index);
            });
        }
    }

    private void SelectBox(int index)
    {
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
