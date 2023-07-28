using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Globalization;
using WordBombServer.Common.Packets.Response;
using System.Security.Cryptography;
using System.Text;
using System;
using WordBombServer.Common.Packets.Request;
using UnityEngine.SceneManagement;
using System.Linq;

public class MenuController : MonoBehaviour
{
    public void Register() => Register(LoginAnonymousUserNameInputField.text, PasswordInputField.text);
    private void Login() => Login(LoginAnonymousUserNameInputField.text, PasswordInputField.text);

    public void PlayGuest() {
        CanvasUtilities.Instance.Toggle(true, Language.Get("SIGNING_IN"));
        Register("Guest" + UnityEngine.Random.Range(1000, 100000),UnityEngine.Random.Range(10000,99999).ToString());
    }


    public void Login(string name, string password)
    {
        if (string.IsNullOrEmpty(password) || !UserValidator.IsValidPassword(password))
        {
            PopupManager.Instance.Show(Language.Get("PASSWORD_LENGTH_ERROR"));
            return;
        }

        string encoded = EncyrptPassword(password);

        if (UserValidator.IsValidName(name))
        {
            UserData.Username = name;
            UserData.Password = password;

            CanvasUtilities.Instance.Toggle(true, Language.Get("SIGNING_IN"));
            WordBombNetworkManager.Instance.SendPacket(new LoginRequest()
            {
                UserName = name,
                Password = encoded,
            });
        }
    }

    public void Register(string name, string password)
    {
        if (!UserValidator.IsValidPassword(password))
        {

            PopupManager.Instance.Show(Language.Get("PASSWORD_LENGTH_ERROR"));
            return;
        }

        string encoded = EncyrptPassword(password);

        if (UserValidator.IsValidName(name))
        {
            CanvasUtilities.Instance.Toggle(true, Language.Get("SIGNING_IN"));

            UserData.Username = name;
            UserData.Password = password;

            WordBombNetworkManager.Instance.SendPacket(new RegisterRequest()
            {
                UserName = name,
                Password = encoded,
                AvatarId = UserData.User.AvatarId,
            });
        }
        else
        {
            PopupManager.Instance.Show(Language.Get("USERNAME_LENGTH_ERROR"));
        }
    }

    private static string EncyrptPassword(string password)
    {
        byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
        byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
        string encoded = BitConverter.ToString(hash)
           .Replace("-", string.Empty)
           .ToLower();
        return encoded;
    }


    private void OnLogin(LoginResponse obj)
    {
        UserData.User.UserName = obj.UserName;
        UserData.User.DisplayName = obj.DisplayName;
        UserData.User.CrownCount = obj.CrownCount;
        UserData.User.EmeraldCount = obj.EmeraldCount;
        UserData.User.AvatarId = obj.AvatarId;
        UserData.User.Experience = obj.Experience;
        UserData.User.TotalLetters = obj.CoinCount;
        UserData.User.UnlockedAvatars = obj.UnlockedAvatars.Split(",").ToList();
        UserData.User.ClaimDay = obj.ClaimDay;
        UserData.LoggedIn = true;
        UserData.LogOut = false;
        CanvasUtilities.Instance.Toggle(false);

        SignInPanel.gameObject.SetActive(false);
        MenuPanel.gameObject.SetActive(true);
        SetupMenuUICallBacks();
        EventBus.OnLogin?.Invoke(obj);
    }

    public void OnJoinDiscordClicked()
    {
        Application.OpenURL("https://discord.gg/RcBnBCxmXP");
    }

    private void CreateLobby()
    {
        LobbyManager.CreateLobby();
    }

    private void FindQuickLobby()
    {
        LobbyManager.FindQuickLobbyAsync();
    }

    public void OnLogout(LogoutResponse response)
    {
        UserData.LogOut = true;
        UserData.LoggedIn = false;
        SceneManager.LoadScene("Menu");
    }

    private void ChangeAvatar()
    {
        var avatar = new SelectAvatarPopup();
        PopupManager.Instance.Show(avatar);
        avatar.OnSubmit += (index) =>
        {
            UserData.User.AvatarId = (byte)index;
            UpdatePlayerProfile();
        };
    }
    private void UpdateDisplayName(UpdateDisplayNameResponse obj)
    {
        UpdatePlayerProfile();
    }

