using System;
using System.Collections;
using UnityEngine;


public class QuestionPopup : IPopup
{
    private string message;
    string okMessage;
    string cancelMessage;
    public QuestionPopup(string message)
    {
        this.message = message;
        this.okMessage = Language.Get("POPUP_YES");
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
        manager.InstantiateElement<PopupButton>(horizontal.Content).Initialize(okMessage, () =>
        {
            OnSubmit?.Invoke();
            manager.Hide(this);
        });
    }

    public void Update()
    {

    }

    public Action OnSubmit;
    public Action OnCancel;
}