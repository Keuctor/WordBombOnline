using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialLetterView : MonoBehaviour,
    IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color BackgroundPointerEnterColor;
    public Color BackgroundPointerExitColor;

    public Color TextPointerEnterColor;
    public Color TextPointerExitColor;

    public TMP_Text Text;
    public Image Background;

    public int Index;
    public string Letter;

    private void Start()
    {
        Text.text = Letter;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!RadialInputController.Instance.Group.interactable) return;
        RadialInputController.Instance.OnClicked(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!RadialInputController.Instance.Group.interactable) return;
        RadialInputController.Instance.OnPointerEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!RadialInputController.Instance.Group.interactable) return;
        RadialInputController.Instance.OnPointerExit(this);
    }
}