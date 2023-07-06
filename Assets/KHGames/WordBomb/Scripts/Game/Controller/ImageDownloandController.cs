

using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageDownloandController : MonoBehaviour
{
    public Action<Texture2D> OnDownloand;
    public Image Image;

    public void DownloandImage(string name,byte language)
    {
        Image.transform.DOScale(Vector3.zero, 0.2f);
        var newUrl = $"https://keugames.com/images";
        if (language == 0)
        {
            newUrl += "_en/";
        }
        else {
            newUrl += "_tr/";
        }
        StartCoroutine(GetTexture(newUrl + name+".png"));
    }

    IEnumerator GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            PopupManager.Instance.Show("{IMAGE_ERROR}");
            Debug.Log(www.error+ " : " + url);
        }
        else
        {
            Image.transform.DOScale(Vector3.one, 0.2f);
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            OnDownloand?.Invoke(myTexture);
            Image.sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
    }
}