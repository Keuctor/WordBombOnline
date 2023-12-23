using ilasm.WordBomb.Initialization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using WordBombServer.Common;

public class LobbyPlayerView : MonoBehaviour
{
    public Image Background;
    public Image ProfileImage;
    public TMP_Text NameLabel;
    public TMP_Text StatusLabel;
    public Image HostIcon;
    public Button KickButton;
    public Button EditAvatar;
    public TMP_Text GameStatusLabel;

    public void SetAvatar(int avatarId)
    {
        this.ProfileImage.sprite = AvatarManager.GetAvatar(avatarId);
    }

    private int _id;
    public Action<int> OnClicked;

    private void OnClick()
    {
        OnClicked?.Invoke(_id);
    }

    public void InitializeView(Player player)
    {
        _id = player.Id;
        KickButton.onClick.RemoveAllListeners();
        EditAvatar.onClick.RemoveAllListeners();

        KickButton.gameObject.SetActive(false);
        EditAvatar.gameObject.SetActive(false);

        HostIcon.transform.gameObject.SetActive(player.Id == MatchmakingService.CurrentRoom.HostId);

        GameStatusLabel.text = player.GameLoaded ? Language.Get("IN_GAME") : "";

        var id = player.Id;
        if (GameSetup.LocalPlayerId == player.Id)
        {
            Background.color = new Color(0.6f, 0.2f, 1, 0.6f);
            EditAvatar.gameObject.SetActive(true);
            EditAvatar.onClick.AddListener(() =>
            {
                var avatar = new SelectAvatarPopup();
                PopupManager.Instance.Show(avatar);
                avatar.OnSubmit += (index) =>
                {
                    LobbyManager.ChangeAvatar(id, index);
                };
            });
        }
        else
        {
            if (MatchmakingService.CurrentRoom.HostId == GameSetup.LocalPlayerId)
            {
                SetKickButtonVisible(id);
            }
            Background.color = new Color(1, 1, 1, 0);
        }
        this.NameLabel.text = player.UserName;

        this.StatusLabel.text = Language.Get("LEVEL", player.Level);

        SetAvatar(player.AvatarId);
    }


    public void SetKickButtonVisible(int playerId)
    {
        KickButton.onClick.RemoveAllListeners();

        KickButton.onClick.AddListener(() =>
        {
            var player = MatchmakingService.CurrentRoom.Players.Find(t => t.Id == playerId);
            if (player != null)
            {
                var question = new QuestionPopup(player.UserName + " adl� oyuncuyu ger�ekten odadan atmak istiyor musun? Odaya tekrar kat�lamayacak.");
                question.OnSubmit += () =>
                {
                    LobbyManager.KickPlayer(playerId);
                };
                PopupManager.Instance.Show(question);
            }
            else
            {
                PopupManager.Instance.Show("{PLAYER_ID_ERROR}");
            }
        });
        KickButton.gameObject.SetActive(true);
    }
}