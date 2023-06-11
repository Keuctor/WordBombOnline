using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


[Serializable]
public class PlayerData
{
    public string UserName;
    public string DisplayName;
    public int TotalLetters;

    public int EmeraldCount;
    public int CrownCount;

    public List<string> UnlockedAvatars = new List<string>();
    public int Level
    {
        get => ((int)(Experience < 100 ? 1 : ((Experience / 100) + 1)));
    }

    public float Experience;
    public float MaxExperience = 100;
    public short AvatarId;
}

public static class UserData
{
    private static PlayerData _user;

    public static PlayerData User
    {
        get
        {
            if (_user == null)
            {
                _user = new PlayerData();
            }
            return _user;
        }
    }

    public static bool LoggedIn;
    public static bool LogOut;

    public static string Password
    {
        get => PlayerPrefs.GetString(nameof(Password), UnityEngine.Random.Range(10000, 99999).ToString());
        set => PlayerPrefs.SetString(nameof(Password), value);
    }

    public static string Username
    {
        get => PlayerPrefs.GetString(nameof(Username));
        set => PlayerPrefs.SetString(nameof(Username), value);
    }

    public static bool IsLobbyPrivate
    {
        get => PlayerPrefs.GetInt(nameof(IsLobbyPrivate), 0) == 1;
        set => PlayerPrefs.SetInt(nameof(IsLobbyPrivate), value ? 1 : 0);
    }

    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat(nameof(SFXVolume), 0.5F);
        set => PlayerPrefs.SetFloat(nameof(SFXVolume), value);
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(nameof(MusicVolume), 0.5F);
        set => PlayerPrefs.SetFloat(nameof(MusicVolume), value);
    }

    public static float BombTickingVolume
    {
        get => PlayerPrefs.GetFloat(nameof(BombTickingVolume), 0.25F);
        set => PlayerPrefs.SetFloat(nameof(BombTickingVolume), value);
    }

    public static byte UILanguage
    {
        get => (byte)PlayerPrefs.GetInt(nameof(UILanguage), CultureInfo.CurrentCulture.Name.Contains("tr") ? 2 : 0);
        set => PlayerPrefs.SetInt(nameof(UILanguage), value);
    }
    public static byte GameLanguage
    {
        get => (byte)PlayerPrefs.GetInt(nameof(GameLanguage), CultureInfo.CurrentCulture.Name.Contains("tr") ? 1 : 0);
        set => PlayerPrefs.SetInt(nameof(GameLanguage), value);
    }

    public static bool GamePrivate
    {
        get => (byte)PlayerPrefs.GetInt(nameof(GamePrivate), 0) == 1;
        set => PlayerPrefs.SetInt(nameof(GamePrivate), value ? 1 : 0);
    }

    public static byte GameMode
    {
        get => (byte)PlayerPrefs.GetInt(nameof(GameMode), 0);
        set => PlayerPrefs.SetInt(nameof(GameMode), value);
    }
    public static byte GameSpeed
    {
        get => (byte)PlayerPrefs.GetInt(nameof(GameSpeed), 1);
        set => PlayerPrefs.SetInt(nameof(GameSpeed), value);
    }

    public static void GiveExperience(float xp)
    {
        if (xp <= 0)
            return;

        var level = UserData.User.Level;
        User.Experience += xp;
        EventBus.OnExperienceChanged?.Invoke(User.Experience);
        if (UserData.User.Level != level)
        {
            EventBus.OnLevelChanged?.Invoke(UserData.User.Level);
        }
    }

    public static void GiveEmerald(int v)
    {
        User.EmeraldCount += v;
        EventBus.OnEmeraldChanged?.Invoke(User.EmeraldCount);
    }
    public static void GiveCrown()
    {
        User.CrownCount += 1;
        EventBus.OnCrownCountChanged?.Invoke(User.CrownCount);
    }

    public static void GiveCoin(int v)
    {
        User.TotalLetters += v;
        EventBus.OnCoinChanged?.Invoke(User.TotalLetters);
    }
    public static RewardScreenModel RewardEarned = null;
}