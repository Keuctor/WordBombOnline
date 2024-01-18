using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct LobbySettingChanges
{
    public byte Language;
    public byte GameMode;
    public byte Speed;
    public bool IsPrivate;
}

public class LobbySettingsPopup : IPopup
{
    private byte selectedLanguage;
    private int selectedGameMode;
    private int selectedSpeed;

    public LobbySettingsPopup(byte language, int gameMode, int selectedSpeed)
    {
        this.selectedLanguage = language;
        this.selectedGameMode = gameMode;
        this.selectedSpeed = selectedSpeed;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            manager.Hide(this);
        }
    }

    public void Cleanup()
    {
    }

    public Action<LobbySettingChanges> OnSubmit;
    IPopupManager manager;
    PopupText gameModInfo;
    PopupToggle gameMode1;
    PopupToggle gameMode2;
    PopupToggle gameMode3;
    PopupToggle gameMode4;
    PopupToggleGroup gameModeToggleGroup;

    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager = manager;

        manager.InstantiateElement<PopupText>(content).Initialize(
            Language.Get("WORDS"), TMPro.TextAlignmentOptions.Center);

        var languageToggleGroup = manager.InstantiateElement<PopupToggleGroup>(content);

        Dictionary<byte, Toggle> toggles = new Dictionary<byte, Toggle>();
        for (int i = 0; i < MatchmakingService.Languages.Length; i++)
        {
            var language = MatchmakingService.Languages[i];
            var toggle  = manager.InstantiateElement<PopupToggle>(languageToggleGroup.Content);
            toggle.Text.text = Language.Get(language.LocalizeName);
            toggle.Toggle.group = languageToggleGroup.ToggleGroup;
            toggle.name = language.Id.ToString();
            toggles.Add(language.Id, toggle.Toggle);
        }
        languageToggleGroup.ToggleGroup.SetAllTogglesOff();
        toggles[selectedLanguage].SetIsOnWithoutNotify(true);
        
     

        if (MatchmakingService.CurrentRoom.GameType == 0)
        {
            manager.InstantiateElement<PopupText>(content).Initialize(
                Language.Get("GAMEMODE"));

            gameModeToggleGroup = manager.InstantiateElement<PopupToggleGroup>(content);


            gameMode1 = manager.InstantiateElement<PopupToggle>(gameModeToggleGroup.Content);
            gameMode2 = manager.InstantiateElement<PopupToggle>(gameModeToggleGroup.Content);
            gameMode3 = manager.InstantiateElement<PopupToggle>(gameModeToggleGroup.Content);

            gameModInfo = manager.InstantiateElement<PopupText>(content);

            gameMode1.Text.text = Language.Get("GAMEMODE_RANDOM");
            gameMode2.Text.text = Language.Get("GAMEMODE_CONTINUOUS");
            gameMode3.Text.text = Language.Get("GAMEMODE_LENGTH_LIMITED");


            gameMode1.Toggle.group = gameModeToggleGroup.ToggleGroup;
            gameMode2.Toggle.group = gameModeToggleGroup.ToggleGroup;
            gameMode3.Toggle.group = gameModeToggleGroup.ToggleGroup;


            manager.InstantiateElement<PopupText>(content).Initialize(
                Language.Get("GAMESPEED"));


            gameMode1.Toggle.isOn = selectedGameMode == 0;
            gameMode2.Toggle.isOn = selectedGameMode == 1;
            gameMode3.Toggle.isOn = selectedGameMode == 2;


            gameMode1.Toggle.onValueChanged.AddListener(RefreshGameModInfo);
            gameMode2.Toggle.onValueChanged.AddListener(RefreshGameModInfo);
            gameMode3.Toggle.onValueChanged.AddListener(RefreshGameModInfo);
        }

        var gameSpeedToggleGroup = manager.InstantiateElement<PopupToggleGroup>(content);
        var gameSpeed1 = manager.InstantiateElement<PopupToggle>(gameSpeedToggleGroup.Content);
        var gameSpeed2 = manager.InstantiateElement<PopupToggle>(gameSpeedToggleGroup.Content);
        var gameSpeed3 = manager.InstantiateElement<PopupToggle>(gameSpeedToggleGroup.Content);

        gameSpeed1.Toggle.group = gameSpeedToggleGroup.ToggleGroup;
        gameSpeed2.Toggle.group = gameSpeedToggleGroup.ToggleGroup;
        gameSpeed3.Toggle.group = gameSpeedToggleGroup.ToggleGroup;

        gameSpeed1.Text.text = Language.Get("SLOW");
        gameSpeed2.Text.text = Language.Get("NORMAL");
        gameSpeed3.Text.text = Language.Get("FAST");

        gameSpeed1.Toggle.isOn = selectedSpeed == 0;
        gameSpeed2.Toggle.isOn = selectedSpeed == 1;
        gameSpeed3.Toggle.isOn = selectedSpeed == 2;

        RefreshGameModInfo(false);

        var horizontalLayout = manager.InstantiateElement<PopupHorizontalLayout>(content);
        manager.InstantiateElement<PopupButton>(horizontalLayout.Content).Initialize(Language.Get("POPUP_OK"), () =>
        {
            byte gameMode = 0;
            if (MatchmakingService.CurrentRoom.GameType == 0)
            {
                gameMode = (byte)(gameMode1.Toggle.isOn ? 0 :
                    gameMode2.Toggle.isOn ? 1 :
                    gameMode3.Toggle.isOn ? 2 : 3);
            }

            var selectedToggle = languageToggleGroup.ToggleGroup.GetFirstActiveToggle();
            byte lang = selectedLanguage;
            if (byte.TryParse(selectedToggle.name,out byte toggleV))
            {
                lang = toggleV;
            }

            var changes = new LobbySettingChanges()
            {
                Language = lang,
                GameMode = gameMode,
                Speed = (byte)(gameSpeed1.Toggle.isOn ? 0 : gameSpeed2.Toggle.isOn ? 1 : 2),
                IsPrivate = MatchmakingService.CurrentRoom.IsPrivate
            };
            OnSubmit?.Invoke(changes);
            manager.Hide(this);
        });
        manager.InstantiateElement<PopupButton>(horizontalLayout.Content)
            .Initialize(Language.Get("POPUP_CANCEL"), () => { manager.Hide(this); });
    }


    private void RefreshGameModInfo(bool val)
    {
        if (MatchmakingService.CurrentRoom.GameType == 1)
            return;

        selectedGameMode = (gameMode1.Toggle.isOn ? 0 : gameMode2.Toggle.isOn ? 1 : gameMode3.Toggle.isOn ? 2 : 3);
        if (selectedGameMode == 0)
        {
            gameModInfo.Initialize(Language.Get("MODINFO_NORMAL"));
        }
        else if (selectedGameMode == 1)
        {
            gameModInfo.Initialize(Language.Get("MODINFO_CONTINUOUS"));
        }
        else if (selectedGameMode == 2)
        {
            gameModInfo.Initialize(Language.Get("MODINFO_COUNT"));
        }
        else if (selectedGameMode == 3)
        {
            gameModInfo.Initialize(Language.Get("MODINFO_IMAGES"));
        }
    }
}