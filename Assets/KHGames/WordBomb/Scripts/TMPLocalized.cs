using TMPro;
using System;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TMPLocalized : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Text;

    [SerializeField]
    private string Reference;

    private void OnValidate()
    {
        Text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        EventBus.OnLanguageChanged += OnLanguageChanged;
    }
    private void OnDisable()
    {
        EventBus.OnLanguageChanged -= OnLanguageChanged;
    }
    private void OnLanguageChanged()
    {
        Text.text = Language.Get(Reference);
    }

    private void Start()
    {
        OnLanguageChanged();
    }
}
