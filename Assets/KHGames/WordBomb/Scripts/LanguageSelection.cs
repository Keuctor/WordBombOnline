using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageSelection : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _title;
    [SerializeField]
    private TMP_Text _continueText;


    [SerializeField]
    private Toggle[] _languageToggles;


    void UpdateUI()
    {
        _title.text = Language.Get("LANGUAGE");
        _continueText.text = Language.Get("POPUP_OK");
    }

    private byte _index;
    private void Start()
    {
        UpdateUI();

        _index = UserData.UILanguage;
        _languageToggles[_index].isOn = true;

        for (int i = 0; i < _languageToggles.Length; i++)
        {
            var n = i;
            _languageToggles[i].onValueChanged.AddListener((b) =>
            {
                _index = (byte)n;
                UserData.UILanguage = (byte)n;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[n];
                UpdateUI();
                EventBus.OnLanguageChanged?.Invoke();
            });
        }
    }
    public void Next()
    {
        Destroy(gameObject);
    }
}

