using DG.Tweening;
using ilasm.WordBomb;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public class KeyController : MonoBehaviour
{
    private bool _isMyTurn;

    [SerializeField]
    private TurnController _turnController;

    public Transform LetterContent;
    public GameObject LetterViewTemplate;

    private List<GameObject> instantiatedLetters = new List<GameObject>();



    public Action<string> OnClientTextChanged;

    private string _clientText;
    private string _previousText;


    public string CurrentLetters = "";



    private float _spaceHoldTime;

    [SerializeField]
    public TMP_Text _focusIndicatorText;

    public Action<int, string> OnClientGuessedWord;
    public Action<int, string> OnClientGuessWrong;
    public Action<int, string> OnClientGuessWrongLimited;
    public Action<int, string> OnClientGuessSameWordUsed;

    private bool _gameEnded;

    public IEnumerator FocusIndicatorEnumerator()
    {
        while (_isMyTurn && _clientText.Length == 0)
        {
            _focusIndicatorText.text = "|";
            if (!_isMyTurn || _clientText.Length != 0) break;
            yield return new WaitForSeconds(0.4f);
            _focusIndicatorText.text = "";
            if (!_isMyTurn || _clientText.Length != 0) break;
            yield return new WaitForSeconds(0.4f);
        }
    }
    private void OnTextChanged(string clientText)
    {
        if (_previousText != clientText)
        {
            _previousText = clientText;
            WordBombNetworkManager.EventListener.SendWordChange(clientText);
            UpdateLetters(clientText);
        }
    }

    public void OnWordUpdate(WordUpdateResponse obj)
    {
        UpdateLetters(obj.Word);
    }

    private void OnSubmitWord(SubmitWordResponse obj)
    {
        if (obj.Id == GameSetup.LocalPlayerId)
        {
            _isMyTurn = true;
        }

        if (obj.FailType == 0)
        {
            OnClientGuessedWord?.Invoke(obj.SenderId, obj.Word);
        }
        else if (obj.FailType == 2)
        {
            OnClientGuessSameWordUsed?.Invoke(obj.SenderId, obj.Word);
        }
        else if (obj.FailType == 1)
        {
            OnClientGuessWrong?.Invoke(obj.SenderId, obj.Word);
        }
        else if (obj.FailType == 3)
        {
            OnClientGuessWrongLimited?.Invoke(obj.SenderId, obj.Word);
        }

        _focusIndicatorText.text = "";
        _clientText = "";
        UpdateLetters("");
    }

    public void SubmitWord()
    {
        if (string.IsNullOrEmpty(_clientText))
            return;

        _isMyTurn = false;
        WordBombNetworkManager.EventListener.SubmitWord(_clientText);
    }

    public void PCPlayerTurnUpdate()
    {
        if (!Input.anyKey) return;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitWord();
            return;
        }
        foreach (KeyCode vKey in KeyCodeUtilities.GetAllowedLetters())
        {
            if (Input.GetKeyDown(vKey))
            {
                var str = vKey.ToString();
                switch (vKey)
                {
                    case KeyCode.Comma:
                        str = "Ö";
                        break;//Ö
                    case KeyCode.Period:
                        str = "Ç";
                        break;//Ç
                    case KeyCode.LeftBracket:
                        str = "Ð";
                        break;//Ð
                    case KeyCode.RightBracket:
                        str = "Ü";
                        break;//Ü
                    case KeyCode.Semicolon:
                        str = "Þ";
                        break;//Þ
                    case KeyCode.Quote:
                        str = (MatchmakingService.CurrentRoom.Language == 0) ? "I" : "Ý";
                        break;//Ý
                }
                if (!string.IsNullOrEmpty(str))
                {
                    _clientText += str;
                    OnClientTextChanged?.Invoke(_clientText);
                }

            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _spaceHoldTime = 0;
            RemoveLastCharacter();
        }

        if (Input.GetKey(KeyCode.Backspace))
        {
            if (_spaceHoldTime > 12)
            {
                _spaceHoldTime = 0;
                RemoveLastCharacter();
            }
            else
            {
                _spaceHoldTime += Time.deltaTime * 80;
            }
        }
    }
    public void RemoveLastCharacter()
    {
        if (_clientText.Length <= 0) return;
        _clientText = _clientText.Remove(_clientText.Length - 1);
        OnClientTextChanged?.Invoke(_clientText);
    }

    private void UpdateLetters(string text)
    {
        text = WordProvider.Censore(text);
        CurrentLetters = text;
        if (_isMyTurn && string.IsNullOrEmpty(text))
        {
            StartCoroutine(FocusIndicatorEnumerator());
        }

        while (instantiatedLetters.Count > text.Length)
        {
            var ob = instantiatedLetters[instantiatedLetters.Count - 1];
            instantiatedLetters.Remove(ob);
            Destroy(ob.gameObject);
        }

        while (instantiatedLetters.Count < text.Length && instantiatedLetters.Count < 25)
        {
            var created = Instantiate(LetterViewTemplate, LetterContent);
            instantiatedLetters.Add(created);
            SoundManager.PlayAudio(Sounds.NewLetter);
        }
        for (int i = 0; i < instantiatedLetters.Count; i++)
        {
            var created = instantiatedLetters[i];
            var textComponent = created.transform.GetChild(0).GetComponent<TMP_Text>();
            textComponent.text = text[i].ToString().ToUpper();

            var comboLength = MatchmakingService.CurrentRoom.Mode == 2 ? LengthController.TargetLength : 6;
            bool isCombo = MatchmakingService.CurrentRoom.Mode == 2 ? text.Length == comboLength : text.Length >= comboLength;

            if (isCombo)
            {
                created.GetComponent<Image>().color = new Color(0.65f, 0.44f, 1, 1);
            }
            else
            {
                created.GetComponent<Image>().color = new Color(0.45f, 0.44f, 1, 1);
            }
        }
    }

    string previousKeyboardString = "";
    bool keyboardStringHasChanged = false;
    TouchScreenKeyboard keyboard;




    public void MobilePlayerTurnUpdate()
    {
        if (keyboard == null)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable,
                false, false, false, false, "", 25);
            keyboardStringHasChanged = true;
            TouchScreenKeyboard.hideInput = true;
        }
        else
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                if (string.IsNullOrEmpty(keyboard.text))
                {
                    keyboard = null;
                    return;
                }
                keyboard = null;
                _isMyTurn = false;
                SubmitWord();
                return;
            }

            if (!TouchScreenKeyboard.visible || !keyboard.active)
            {
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable,
              false, false, false, false, "", 25);
                keyboard.active = true;
            }

            keyboard.text = keyboard.text.Trim();
            keyboard.text = Regex.Replace(this.keyboard.text, @"[^a-zA-ZðüþöçýÝÐÜÞÖÇ]", string.Empty);

            _clientText = keyboard.text.ToUpper(MatchmakingService.CurrentRoom.Language == 0 ? enCulture : trCulture);

            if (previousKeyboardString != keyboard.text)
            {
                previousKeyboardString = keyboard.text;
                keyboardStringHasChanged = true;
            }

            if (!keyboardStringHasChanged)
                return;

            OnClientTextChanged?.Invoke(_clientText);
        }
    }


    private void Update()
    {
        if (_isMyTurn && !_gameEnded)
        {
            if (Application.isMobilePlatform)
            {
                MobilePlayerTurnUpdate();
            }
            else
            {
                PCPlayerTurnUpdate();
            }
        }
    }


    private void OnGameEnd(MatchWinnerResponse obj)
    {
        if (Application.isMobilePlatform)
        {
            if (keyboard != null)
            {
                keyboard.active = false;
                keyboard = null;
            }
        }

        _gameEnded = true;
        _isMyTurn = false;
        _clientText = "";
        StopCoroutine(FocusIndicatorEnumerator());
    }

    private void OnClientTurn()
    {
        _isMyTurn = true;
        _clientText = "";
        UpdateLetters("");
    }

    private void OnPlayerTurn(int obj)
    {
        if (Application.isMobilePlatform)
        {
            if (keyboard != null)
            {
                keyboard.active = false;
                keyboard = null;
            }
        }

        _isMyTurn = false;
        _clientText = "";
        UpdateLetters("");
        StopCoroutine(FocusIndicatorEnumerator());
    }

    CultureInfo enCulture;
    CultureInfo trCulture;

    private void OnEnable()
    {
        enCulture = CultureInfo.GetCultureInfo("en-US");
        trCulture = CultureInfo.GetCultureInfo("tr-TR");
        WordBombNetworkManager.EventListener.OnWordUpdate += OnWordUpdate;
        WordBombNetworkManager.EventListener.OnSubmitWord += OnSubmitWord;
        _turnController.OnPlayerTurn += OnPlayerTurn;
        _turnController.OnClientTurn += OnClientTurn;
        WordBombNetworkManager.EventListener.OnMatchWinner += OnGameEnd;
        OnClientTextChanged += OnTextChanged;
    }



    private void OnDisable()
    {
        _turnController.OnPlayerTurn -= OnPlayerTurn;
        _turnController.OnClientTurn -= OnClientTurn;
        OnClientTextChanged -= OnTextChanged;
        WordBombNetworkManager.EventListener.OnWordUpdate -= OnWordUpdate;
        WordBombNetworkManager.EventListener.OnMatchWinner -= OnGameEnd;
        WordBombNetworkManager.EventListener.OnSubmitWord -= OnSubmitWord;
    }
}

public static class KeyboardRect
{

    public static float GetHeight()
    {
        if (Application.isEditor)
        {
            return 0f;
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                var unityPlayer = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");
                var view = unityPlayer.Call<AndroidJavaObject>("getView");
                if (view == null) return 0;
                int result;
                using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    view.Call("getWindowVisibleDisplayFrame", rect);
                    result = Screen.height - rect.Call<int>("height");
                }
                if (TouchScreenKeyboard.hideInput) return result;
                var softInputDialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
                var window = softInputDialog?.Call<AndroidJavaObject>("getWindow");
                var decorView = window?.Call<AndroidJavaObject>("getDecorView");
                if (decorView == null) return result;
                var decorHeight = decorView.Call<int>("getHeight");
                result += decorHeight;
                return result;
            }
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var area = TouchScreenKeyboard.area;
            return area.height;
        }
        return 0f;
    }
}