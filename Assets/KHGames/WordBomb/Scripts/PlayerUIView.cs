using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUIView : MonoBehaviour
{
    public Image Background;
    public GameObject HeartPanel;
    public GameObject HeartTemplate;
    public TMP_Text PlayerNameLabel;
    public TMP_Text LastThingTyped;
    public Image PlayerIcon;
    public TMP_Text WordScoreLabel;
    public GameObject ExplosionEffect;

    public Image Circle;

    public Image ClientImage;
    public TMP_Text ComboText;
    public GameObject ComboEffect;

    private int _health;
    public Image DisconnectImage;

    private void SetComboCount(int comboCount)
    {
        ComboText.text = comboCount.ToString() + "x";
    }


    public void SetCombo(int comboCount)
    {
        ComboEffect.gameObject.SetActive(comboCount >= 5);
        SetComboCount(comboCount);
    }

    public void Disconnected() {
        DisconnectImage.gameObject.SetActive(false);
    }

    public void Eliminated()
    {
        SetCombo(0);
        ComboText.gameObject.SetActive(false);
        StartCoroutine(InstantiateExplosionEffect());
        ClientImage.color = new Color(ClientImage.color.r, ClientImage.color.g, ClientImage.color.b, 0.3f);
        Circle.color = new Color(Circle.color.r, Circle.color.g, Circle.color.b, 0.3f);
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 0.3f);
        
        
        PlayerIcon.color = new Color(PlayerIcon.color.r, PlayerIcon.color.g, PlayerIcon.color.b, 0.3f);
        PlayerNameLabel.color = new Color(PlayerNameLabel.color.r, PlayerNameLabel.color.g, PlayerNameLabel.color.b, 0.3f);
        HeartPanel.gameObject.SetActive(false);
        IEnumerator InstantiateExplosionEffect()
        {
            var explosion = Instantiate(ExplosionEffect, transform);
            yield return new WaitForSeconds(explosion.GetComponent<ParticleSystem>().main.duration);
            Destroy(explosion);
        }
    }

    public void DecreaseHealth()
    {
        var heartPanelTransform = HeartPanel.gameObject.transform;
        heartPanelTransform.transform.GetChild(0)
            .transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce)
            .OnComplete(() =>
            {
                Destroy(heartPanelTransform.transform.GetChild(0).gameObject);
            }
        );
        transform.DOShakeScale(0.6f);
    }

    public void SetWordScore(int score)
    {
        WordScoreLabel.text = score + "";
    }

    public void SetHealth(int health)
    {
        for (int i = 0; i < HeartPanel.transform.childCount; i++)
        {
            Destroy(HeartPanel.transform.GetChild(i).gameObject);
        }
        for (int x = 0; x < health; x++)
        {
            IncreaseHealth();
        }
    }

    public void IncreaseHealth()
    {
        var hTemp = Instantiate(HeartTemplate, HeartPanel.transform);
        hTemp.transform.localScale = Vector3.zero;
        hTemp.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBounce); ;
    }

}
