using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupButton : PopupElement
{
    [SerializeField] private TMP_Text Text;
    [SerializeField] private Button Button;

    public void Initialize(string text, Action action)
    {
        Text.text = text;
        Button.onClick.AddListener(() => { action?.Invoke(); });
    }
}