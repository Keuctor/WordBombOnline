using DG.Tweening;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public class EmeraldController : MonoBehaviour
{
    public EmeraldCircleView EmeraldCircleView;
    public Transform Content;

    public List<EmeraldCircleView> emeraldViews = new List<EmeraldCircleView>();

    public int Index;

    public static int CollectedEmerald;

    [SerializeField]
    public TMP_Text _totalEmeraldCountText;

    [SerializeField]
    private Image _emeraldIcon;

    private void Start()
    {
        CollectedEmerald = 0;
    }
    private void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnStartCountdown += OnGameStart;
        WordBombNetworkManager.EventListener.OnSubmitWord += OnSumbitWord;
        WordBombNetworkManager.EventListener.OnUpdateUserData += OnUpdateUser;

    }

    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnStartCountdown -= OnGameStart;
        WordBombNetworkManager.EventListener.OnSubmitWord -= OnSumbitWord;
        WordBombNetworkManager.EventListener.OnUpdateUserData -= OnUpdateUser;
    }

    private void OnSumbitWord(SubmitWordResponse obj)
    {
        if (obj.FailType == 0)
        {
            if (GameSetup.LocalPlayerId == obj.SenderId)
            {
                emeraldViews[Index].Fill();
                Index++;
                if (Index > emeraldViews.Count - 1)
                {
                    StartCoroutine(GiveEmeraldEnumerator());
                }
            }
        }
        else if (obj.FailType == 1)
        {
            if (GameSetup.LocalPlayerId == obj.SenderId)
            {
                if (Index > 0)
                {
                    Index--;
                }
                emeraldViews[Index].UnFill();
            }
        }
    }
    private void OnUpdateUser(UpdateUserData obj)
    {
        if (obj.Id == GameSetup.LocalPlayerId)
        {
            if (obj.Emerald > 0)
            {
                CollectedEmerald += obj.Emerald;
                StartCoroutine(GiveEmeraldEnumerator());
            }
        }
    }

    private IEnumerator GiveEmeraldEnumerator()
    {

        yield return new WaitForSeconds(0.5f);
        foreach (var p in emeraldViews)
        {
            p.UnFill();
        }
        var sq = DOTween.Sequence();
        var scale = _emeraldIcon.transform.localScale;
        sq.Append(_emeraldIcon.transform.DOScale(3f, 0.7f).SetEase(Ease.OutBounce));
        sq.Append(_emeraldIcon.transform.DOScale(scale.x, 0.5f));

        _totalEmeraldCountText.text = CollectedEmerald.ToString();


        sq.Append(_totalEmeraldCountText.transform.DOScale(1.5f, 0.3f));
        sq.Append(_totalEmeraldCountText.transform.DOScale(1f, 0.5f));
        Index = 0;
    }

    private void OnGameStart(StartCountdownResponse obj)
    {
        StartCoroutine(InstantiateEmeraldViews(obj.Countdown));
    }
    private IEnumerator InstantiateEmeraldViews(int countDown)
    {
        yield return new WaitForSeconds(countDown);
        Content.gameObject.SetActive(true);
        for (int i = 0; i < 6; i++)
        {
            var circle = Instantiate(EmeraldCircleView, Content);
            circle.transform.SetAsFirstSibling();

            emeraldViews.Add(circle);
        }
        emeraldViews.Reverse();
    }
}
