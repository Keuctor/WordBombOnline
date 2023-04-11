using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Response;

public class XPController : MonoBehaviour
{
    public static int CollectedXP;

    private void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnUpdateUserData += OnUpdateData;
    }
    private void OnUpdateData(UpdateUserData obj)
    {
        if (GameSetup.LocalPlayerId == obj.Id)
        {
            CollectedXP += obj.XP;
        }
    }
    private void Start()
    {
        CollectedXP = 0;
    }
}
