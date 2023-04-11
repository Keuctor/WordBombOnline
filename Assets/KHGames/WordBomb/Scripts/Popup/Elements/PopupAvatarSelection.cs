using DG.Tweening;
using ilasm.WordBomb;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class PopupAvatarSelection : PopupElement
{
    public InstantiateTemplate<PopupAvatarSelectionItem> Item;
    public int SelectedIndex;
    private PopupAvatarSelectionItem selectedAvatarView;

    public void Start()
    {
        var avatars = AvatarManager.Avatars;
        SelectedIndex = UserData.User.AvatarId;
        for (int i = 0; i < AvatarManager.Avatars.Count; i++)
        {
            if (PlayerPrefs.HasKey(avatars[i].Name) || i < 6)
            {
                var view = Item.Instantiate();
                view.Avatar.sprite = avatars[i].Sprite;

                view.transform.localScale = Vector3.zero;

                if (SelectedIndex == i)
                {
                    view.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
                    view.Outline.effectColor = Color.green;
                    selectedAvatarView = view;
                }
                else
                {
                    view.transform.DOScale(0.9f, 0.4f).SetEase(Ease.OutBounce);
                    view.Outline.effectColor = Color.white;
                }

                int currentIndex = i;
                view.Button.onClick
                    .AddListener(() =>
                    {
                        if (selectedAvatarView != null)
                        {
                            selectedAvatarView.transform.DOScale(0.9f, 0.4f).SetEase(Ease.Flash);
                            selectedAvatarView.Outline.effectColor = Color.white;
                        }

                        selectedAvatarView = view;

                        selectedAvatarView.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
                        selectedAvatarView.Outline.effectColor = Color.green;
                        SelectedIndex = currentIndex;
                    });
            }
        }
    }
}