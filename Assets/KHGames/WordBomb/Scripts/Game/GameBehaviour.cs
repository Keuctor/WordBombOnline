using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Response;

public class GameBehaviour : MonoBehaviour
{
    [Header("Player UI")] [SerializeField] private PlayerUIView _playerUITemplate;

    [SerializeField] private Transform _playerUIContent;

    [Header("UI")] [SerializeField] private TMP_Text _gameStatusLabel;

    [SerializeField] public TMP_Text _currentWordLabel;

    [Header("Controller")] [SerializeField]
    private BombController _bombController;

    [SerializeField] private TurnController _turnController;

    [SerializeField] private KeyController _keyboardController;

    private bool _isGameStarted;

    [SerializeField] private TMP_Text _screenComboText;

    [SerializeField] private GameObject _screenComboGameObject;

    [SerializeField] private WinScreenUIController _winScreenTemplate;

    [SerializeField] private Transform _winScreenContent;

    public Dictionary<int, PlayerUIView> _players = new Dictionary<int, PlayerUIView>();


    public TMP_Text WaitingForPlayerText;


    public RadialInputController RadialInputControllerTemplate;
    public static RadialInputController RadialInput;

    public GameObject[] OriginalModeObjects;

    private void OnEnable()
    {
        _keyboardController.OnClientGuessWrong += OnPlayerGuessWrong;
        _keyboardController.OnClientGuessSameWordUsed += OnPlayerGuessWrong;
        _keyboardController.OnClientGuessedWord += OnPlayerGuessed;
        WordBombNetworkManager.EventListener.OnTurnChanged += OnTurnChanged;
        WordBombNetworkManager.EventListener.OnPlayerLoadGameSceneComplete += OnLoadSceneCompletedByPlayer;
        WordBombNetworkManager.EventListener.OnMatchWinner += OnMatchWinner;
        WordBombNetworkManager.EventListener.OnPlayerEliminate += OnPlayerEliminated;
        WordBombNetworkManager.EventListener.OnPlayerDecreaseHealth += OnPlayerDecreaseHealth;

        if (MatchmakingService.CurrentRoom.GameType != 0)
        {
            for (int i = 0; i < OriginalModeObjects.Length; i++)
                OriginalModeObjects[i].gameObject.SetActive(false);
        }
    }

    private void OnLoadSceneCompletedByPlayer(PlayerLoadedResponse obj)
    {
        if (obj.TotalPlayer == obj.LoadedPlayerCount)
        {
            WaitingForPlayerText.gameObject.SetActive(false);
        }
        else
        {
            WaitingForPlayerText.text = Language.Get("WAITING_FOR_PLAYERS", obj.LoadedPlayerCount, obj.TotalPlayer);
        }
    }

    private void OnPlayerGuessWrong(int id, string obj)
    {
        if (_players.TryGetValue(id, out PlayerUIView view))
        {
            var targetPlayer = MatchmakingService.CurrentRoom.InGamePlayers.Find(t => t.Id == id);
            targetPlayer.Combo = 1;
            view.SetCombo(targetPlayer.Combo);

            if (GameSetup.LocalPlayerId == id)
            {
                _screenComboText.text = "1x";
            }
        }
    }

