using System;
using UnityEngine;

public class CreateLobbyPopup : IPopup
{
    private string message;
    string createLobby;
    string cancelMessage;
    public CreateLobbyPopup(string message)
    {
        this.message = message;
        this.createLobby = "CREATE";
        this.cancelMessage =  Language.Get("POPUP_NO");
    }
    public void Cleanup()
    {
    }
    public void Initialize(IPopupManager manager, Transform content)
    {
        manager.InstantiateElement<PopupText>(content).Initialize(message);

        var horizontal = manager.InstantiateElement<PopupHorizontalLayout>(content);
        
        manager.InstantiateElement<PopupButton>(horizontal.Content).Initialize(cancelMessage, () =>
        {
            OnCancel?.Invoke();
            manager.Hide(this);
        });
    }
    public Action OnSubmit;
    public Action OnCancel;

    public void Update() { 
        
    }
}