using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common;

public struct WinScreenOwnedStat
{
    public string Owner;
    public string Stat;
    public string Value;
}
public struct WinScreenDetails
{
    public Sprite FirstPlaceAvatar;
    public string FirstPlaceOwner;
    public string FirstPlaceScore;
    public int EarnedXP;
    public int EarnedCoin;
    public int EarnedEmerald;
    public Player[] Players;
}

public class WinScreenUIController : MonoBehaviour
{
    [SerializeField]
    private WinScreenStatUIView _WinScreenStatViewTemplate;
    [SerializeField]
    private Transform _WinScreenStatViewContent;

    [SerializeField]
    private WinScreenPlayerView _WinScreenPlayerViewTemplate;

    [SerializeField]
    private Transform _WinScreenPlayerViewContent;

    public Action OnEnd;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Image _durationImage;

    [Header("UI")]
    [SerializeField]
    private Image _firstPlaceAvatar;

    [SerializeField]
    private RectTransform _earningsPanel, _firstPlayerPanel, _statsPanel, _effectPanel, _playersPanel;


    [SerializeField]
    private TMP_Text _firstPlaceNameLabel,
        _firstPlaceScoreLabel,
        _earnedXPLabel,
        _earnedCoinLabel,
        _earnedEmeraldLabel;


    public void ShowWinScreen(int duration)
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.4f);
        _durationImage.DOFillAmount(1, duration);

        _playersPanel.transform.localScale = Vector3.zero;
        StartCoroutine(MoveRectPanelCoroutine(_earningsPanel, 0, 300, 0.5f));
        StartCoroutine(MoveRectPanelCoroutine(_firstPlayerPanel, -900, 0, 0.8f));
        StartCoroutine(MoveRectPanelCoroutine(_statsPanel, 0, -300, 0.3f));

        _statsPanel.transform.localScale = Vector3.zero;
        _firstPlayerPanel.transform.localScale = Vector3.zero;

        _effectPanel.transform.DOLocalRotate(new Vector3(0, 0, 100F), 1f).SetLoops(-1, LoopType.Incremental)
           .SetEase(Ease.Linear);

        _playersPanel.DOScale(1f, 0.2f).SetDelay(0.5f);

        _statsPanel.DOScale(1, 0.1f).SetDelay(0.5f);
        _firstPlayerPanel.DOScale(1, 0.1f).SetDelay(0.4f);
    }



    private IEnumerator MoveRectPanelCoroutine(RectTransform rect, int xOffset, int yOffset, float delay)
    {
        var initialPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(initialPos.x - xOffset, initialPos.y - yOffset);
        yield return new WaitForSeconds(delay);
        rect.DOAnchorPos(initialPos, 0.8f);
    }
    private IEnumerator ShowWinScreenCoroutine(int duration)
    {
        yield return new WaitForSeconds(duration);
        OnEnd?.Invoke();
    }


    public void SetStats(WinScreenDetails details)
    {
        _firstPlaceAvatar.sprite = details.FirstPlaceAvatar;
        _firstPlaceNameLabel.text = details.FirstPlaceOwner;
        _firstPlaceScoreLabel.text = details.FirstPlaceScore;

        _earnedXPLabel.text = details.EarnedXP + " XP";
        _earnedCoinLabel.text = "+" + details.EarnedCoin;
        _earnedEmeraldLabel.text = "+" + details.EarnedEmerald;

        foreach (var p in details.Players)
        {
            var pView = Instantiate(_WinScreenPlayerViewTemplate, _WinScreenPlayerViewContent);
            pView.SetView(p.UserName, p.EliminationOrder - 1, p.Score, AvatarManager.GetAvatar(p.AvatarId));
        }
    }
    public void AddPlayer()
    {

    }

    public void AddStat(WinScreenOwnedStat stat)
    {
        var st = Instantiate(_WinScreenStatViewTemplate, _WinScreenStatViewContent);
        st.StatLabel.text = stat.Stat;
        st.ValueLabel.text = stat.Value;
        st.StatOwnerLabel.text = stat.Owner;
    }

    private void OnDisable()
    {
        _effectPanel?.DOKill();
        _statsPanel?.DOKill();
        _firstPlayerPanel?.DOKill();
        _earningsPanel?.DOKill();
        _canvasGroup?.DOKill();
    }
}