    private void OnPlayerGuessed(int id, string obj)
    {
        if (_players.TryGetValue(id, out PlayerUIView view))
        {
            var targetPlayer = MatchmakingService.CurrentRoom.InGamePlayers.Find(t => t.Id == id);
            if (obj.Length >= 6 || (MatchmakingService.CurrentRoom.Mode == 2) ||
                (MatchmakingService.CurrentRoom.Mode == 3))
            {
                targetPlayer.Combo++;
            }
            else
            {
                targetPlayer.Combo = 1;
            }

            view.SetCombo(targetPlayer.Combo);
            view.LastThingTyped.text = WordProvider.Censore(obj);

            if (GameSetup.LocalPlayerId == id)
            {
                _screenComboText.text = (targetPlayer.Combo) + "x";
            }
        }
    }

    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnTurnChanged -= OnTurnChanged;
        WordBombNetworkManager.EventListener.OnMatchWinner -= OnMatchWinner;
        WordBombNetworkManager.EventListener.OnPlayerEliminate -= OnPlayerEliminated;
        WordBombNetworkManager.EventListener.OnPlayerDecreaseHealth -= OnPlayerDecreaseHealth;
        WordBombNetworkManager.EventListener.OnPlayerLoadGameSceneComplete -= OnLoadSceneCompletedByPlayer;
    }

    private void OnPlayerEliminated(EliminatePlayerResponse obj)
    {
        var eliminatedPlayer = MatchmakingService.CurrentRoom.InGamePlayers.FirstOrDefault(t => t.Id == obj.Id);

        var totalPlayers = MatchmakingService.CurrentRoom.InGamePlayers;
        var totalDeadPlayers = totalPlayers.Count(t => t.Id != obj.Id && t.IsDead);

        eliminatedPlayer.EliminationOrder = totalPlayers.Count - totalDeadPlayers;

        if (_players.TryGetValue(obj.Id, out PlayerUIView view))
        {
            view.Eliminated();
            if (obj.Reason == 1)
            {
                view.Disconnected();
            }

            view.ComboText.gameObject.SetActive(true);
            view.ComboText.text = $"{eliminatedPlayer.EliminationOrder}.";
        }
    }

    private void OnMatchWinner(MatchWinnerResponse obj)
    {
        TurnController.IsMyTurn = false;
        if (RadialInput != null)
        {
            Destroy(RadialInput.gameObject);
        }


        var winScreen = Instantiate(_winScreenTemplate, _winScreenContent);


        var winnerPlayer = MatchmakingService.CurrentRoom.InGamePlayers.First(t => t.Id == obj.Id);
        winnerPlayer.CrownCount++;

        if (winnerPlayer.Id == GameSetup.LocalPlayerId && TurnController.Round >= 10)
        {
            UserData.GiveCrown();
        }

        var firstTwoPlayers =
            MatchmakingService.CurrentRoom.InGamePlayers.Where(t => t.EliminationOrder <= 3)
                .OrderBy(t => t.EliminationOrder)
                .Take(2)
                .ToArray();

        winScreen.SetStats(new WinScreenDetails()
        {
            EarnedCoin = CoinController.CollectedCoin,
            EarnedXP = XPController.CollectedXP,
            EarnedEmerald = EmeraldController.CollectedEmerald,
            FirstPlaceAvatar = AvatarManager.GetAvatar(winnerPlayer.AvatarId),
            FirstPlaceOwner = winnerPlayer.UserName,
            FirstPlaceScore = winnerPlayer.Score.ToString(),
            Players = firstTwoPlayers
        });

        winScreen.AddStat(new WinScreenOwnedStat()
        {
            Owner = GuessWordController.LongestWordOwnerName,
            Value = GuessWordController.LongestWord,
            Stat = Language.Get("GAME_LONGESTWORD")
        });


        var highestScore = MatchmakingService.CurrentRoom.InGamePlayers.OrderByDescending(t => t.Score).First();

        winScreen.AddStat(new WinScreenOwnedStat()
        {
            Owner = highestScore.UserName,
            Value = highestScore.Score + "",
            Stat = Language.Get("GAME_HIGHEST_SCORE"),
        });

        var highestCombo = MatchmakingService.CurrentRoom.InGamePlayers.OrderByDescending(t => t.MaxCombo).First();
        winScreen.AddStat(new WinScreenOwnedStat()
        {
            Owner = highestCombo.UserName,
            Value = highestCombo.MaxCombo + "",
            Stat = Language.Get("GAME_HIGHEST_COMBO"),
        });


        winScreen.ShowWinScreen(obj.Countdown);

        _screenComboGameObject.gameObject.SetActive(false);
        _bombController.Stop();
        _turnController.Stop();
        _gameStatusLabel.text = "";
        StartCoroutine(ReturnToLobby(obj.Countdown));
    }

    IEnumerator ReturnToLobby(int countdown)
    {
        yield return new WaitForSeconds(countdown);
        SceneManager.LoadScene("Lobby");
    }

    private void OnPlayerDecreaseHealth(PlayerDecreaseHealthResponse obj)
    {
        if (_players.TryGetValue(obj.Id, out PlayerUIView view))
        {
            view.DecreaseHealth();
        }
    }

    public void OnTurnChanged(TurnChangedResponse response)
    {
        if (!_isGameStarted)
            return;

        TurnController.IsMyTurn = response.Id == GameSetup.LocalPlayerId;
        _bombController.Timer(response.Timer, response.Index, MatchmakingService.CurrentRoom.InGamePlayers.Count);
        _turnController.SetTurn(response.Id, response.Round);

        if (MatchmakingService.CurrentRoom.GameType == 1)
        {
            RadialInput.SetInteractable(TurnController.IsMyTurn);
            RadialInput.ShowLetters(response.NewWordPart);
            _currentWordLabel.text = "";
        }
        else
        {
            _currentWordLabel.text = response.NewWordPart;
        }
    }

    public void SetStatusText(string text)
    {
        _gameStatusLabel.text = text;
    }


    public void CreatePlayers()
    {
        foreach (var p in MatchmakingService.CurrentRoom.InGamePlayers)
        {
            CreatePlayerUI(p);
        }
    }

    public void StartGame(string part, int timer)
    {
        if (MatchmakingService.CurrentRoom.GameType == 0)
        {
            _currentWordLabel.text = part;
        }
        else if (MatchmakingService.CurrentRoom.GameType == 1) //radial
        {
            RadialInput = Instantiate(RadialInputControllerTemplate);
            RadialInput.ShowLetters(part);
        }

        _isGameStarted = true;
        for (int i = 0; i < MatchmakingService.CurrentRoom.InGamePlayers.Count; i++)
        {
            if (!MatchmakingService.CurrentRoom.InGamePlayers[i].IsDead)
            {
                TurnController.IsMyTurn = MatchmakingService.CurrentRoom.InGamePlayers[i].Id == GameSetup.LocalPlayerId;

                _bombController.Timer(timer, i, MatchmakingService.CurrentRoom.InGamePlayers.Count);
                _turnController.SetTurn(MatchmakingService.CurrentRoom.InGamePlayers[i].Id, 1);

                if (MatchmakingService.CurrentRoom.GameType == 1)
                {
                    RadialInput.SetInteractable(TurnController.IsMyTurn);
                }
                break;
            }
        }
    }

    public void CreatePlayerUI(Player player)
    {
        var ui = Instantiate(_playerUITemplate, _playerUIContent);
        ui.PlayerNameLabel.text = player.UserName;
        ui.SetHealth(2);

        ui.ClientImage.gameObject.SetActive(player.Id == GameSetup.LocalPlayerId);
        ui.Background.gameObject.SetActive(player.Id != GameSetup.LocalPlayerId);
        ui.Circle.gameObject.SetActive(player.Id != GameSetup.LocalPlayerId);
        ui.PlayerIcon.sprite = AvatarManager.GetAvatar(player.AvatarId);
        this._players.Add(player.Id, ui);
    }
}