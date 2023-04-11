using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenPlayerView : MonoBehaviour
{

    [SerializeField]
    private Sprite[] _highScorePlaceSprites;

    [Header("UI")]
    [SerializeField]
    private Image _playerIcon;

    [SerializeField]
    private TMP_Text _playerNameText, _playerScoreTitleText, _playerScoreValueText;

    [SerializeField]
    private Image _highScoreplaceIcon;

    public void SetView(string name,int order, int score, Sprite playerIcon) {
        this._playerNameText.text = name;
        this._highScoreplaceIcon.sprite = _highScorePlaceSprites[order];
        this._playerScoreTitleText.text = Language.Get("SCORE");
        this._playerScoreValueText.text = score.ToString();
        this._playerIcon.sprite = playerIcon;
    }
    
}
