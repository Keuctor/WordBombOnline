using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FadeInAudio : MonoBehaviour
{
    public void FadeIn(float targetVolume, bool destroyAfterFadeIn)
    {
        StartCoroutine(FadeIn(this.GetComponent<AudioSource>(), targetVolume, destroyAfterFadeIn));
    }
    private IEnumerator FadeIn(AudioSource source, float targetVolume, bool destroyAfterFadeIn)
    {
        while (source.volume < targetVolume)
        {
            source.volume += 0.05f;
            yield return new WaitForSeconds(0.4f);
        }
        if (destroyAfterFadeIn)
        {
            Destroy(gameObject);
        }
    }
}