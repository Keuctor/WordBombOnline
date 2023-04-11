using DG.Tweening;
using ilasm.WordBomb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour, IPopupManager
{
    public static PopupManager Instance;
    public static bool Active { get; set; }
    void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private List<PopupElement> Elements = new List<PopupElement>();

    public InstantiateTemplate<PopupContainer> PopupContainerTemplate;

    Dictionary<IPopup, GameObject> createdPopups = new Dictionary<IPopup, GameObject>();

    private IPopup _activePopup;

    public T GetElement<T>() where T : PopupElement
    {
        foreach (var element in Elements)
        {
            if (element.GetType() == typeof(T))
                return element as T;
        }
        return null;
    }

    public T InstantiateElement<T>(Transform content) where T : PopupElement
    {
        return Instantiate(GetElement<T>(), content);
    }
    public void Show(string message)
    {
        PopupManager.Instance.Show(new MessagePopup(message));
    }
    public void Show(IPopup popup)
    {
        var popupContainer = PopupContainerTemplate.Instantiate();
        popup.Initialize(this, popupContainer.Content);
        createdPopups.Add(popup, popupContainer.gameObject);

        LayoutRebuilder.ForceRebuildLayoutImmediate(popupContainer.transform.GetComponent<RectTransform>());

        popupContainer.GetComponent<CanvasGroup>().DOFade(1, 0.25f);
        popupContainer.transform.GetChild(0).GetComponent<LayoutElement>().ignoreLayout = true;
        var rectTransform = popupContainer.transform.GetChild(0).GetComponent<RectTransform>();
        var pos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y);
        rectTransform.DOAnchorPos(pos, 0.25f);
        _activePopup = popup;
        Active = true;
    }

    public void Hide(IPopup popup)
    {
        var obj = createdPopups[popup];
        var rectTransform = obj.transform.GetChild(0).GetComponent<RectTransform>();
        rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y), 0.25f);
        obj.GetComponent<CanvasGroup>().DOFade(0, 0.25f);
        Destroy(obj, 0.3f);
        createdPopups.Remove(popup);
        popup.Cleanup();
        StartCoroutine(Deactive());
    }

    IEnumerator Deactive()
    {
        yield return new WaitForEndOfFrame();
        Active = false;
        _activePopup = null;
    }


    void Update() {

        if (Active)
            _activePopup?.Update();
    }
}