    private void UpdatePlayerProfile()
    {
        UserName.text = UserData.User.UserName;
        DisplayNameLabel.text = UserData.User.DisplayName;

        DisplayNameLabel.gameObject.SetActive(UserData.User.UserName != UserData.User.DisplayName);

        if (ProfilePicture != null)
            ProfilePicture.sprite = AvatarManager.GetAvatar(UserData.User.AvatarId);

        if (AnonymousProfilePicture != null)
            AnonymousProfilePicture.sprite = AvatarManager.GetAvatar(UserData.User.AvatarId);

        WordBombNetworkManager.EventListener.ChangeAvatar(UserData.User.AvatarId);

    }

    private void LoginLobby()
    {
        EnterInputPopup enterCodePopup = new EnterInputPopup(Language.Get("ENTER_LOBBY_CODE"));
        enterCodePopup.OnSubmit += (roomCode) => { LobbyManager.JoinLobby(roomCode); };
        PopupManager.Instance.Show(enterCodePopup);
    }

    private void Start()
    {
        MatchmakingService.CurrentRoom = null;
        if (!UserData.LoggedIn)
        {
            if (!UserData.LogOut)
            {
                if (PlayerPrefs.HasKey(nameof(UserData.Username)) && PlayerPrefs.HasKey(nameof(UserData.Password)))
                {
                    CanvasUtilities.Instance.Toggle(true, Language.Get("SIGNING_IN"));
                    WordBombNetworkManager.Instance.SendPacket(new LoginRequest()
                    {
                        UserName = UserData.Username,
                        Password = EncyrptPassword(UserData.Password),
                    });
                }
            }


            SetupSignInUICallbacks();
            SignInPanel.gameObject.SetActive(true);
            MenuPanel.gameObject.SetActive(false);
        }
        else
        {
            SetupMenuUICallBacks();
            SignInPanel.gameObject.SetActive(false);
            MenuPanel.gameObject.SetActive(true);
        }
    }


    private void SetupMenuUICallBacks()
    {
        UserName.text = UserData.User.UserName;
        DisplayNameLabel.text = UserData.User.DisplayName;

        DisplayNameLabel.gameObject.SetActive(UserData.User.UserName != UserData.User.DisplayName);


        CrownCountText.text = UserData.User.CrownCount.ToString();
        EmeraldCountText.text = UserData.User.EmeraldCount.ToString();
        CollecttedLetterText.text = UserData.User.TotalLetters.ToString();
        ProfilePicture.sprite = AvatarManager.GetAvatar(UserData.User.AvatarId);
        CreateLobbyButton.onClick.AddListener(CreateLobby);
        LoginLobbyButton.onClick.AddListener(LoginLobby);
        FindLobbyButton.onClick.AddListener(FindQuickLobby);
        ChangeAvatarButton.onClick.AddListener(ChangeAvatar);
        EditNameButton.onClick.AddListener(ChangeName);
        XPSlider.maxValue = UserData.User.MaxExperience;
        XPSlider.minValue = 0;
        XPSlider.SetValueWithoutNotify(UserData.User.Experience % 100);
        LevelText.text = Language.Get("LEVEL", UserData.User.Level);
        EventBus.OnExperienceChanged += OnExperienceChanged;
        EventBus.OnLevelChanged += OnLevelChanged;
        EventBus.OnCoinChanged += OnCoinChanged;
        EventBus.OnEmeraldChanged += OnEmeraldChanged;
        EventBus.OnCrownCountChanged += OnCrownCountChanged;
    }

    [Header("LoginPanel")]

    public RectTransform LoginRegisterPanel;
    public RectTransform AvatarSelectionPanel;
    public TMP_Text LoginRegisterText;
    public Button GuestLoginButton;

