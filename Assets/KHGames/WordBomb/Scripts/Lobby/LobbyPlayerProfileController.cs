using DG.Tweening;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common;

public class LobbyPlayerProfileController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private RectTransform _playerProfile;

    [SerializeField]
    private TMP_Text _nameLabel, _crownLabel, _levelLabel;

    [SerializeField]
    private Image _playerProfileIcon;

    public Action<int> OnSendGift;

    private int _selectedPlayerId;

    [SerializeField]
    private RectTransform _giftOptionsPanel;

    public void Show(Player player)
    {
        PopupManager.Active = true;
        _canvasGroup.alpha = 0;
        _selectedPlayerId = player.Id;
        _playerProfile.anchoredPosition = new Vector2(0, -600);

        _canvasGroup.DOFade(1, 0.2f);
        _playerProfile.DOAnchorPos(new Vector2(0, 30), 0.2f);

        _nameLabel.text = player.UserName;
        _crownLabel.text = player.CrownCount.ToString();
        _levelLabel.text = Language.Get("LEVEL", player.Level);

        _playerProfileIcon.sprite = AvatarManager.GetAvatar(player.AvatarId);

        if (GameSetup.LocalPlayerId == player.Id) {
            _giftOptionsPanel.gameObject.SetActive(false);
        }
    }
    public void Close()
    {
        PopupManager.Active = false;
        _canvasGroup.DOFade(0, 0.2f);
        _playerProfile.DOAnchorPos(new Vector2(0, -600), 0.2f);
        Destroy(gameObject, 0.25f);
    }
    public void GiftHeart()
    {
        OnSendGift?.Invoke(_selectedPlayerId);
        Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }
}
