using System;
using System.Collections.Generic;
using System.Linq;
using ilasm.WordBomb.Initialization;
using UnityEngine;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Request;
using WordBombServer.Common.Packets.Response;


public class NetworkRoomEventListener
{
    private WordBombNetworkManager NetworkManager;

    public Action<Lobby> OnCreateRoom;
    public Action<Lobby> OnJoinRoom;
    public Action OnRoomSettingsChange;
    public Action<Player> OnPlayerJoinRoom;
    public Action<int> OnPlayerLeave;
    public Action<int> OnPlayerKicked;
    public Action<TurnChangedResponse> OnTurnChanged;
    public Action<MatchWinnerResponse> OnMatchWinner;
    public Action<Player> OnPlayerUpdate;
    public Action<string> OnOperationFail;
    public Action OnGameStart;
    public Action<StartCountdownResponse> OnStartCountdown;
    public Action<LobbiesQueryResponse> OnLobbyQuery;
    public Action<SubmitWordResponse> OnSubmitWord;
    public Action<GiftPlayerResponse> OnGiftPlayer;
    public Action<UpdateDisplayNameResponse> OnUpdateDisplayName;
    public Action<LoginResponse> OnLogin;
    public Action<QuickGameResponse> OnQuickGame;
    public Action<LogoutResponse> OnLogout;
    public Action<EliminatePlayerResponse> OnPlayerEliminate;
    public Action<PlayerDecreaseHealthResponse> OnPlayerDecreaseHealth;
    public Action<LeaderboardResponse> OnLeaderboard;
    public Action<int> OnHostChanged;
    public Action<int, string> OnChatMessageReceive;
    public Action<WordUpdateResponse> OnWordUpdate;
    public Action<UpdateUserData> OnUpdateUserData;

    public Action<Player[]> OnPlayersInRoom;
    public Action OnLeaveRoom;
    public static Lobby CurrentRoom;


