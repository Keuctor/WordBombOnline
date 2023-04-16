using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class SettingsPopup : IPopup
{
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
    IPopupManager manager;

    public Action OnVolumeChanged;

    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager = manager;

        manager.InstantiateElement<PopupText>(content).Initialize(
            Language.Get("SETTINGS_SETTINGS"), TMPro.TextAlignmentOptions.Center);
        manager.InstantiateElement<PopupText>(content).Initialize(
            Language.Get("SETTINGS_SOUND"));
        manager.InstantiateElement<PopupSlider>(content).Initialize(
            UserData.SFXVolume, (newSfxVolume =>
            {
                UserData.SFXVolume = newSfxVolume;
                EventBus.OnVolumeChanged?.Invoke();
            }));


        manager.InstantiateElement<PopupText>(content).Initialize(
           Language.Get("SETTINGS_BOMB_SOUND"));

        manager.InstantiateElement<PopupSlider>(content).Initialize(UserData.BombTickingVolume,
          (newBombTickingVolume) =>
          {
              UserData.BombTickingVolume = newBombTickingVolume;
              EventBus.OnVolumeChanged?.Invoke();
          });

        var horizontalLayout = manager.InstantiateElement<PopupHorizontalLayout>(content);
        manager.InstantiateElement<PopupButton>(horizontalLayout.Content).Initialize(Language.Get("POPUP_OK"), () =>
        {
            manager.Hide(this);
        });
    }
}