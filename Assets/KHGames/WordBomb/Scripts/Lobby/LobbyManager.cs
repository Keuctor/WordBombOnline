using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Response;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager _instance;

    public static Action<Lobby> OnJoinedLobby;
    public static List<Player> Players => MatchmakingService.CurrentRoom.Players;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (_instance != this) return;
        LobbyView.LobbySelected += JoinLobby;
        WordBombNetworkManager.EventListener.OnCreateRoom += OnCreateRoom;
        WordBombNetworkManager.EventListener.OnJoinRoom += OnJoinRoom;
        WordBombNetworkManager.EventListener.OnPlayerLeave += OnPlayerLeavedRoom;
        WordBombNetworkManager.EventListener.OnHostChanged += OnHostChanged;
        WordBombNetworkManager.EventListener.OnOperationFail += OnOperationFailed;
    }


    public void OnHostChanged(int hostId)
    {
        MatchmakingService.CurrentRoom.HostId = hostId;
    }

    private void OnOperationFailed(string obj)
    {
        CanvasUtilities.Instance.Toggle(false);
        PopupManager.Instance.Show(Language.Get(obj));
    }

    private void OnPlayerLeavedRoom(int obj)
    {
        var find = MatchmakingService.CurrentRoom.Players.FirstOrDefault(t => t.Id == obj);
        MatchmakingService.CurrentRoom.Players.Remove(find);
    }


    public static void OnJoinedLobby_Callback(Lobby room)
    {
        CanvasUtilities.Instance.Toggle(false);
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        OnJoinedLobby?.Invoke(room);
    }
    public static string GetLobbyModeTitle(byte mode) {
        return Language.Get(mode == 0 ? "GAMEMODE_RANDOM" : mode == 1 ? "GAMEMODE_CONTINUOUS" : mode == 2 ? "GAMEMODE_LENGTH_LIMITED" : "GAMEMODE_IMAGES");
    }

    public static void CreateLobby()
    {
        CanvasUtilities.Instance.Toggle(true, Language.Get("CREATING_LOBBY"));
        WordBombNetworkManager.EventListener.CreateRoom(UserData.GameMode, UserData.GameLanguage, UserData.GameSpeed, UserData.GamePrivate);
    }

    public static void LeaveLobby()
    {
        CanvasUtilities.Instance.Toggle(true, Language.Get("LEAVING_LOBBY"), true);
        WordBombNetworkManager.EventListener.LeaveRoom();
        AsyncOperation operation = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        operation.completed += (asyncOperation =>
        {
            CanvasUtilities.Instance.Toggle(false);
        });
    }


    private void OnCreateRoom(Lobby room)
    {
        OnJoinedLobby_Callback(room);
    }
    private void OnJoinRoom(Lobby room)
    {
        OnJoinedLobby_Callback(room);
    }


    public static void JoinLobby(Lobby room)
    {
        CanvasUtilities.Instance.Toggle(true, Language.Get("JOINING_LOBBY"));

        OnJoinedLobby_Callback(room);
    }

    public static void JoinLobby(string lobbyCode)
    {
        WordBombNetworkManager.EventListener.JoinRoom(lobbyCode);
    }

    public static void ChangeAvatar(int id, int index)
    {
        WordBombNetworkManager.EventListener.ChangeAvatar(index);
    }

    public static void FindQuickLobbyAsync()
    {
        WordBombNetworkManager.EventListener.RequestQuickGame();
    }


    public static void KickPlayer(int id)
    {
        WordBombNetworkManager.EventListener.KickPlayer(id);
    }


    private void OnDisable()
    {
        if (_instance != this) return;
        LobbyView.LobbySelected -= JoinLobby;
        WordBombNetworkManager.EventListener.OnCreateRoom -= OnCreateRoom;
        WordBombNetworkManager.EventListener.OnJoinRoom -= OnJoinRoom;
        WordBombNetworkManager.EventListener.OnPlayerLeave -= OnPlayerLeavedRoom;
        WordBombNetworkManager.EventListener.OnOperationFail -= OnOperationFailed;
        WordBombNetworkManager.EventListener.OnHostChanged -= OnHostChanged;
    }
}