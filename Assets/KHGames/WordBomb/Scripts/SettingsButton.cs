using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
   
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnSettingsClicked);
    }

    private void OnSettingsClicked()
    {
        PopupManager.Instance.Show(new SettingsPopup());
    }
}
