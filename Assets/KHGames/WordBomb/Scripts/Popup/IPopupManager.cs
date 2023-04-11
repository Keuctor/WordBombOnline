using System.Collections;
using UnityEngine;

public interface IPopupManager
{
    T GetElement<T>() where T : PopupElement;
    T InstantiateElement<T>(Transform content) where T : PopupElement;

    void Show(IPopup popup);
    void Hide(IPopup popup);
}
