using DG.Tweening;
using ilasm.WordBomb.Initialization;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    [Header("UI")]
    public Image _border;
    public Image _background;
    public TMP_Text _infoText;

    public Action OnClientTurn;
    public Action<int> OnPlayerTurn;
    public TMP_Text RoundText;

    public static short Round;

    public static bool IsMyTurn;
    public void Stop()
    {
        _background.color = new Color(0.3f, 1f, 0.3f);
        _border.gameObject.SetActive(false);
    }

    public void SetTurn(int id,short round)
    {
        if (GameSetup.LocalPlayerId == id)
        {
            ClientTurn();
            return;
        }
        Round = round;
        RoundText.text = Language.Get("ROUND", round);
        PlayerTurn(id);
    }

    private void ClientTurn()
    {
        _border.gameObject.SetActive(true);
        _background.DOColor(new Color(0.5f, 1f, 0.5f), 0.2f);
        SoundManager.PlayAudio(Sounds.YourTurn);
        _infoText.text = Language.Get("YOUR_TURN");
        OnClientTurn?.Invoke();
    }

    private void PlayerTurn(int id)
    {
        _border.gameObject.SetActive(false);
        SoundManager.PlayAudio(Sounds.ChangedPlayer);
        _background.DOColor(Color.white, 0.3f);
        _infoText.text = Language.Get("USERNAME_IS_PLAYING", MatchmakingService.CurrentRoom.InGamePlayers.Find(t => t.Id == id).UserName);
        OnPlayerTurn?.Invoke(id);
    }
}
