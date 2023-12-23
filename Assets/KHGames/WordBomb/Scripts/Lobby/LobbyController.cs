using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using ilasm.WordBomb.Chat;
using ilasm.WordBomb.Initialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Request;
using WordBombServer.Common.Packets.Response;

public class LobbyController : MonoBehaviour
{
    public Dictionary<int, LobbyPlayerView>
        _lobbyPlayerViews = new();

    [NonSerialized] public List<GameObject> EmptyViews = new();

    #region UI

    public LobbyPlayerView PlayerView;
    public GameObject EmptyPlayerView;
    public Transform ViewContent;
    public Button StartGameButton;
    public TMP_Text LobbyCodeText;
    public Button BackButton;
    public Button LobbySettingsButton;

    public TMP_Text LobbyLanguage;
    public TMP_Text LobbyMode;
    public TMP_Text LobbySped;


    #endregion


    public Toggle RoomLockToggle;

    public ChatManager ChatManager;
    public Canvas ChatCanvas;

    [Header("Player Profile")]
    [SerializeField]
    private LobbyPlayerProfileController _playerProfileTemplate;
    [SerializeField]
    private Transform _playerProfileContent;

    private void OnEnable()
    {
        var room = MatchmakingService.CurrentRoom;

        bool isHost = GameSetup.LocalPlayerId == room.HostId;

        StartGameButton.gameObject.SetActive(isHost);
        LobbySettingsButton.gameObject.SetActive(isHost);

        RoomLockToggle.SetIsOnWithoutNotify(room.IsPrivate);

        LobbyCodeText.text = room.Code;
        foreach (var player in room.Players)
        {
            CreateView(player);
        }
        LobbySettingsButton.onClick.AddListener(OnLobbySettingsClicked);
        StartGameButton.onClick.AddListener(StartGame);
        RoomLockToggle.interactable = isHost;
        RoomLockToggle.onValueChanged.AddListener(OnRoomLockChange);

        WordBombNetworkManager.EventListener.OnPlayerLeave += OnPlayerLeavedRoom;
        WordBombNetworkManager.EventListener.OnPlayerJoinRoom += OnPlayerJoinedRoom;
        WordBombNetworkManager.EventListener.OnHostChanged += OnHostChanged;
        WordBombNetworkManager.EventListener.OnRoomSettingsChange += OnSettingChanged;
        WordBombNetworkManager.EventListener.OnPlayerKicked += OnPlayerKicked;
        WordBombNetworkManager.EventListener.OnPlayerUpdate += OnPlayerUpdate;
        WordBombNetworkManager.EventListener.OnGameStart += OnStartGame;
        WordBombNetworkManager.EventListener.OnMatchWinner += OnMatchWinner;

        OnSettingChanged();
    }

    public void OnLobbyCodeClicked()
    {
        StartCoroutine(LobbyCodeClickedEnumerator());
    }

    public IEnumerator LobbyCodeClickedEnumerator()
    {
        var st = LobbyCodeText.text;
        GUIUtility.systemCopyBuffer = st;
        LobbyCodeText.text = Language.Get("ROOMCODE_COPIED");
        LobbyCodeText.raycastTarget = false;
        yield return new WaitForSeconds(2f);
        LobbyCodeText.raycastTarget = true;
        LobbyCodeText.text = st;
    }

    private void OnMatchWinner(MatchWinnerResponse obj)
    {
        foreach (var p in _lobbyPlayerViews)
        {
            p.Value.GameStatusLabel.text = "";
        }
    }

    private void OnPlayerUpdate(Player obj)
    {
        if (_lobbyPlayerViews.TryGetValue(obj.Id, out var view))
        {
            view.SetAvatar(obj.AvatarId);
        }
    }

    private void OnPlayerKicked(int obj)
    {
        if (GameSetup.LocalPlayerId == obj)
        {
            LobbyManager.LeaveLobby();
            PopupManager.Instance.Show(Language.Get("YOU_ARE_KICKED"));
        }
    }

    private void OnRoomLockChange(bool value)
    {
        var room = MatchmakingService.CurrentRoom;
        if (GameSetup.LocalPlayerId == room.HostId)
        {
            WordBombNetworkManager.EventListener.UpdateLobbySettings(room.Language, room.Mode,
                   room.Speed, value);
            RoomLockToggle.isOn = value;
            StartCoroutine(ResetRoomLockToggle());
        }
    }
    public IEnumerator ResetRoomLockToggle()
    {
        RoomLockToggle.interactable = false;
        yield return new WaitForSecondsRealtime(1.2f);
        RoomLockToggle.interactable = true;
    }

