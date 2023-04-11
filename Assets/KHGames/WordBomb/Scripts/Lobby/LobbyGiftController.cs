using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public struct LobbyGiftQueue
{
    public string Owner;
    public int AvatarId;
}
public class LobbyGiftController : MonoBehaviour
{
    public RectTransform GiftHeartTemplate;
    public Transform GiftContent;

    [SerializeField]
    private Image _giftOwnerPlayerIcon;

    [SerializeField]
    private TMP_Text _giftOwnerName;

    [SerializeField]
    private RectTransform _senderPanel;
    [SerializeField]
    private CanvasGroup _senderPanelCanvasGroup;

    private Queue<LobbyGiftQueue> _playings = new Queue<LobbyGiftQueue>();

    public void AddQueue(LobbyGiftQueue avatar)
    {
        _playings.Enqueue(avatar);
    }

    private void Start()
    {
        StartCoroutine(PlayCoroutine());
    }
    private void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnGiftPlayer += OnGifted;
    }

    private void OnGifted(GiftPlayerResponse obj)
    {
        var p = MatchmakingService.CurrentRoom.Players.FirstOrDefault(t => t.Id == obj.Sender);
        if (p != null)
        {

            AddQueue(new LobbyGiftQueue()
            {
                AvatarId = p.AvatarId,
                Owner = p.UserName,
            });
        }
    }

    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnGiftPlayer -= OnGifted;
        StopAllCoroutines();
        _senderPanel?.DOKill();
        _senderPanelCanvasGroup?.DOKill();

        
        
    }

    private IEnumerator PlayCoroutine()
    {
        while (true)
        {
            if (_playings.Count > 0)
            {
                var element = _playings.Dequeue();
                Show(element);
                yield return new WaitForSeconds(3f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    

    public void Show(LobbyGiftQueue avatar)
    {
        _senderPanelCanvasGroup.alpha = 0;
        _senderPanelCanvasGroup.DOFade(1, 0.5f);

        var y = _senderPanel.anchoredPosition.y;

        _senderPanel.anchoredPosition = new Vector2(0, y - 200);
        _senderPanel.DOAnchorPos(new Vector2(0, y), 0.5f);

        _giftOwnerName.text = avatar.Owner;
        _giftOwnerPlayerIcon.sprite = AvatarManager.GetAvatar(avatar.AvatarId);

        StartCoroutine(GiftHeartAnimation());
    }

    public IEnumerator GiftHeartAnimation()
    {
        for (int i = 0; i < 12; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.100f, 0.200f));
            var gift = Instantiate(GiftHeartTemplate, GiftContent);
            gift.gameObject.SetActive(true);
            gift.transform.localScale = Vector3.zero;
            gift.transform.DOScale(UnityEngine.Random.Range(0.4f, 2.3f), 0.2f);
            gift.DOAnchorPos(new Vector2(gift.anchoredPosition.x + UnityEngine.Random.Range(-100, 100), gift.anchoredPosition.y + UnityEngine.Random.Range(150, 250)), UnityEngine.Random.Range(1, 1.5f));
            gift.GetComponent<Image>().DOColor(Color.clear, 1f);
            Destroy(gift.gameObject, 1f);
        }
        yield return new WaitForSeconds(1);
        _senderPanelCanvasGroup.DOFade(0, 0.5f);
    }
}
