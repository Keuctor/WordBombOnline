using ilasm.WordBomb;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.PickerWheelUI;
using TMPro;
using System.Collections;
using WordBombServer.Common.Packets.Request;

public class RouletteBehaviour : MonoBehaviour
{
    public Button StartRouletteButton;

    public Button BackButton;

    public PickerWheel PickerWheel;

    public List<WheelOfFortuneBoxScriptable> Boxes = new List<WheelOfFortuneBoxScriptable>();

    public InstantiateTemplate<WheelOfFortuneBoxView> BoxViewTemplate;


    public TMP_Text PriceText;

    private bool spining;
    public void OnBackClicked() {
        Destroy(gameObject);
    }
    public bool TestMode;

    private void Start()
    {
        

        PickerWheel.onSpinEndEvent += OnSpinEnd;
        PickerWheel.onSpinStartEvent += OnSpinStart;
        StartRouletteButton.gameObject.SetActive(false);
        for (int i = 0; i < Boxes.Count; i++)
        {
            var view = BoxViewTemplate.Instantiate();
            view.Text.text = Boxes[i].Name;
            view.Image.sprite = Boxes[i].Icon;
            var index = i;
            view.Button.onClick.AddListener(() =>
            {
                SelectBox(index);
            });
        }
    }

    public RectTransform SpiningCircle;

    private int totalBoxElement;
    private int selectedIndex = 0;
    public int Price;
    private void SelectBox(int index)
    {
        totalBoxElement = 0;
        selectedIndex = index;
        PickerWheel.wheelPieces = new List<WheelPiece>();
        Price = Boxes[index].Price;
        PriceText.text = Price.ToString();
        for (int x = 0; x < Boxes[index].BoxContainer.Count; x++)
        {
            if (!PlayerPrefs.HasKey(Boxes[index].BoxContainer[x].name))
            {
                PickerWheel.wheelPieces.Add(new WheelPiece()
                {
                    Chance = 100,
                    Icon = Boxes[index].BoxContainer[x],
                    Index = x,
                    _weight = 1,
                });
                totalBoxElement++;
            }
        }
        if (totalBoxElement > 0)
        {
            PickerWheel.SetupPieces();
            StartRouletteButton.gameObject.SetActive(true);
        }
        else
        {
            PickerWheel.ClearPieces();
            StartRouletteButton.gameObject.SetActive(false);
        }
    }

    private void OnSpinStart()
    {   spining = false;
        StartRouletteButton.interactable = false;
        BackButton.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        spining = true;
    }

    private void OnDisable()
    {
        spining = false;
    }

    private void OnSpinEnd(WheelPiece arg0)
    {
        StartCoroutine(ShowSpinEnd(arg0));
    }

    private IEnumerator ShowSpinEnd(WheelPiece arg0)
    {
        yield return new WaitForSeconds(0.25f);
        spining = true;
        PlayerPrefs.SetInt(arg0.Icon.name, 1);
        CanvasUtilities.Instance.ShowNewAvatarUnlocked(arg0.Icon);
        StartRouletteButton.interactable = true;
        BackButton.gameObject.SetActive(true);
        SelectBox(selectedIndex);
    }

    private void LateUpdate()
    {
        if (spining)
        {
            SpiningCircle.localEulerAngles = new UnityEngine.Vector3(
                SpiningCircle.localEulerAngles.x, SpiningCircle.localEulerAngles.y, SpiningCircle.localEulerAngles.z + 10 * Time.deltaTime);
        }
    }

    public void RouletteSkin()
    {
        if (TestMode) {
            PickerWheel.Spin();
            return;
        }
        if (totalBoxElement >= 0)
        {
            if (UserData.User.EmeraldCount >= Price)
            {
                WordBombNetworkManager.Instance.SendPacket(new UnlockAvatarRequest() { 
                    Price = (byte)Price
                });
                PickerWheel.Spin();
                UserData.GiveEmerald(-Price);
            }
            else
            {
                PopupManager.Instance.Show(Language.Get("BOX_DONTHAVEENOUGHCOINS"));
            }
        }
        else
        {
            PopupManager.Instance.Show(Language.Get("BOX_YOUVE_UNLOCKED_EVERYTHING"));
        }
    }
}