    private void OnSettingChanged()
    {
        LobbyLanguage.text = MatchmakingService.CurrentRoom.Language == 0 ? Language.Get("LANGUAGE_ENGLISH") : Language.Get("LANGUAGE_TURKISH");


            LobbyMode.text = MatchmakingService.CurrentRoom.Mode == 0 ? 
                Language.Get("GAMEMODE_RANDOM") :
                MatchmakingService.CurrentRoom.Mode == 1 ?
                    Language.Get("GAMEMODE_CONTINUOUS") : 
                    Language.Get("GAMEMODE_LENGTH_LIMITED");
            
            
        LobbySped.text = MatchmakingService.CurrentRoom.Speed == 0 ? Language.Get("SLOW") :
            MatchmakingService.CurrentRoom.Speed == 1 ? Language.Get("NORMAL") : Language.Get("FAST");
        RoomLockToggle.SetIsOnWithoutNotify(MatchmakingService.CurrentRoom.IsPrivate);
    }

    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnPlayerLeave -= OnPlayerLeavedRoom;
        WordBombNetworkManager.EventListener.OnPlayerJoinRoom -= OnPlayerJoinedRoom;
        WordBombNetworkManager.EventListener.OnHostChanged -= OnHostChanged;
        WordBombNetworkManager.EventListener.OnRoomSettingsChange -= OnSettingChanged;
        WordBombNetworkManager.EventListener.OnPlayerKicked -= OnPlayerKicked;
        WordBombNetworkManager.EventListener.OnPlayerUpdate -= OnPlayerUpdate;
        WordBombNetworkManager.EventListener.OnGameStart -= OnStartGame;
        WordBombNetworkManager.EventListener.OnMatchWinner -= OnMatchWinner;
    }


    private void OnStartGame()
    {
        var async = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        async.completed += (o) =>
        {
            WordBombNetworkManager.EventListener.SendLoadingCompletedRequest();
        };
    }

    private void StartGame()
    {
        if (MatchmakingService.CurrentRoom.Mode == 3)
        {
            var question = new MessagePopup("This mod is currently not available. Great game modes are on the way with the big update!");
            PopupManager.Instance.Show(question);
            return;
        }
        
        if (MatchmakingService.CurrentRoom.Players.Count < 2)
        {
            var question = new QuestionPopup(Language.Get("RECOMMENDED_USER_COUNT"));
            question.OnSubmit += () =>
            {
                WordBombNetworkManager.EventListener.StartGame();
            };
            PopupManager.Instance.Show(question);
        }
        else
        {
            WordBombNetworkManager.EventListener.StartGame();
        }
    }

    private void OnPlayerJoinedRoom(Player player)
    {
        CreateView(player);
    }

    private void OnPlayerLeavedRoom(int id)
    {
        RemoveView(id);
    }
    private void OnHostChanged(int newHostId)
    {
        bool isHost = GameSetup.LocalPlayerId == newHostId;

        _lobbyPlayerViews[newHostId].HostIcon.transform.parent.gameObject.SetActive(true);

        LobbySettingsButton.gameObject.SetActive(isHost);
        StartGameButton.gameObject.SetActive(isHost);
        RoomLockToggle.interactable = isHost;

        if (isHost)
        {
            foreach (var p in _lobbyPlayerViews)
            {
                if (p.Key != GameSetup.LocalPlayerId)
                {
                    p.Value.SetKickButtonVisible(p.Key);
                }
            }
        }
    }


    private void OnLobbySettingsClicked()
    {
        var lobbySettings = new LobbySettingsPopup(MatchmakingService.CurrentRoom.Language,
            MatchmakingService.CurrentRoom.Mode, MatchmakingService.CurrentRoom.Speed);
        lobbySettings.OnSubmit += (o) =>
        {
            UserData.GameLanguage = (byte)o.Language;
            UserData.GameMode = (byte)o.GameMode;
            UserData.GameSpeed = (byte)o.Speed;

            if (o.GameMode != MatchmakingService.CurrentRoom.Mode ||
            o.Language != MatchmakingService.CurrentRoom.Language || o.Speed != MatchmakingService.CurrentRoom.Speed)
            {
                WordBombNetworkManager.EventListener.UpdateLobbySettings(o.Language, o.GameMode,
                    o.Speed, o.IsPrivate);

                MatchmakingService.CurrentRoom.Language = o.Language;
                MatchmakingService.CurrentRoom.Mode = o.GameMode;
                MatchmakingService.CurrentRoom.Speed = o.Speed;
                MatchmakingService.CurrentRoom.IsPrivate = o.IsPrivate;
                OnSettingChanged();
            }
        };
        PopupManager.Instance.Show(lobbySettings);
    }


    public void RemoveView(int id)
    {
        Destroy(_lobbyPlayerViews[id].gameObject);
        _lobbyPlayerViews.Remove(id);

        UpdateEmptyViews();
    }

    public void CreateView(Player player)
    {
        var plView = Instantiate(PlayerView, ViewContent);
        plView.InitializeView(player);
        plView.OnClicked += ShowPlayerProfile;
        _lobbyPlayerViews.Add(player.Id, plView);

        UpdateEmptyViews();
    }

    public void ShowPlayerProfile(int id)
    {
        var pView = Instantiate(_playerProfileTemplate, _playerProfileContent);
        pView.Show(MatchmakingService.CurrentRoom.GetPlayer(id));
        pView.OnSendGift += (id) =>
        {
            WordBombNetworkManager.EventListener.GiftPlayer(id);
        };
    }

    private void UpdateEmptyViews()
    {
        for (int i = 0; i < EmptyViews.Count; i++)
        {
            Destroy(EmptyViews[i]);
        }

        EmptyViews.Clear();
        var totalNeeded = 8 - _lobbyPlayerViews.Count;
        for (int x = 0; x < totalNeeded; x++)
        {
            var emptView = Instantiate(EmptyPlayerView, ViewContent);
            EmptyViews.Add(emptView);
        }
    }

    private void Update()
    {
        if (!PopupManager.Active)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackPressed();
            }
        }
    }

    public void OnBackPressed()
    {
        var chatController = FindObjectOfType<ChatController>();
        if (chatController != null && chatController.ChatObject.activeSelf)
        {
            chatController.Toggle();
        }
        else
        {
            var newMessage = new QuestionPopup(Language.Get("GAME_LEAVE_WARNING"));
            PopupManager.Instance.Show(newMessage);
            newMessage.OnSubmit += LobbyManager.LeaveLobby;
        }
    }
}