    private bool login;
    public void ShowLoginPanel()
    {
        login = !login;
        if (login)
        {
            LoginRegisterPanel.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            LoginRegisterPanel.DOLocalMoveX(-900, 0.3f).OnComplete(() =>
            {
                GuestLoginButton.gameObject.SetActive(false);
                LoginRegisterPanel.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
                LoginRegisterPanel.localPosition = new Vector2(900, 0);
                LoginRegisterPanel.DOLocalMoveX(0, 0.3f);
                AvatarSelectionPanel.gameObject.SetActive(false);
                LoginButton.gameObject.SetActive(true);
                RegisterButton.gameObject.SetActive(false);
                LoginRegisterText.text = Language.Get("Menu_OrRegister");
            });
        }
        else
        {
            LoginRegisterPanel.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            LoginRegisterPanel.DOLocalMoveX(900, 0.3f).OnComplete(() =>
            {
                LoginRegisterPanel.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
                LoginRegisterPanel.localPosition = new Vector2(-900, 0);
                LoginRegisterPanel.DOLocalMoveX(0, 0.3f);
                AvatarSelectionPanel.gameObject.SetActive(true);
                LoginButton.gameObject.SetActive(false);
                RegisterButton.gameObject.SetActive(true);
                LoginRegisterText.text = Language.Get("Menu_OrLogin");
                GuestLoginButton.gameObject.SetActive(true);
            });
        }
    }

    private void OnLanguageChanged()
    {
        LevelText.text = Language.Get("LEVEL", UserData.User.Level);
    }

    private void OnQuickGame(QuickGameResponse obj)
    {
        if (string.IsNullOrEmpty(obj.RoomCode))
        {
            PopupManager.Instance.Show(Language.Get("CANT_QUICK_FIND_LOBBY"));
        }
        else
        {
            LobbyManager.JoinLobby(obj.RoomCode);
        }
    }

    private void OnEnable()
    {
        EventBus.OnLanguageChanged += OnLanguageChanged;
        WordBombNetworkManager.EventListener.OnQuickGame += OnQuickGame;
        WordBombNetworkManager.EventListener.OnLogin += OnLogin;
        WordBombNetworkManager.EventListener.OnUpdateDisplayName += UpdateDisplayName;
        WordBombNetworkManager.EventListener.OnLogout += OnLogout;
        WordBombNetworkManager.OnDisconnectedFromServer += OnDisconnected;
    }


    public void OnDisconnected()
    {
        UserData.LoggedIn = false;
    }


    private void OnDisable()
    {
        EventBus.OnLanguageChanged -= OnLanguageChanged;
        EventBus.OnExperienceChanged -= OnExperienceChanged;
        EventBus.OnLevelChanged -= OnLevelChanged;
        EventBus.OnCoinChanged -= OnCoinChanged;
        EventBus.OnEmeraldChanged -= OnEmeraldChanged;
        WordBombNetworkManager.EventListener.OnLogout -= OnLogout;
        WordBombNetworkManager.EventListener.OnQuickGame -= OnQuickGame;
        WordBombNetworkManager.EventListener.OnLogin -= OnLogin;
        WordBombNetworkManager.EventListener.OnUpdateDisplayName -= UpdateDisplayName;
        WordBombNetworkManager.OnDisconnectedFromServer += OnDisconnected;

    }
    private GameObject _languageObject;

    public void OnMenuSettingsClicked()
    {
        PopupManager.Instance.Show(new MenuSettingsPopup()
        {

        });
    }


    private void OnEmeraldChanged(int obj)
    {
        EmeraldCountText.text = obj.ToString();
    }

    private void OnCoinChanged(int obj)
    {
        CollecttedLetterText.text = obj.ToString();
    }

    private void OnCrownCountChanged(int obj)
    {
        CrownCountText.text = obj.ToString();
    }


    private void ChangeName()
    {
        EnterInputPopup popup = new EnterInputPopup(Language.Get("CHANGE_USERNAME_POPUP"),
            UserData.User.UserName, "", true, 10);

        popup.OnSubmit += (st) =>
        {
            if (UserData.User.DisplayName == st)
                return;

            if (UserValidator.IsValidName(st))
            {
                WordBombNetworkManager.Instance.SendPacket(new UpdateDisplayNameRequest()
                {
                    DisplayName = st,
                });
            }
            else
            {
                PopupManager.Instance.Show(
                    new MessagePopup(
                        Language.Get("USERNAME_LENGTH_ERROR")));
            }
        };
        PopupManager.Instance.Show(popup);
    }

    public void OnSuggestWordClicked()
    {
        var input = new SuggestWordPopup();
        PopupManager.Instance.Show(input);

        input.OnSubmit += (string st, byte language) =>
        {
            if (st.Length < 2 || st.Length >= 20)
            {
                PopupManager.Instance.Show(Language.Get("SUGGEST_WORD_LENGTH_ERROR"));
                return;
            }

            var enCulture = CultureInfo.GetCultureInfo("en-US");
            var trCulture = CultureInfo.GetCultureInfo("tr-TR");

            var gues = st.ToUpper(language == 0 ? enCulture : trCulture);
            WordBombNetworkManager.SendWordSuggestion(gues, language);
        };
    }

