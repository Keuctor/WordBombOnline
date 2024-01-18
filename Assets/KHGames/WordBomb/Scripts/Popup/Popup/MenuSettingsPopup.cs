using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using WordBombServer.Common.Packets.Request;
using Object = UnityEngine.Object;

public class MenuSettingsPopup : IPopup
{
    public MenuSettingsPopup()
    {
        EventBus.OnLanguageChanged += RefreshUILanguage;
    }

    private void RefreshUILanguage()
    {
        langTR.Text.text = Language.Get("LANGUAGE_TURKISH");
        langEN.Text.text = Language.Get("LANGUAGE_ENGLISH");
        submit.TextComponent.text = Language.Get("POPUP_OK");
        title.Initialize(Language.Get("LANGUAGE"));
        logoutButton.TextComponent.text = Language.Get("LOGOUT");
    }

    public void Cleanup()
    {
        EventBus.OnLanguageChanged += RefreshUILanguage;
    }

    PopupToggle langTR;
    PopupToggle langEN;
    PopupButton submit;
    PopupText title;
    PopupButton logoutButton;


    private bool _shown;
    IPopupManager manager;

    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager = manager;
        title = manager.InstantiateElement<PopupText>(content);

        var toggleGroup = manager.InstantiateElement<PopupToggleGroup>(content);

        langTR = manager.InstantiateElement<PopupToggle>(toggleGroup.Content);
        langEN = manager.InstantiateElement<PopupToggle>(toggleGroup.Content);

        langEN.Toggle.group = toggleGroup.ToggleGroup;
        langTR.Toggle.group = toggleGroup.ToggleGroup;

        langEN.SetIcon(CanvasUtilities.Instance.GetSprite(SpriteTag.ENGLISHICON));
        langTR.SetIcon(CanvasUtilities.Instance.GetSprite(SpriteTag.TURKISHICON));


        logoutButton = manager.InstantiateElement<PopupButton>(content);
        logoutButton.Initialize(Language.Get("LOGOUT"), () =>
        {
            var popup = new QuestionPopup(Language.Get("RUSURE_LOGOUT"));
            popup.OnSubmit += () =>
            {
                WordBombNetworkManager.Instance.SendPacket(new LogoutRequest());
                manager.Hide(this);
            };
            PopupManager.Instance.Show(popup);
        });


        langEN.Toggle.isOn = UserData.UILanguage == 0;
        langTR.Toggle.isOn = UserData.UILanguage == 1;

        langTR.Toggle.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                UserData.UILanguage =  1;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2];
                EventBus.OnLanguageChanged?.Invoke();
            }
        });
        langEN.Toggle.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                UserData.UILanguage = 0;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                EventBus.OnLanguageChanged?.Invoke();
            }
        });


        var deleteMyAccount = this.manager.InstantiateElement<PopupButton>(content);
        deleteMyAccount.Initialize(Language.Get("DELETE_MY_ACCOUNT"), () =>
        {
            if (UserData.User.DisplayName.ToLower().Contains("keuctor")
                || UserData.User.UserName.ToLower().Contains("keuctor"))
            {
                PopupManager.Instance.Show(
                    "Malesef canım. Sen silemezsin.Burada benim kurallarım geçerli sjdsj");
                return;
            }

            var q = new QuestionPopup(Language.Get("ARE_YOU_SURE_DELETE"));
            q.OnSubmit += () =>
            {
                UserData.LogOut = true;
                UserData.LoggedIn = false;
                manager.Hide(this);
                WordBombNetworkManager.Instance.DeleteAccount();
            };
            PopupManager.Instance.Show(q);
        });


        var btnShowDevicePass = this.manager.InstantiateElement<PopupButton>(content);
        btnShowDevicePass.Initialize(Language.Get("SHOW_DEVICE_PASS"), () =>
        {
            btnShowDevicePass.gameObject.SetActive(false);
            var devicePass = manager.InstantiateElement<PopupText>(content);
            devicePass.Initialize(MenuController.DevicePassword);

            var copy = manager.InstantiateElement<PopupButton>(content);

            copy.Initialize(Language.Get("COPY"), () =>
            {
                GUIUtility.systemCopyBuffer = MenuController.DevicePassword;
                devicePass.Initialize(Language.Get("ROOMCODE_COPIED"));
                Object.Destroy(copy.gameObject);
            });
        });

        submit = manager.InstantiateElement<PopupButton>(content);
        submit.Initialize(Language.Get("POPUP_OK"), () =>
        {
            OnSubmit?.Invoke();
            manager.Hide(this);
        });

        RefreshUILanguage();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSubmit?.Invoke();
            manager.Hide(this);
        }
    }

    public Action OnSubmit;
}