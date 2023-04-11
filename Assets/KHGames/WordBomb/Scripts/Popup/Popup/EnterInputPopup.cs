using System;
using UnityEngine;





public class EnterInputPopup : IPopup
{
    public string Title;
    public string OkText;
    public string CancelText;
    public string PlaceHolder;
    public string DefaultText;
    public short Price = -1;
    public EnterInputPopup(string title, string defaultText = "", string placeHolder = "", bool hasPrice = false, short price = -1)
    {
        this.Title = title;
        this.OkText = Language.Get("POPUP_OK");
        this.CancelText = Language.Get("POPUP_CANCEL");
        this.PlaceHolder = placeHolder;
        this.DefaultText = defaultText;
        if (hasPrice)
        {
            this.Price = price;
        }
    }
    public Action<string> OnSubmit;
    public Action OnCancel;
    private PopupInput input;
    private IPopupManager manager;
    public void Initialize(IPopupManager manager, Transform content)
    {
        this.manager = manager;
        var text = manager.InstantiateElement<PopupText>(content);
        text.Initialize(Title, TMPro.TextAlignmentOptions.Center);
        input = manager.InstantiateElement<PopupInput>(content);
        input.Initialize(DefaultText, PlaceHolder);
        var horizontal = manager.InstantiateElement<PopupHorizontalLayout>(content);
        var cancelButton = manager.InstantiateElement<PopupButton>(horizontal.Content);


        PopupButton okButton = null;

        if (this.Price > 0)
        {
            var button = manager.InstantiateElement<PopupButtonWithPrice>(horizontal.Content);
            button.PriceText.text = this.Price.ToString();
            okButton = button;
        }
        else
        {
            okButton = manager.InstantiateElement<PopupButton>(horizontal.Content);
        }

        okButton.Initialize(OkText, () =>
        {
            OnSubmit?.Invoke(input.Text);
            manager.Hide(this);
        });
        cancelButton.Initialize(CancelText, () =>
        {
            OnCancel?.Invoke();
            manager.Hide(this);
        });

        

        input.Activate();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit?.Invoke(input.Text);
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

    public void Cleanup()
    {

    }
}