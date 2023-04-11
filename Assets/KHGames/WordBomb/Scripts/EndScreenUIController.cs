using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EndScreenProfile
{
    public GameObject Parent;
    public Image Avatar;
    public TMP_Text Name;
    public TMP_Text LetterCount;
}

public struct EndScreenPlayerData
{
    public Sprite Avatar;
    public string Name;
    public int LetterCount;
}

public class EndScreenUIController : MonoBehaviour
{
    public List<EndScreenProfile> EndScreenProfiles
        = new List<EndScreenProfile>();

    public TMP_Text LongestWordOwnerName;
    public TMP_Text LongestWordText;

    public RectTransform LongestWordRectTransform;
    public RectTransform FirstPlaceRectTransform;
    public RectTransform SecondPlaceRectTransform;

    public CanvasGroup CanvasGroup;
    public Image BackgroundEffect;

    public void Initialize(List<EndScreenPlayerData> playerData, string longestWordOwner, string longestWord)
    {
        for (int i = 0; i < playerData.Count; i++)
        {
            var pData = playerData[i];
            var ui = EndScreenProfiles[i];
            ui.Parent.gameObject.SetActive(true);
            ui.Name.text = pData.Name;
            ui.Avatar.sprite = pData.Avatar;
            ui.LetterCount.text = Language.Get("USERDATA_GOLD") + pData.LetterCount;
        }
        this.LongestWordText.text = longestWord;
        this.LongestWordOwnerName.text = longestWordOwner;
    }

    private void Start()
    {
        FirstPlaceRectTransform.localScale = Vector3.zero;
        SecondPlaceRectTransform.localScale = Vector3.zero;
        LongestWordRectTransform.localScale = Vector3.zero;
        CanvasGroup.alpha = 0;

        CanvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            FirstPlaceRectTransform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
            {
                SecondPlaceRectTransform.DOScale(Vector3.one, 1f).SetDelay(0.5f).OnComplete(() =>
                {
                    LongestWordRectTransform.DOScale(Vector3.one, 0.5f);
                });
            });
        });

        BackgroundEffect.transform.DOLocalRotate(new Vector3(0, 0, 100F), 1f).SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }
}