    private void OnLevelChanged(int obj)
    {
        LevelText.text = Language.Get("LEVEL", obj);
        XPSlider.maxValue = UserData.User.MaxExperience;
        XPSlider.SetValueWithoutNotify(UserData.User.Experience);
    }

    private void OnExperienceChanged(float obj)
    {
        var xp = obj % 100;
        this.XPSlider.DOValue(xp, 0.2F).SetEase(Ease.OutBounce);
    }

    public void OpenLobbyBrowser()
    {
        LobbyBrowser.gameObject.SetActive(true);
        LobbyBrowser.Open();
    }
    public void CloseLobbyBrowser()
    {
        LobbyBrowser.gameObject.SetActive(false);
        LobbyBrowser.Close();
    }

    public RouletteBehaviour RouletteBehaviourTemplate;

    public void OnPickerWhellClicked()
    {
        Instantiate(RouletteBehaviourTemplate, LeaderboardContent);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            EnterCode();
        }
    }

    public void EnterCode()
    {
        var cheats = new EnterInputPopup(Language.Get("CODE"))
        {

        };

        cheats.OnInitialize += (manager, content) =>
        {
            var codeText = manager.InstantiateElement<PopupText>(content);
            codeText.Initialize(Language.Get("JOINDISCORD_FORCODE"));
            codeText.TextComponent.fontSize = 15;
        };


        PopupManager.Instance.Show(cheats);
        cheats.OnSubmit += (string st) =>
        {
            if (string.IsNullOrEmpty(st)) return;

            if (st.Length < 50)
            {
                WordBombNetworkManager.Instance.SendPacket(new CheatCodeRequest()
                {
                    Code = st
                });
            }
        };
    }

    public GameObject SignInPanel;
    public GameObject MenuPanel;

    #region MenuUI
    [Header("Menu UI")]

    public TMP_Text UserName;
    public Image ProfilePicture;
    public Button CreateLobbyButton;
    public Button EditNameButton;
    public Button LoginLobbyButton;
    public Button FindLobbyButton;
    public Button ChangeAvatarButton;
    public TMP_Text CrownCountText;
    public TMP_Text EmeraldCountText;
    public TMP_Text CollecttedLetterText;
    public Slider XPSlider;
    public TMP_Text LevelText;
    public GameObject LanguageSelectionTemplate;
    public Transform LanguageSelectionContent;
    public LobbyBrowser LobbyBrowser;
    public TMP_Text DisplayNameLabel;

    public LeaderboardController LeaderboardTemplate;
    public Transform LeaderboardContent;
    #endregion



    #region SignInUI

    public Button RegisterButton;
    public Button LoginButton;
    public TMP_InputField LoginAnonymousUserNameInputField;
    public Image AnonymousProfilePicture;
    public Button AnnymousChangeProfilePictureButton;

    public TMP_InputField PasswordInputField;
    private static string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static System.Random random = new System.Random();

    public void SetupSignInUICallbacks()
    {
        if (PlayerPrefs.HasKey(nameof(UserData.Password)))
        {
            LoginAnonymousUserNameInputField.text = UserData.Username;
            PasswordInputField.text = UserData.Password;
        }

        RegisterButton.onClick.AddListener(Register);
        LoginButton.onClick.AddListener(Login);
        AnonymousProfilePicture.sprite = AvatarManager.GetAvatar(UserData.User.AvatarId);
        AnnymousChangeProfilePictureButton.onClick.AddListener(ChangeAvatar);
    }

    public void TogglePasswordInputVisibility()
    {
        if (PasswordInputField.inputType == TMP_InputField.InputType.Password)
        {
            PasswordInputField.inputType = TMP_InputField.InputType.Standard;
        }
        else
        {
            PasswordInputField.inputType = TMP_InputField.InputType.Password;
        }
        PasswordInputField.ForceLabelUpdate();

    }


    public void ShowLeaderboard()
    {
        Instantiate(LeaderboardTemplate, LeaderboardContent);
    }

    private void OnDestroy()
    {
        EventBus.OnExperienceChanged -= OnExperienceChanged;
        EventBus.OnLevelChanged -= OnLevelChanged;
    }

    #endregion

}