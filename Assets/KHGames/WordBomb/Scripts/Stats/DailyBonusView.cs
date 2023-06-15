
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusView : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Image BackgroundEffect;

    public Image[] Days;
    public Action OnClaimed;

    private void Start()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.DOFade(1, 0.5f);
        SoundManager.PlayAudio(Sounds.YourTurn);


        BackgroundEffect.transform.DOLocalRotate(new Vector3(0, 0, 100F), 1f).SetLoops(-1, LoopType.Incremental)
         .SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        BackgroundEffect?.DOKill();
    }

    
    public void OnClaimButtonClicked() {
        OnClaimed?.Invoke();
        CanvasGroup.DOFade(0, 0.3f);
        Destroy(gameObject, 0.5f);
    }
}
