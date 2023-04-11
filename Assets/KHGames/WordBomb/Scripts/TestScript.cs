using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{

    public LobbyGiftController _template;
    public Transform _content;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(_template, _content);

            FindObjectOfType<LobbyGiftController>().AddQueue(new LobbyGiftQueue()
            {
                Owner = "Anon"+UnityEngine.Random.Range(1000,9999),
                AvatarId = UnityEngine.Random.Range(0, 10)
            });
        }
    }

    private void WinScreenTest()
    {
        WinScreenUIController winScreen = null;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            winScreen.AddStat(new WinScreenOwnedStat()
            {
                Owner = "Hector",
                Stat = "En Yüksek Puan",
                Value = "252"
            });
            winScreen.AddStat(new WinScreenOwnedStat()
            {
                Owner = "Keuctor",
                Stat = "En Yüksek Kombo",
                Value = "5x"
            });
            winScreen.AddStat(new WinScreenOwnedStat()
            {
                Owner = "Hector",
                Stat = "En Uzun Kelime",
                Value = "Memleketli"
            });

            winScreen.SetStats(new WinScreenDetails()
            {

                EarnedCoin = 25,
                EarnedEmerald = 25,
                EarnedXP = 25,
                FirstPlaceAvatar = AvatarManager.GetAvatar(1),
                FirstPlaceOwner = "Hector",
                FirstPlaceScore = "252"
            });
            winScreen.ShowWinScreen(10);
        }
    }

}
