
using UnityEngine;
using UnityEngine.UI;

public class PopupToggleGroup : PopupElement
{
    public ToggleGroup ToggleGroup => GetComponent<ToggleGroup>();
    public Transform Content;
}
