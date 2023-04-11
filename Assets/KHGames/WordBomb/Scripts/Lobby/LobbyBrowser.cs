using ilasm.WordBomb;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public class LobbyBrowser : MonoBehaviour
{
    public InstantiateTemplate<LobbyView> LobbyViewTemplate;
    [SerializeField] private GameObject NoLobbiesText;
    private readonly List<LobbyView> _currentLobbySpawns = new();
    private float _nextRefreshTime;
    [SerializeField] private float LobbyRefreshRate = 2;
    public Image ImageLobbyRefreshed;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> _lobbyRefreshedTweenerCore;
    
    private void Update()
    {
        if (Time.time >= _nextRefreshTime) FetchLobbies();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Close();
        }
    }

    private bool _closed;
    public void Open() {
        _closed = false;
        FetchLobbies();
        WordBombNetworkManager.EventListener.OnLobbyQuery += OnLobbyResponse;
        foreach (Transform child in LobbyViewTemplate.Container) Destroy(child.gameObject);
        _currentLobbySpawns.Clear();
    }
    public void Close() {

        _closed = true;
        WordBombNetworkManager.EventListener.OnLobbyQuery -= OnLobbyResponse;
        foreach (Transform child in LobbyViewTemplate.Container) Destroy(child.gameObject);
        _currentLobbySpawns.Clear();
    }

    private void OnDisable()
    {
        Close();
    }


    private  void FetchLobbies()
    {
        _nextRefreshTime = Time.time + LobbyRefreshRate;
        if (MatchmakingService.CurrentRoom != null) return;
        WordBombNetworkManager.EventListener.RequestLobbyQuery();
    }

    void OnLobbyResponse(LobbiesQueryResponse response) {
        if (_closed) return;

        for (int i = 0; i < _currentLobbySpawns.Count; i++){
            Destroy(_currentLobbySpawns[i].gameObject);
        }
        _currentLobbySpawns.Clear();

        foreach (var lobby in response.Lobbies) {
            var panel = LobbyViewTemplate.Instantiate();
            panel.Init(lobby);
            _currentLobbySpawns.Add(panel);
        }
        NoLobbiesText.SetActive(!_currentLobbySpawns.Any());
    }
}