using DG.Tweening;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common;

public class GuessWordController : MonoBehaviour
{
    [SerializeField]
    private KeyController keyboardController;

    [SerializeField]
    private Image _border, _background;

    [SerializeField]
    private TMP_Text _infoText;

    [SerializeField]
    private TMP_Text _longestWordText, longestWordOwner;

    private int _longestTextLength;

    public static string LongestWordOwnerName;
    public static string LongestWord;

    private void Start()
    {
        LongestWordOwnerName = "";
        LongestWord = "";
    }

    private void OnEnable()
    {
        _longestTextLength = 0;
        keyboardController.OnClientGuessedWord += OnClientGuessedWord;
        keyboardController.OnClientGuessSameWordUsed += OnClientGuessSameWordUsed;
        keyboardController.OnClientGuessWrong += OnClientGuessedWrong;
        keyboardController.OnClientGuessWrongLimited += OnClientGuessLengthWrong;

    }
    private void OnDisable()
    {
        keyboardController.OnClientGuessedWord -= OnClientGuessedWord;
        keyboardController.OnClientGuessSameWordUsed -= OnClientGuessSameWordUsed;
        keyboardController.OnClientGuessWrong -= OnClientGuessedWrong;
        keyboardController.OnClientGuessWrongLimited -= OnClientGuessLengthWrong;
    }

    private void OnClientGuessedWrong(int id, string word)
    {
        _infoText.text = Language.Get("GAME_WRONG");
        AlertBackground();
    }
    private void OnClientGuessLengthWrong(int id, string word)
    {
        _infoText.text = Language.Get("SUBMIT_WORD_LENGTH_ERROR");
        AlertBackground();
    }

    private void AlertBackground()
    {
        var sq = DOTween.Sequence();
        var color = _background.color;
        sq.Append(_background.DOColor(new Color(1, color.g, color.b), 0.1f));
        sq.Append(_background.DOColor(color, 0.05f));
        SoundManager.PlayAudio(Sounds.Wrong);
    }

    private void OnClientGuessSameWordUsed(int id, string word)
    {
        _infoText.text = Language.Get("GAME_WRITTENBEFORE");
        AlertBackground();
    }

    private void OnClientGuessedWord(int id, string word)
    {
        if (GameSetup.LocalPlayerId == id)
        {
            SoundManager.PlayAudio(Sounds.Correct);
        }

        var p = MatchmakingService.CurrentRoom.InGamePlayers.Find(t => t.Id == id);
        if (p != null)
        {
            if (_longestTextLength < word.Length)
            {
                _longestTextLength = word.Length;
                _longestWordText.text = WordProvider.Censore(word);
                longestWordOwner.text = p.UserName;
                LongestWordOwnerName = p.UserName;
                LongestWord = WordProvider.Censore(word);
            }
        }
        else
        {
            _longestWordText.text = "{GUESSWORD_ERROR}";
            longestWordOwner.text = "{GUESSWORD_ERROR}";
            _longestTextLength = 0;
        }
    }
}
