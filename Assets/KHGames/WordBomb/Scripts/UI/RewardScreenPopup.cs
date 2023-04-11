using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    [Serializable]
    public class RewardScreenModel
    {
        public int CrownCount;
        public int EmeraldCount;
        public string Name;
        public float RewardedXP;
        public float TotalXp;
        public int Position;
        public int TotalPlayers;
        public int WordScore;
    }
    public class RewardScreenPopup : MonoBehaviour
    {
        public TMP_Text CrownLabel;
        public TMP_Text EmeraldLabel;
        public TMP_Text NameLabel;

        public TMP_Text XPLabel;
        public Button ContinueButton;
        public TMP_Text Description;
        public TMP_Text WordScore;
        public Image PlayerIcon;
        public Slider XPSlider;
        public TMP_Text LevelText;
        private void Awake()
        {
            transform.localScale = Vector3.zero;
        }
        public void InitializeView(RewardScreenModel model)
        {
            transform.DOScale(1, 0.5f);
            XPSlider.minValue = 0;
            XPSlider.maxValue = UserData.User.MaxExperience;
            XPSlider.value = UserData.User.Experience;
            LevelText.text = Language.Get("LEVEL", (short)(UserData.User.Experience < 100 ? 1 : ((UserData.User.Experience / 100) + 1)));
            StartCoroutine(XPCoroutine(model));
            PlayerIcon.sprite = AvatarManager.GetAvatar(UserData.User.AvatarId);
            Description.text =  Language.Get("GAME_END_PLAYER_LEADERBOARD",
                model.TotalPlayers,model.Position);
            CrownLabel.text = model.CrownCount.ToString();
            EmeraldLabel.text = model.EmeraldCount.ToString();
            NameLabel.text = model.Name;
            XPLabel.text = "+" + model.RewardedXP.ToString() + "XP";
            ContinueButton.onClick.AddListener(OnContinueClicked);
            WordScore.text = model.WordScore.ToString();
        }

        private IEnumerator XPCoroutine(RewardScreenModel model)
        {
            yield return new WaitForSeconds(0.5f);
            XPSlider.DOValue(UserData.User.Experience + model.RewardedXP, 2f);
        }

        private void OnContinueClicked()
        {
            Destroy(gameObject);
        }
    }