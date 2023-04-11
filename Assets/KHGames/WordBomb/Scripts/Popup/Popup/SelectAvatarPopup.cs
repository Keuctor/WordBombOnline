using System;
using UnityEngine;

public class SelectAvatarPopup : IPopup
{
    private string Title;
    private string OkText;
    private string CancelText;

    public SelectAvatarPopup()
    {
        this.Title = Language.Get("CHANGE_AVATAR");
        this.OkText = Language.Get("POPUP_OK");
        this.CancelText = Language.Get("POPUP_CANCEL");
    }

    IPopupManager manager;
    PopupAvatarSelection avatarSelection;

    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager = manager;
        manager.InstantiateElement<PopupText>(content)
           .Initialize(Title);

        avatarSelection = manager.InstantiateElement<PopupAvatarSelection>(content);

        var horizontal = manager.InstantiateElement<PopupHorizontalLayout>(content);
        var cancelButton = manager.InstantiateElement<PopupButton>(horizontal.Content);
        var okButton = manager.InstantiateElement<PopupButton>(horizontal.Content);
        okButton.Initialize(OkText, () =>
        {
            OnSubmit?.Invoke(avatarSelection.SelectedIndex);
            manager.Hide(this);
        });
        cancelButton.Initialize(CancelText, () =>
        {
            OnCancel?.Invoke();
            manager.Hide(this);
        });
    }

    public void Cleanup()
    {
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit?.Invoke(avatarSelection.SelectedIndex);
            manager.Hide(this);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancel?.Invoke();
                manager.Hide(this);
            }
        }
    }


    public Action<int> OnSubmit;
    public Action OnCancel;
}
