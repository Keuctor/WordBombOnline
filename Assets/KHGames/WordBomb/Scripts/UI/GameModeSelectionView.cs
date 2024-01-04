using System;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelectionView : MonoBehaviour
{
    public GameModeView GameViewTemplate;
    public Transform ViewContent;

    public Action<int> OnGameModeSelected;
    public Button CloseButton;

    private void Start()
    {
        CloseButton.onClick.AddListener(OnClose);
        for (int i = 0; i < CanvasUtilities.Instance.GameModes.Length; i++)
        {
            var view = Instantiate(GameViewTemplate, ViewContent);
            var mode = CanvasUtilities.Instance.GameModes[i];
            view.NameText.text = Language.Get(mode.Name);
            view.DetailText.text = Language.Get(mode.Description);
            view.ModeImage.sprite = mode.Image;
            var index = i;
            view.SelectModeButton.onClick.AddListener(() => { OnGameModeSelected?.Invoke(index); });
        }
    }

    private void OnClose()
    {
        Destroy(gameObject);
    }
}