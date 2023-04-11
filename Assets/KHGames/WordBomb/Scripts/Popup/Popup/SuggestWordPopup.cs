using System;
using UnityEngine;

public class SuggestWordPopup : IPopup
{
    public Action<string, byte> OnSubmit;

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit?.Invoke(input.Text, enLanguage.Toggle.isOn ? (byte)0 : (byte)1);
            manager.Hide(this);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            manager.Hide(this);
        }
    }

    PopupInput input;
    PopupToggle enLanguage;

    IPopupManager manager;
    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager = manager;
        manager.InstantiateElement<PopupText>(content).Initialize(
        Language.Get("SUGGEST_WORD"), TMPro.TextAlignmentOptions.Center);


        input = manager.InstantiateElement<PopupInput>(content);
        input.Initialize("", "");
        input.Activate();

        var languageToggleGroup = manager.InstantiateElement<PopupToggleGroup>(content);


        enLanguage = manager.InstantiateElement<PopupToggle>(languageToggleGroup.Content);
        var trLanguage = manager.InstantiateElement<PopupToggle>(languageToggleGroup.Content);


        enLanguage.Text.text = Language.Get("LANGUAGE_ENGLISH");
        trLanguage.Text.text = Language.Get("LANGUAGE_TURKISH");


        enLanguage.Toggle.group = languageToggleGroup.ToggleGroup;
        trLanguage.Toggle.group = languageToggleGroup.ToggleGroup;

        var selectedLanguage = UserData.UILanguage;
        enLanguage.Toggle.isOn = selectedLanguage == 0 || selectedLanguage == 1;
        trLanguage.Toggle.isOn = selectedLanguage == 2;


        var horizontalLayout = manager.InstantiateElement<PopupHorizontalLayout>(content);
        manager.InstantiateElement<PopupButton>(horizontalLayout.Content).Initialize(Language.Get("POPUP_SEND"), () =>
        {
            OnSubmit?.Invoke(input.Text, enLanguage.Toggle.isOn ? (byte)0 : (byte)1);
            manager.Hide(this);
        });
        manager.InstantiateElement<PopupButton>(horizontalLayout.Content).Initialize(Language.Get("POPUP_CANCEL"), () =>
        {
            manager.Hide(this);
        });
    }

    public void Cleanup()
    {
    }
}
