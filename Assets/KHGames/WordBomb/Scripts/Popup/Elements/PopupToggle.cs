

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupToggle : PopupElement
{
    public Toggle Toggle;
    public TMP_Text Text;
    public Image Icon;

    public void SetIcon(Sprite icon) {
        Icon.sprite = icon;
        Icon.gameObject.SetActive(true);
    }
}
