using System;
using System.Collections;
using UnityEngine;


public class MessagePopup : IPopup
{
    private string message;
    string okMessage;
    public MessagePopup(string message)
    {
        this.message = message;
        this.okMessage =  Language.Get("POPUP_OK");
    }
    public void Cleanup()
    {
    }

    IPopupManager manager;
    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager= manager;
        manager.InstantiateElement<PopupText>(content).Initialize(message);
        manager.InstantiateElement<PopupButton>(content).Initialize(okMessage, () =>
        {
            OnSubmit?.Invoke();
            manager.Hide(this);
        });
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            OnSubmit?.Invoke();
            manager.Hide(this);
        }
    }

    public Action OnSubmit;
}