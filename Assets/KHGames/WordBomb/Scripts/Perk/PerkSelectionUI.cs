using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using WordBombServer.Common.Packets.Response;
using System.Collections;
using System.Collections.Generic;

public class PerkSelectionUI : MonoBehaviour
{
    public Transform PerkContent;
    public PerkUIView PerkUIViewTemplate;

    public PerkScriptableObject Perks;

    [NonSerialized]
    public PerkData _selectedPerkData;
    public bool isPerkSelected;

    public Image TimerFillImage;

    private void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnStartCountdown += OnStartCountdown;
    }
    private void OnDisable()
    {
        TimerFillImage.DOKill();
        WordBombNetworkManager.EventListener.OnStartCountdown -= OnStartCountdown;
    }

    private void OnStartCountdown(StartCountdownResponse obj)
    {
        TimerFillImage.DOFillAmount(1, obj.Countdown);
        StartCoroutine(OnCountdownEnd(obj.Countdown));
    }
    public IEnumerator OnCountdownEnd(int countdown)
    {

        yield return new WaitForSeconds(countdown);
        if (!isPerkSelected)
        {
            SendSelectedPerkRequest(_selectedPerkData);
            Destroy(gameObject);
        }
    }


    public List<PerkUIView> _createdPerkUIviews = new List<PerkUIView>();
    void Start()
    {

        var randomizedDefaultPerks = Perks.DefaultPerks.OrderBy(t => Guid.NewGuid()).ToList();
        var selectedGoldenPerk = Perks.GoldenPerks[UnityEngine.Random.Range(0, Perks.GoldenPerks.Count - 1)];

        _selectedPerkData = randomizedDefaultPerks[0];

        CreatePerkUI(randomizedDefaultPerks[0]);
        CreatePerkUI(randomizedDefaultPerks[1]);
        CreatePerkUI(selectedGoldenPerk);

    }


    public void CreatePerkUI(PerkData perkData)
    {
        var view = Instantiate(PerkUIViewTemplate, PerkContent);
        _createdPerkUIviews.Add(view);
        view.PerkIcon.sprite = perkData.Icon;
        view.PerkTitle.text = Language.Get(perkData.TitleLocaleId);
        view.PerkDescription.text = Language.Get(perkData.DescriptionLocaleId);
        view.SetPrice(perkData.Price);


        var perk = perkData;
        view.OnClicked += () =>
        {
            if (!isPerkSelected)
            {
                foreach (var v in _createdPerkUIviews) {
                    if (view != v) {
                        v.transform.DOScale(0, 0.4F);
                    }
                }
                isPerkSelected = true;
                _selectedPerkData = perk;
                view.Outline.enabled = true;
                if (UserData.User.EmeraldCount >= perk.Price)
                {
                    SendSelectedPerkRequest(perk);
                    Destroy(gameObject, 1);
                }
            }
        };
    }
    public void SendSelectedPerkRequest(PerkData perk)
    {
        WordBombNetworkManager.EventListener.SelectPerk((int)perk.PerkType);
    }
}
