using DG.Tweening;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Response;

public class GameInitializer : MonoBehaviour
{
    private WordBombNetworkManager NetworkManager;
    public TMP_Text TimerText;
    public GameObject CountDownGameObject;
    public GameBehaviour GameBehaviour;


    [SerializeField]
    private GameObject _firstPlayerPanel;

    [SerializeField]
    private TMP_Text _lobbyModInfoText;

    [SerializeField]
    private Button _backButton;

    private void OnEnable()
    {
        NetworkManager = WordBombNetworkManager.Instance;
        WordBombNetworkManager.EventListener.OnStartCountdown += OnStartCountdown;
        _backButton.onClick.AddListener(OnBackClicked);
    }

    private void Update()
    {
        if (!PopupManager.Active)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackClicked();
            }
        }
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(OnBackClicked);
        WordBombNetworkManager.EventListener.OnStartCountdown -= OnStartCountdown;
    }


    private void OnBackClicked()
    {
        var newMessage = new QuestionPopup(Language.Get("GAME_LEAVE_WARNING"));
        PopupManager.Instance.Show(newMessage);
        newMessage.OnSubmit += LobbyManager.LeaveLobby;
    }


    private void OnStartCountdown(StartCountdownResponse response)
    {
        var lobby = MatchmakingService.CurrentRoom;
        lobby.InGamePlayers = new List<Player>();
        foreach (var id in response.OrderOfPlayers)
        {
            foreach (var p in lobby.Players)
            {
                if (p.Id == id)
                {
                    p.IsDead = false;
                    p.Combo = 1;
                    p.MaxCombo = 0;
                    p.EliminationOrder = response.OrderOfPlayers.Length + 2;
                    p.Score = 0;
                    p.LongestWord = "";
                    lobby.InGamePlayers.Add(p);
                    break;
                }
            }
        }


        if (lobby.GameType == 0)
        {
            if (lobby.Mode == 0)
            {
                _lobbyModInfoText.text = Language.Get("MODINFO_NORMAL");
            }
            else if (lobby.Mode == 1)
            {
                _lobbyModInfoText.text = Language.Get("MODINFO_CONTINUOUS");
            }
            else if (lobby.Mode == 2)
            {
                _lobbyModInfoText.text = Language.Get("MODINFO_COUNT");
            }
        }
        else
        {
            _lobbyModInfoText.text = Language.Get("RADIAL_GAME_INFO");
        }

        CountDownGameObject.gameObject.SetActive(true);
        CountDownGameObject.gameObject.GetComponent<Image>().DOColor(Color.clear, response.Countdown);
        TimerText.text = response.Countdown.ToString();
        StartCoroutine(StartCountDownCoroutine(response));
    }

    public IEnumerator StartCountDownCoroutine(StartCountdownResponse response)
    {
        GameBehaviour.CreatePlayers();
        if (MatchmakingService.CurrentRoom.InGamePlayers[0].Id == GameSetup.LocalPlayerId)
        {
            _firstPlayerPanel.gameObject.SetActive(true);
        }

       

        while (response.Countdown > 0)
        {
            if (response.Countdown == 3)
            {
                SoundManager.PlayAudio(Sounds.Countdown);
            }
            yield return new WaitForSecondsRealtime(1);
            response.Countdown--;
            TimerText.text = response.Countdown.ToString();
        }

        CountDownGameObject.gameObject.SetActive(false);
        GameBehaviour.StartGame(response.FirstWordPart, response.Timer);
    }
}