    public NetworkRoomEventListener(WordBombNetworkManager networkManager)
    {
        this.NetworkManager = networkManager;
        NetworkManager.NetPacketProcessor.SubscribeReusable<FailedOperationResponse>(OnOperationFailed);

        NetworkManager.NetPacketProcessor.SubscribeReusable<CreateRoomResponse>(OnCreateRoomPacketReceived);
        NetworkManager.NetPacketProcessor.SubscribeReusable<JoinRoomResponse>(OnJoinRoomPacketReceived);
        NetworkManager.NetPacketProcessor.SubscribeReusable<PlayersInRoomResponse>(OnPlayersInRoomReceived);
        NetworkManager.NetPacketProcessor.SubscribeReusable<RoomHostChangedResponse>(OnRoomHostChangedPacketReceived);
        NetworkManager.NetPacketProcessor.SubscribeReusable<PlayerJoinedResponse>(OnPlayerJoinedRoom);
        NetworkManager.NetPacketProcessor.SubscribeReusable<PlayerLeftResponse>(OnPlayerLeaveRoom);
        NetworkManager.NetPacketProcessor.SubscribeReusable<RoomSettingsChangedResponse>(OnRoomSettingsChanged);
        NetworkManager.NetPacketProcessor.SubscribeReusable<ChatMessageResponse>(OnChatMessageReceived);
        NetworkManager.NetPacketProcessor.SubscribeReusable<KickPlayerResponse>(OnKickPlayer);
        NetworkManager.NetPacketProcessor.SubscribeReusable<UpdatePlayerInfoResponse>(OnUpdatePlayer);
        NetworkManager.NetPacketProcessor.SubscribeReusable<GameStartResponse>(OnGameStartResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<StartCountdownResponse>(OnGameStartCountdownResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<TurnChangedResponse>(OnGameTurnChanged);
        NetworkManager.NetPacketProcessor.SubscribeReusable<EliminatePlayerResponse>(OnEliminatedPlayer);
        NetworkManager.NetPacketProcessor.SubscribeReusable<PlayerDecreaseHealthResponse>(OnPlayerDecrasedHealth);
        NetworkManager.NetPacketProcessor.SubscribeReusable<MatchWinnerResponse>(OnMatchWinnerResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<WordUpdateResponse>(OnWordUpdateResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<SubmitWordResponse>(OnSubmitWordResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<GiftPlayerResponse>(OnGiftPlayerResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<LobbiesQueryResponse>(OnLobbiesQueryResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<QuickGameResponse>(OnQuickGameResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<LoginResponse>(OnLoginResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<UpdateDisplayNameResponse>(OnUpdateDisplayNamePlayerResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<UnlockAvatarResponse>(OnUnlockAvatarResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<LeaderboardResponse>(OnLeaderboardResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<UpdateUserData>(OnUpdateUserResponse);
        NetworkManager.NetPacketProcessor.SubscribeReusable<LogoutResponse>(OnLogoutResponse);
    }

    private void OnLogoutResponse(LogoutResponse obj)
    {
        OnLogout?.Invoke(obj);
    }

    private void OnUpdateUserResponse(UpdateUserData obj)
    {
        if (GameSetup.LocalPlayerId == obj.Id)
        {
            UserData.GiveEmerald(obj.Emerald);
            UserData.GiveCoin(obj.Coin);
            UserData.GiveExperience(obj.XP);
        }
        OnUpdateUserData?.Invoke(obj);
    }

    private void OnLeaderboardResponse(LeaderboardResponse obj)
    {
        OnLeaderboard?.Invoke(obj);
    }

    private void OnUnlockAvatarResponse(UnlockAvatarResponse obj)
    {
        UserData.User.EmeraldCount = obj.EmeraldCount;
        EventBus.OnEmeraldChanged?.Invoke(UserData.User.EmeraldCount);
    }

    private void OnUpdateDisplayNamePlayerResponse(UpdateDisplayNameResponse obj)
    {
        UserData.User.DisplayName = obj.DisplayName;
        UserData.User.EmeraldCount = obj.EmeraldCount;
        EventBus.OnEmeraldChanged?.Invoke(UserData.User.EmeraldCount);
        OnUpdateDisplayName?.Invoke(obj);
    }

    private void OnLoginResponse(LoginResponse obj)
    {
        OnLogin?.Invoke(obj);
    }

    private void OnQuickGameResponse(QuickGameResponse obj)
    {
        OnQuickGame?.Invoke(obj);
    }
    private void OnLobbiesQueryResponse(LobbiesQueryResponse obj)
    {
        OnLobbyQuery?.Invoke(obj);
    }
    private void OnGiftPlayerResponse(GiftPlayerResponse obj)
    {
        OnGiftPlayer?.Invoke(obj);
    }
    private void OnSubmitWordResponse(SubmitWordResponse obj)
    {
        OnSubmitWord?.Invoke(obj);
    }
    private void OnWordUpdateResponse(WordUpdateResponse obj)
    {
        OnWordUpdate?.Invoke(obj);
    }
    private void OnMatchWinnerResponse(MatchWinnerResponse obj)
    {
        OnMatchWinner?.Invoke(obj);
    }
    private void OnPlayerDecrasedHealth(PlayerDecreaseHealthResponse obj)
    {
        OnPlayerDecreaseHealth?.Invoke(obj);
    }
    private void OnEliminatedPlayer(EliminatePlayerResponse obj)
    {
        var target = MatchmakingService.CurrentRoom.InGamePlayers.FirstOrDefault(t => t.Id == obj.Id);
        if (target != null)
        {
            target.IsDead = true;
        }
        else
        {
            Debug.LogError("{CLIENT_NOT_EXIST_PLAYER_ELIMINATION}");
        }
        OnPlayerEliminate?.Invoke(obj);
    }
    private void OnGameTurnChanged(TurnChangedResponse obj)
    {
        OnTurnChanged?.Invoke(obj);
    }
    private void OnGameStartCountdownResponse(StartCountdownResponse obj)
    {
        OnStartCountdown?.Invoke(obj);
    }
    private void OnGameStartResponse(GameStartResponse obj)
    {
        OnGameStart?.Invoke();
    }
    public void OnUpdatePlayer(UpdatePlayerInfoResponse playerInfo)
    {
        var room = MatchmakingService.CurrentRoom;
        if (room == null)
        {
            return;
        }
        var p = room.Players.SingleOrDefault(t => t.Id == playerInfo.Id);
        if (p != null)
        {
            p.AvatarId = playerInfo.AvatarId;
            p.Level = playerInfo.Level;
            OnPlayerUpdate?.Invoke(p);
        }
    }
    private void OnKickPlayer(KickPlayerResponse obj)
    {
        OnPlayerKicked?.Invoke(obj.Id);
    }
    private void OnChatMessageReceived(ChatMessageResponse obj)
    {
        OnChatMessageReceive?.Invoke(obj.Id, obj.Message);
    }

    private void OnRoomHostChangedPacketReceived(RoomHostChangedResponse obj)
    {
        OnHostChanged?.Invoke(obj.Id);
    }

    private void OnOperationFailed(FailedOperationResponse obj)
    {
        OnOperationFail?.Invoke(obj.Error);
    }

    private void OnPlayerLeaveRoom(PlayerLeftResponse obj)
    {
        OnPlayerLeave?.Invoke(obj.Id);
    }

    private void OnPlayerJoinedRoom(PlayerJoinedResponse obj)
    {
        MatchmakingService.CurrentRoom.Players.Add(obj.Player);
        if (obj.Player.GameLoaded)
        {
            MatchmakingService.CurrentRoom.InGamePlayers.Add(obj.Player);
        }
        OnPlayerJoinRoom?.Invoke(obj.Player);
    }

    private void OnPlayersInRoomReceived(PlayersInRoomResponse obj)
    {
        OnPlayersInRoom?.Invoke(obj.Players);
    }
    private void OnRoomSettingsChanged(RoomSettingsChangedResponse obj)
    {
        CurrentRoom.Language = obj.NewLanguage;
        CurrentRoom.Mode = obj.NewMode;
        CurrentRoom.Speed = obj.NewSpeed;
        CurrentRoom.IsPrivate = obj.NewLobbyIsPrivate;

        OnRoomSettingsChange?.Invoke();
        //MatchmakingService.CurrentRoom = CurrentRoom;
    }
    private void OnJoinRoomPacketReceived(JoinRoomResponse obj)
    {
        CurrentRoom = new Lobby
        {
            Language = obj.GameLanguage,
            Mode = obj.GameMode,
            IsPrivate = obj.IsPrivate,
            Title = obj.RoomTitle,
            HostId = obj.HostId,
            Code = obj.RoomCode,
            Speed = obj.GameSpeed,
            Players = obj.Players.ToList(),
            InGamePlayers = obj.Players.Where(t => t.GameLoaded).ToList()
        };
        CurrentRoom.Players.Add(GetPlayer());
        MatchmakingService.CurrentRoom = CurrentRoom;
        OnJoinRoom?.Invoke(CurrentRoom);
    }
    public Player GetPlayer()
    {
        return new Player()
        {
            Id = GameSetup.LocalPlayerId,
            Level = UserData.User.Level,
            UserName = UserData.User.DisplayName,
            AvatarId = UserData.User.AvatarId,
            CrownCount = UserData.User.CrownCount,
        };
    }

    private void OnCreateRoomPacketReceived(CreateRoomResponse obj)
    {
        CurrentRoom = new Lobby
        {
            Language = obj.GameLanguage,
            Mode = obj.GameMode,
            IsPrivate = obj.IsPrivate,
            Title = obj.RoomTitle,
            HostId = GameSetup.LocalPlayerId,
            Code = obj.RoomCode,
            Speed = obj.GameSpeed,
            Players = new List<Player>()
            {
                GetPlayer()
            }
        };
        MatchmakingService.CurrentRoom = CurrentRoom;
        OnCreateRoom?.Invoke(CurrentRoom);
    }

    public void CreateRoom(byte mode, byte language, byte speed, bool isPrivate)
    {
        NetworkManager.SendPacket(new CreateRoomRequest()
        {
            IsPrivate = isPrivate,
            GameMode = mode,
            GameLanguage = language,
            GameSpeed = speed,
            IsMobile = Application.isMobilePlatform
        });
    }

    public void JoinRoom(string roomCode)
    {
        NetworkManager.SendPacket(new JoinRoomRequest()
        {
            RoomCode = roomCode,
            IsMobile = Application.isMobilePlatform
        });
    }

    public void SendWordChange(string word)
    {
        var wordChanged = new WordChangeRequest()
        {
            Word = word
        };
        NetworkManager.SendPacket(wordChanged);
    }

    public void LeaveRoom()
    {
        CurrentRoom = null;
        NetworkManager.SendPacket(new LeaveRoomRequest());
    }
    public void UpdateLobbySettings(byte language, byte mode, byte speed, bool isPrivate)
    {
        UserData.GameLanguage = language;
        UserData.GameMode = mode;
        UserData.GameSpeed = speed;
        UserData.IsLobbyPrivate = isPrivate;

        NetworkManager.SendPacket(new ChangeRoomSettingRequest()
        {
            Language = language,
            Mode = mode,
            Speed = speed,
            IsPrivate = isPrivate
        });
    }

    public void SendChatMessage(int playerId, string inputText)
    {
        if (inputText.Length > 100)
            inputText = inputText.Substring(0, 100);

        NetworkManager.SendPacket(new ChatMessageRequest()
        {
            Message = inputText
        });

        OnChatMessageReceived(new ChatMessageResponse()
        {
            Id = playerId,
            Message = inputText
        });
    }

    public void KickPlayer(int id)
    {
        NetworkManager.SendPacket(new KickPlayerRequest()
        {
            Id = id
        });
    }
    public void RequestQuickGame()
    {
        NetworkManager.SendPacket(new QuickGameRequest()
        {
            Language = UserData.GameLanguage
        });
    }

    public void ChangeAvatar(int index)
    {
        NetworkManager.SendPacket(new UpdatePlayerInfoRequest()
        {
            AvatarId = (short)index
        });

        UserData.User.AvatarId = (short)index;
        OnUpdatePlayer(new UpdatePlayerInfoResponse()
        {
            AvatarId = (short)index,
            Id = GameSetup.LocalPlayerId
        });
    }


    public void StartGame()
    {
        NetworkManager.SendPacket(new GameStartRequest());
    }

    public void GiftPlayer(int id)
    {
        NetworkManager.SendPacket(new GiftPlayerRequest()
        {
            TargetId = id
        });
    }

    public void SendLoadingCompletedRequest()
    {
        var loadingComplete = new LoadingCompleteRequest();
        NetworkManager.SendPacket(loadingComplete);
    }

    public void SubmitWord(string clientText)
    {
        NetworkManager.SendPacket(new SubmitWordRequest()
        {
            Word = clientText
        });
    }

    internal void RequestLobbyQuery()
    {
        NetworkManager.SendPacket(new GetLobbiesRequest()
        {
        });
    }

}