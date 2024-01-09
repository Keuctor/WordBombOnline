using System;
using TMPro;
using UnityEngine;
using WordBombServer.Common;

public class LobbyView : MonoBehaviour
{
    public TMP_Text LobbyName;
    public TMP_Text LobbyPlayerCount;
    public LobbyInfo Room { get; private set; }
    public TMP_Text LanguageText;
    public TMP_Text Mode;

    public static event Action<string> LobbySelected;
    public void Init(LobbyInfo room)
    {
        UpdateDetails(room);
    }
    public void UpdateDetails(LobbyInfo room)
    {
        Room = room;
        LobbyName.text = room.Title;
        LobbyPlayerCount.text = $"{room.PlayerCount}/8";
        LanguageText.text = Language.Get(room.Language == 0 ? "LANGUAGE_ENGLISH" : "LANGUAGE_TURKISH");
        Mode.text = LobbyManager.GetLobbyModeTitle();
    }

    public void Clicked()
    {
        LobbySelected?.Invoke(Room.Code);
    }
}
