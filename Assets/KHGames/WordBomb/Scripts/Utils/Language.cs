using UnityEngine.Localization.Settings;

public static class Language
{
    public static string Get(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("Localization",
            key);
    }

    public static string Get(string key, params object[] value)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("Localization",
            key, null, FallbackBehavior.UseProjectSettings, value);
    }
}