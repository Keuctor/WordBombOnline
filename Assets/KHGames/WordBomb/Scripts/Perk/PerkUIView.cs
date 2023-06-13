using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkUIView : MonoBehaviour
{
    public Image PerkIcon;
    public TMP_Text PerkDescription;
    public TMP_Text PerkTitle;

    public TMP_Text PriceText;
    public GameObject PriceTag;

    public UIGradient Background;
    public Outline Outline;

    public void SetPrice(int price)
    {
        if (price <= 0) return;

        if (UserData.User.EmeraldCount < price)
        {
            GetComponent<Button>().interactable = false;
        }

        PriceTag.gameObject.SetActive(true);
        PriceText.text = price.ToString();

        Background.color1 = new Color(0.937f, 1f, 0.476f, 1f);
        Background.color2 = new Color(0.696f, 0.212f, 1f, 1f);
    }

    public void OnClick()
    {
        OnClicked?.Invoke();
    }

    public event Action OnClicked;
}
