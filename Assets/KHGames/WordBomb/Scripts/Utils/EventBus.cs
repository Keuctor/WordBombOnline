using System;


public static class EventBus
{
    public static Action OnBeforeGameStart;
    public static Action OnGameStart;
    public static Action<float> OnExperienceChanged;
    public static Action<int> OnLevelChanged;
    public static Action<int> OnCoinChanged;
    public static Action<int> OnEmeraldChanged;
    public static Action<int> OnCrownCountChanged;
    public static Action OnVolumeChanged;
    public static Action OnLanguageChanged;
}
