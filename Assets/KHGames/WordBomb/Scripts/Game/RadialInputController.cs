using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;


public class LineConnection
{
    public int Index;
    public Vector2 BeginPos;
    public Vector2 EndPos;
    public UILineRenderer LineRenderer;
    public RadialLetterView View;
}

public class RadialInputController : MonoBehaviour
{
    public UILineRenderer UILineRendererTemplate;
    public Transform LineRendererContent;
    public RadialLetterView LetterViewTemplate;
    public Transform LetterViewContent;

    public Canvas Canvas;

    public static RadialInputController Instance;

    public CanvasGroup Group;

    public Image DiamondIcon;

    private List<RadialLetterView> _letters = new List<RadialLetterView>();

    public GameObject[] FillObjects;
    
    
    private RadialLetterView _startLetter;

    private List<LineConnection> _lineConnections
        = new List<LineConnection>();

    public Camera Cam;
    public TMP_Text OutputText;
    
    public TMP_Text InfoText;

    public Button RefreshButton;
    
    public Image Background;
    public string Output;
    public bool Sent;


    public Image border;

    public void SetInteractable(bool interactable)
    {
        Background.gameObject.SetActive(interactable);
        Group.interactable = interactable;
        Group.alpha = interactable ? 1 : 0.75f;
        border.color = interactable ? new Color(0.9f, 0, 1, 1) : new Color(0.2f,0.2f,0.2f,1);
    }

    public void SetText(string text)
    {
        for (int x = 0; x < _letters.Count; x++)
        {
            var target = _letters[x];
            target.Background.color = _letters[x].BackgroundPointerExitColor;
            target.Text.color = _letters[x].TextPointerExitColor;
        }

        for (int i = 0; i < text.Length; i++)
        {
            for (int x = 0; x < _letters.Count; x++)
            {
                var target = _letters[x];
                if (target.Letter[0] == text[i])
                {
                    target.Background.color = _letters[x].BackgroundPointerEnterColor;
                    target.Text.color = _letters[x].TextPointerEnterColor;
                    target.Background.rectTransform.DOShakeScale(0.5f, Vector3.one * 0.25f);
                    break;
                }
            }
        }

        OutputText.text = text;
        Output = text;
       
        SetupFillObjects();
    }

    private void Awake()
    {
        Cam = FindObjectOfType<Camera>();
        Canvas.worldCamera = Cam;
        Instance = this;
    }

    public void SetupFillObjects()
    {
        for (int i = 0; i < FillObjects.Length; i++)
        {
            if(i<_lineConnections.Count)
                FillObjects[i].gameObject.SetActive(true);
            else 
                FillObjects[i].gameObject.SetActive(false);
        }
    }

    public void ShowLetters(string letters)
    {
        Clear();
        foreach (Transform t in LetterViewContent)
            Destroy(t.gameObject);

        _letters.Clear();

        var arr = letters.ToCharArray().OrderBy(t => Guid.NewGuid()).ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            var c = arr[i];
            var letter = Instantiate(LetterViewTemplate, LetterViewContent);
            letter.Letter = c + "";
            letter.Index = i;
            letter.transform.localScale = Vector3.zero;
            letter.transform.DOScale(Vector3.one, 0.3f);
            _letters.Add(letter);
        }

      

