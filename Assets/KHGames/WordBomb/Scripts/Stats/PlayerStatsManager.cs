
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using WordBombServer.Common.Packets.Response;

public class PlayerStatsManager : MonoBehaviour
{
    public DailyBonusView DailyBonusView;
    public Transform DailyBonusContent;
    public void OnEnable()
    {
        EventBus.OnLogin += OnLogin;
    }
    private void OnDisable()
    {
        EventBus.OnLogin -= OnLogin;
    }

    public void ShowStats() { 
        
    }

    private void OnLogin(LoginResponse data)
    {
        if (data.ClaimDay != 0)
        {
            var view = Instantiate(DailyBonusView, DailyBonusContent);
            
            view.OnClaimed += () =>
            {
                if (!string.IsNullOrEmpty(data.UnlockAvatar))
                {
                    CanvasUtilities.Instance.ShowNewAvatarUnlocked(AvatarManager.GetAvatarByName(data.UnlockAvatar));
                    UserData.User.UnlockedAvatars.Add(data.UnlockAvatar);
                }
            };
            for (int i = 0; i < data.ClaimDay; i++)
            {
                view.Days[i].transform.GetChild(view.Days[i].transform.childCount - 1).GetComponent<Image>().
                    color = Color.green;
                view.Days[i].GetComponent<CanvasGroup>().alpha = 1;
                view.Days[i].color = Color.white;
            }
        }
    }
}
