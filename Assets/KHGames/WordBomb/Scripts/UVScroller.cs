
using UnityEngine;
using UnityEngine.UI;

public class UVScroller : MonoBehaviour
{
    [SerializeField]private RawImage rawImage;
    [SerializeField] private float x,y;
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position+(new Vector2(x,y)*Time.deltaTime),rawImage.uvRect.size);
    }
}
