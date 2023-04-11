using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmeraldCircleView : MonoBehaviour
{
    [SerializeField]
    private Image _fillImage;
    public void Fill() {
        _fillImage.DOFillAmount(1, 0.5f);
    }
    public void UnFill() {
        _fillImage.DOFillAmount(0, 0.5f);
    }

}
