using System.Collections;
using TMPro;
using UnityEngine;
public class PopupText : PopupElement
{
    [SerializeField]
    private TMP_Text Text;

    public TMP_Text TextComponent => Text;
    public void Initialize(string text, TextAlignmentOptions textAlignment
         = TextAlignmentOptions.TopLeft)
    {
        Text.text = text;
        Text.alignment = textAlignment;
    }
}
