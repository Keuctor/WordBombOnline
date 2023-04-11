using System.Collections;
using TMPro;
using UnityEngine;


public class PopupInput : PopupElement
{
    [SerializeField]
    private TMP_InputField InputField;
    public void Initialize(string defaultText, string placeHolder)
    {
        InputField.text = defaultText;
        InputField.placeholder.GetComponent<TMP_Text>().text = placeHolder;
    }
    public void Activate() {
        InputField.ActivateInputField();
    }
    public string Text => InputField.text;
}