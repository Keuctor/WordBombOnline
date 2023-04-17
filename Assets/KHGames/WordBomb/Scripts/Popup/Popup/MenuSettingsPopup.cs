

using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using WordBombServer.Common.Packets.Request;

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
            popup.OnSubmit += () => {
                WordBombNetworkManager.Instance.SendPacket(new LogoutRequest());
                manager.Hide(this);
            };
            PopupManager.Instance.Show(popup);
        });
        

        langEN.Toggle.isOn = UserData.UILanguage == 0;
        langTR.Toggle.isOn = UserData.UILanguage == 2;

        langTR.Toggle.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                UserData.UILanguage = 2;
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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSubmit?.Invoke();
            manager.Hide(this);
        }
    }

    public Action OnSubmit;
}