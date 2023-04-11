using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ilasm.WordBomb.Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombController : MonoBehaviour
{
    public GameObject BombObject;
    public AudioSource BombAudioSource;

    [Header("UI")]
    public Image Fill;
    public Image Bomb;
    public Image Arrow;

    public GameObject BombEffect;

    private TweenerCore<float, float, FloatOptions> fillTween;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> arrowTween;

    public Sprite[] BombSprites;


    public void Stop()
    {
        SoundManager.StopAudio(Sounds.TensionRising);
        _time = -1;
        fillTween?.Kill();
        arrowTween?.Kill();
        Fill.gameObject.SetActive(false);
        Bomb.gameObject.SetActive(false);
        Arrow.gameObject.SetActive(false);
        BombObject.gameObject.SetActive(false);
    }

    private int _time = -1;
    private Coroutine _coroutine;

    /// <summary>
    /// Time as a second
    /// </summary>
    /// <param name="time"></param>
    public void Timer(int time, int selectedIndex, int totalPlayers)
    {
        BombAudioSource.volume = UserData.BombTickingVolume * 0.50f;
        Fill.fillAmount = 1;

        if (_time == -1)
        {
            _time = time;
            OnTimeChanged(false);
        }

        if (_time != time)
        {
            if (_time < time)
            {
                BombAudioSource.pitch -= 0.025f;
            }
            else
            {
                BombAudioSource.pitch += 0.025f;
            }
            _time = time;
            OnTimeChanged(true);
        }

        fillTween?.Kill();
        arrowTween?.Kill();

        BombObject.gameObject.SetActive(true);


        fillTween = Fill.DOFillAmount(0, time);
        arrowTween = Arrow.transform.DOLocalRotate(new Vector3(0, 0, (360 / totalPlayers) * selectedIndex), 0.3f)
                .SetEase(Ease.OutFlash);
        Bomb.transform.localScale = Vector3.one;


        StopTimerCoroutine();

        if (TurnController.IsMyTurn)
        {
            _coroutine = StartCoroutine(TimerCountdownCoroutine(_time));
        }
    }
    public void StopTimerCoroutine()
    {
        if (_bombTimerSource != null)
        {
            _bombTimerSource.Stop();
        }

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    public AudioSource _bombTimerSource;

    public IEnumerator TimerCountdownCoroutine(int time)
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(1);
            if (!TurnController.IsMyTurn)
            {
                StopTimerCoroutine();
                break;
            }

            if (time == 2)
            {
                if (_bombTimerSource == null)
                {
                    _bombTimerSource = SoundManager.PlayAudioTracked(Sounds.BombTimerNearEnd, 0.25f, false);
                }
                else if (_bombTimerSource != null)
                {
                    _bombTimerSource.Play();
                }
            }
        }
    }


    private bool _sizeUp;
    private void Update()
    {
        if (_time != -1)
        {
            if (_sizeUp)
            {
                if (Bomb.transform.localScale.x < 1.15f)
                {
                    Bomb.transform.localScale = Bomb.transform.localScale +
                        new Vector3(Time.deltaTime * (20 / _time), Time.deltaTime * (20 / _time), Time.deltaTime * (20 / _time));
                }
                else
                {
                    _sizeUp = false;
                }
            }
            else
            {
                Bomb.transform.localScale = Bomb.transform.localScale - new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
                if (Bomb.transform.localScale.x <= 1f)
                {
                    _sizeUp = true;
                }
            }
        }
    }

    private void OnTimeChanged(bool effect)
    {
        if (_time < 20)
        {
            Bomb.sprite = BombSprites[0];
        }
        if (_time < 12)
        {
            Bomb.sprite = BombSprites[1];
        }
        if (_time < 8)
        {
            Bomb.sprite = BombSprites[2];
        }

        if (effect)
            StartCoroutine(BombEffectDisabler());


        if (_time >= 8)
        {
            if (SoundManager.IsPlaying(Sounds.TensionRising))
                SoundManager.StopAudio(Sounds.TensionRising);
        }
        else
        {
            if (!SoundManager.IsPlaying(Sounds.TensionRising))
            {
                SoundManager.PlayAudio(Sounds.TensionRising, true, 0.1f, true);
            }
        }
    }

    private IEnumerator BombEffectDisabler()
    {
        BombEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        BombEffect.gameObject.SetActive(false);
    }

    private void OnVolumeChanged()
    {
        BombAudioSource.volume = UserData.BombTickingVolume;
    }

    private void OnEnable()
    {
        EventBus.OnVolumeChanged += OnVolumeChanged;
    }
    private void OnDisable()
    {
        EventBus.OnVolumeChanged -= OnVolumeChanged;
        fillTween?.Kill();
        arrowTween?.Kill();
    }
}
