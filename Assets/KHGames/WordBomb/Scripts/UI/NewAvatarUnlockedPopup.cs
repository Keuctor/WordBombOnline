
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewAvatarUnlockedPopup : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Image Background;
    public TMP_Text Title;

    public Image BackgroundEffect;
    public Image AvatarImage;
    public Button CloseButton;
   
    Sequence sq;
    DG.Tweening.Core.TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions> tweeningEffect;


    internal void Init(Sprite avatar)
    {
        SoundManager.PlayAudio(Sounds.NewAvatarUnlocked);

        this.AvatarImage.sprite = avatar;
        tweeningEffect =  BackgroundEffect.transform.DOLocalRotate(new Vector3(0, 0, 100F), 1f).SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
        CloseButton.onClick.AddListener(OnCloseClicked);
        var titleRect = this.Title.GetComponent<RectTransform>();
        CloseButton.transform.localScale = Vector3.zero;


        this.AvatarImage.transform.localScale = Vector3.zero;

        var tempFloat = titleRect.anchoredPosition.y;
        titleRect.anchoredPosition = new Vector2(0, 200);

        sq = DOTween.Sequence();
        sq.Append(CanvasGroup.DOFade(1, 0.5f));
        sq.Append(titleRect.DOAnchorPos(new Vector2(0, tempFloat), 1f));
        sq.Append(this.AvatarImage.transform.DOScale(Vector3.one, 0.5f));
        sq.Append(CloseButton.transform.DOScale(1, 0.5f));
    }

    private void OnCloseClicked()
    {
        tweeningEffect.Kill();
        sq.Kill();
        transform.DOKill();
        Destroy(gameObject);
    }
}
