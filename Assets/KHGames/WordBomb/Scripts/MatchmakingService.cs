using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Globalization;

public static class MatchMakingConstants
{
    public const string LEVEL = "LEVEL";
    public const string NAME = "NAME";
    public const string AVATARID = "AVATARID";
}


public class GameLanguage
{
    public byte Id;
    public string LocalizeName;
    public CultureInfo CultureInfo;
}

public static class MatchmakingService
{
    //TODO DELETE THIS
    public static Lobby CurrentRoom;

    private static GameLanguage[] _languages;

    public static GameLanguage[] Languages
    {
        get
        {
            InitializeLanguages();
            return _languages;
        }
    }

    public static GameLanguage GetLanguage(byte id)
    {
        InitializeLanguages();
        for (int i = 0; i < _languages.Length; i++)
            if (_languages[i].Id == id)
                return _languages[i];
        return null;
    }

    private static void InitializeLanguages()
    {
        if (_languages == null || _languages.Length == 0)
        {
            _languages = new GameLanguage[5]
            {
                new GameLanguage()
                {
                    LocalizeName = "LANGUAGE_ENGLISH",
                    Id = 0,
                    CultureInfo = CultureInfo.GetCultureInfo("en-US")
                },
                new GameLanguage()
                {
                    LocalizeName = "LANGUAGE_TURKISH",
                    Id = 1,
                    CultureInfo = CultureInfo.GetCultureInfo("tr-TR")
                },
                new GameLanguage()
                {
                    LocalizeName = "LANG_FRENCH",
                    Id = 2,
                    CultureInfo =  CultureInfo.GetCultureInfo("fr-FR")
                },
                new GameLanguage()
                {
                    LocalizeName = "LANG_PORT",
                    Id = 3,
                    CultureInfo = CultureInfo.GetCultureInfo("pt-BR")
                },
                new GameLanguage()
                {
                    LocalizeName = "LANG_ESPANOL",
                    Id = 4,
                    CultureInfo = CultureInfo.GetCultureInfo("es-ES")
                },
            };
        }
    }
}