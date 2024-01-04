using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum SpriteTag
{
    ENGLISHICON,
    TURKISHICON,
}

[Serializable]
public class SpritePackage
{
    public Sprite Icon;
    public SpriteTag Tag;
}


[Serializable]
public struct GameModeData
{
    public string Name;
    public string Description;
    public Sprite Image;
}


public class CanvasUtilities : MonoBehaviour
{
    public Image _loadingImage;
    public static CanvasUtilities Instance;

    [SerializeField] private CanvasGroup _loader;
    [SerializeField] private float _fadeTime;
    [SerializeField] private TMP_Text _loaderText, _errorText;

    private TweenerCore<float, float, FloatOptions> _tween;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> _loadingTween;
    public RewardScreenPopup RewardScreenPopupTemplate;
    public NotificationPopup NotificationPopupTemplate;
    public NewAvatarUnlockedPopup NewAvatarUnlockedPopup;

    public TMP_Text IdText;

    [SerializeField]
    private List<SpritePackage> GlobalSprites = new List<SpritePackage>();

     public GameModeData[] GameModes;
    public static bool SimulatePause;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            SimulatePause = !SimulatePause;
        }
        IdText.text = SimulatePause ? "PAUSED" : GameSetup.Version;
    }
    public Sprite GetSprite(SpriteTag tag)
    {
        foreach (var item in GlobalSprites)
        {
            if (item.Tag == tag)
                return item.Icon;
        }
        return null;
    }

    public TMP_Text Log;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        IdText.text += GameSetup.Version;

        Toggle(false, instant: true);
        EventBus.OnLevelChanged += OnLevelChanged;
    }

    private void OnLevelChanged(int obj)
    {
        Instance.ShowNotification(Language.Get("LEVEL_UP", obj), 3);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        EventBus.OnLevelChanged -= OnLevelChanged;
        _tween?.Kill();
        _loadingTween?.Kill();
    }

    public void Toggle(bool on, string text = null, bool instant = false)
    {
        _loaderText.text = text;
        _loader.gameObject.SetActive(on);
        _tween?.Kill();
        _tween = _loader.DOFade(on ? 1 : 0, instant ? 0 : _fadeTime);
        _loadingTween?.Kill();
        _loadingTween = _loadingImage.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).
             SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
    }


    public void ShowNotification(string v, int time)
    {
        var notification = Instantiate(NotificationPopupTemplate, transform);
        StartCoroutine(notification.Initialize(v, time));
    }
    public void ShowNewAvatarUnlocked(Sprite avatar)
    {
        var pop = Instantiate(NewAvatarUnlockedPopup, transform);
        pop.Init(avatar);
    }
    public void ShowRewardScreen(RewardScreenModel model)
    {
        var panel = Instantiate(RewardScreenPopupTemplate, transform);
        panel.InitializeView(model);
    }

    public void ShowError(string error)
    {
        _errorText.text = error;
        _errorText.DOFade(1, _fadeTime).OnComplete(() => { _errorText.DOFade(0, _fadeTime).SetDelay(1); });
    }
}