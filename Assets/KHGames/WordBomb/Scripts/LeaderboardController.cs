
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Request;
using WordBombServer.Common.Packets.Response;

public class LeaderboardController : MonoBehaviour
{

    [Header("First Place UI")]
    [SerializeField]
    private TMP_Text FirstPlaceNameLabel;
    [SerializeField]
    private TMP_Text FirstPlaceScoreLabel, FirstPlaceWinCountLabel;
    [SerializeField]
    private Image FirstPlaceAvatarImage;

    [SerializeField]
    private LeaderboardPlayerView LeaderboardPlayerViewTemplate;

    [SerializeField]
    private Transform LeaderboardPlayerViewContent;

    public Sprite SecondPlaceSprite;
    public Sprite ThirdPlaceSprite;
    private bool _destroyed;

    [SerializeField]
    private Image BackgroundEffect;

    public void OnBack()
    {
        _destroyed = true;
        Destroy(gameObject);
    }


    private static float _lastRequest = -1;
    private void Start()
    {
        CanvasUtilities.Instance.Toggle(true, Language.Get("LOADING"));
        StartCoroutine(OpenLeaderboard());
    }


    private IEnumerator OpenLeaderboard()
    {
        if (Mathf.Abs(Time.realtimeSinceStartup - _lastRequest) <= 2)
        {
            yield return new WaitForSeconds(2f);
        }

        _lastRequest = Time.realtimeSinceStartup;
        WordBombNetworkManager.Instance.SendPacket(new LeaderboardRequest());

        BackgroundEffect.transform.DOLocalRotate(new Vector3(0, 0, 100F), 1f).SetLoops(-1, LoopType.Incremental)
         .SetEase(Ease.Linear);

        WordBombNetworkManager.EventListener.OnLeaderboard += OnLeaderboardResponse;
    }

    private void OnLeaderboardResponse(LeaderboardResponse obj)
    {
        CanvasUtilities.Instance.Toggle(false);

        if (!gameObject.activeSelf || _destroyed) return;

        for (int i = 0; i < obj.LeaderboardData.Count; i++)
        {
            var data = obj.LeaderboardData[i];
            if (i == 0)
            {
                FirstPlaceNameLabel.text = data.DisplayName;
                FirstPlaceScoreLabel.text = data.CoinCount.ToString();
                FirstPlaceWinCountLabel.text = data.WinCount.ToString();
                FirstPlaceAvatarImage.sprite = AvatarManager.GetAvatar(data.AvatarID);
                continue;
            }

            var leaderboardView = Instantiate(LeaderboardPlayerViewTemplate, LeaderboardPlayerViewContent);
            leaderboardView.CoinCountLabel.text = data.CoinCount.ToString();
            leaderboardView.WinCountLabel.text = data.WinCount.ToString();
            leaderboardView.DisplayNameLabel.text = data.DisplayName;
            leaderboardView.AvatarImage.sprite = AvatarManager.GetAvatar(data.AvatarID);

            leaderboardView.PlaceText.text = (i + 1).ToString();
            if (i == 1)
            {
                leaderboardView.PlaceImage.sprite = SecondPlaceSprite;
            }
            else if (i == 2)
            {
                leaderboardView.PlaceImage.sprite = ThirdPlaceSprite;
            }
            else
            {
                leaderboardView.PlaceImage.GetComponent<Image>().enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        BackgroundEffect.transform.DOKill();
        WordBombNetworkManager.EventListener.OnLeaderboard -= OnLeaderboardResponse;
        _destroyed = true;
    }
}
