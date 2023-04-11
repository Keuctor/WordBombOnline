using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupSlider : PopupElement
{
    [SerializeField]
    private Slider Slider;
    public void Initialize(float value, Action<float> onValueChanged)
    {
        Slider.SetValueWithoutNotify(value);
        Slider.onValueChanged.AddListener((f) => { onValueChanged?.Invoke(f); });
    }
}