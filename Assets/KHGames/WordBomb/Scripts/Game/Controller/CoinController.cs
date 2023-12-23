using DG.Tweening;
using ilasm.WordBomb.Initialization;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public class CoinController : MonoBehaviour
{
    public TMP_Text CoinText;

    public static int CollectedCoin;

    [SerializeField]
    private GameBehaviour _gameBehaviour;

    [SerializeField]
    private Transform _letterContent;

    [SerializeField]
    private RectTransform _letterGoldCoinTemplate, _letterGoldCoinContent, _letterGoldCoinGoToTransform;

    void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnSubmitWord += OnSubmitWord;
        WordBombNetworkManager.EventListener.OnUpdateUserData += OnUpdateData;

    }
    private void Start()
    {
        CollectedCoin = 0;
    }

    private void OnUpdateData(UpdateUserData obj)
    {
        if (_gameBehaviour._players.TryGetValue(obj.Id, out var view))
        {
            var player = MatchmakingService.CurrentRoom.InGamePlayers.FirstOrDefault(t => t.Id == obj.Id);
            if (player != null)
            {
                var score = obj.Coin;
                player.Score += score;

                view.SetWordScore(player.Score);
            }
        }

        if (GameSetup.LocalPlayerId == obj.Id)
        {
            CollectedCoin += obj.Coin;
            StartCoroutine(UpdateCoin());
        }
    }

    private void OnSubmitWord(SubmitWordResponse obj)
    {
        if (obj.FailType == 0)
        {
            if (GameSetup.LocalPlayerId == obj.SenderId)
            {
                var golds = new RectTransform[obj.Word.Length];
                for (int i = 0; i < obj.Word.Length; i++)
                {
                    var targetGold = Instantiate(_letterGoldCoinTemplate, _letterContent);
                    golds[i] = targetGold;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_letterContent as RectTransform);
                for (int i = 0; i < golds.Length; i++)
                {
                    golds[i].transform.SetParent(_letterContent.parent);
                    var scale = golds[i].transform.localScale;
                    golds[i].transform.localScale = Vector3.zero;
                    golds[i].DOScale(scale, 0.5f).SetEase(Ease.OutBounce);

                    var sq = DOTween.Sequence();
                    sq.Append(golds[i].DORotate(new Vector3(0, -180, 0), 0.5f));
                    sq.Append(golds[i].DOMove(_letterGoldCoinGoToTransform.position, 0.5f));
                    sq.Append(golds[i].DOScale(0, 0.4f));
                    Destroy(golds[i].gameObject, 1.5f);
                }
                StartCoroutine(UpdateCoin());
            }
        }
    }
    public IEnumerator UpdateCoin()
    {
        yield return new WaitForSeconds(1.4f);
        var sq = DOTween.Sequence();
        sq.Append(CoinText.rectTransform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce));
        sq.Append(CoinText.rectTransform.DOScale(1f, 0.5f));
        CoinText.text = CollectedCoin.ToString();
    }

    void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnSubmitWord -= OnSubmitWord;
        WordBombNetworkManager.EventListener.OnUpdateUserData -= OnUpdateData;
    }
}