        StartCoroutine(LetterViewContent.GetComponent<RadialView>().RefreshCoroutine());
    }

    public void Randomize()
    {
        Random random = new Random();
        List<Transform> childList = new List<Transform>();

        foreach (Transform child in LetterViewContent)
        {
            childList.Add(child);
        }

        childList = childList.OrderBy(t => random.Next()).ToList();

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].SetSiblingIndex(i);
        }

        StartCoroutine(LetterViewContent.GetComponent<RadialView>().RefreshCoroutine());
        SoundManager.PlayAudio(Sounds.Click);
    }

    public void OnClicked(RadialLetterView view)
    {
        Clear();
        SoundManager.PlayAudio(Sounds.BLUP,0.5f);
        var connection = new LineConnection()
        {
            BeginPos = ScreenToCanvasPosition(Cam.WorldToScreenPoint(view.GetComponent<RectTransform>().position)),
            Index = view.Index,
            EndPos = ScreenToCanvasPosition(Input.mousePosition),
            LineRenderer = Instantiate(UILineRendererTemplate, LineRendererContent),
            View = view,
        };
        _lineConnections.Add(connection);
        connection.LineRenderer.transform.SetSiblingIndex(connection.LineRenderer.transform.GetSiblingIndex() - 2);
        view.Background.color = view.BackgroundPointerEnterColor;
        view.Text.color = view.TextPointerEnterColor;
        view.Background.rectTransform.DOKill();
        view.Background.rectTransform.localScale = (Vector3.one);
        view.Background.rectTransform.DOShakeScale(0.5f, Vector3.one * 0.5f);
        SetupFillObjects();
    }

    private void Clear()
    {
        for (int i = 0; i < _lineConnections.Count; i++)
        {
            var v = _lineConnections[i].View;
            v.Background.color = v.BackgroundPointerExitColor;
            v.Text.color = v.TextPointerExitColor;
            Destroy(_lineConnections[i].LineRenderer.gameObject);
        }
        _lineConnections.Clear();
        SetupFillObjects();
    }


    public void OnPointerEnter(RadialLetterView view)
    {
        if (_lineConnections.Count == 0)
            return;

        if (_lineConnections.Any(t => t.Index == view.Index))
        {
            var line = _lineConnections.FirstOrDefault(t => t.Index == view.Index);
            if (_lineConnections.IndexOf(line) == _lineConnections.Count - 2)
            {
                var lastElement = _lineConnections[^1];
                Destroy(lastElement.LineRenderer.gameObject);
                lastElement.View.Background.color = lastElement.View.BackgroundPointerExitColor;
                lastElement.View.Text.color = lastElement.View.TextPointerExitColor;
                _lineConnections.Remove(lastElement);
            }

            return;
        }

        var lastLine = _lineConnections[_lineConnections.Count - 1];

        var rect = view.GetComponent<RectTransform>();
        lastLine.EndPos = ScreenToCanvasPosition(Cam.WorldToScreenPoint(rect.position));


        var connection = new LineConnection()
        {
            BeginPos = ScreenToCanvasPosition(Cam.WorldToScreenPoint(view.GetComponent<RectTransform>().position)),
            Index = view.Index,
            EndPos = ScreenToCanvasPosition(Input.mousePosition),
            LineRenderer = Instantiate(UILineRendererTemplate, LineRendererContent),
            View = view
        };
        connection.LineRenderer.transform.SetSiblingIndex(connection.LineRenderer.transform.GetSiblingIndex() - 2);
        _lineConnections.Add(connection);

        view.Background.color = view.BackgroundPointerEnterColor;
        view.Text.color = view.TextPointerEnterColor;
        view.Background.rectTransform.DOShakeScale(0.5f, Vector3.one * 0.5f);
        
        SoundManager.PlayAudio(Sounds.BLUP,_lineConnections.Count*0.5f);
        SetupFillObjects();
    }

    public void OnPointerExit(RadialLetterView radialLetterView)
    {
    }

    private void Update()
    {
        if (!TurnController.IsMyTurn)
            return;
        
        var text = "";
        for (int i = 0; i < _lineConnections.Count; i++)
        {
            var connection = _lineConnections[i];

            text += connection.View.Letter;

            if (i == _lineConnections.Count - 1)
            {
                connection.EndPos = ScreenToCanvasPosition(Input.mousePosition);
            }

            connection.LineRenderer.startPoint = connection.BeginPos;
            connection.LineRenderer.endPoint = connection.EndPos;
            connection.LineRenderer.SetVerticesDirty();
        }

        OutputText.text = text;
        Output = text;

        if (Application.isMobilePlatform)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                Send();
                Clear();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Send();
                Clear();
            }
        }
    }

    private void Send()
    {
        Sent = true;
    }

    public Vector3 ScreenToCanvasPosition(Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
            screenPosition.y / Screen.height,
            0);
        return ViewportToCanvasPosition(viewportPosition);
    }

    public Vector3 ViewportToCanvasPosition(Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = Canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}