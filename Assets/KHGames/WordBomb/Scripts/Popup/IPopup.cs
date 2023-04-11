using System.Collections;
using UnityEngine;

public interface IPopup
{
    void Initialize(IPopupManager manager, Transform content);
    void Cleanup();
    void Update();
}

