using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialView : MonoBehaviour
{
    public Transform Content;
    public float Radius = 120;
    public float Angle = 0;
    
    private void OnValidate()
    {
        Refresh();
    }

    private void OnEnable()
    {
        StartCoroutine(RefreshCoroutine());
    }

    public IEnumerator RefreshCoroutine()
    {
        Angle = 0;
        yield return new WaitForEndOfFrame();
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            float angle = i * 2 * Mathf.PI / Content.transform.childCount;
            float x = Mathf.Cos(angle + Angle) * (Radius);
            float y = Mathf.Sin(angle + Angle) * (Radius);
            var child = Content.transform.GetChild(i);
            child.transform.position = transform.position + new Vector3(x, y, 0);
        }
    }
}