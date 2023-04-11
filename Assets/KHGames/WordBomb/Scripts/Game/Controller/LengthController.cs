using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public class LengthController : MonoBehaviour
{
    [SerializeField]
    private GameObject LengthObject;

    [SerializeField]
    private Image _lengthObjectImageComponent;

    [SerializeField]
    private TMP_Text lengthLabel;

    [SerializeField]
    private KeyController _keyController;

    [SerializeField]
    private TMP_Text _maxLengthText;

    public static byte TargetLength;
    Lobby _lobby;
    private void OnEnable()
    {
        _lobby = MatchmakingService.CurrentRoom;
        TargetLength = 0;
        WordBombNetworkManager.EventListener.OnTurnChanged += OnTurnChanged;
        WordBombNetworkManager.EventListener.OnStartCountdown += OnStartCountdown;
    }

    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnTurnChanged -= OnTurnChanged;
        WordBombNetworkManager.EventListener.OnStartCountdown -= OnStartCountdown;
    }

    private void OnStartCountdown(StartCountdownResponse obj)
    {
        LengthObject.gameObject.SetActive(false);
        StartCoroutine(StartCountdown(obj));
    }
    private IEnumerator StartCountdown(StartCountdownResponse obj)
    {
        yield return new WaitForSeconds(obj.Countdown);
        OnTurnChanged(new TurnChangedResponse()
        {
            TargetLength = obj.TargetLength,
            NewWordPart = obj.FirstWordPart,
        });
    }

    private void Update()
    {
        if (_lobby.Mode == 2)
        {
            if (TargetLength == _keyController.CurrentLetters.Length)
            {
                _lengthObjectImageComponent.color = new Color(0.33f, 0.76f, 0.27f, 0.68f);
            }
            else
            {
                _lengthObjectImageComponent.color = new Color(0.76f, 0.35f, 0.27f, 0.68f);
            }

            lengthLabel.text = "" + Mathf.Max(0, (TargetLength - _keyController.CurrentLetters.Length));
        }
    }

    private void OnTurnChanged(TurnChangedResponse obj)
    {
        _maxLengthText.text = obj.TargetLength + "";
        LengthObject.gameObject.SetActive(obj.TargetLength > 0);
        TargetLength = obj.TargetLength;
        lengthLabel.text = TargetLength + "";
    }
}
