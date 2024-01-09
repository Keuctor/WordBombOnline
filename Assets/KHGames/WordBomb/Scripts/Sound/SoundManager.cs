using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Object = UnityEngine.Object;

public struct PlayingSound {
    public AudioSource Source;
    public float InitialVolume;
}

public static class SoundManager
{
    public static SoundScriptableData SoundData;

    public static Dictionary<Sounds, PlayingSound> playingSongs = new();
    
    private static void LoadSounds()
    {
        SoundData = Resources.Load<SoundScriptableData>("Sound/GameSound");
        EventBus.OnVolumeChanged += OnVolumeChanged;
    }

    private static void OnVolumeChanged()
    {
        foreach (var p in playingSongs)
        {
            p.Value.Source.volume = p.Value.InitialVolume *  UserData.SFXVolume;
        }
    }

    public static void StopAudio(Sounds sound)
    {
        if (playingSongs.TryGetValue(sound, out PlayingSound obj))
        {
            obj.Source.DOFade(0, 0.3f);
            playingSongs.Remove(sound);
            Object.Destroy(obj.Source.gameObject,0.3f);
        }
    }
    public static bool IsPlaying(Sounds sound) {
        return playingSongs.ContainsKey(sound);
    }

    public static AudioSource PlayAudioTracked(Sounds sound, float volume = 0.5f, bool fadeIn = false)
    {
        if (SoundData == null)
            LoadSounds();

        var s = SoundData.Get(sound);
        if (s == null) return null;
        var gm = new GameObject("Sound_" + sound);
        var src = gm.AddComponent<AudioSource>();
        if (fadeIn)
        {
            src.volume = 0;
            var fadeInComp = gm.AddComponent<FadeInAudio>();
            fadeInComp.FadeIn(volume * UserData.SFXVolume, false);
        }
        else
        {
            src.volume = volume * UserData.SFXVolume;
        }
        src.clip = s.Clip[UnityEngine.Random.Range(0, s.Clip.Length - 1)];
        src.Play();
        return src;
    }

    public static void PlayAudio(Sounds sound, float pitch)
    {
        PlayAudio(sound,false,0.5f,false,pitch);
    }

    public static void PlayAudio(Sounds sound, bool loop = false, float volume = 0.5f, bool fadeIn = false,float pitch=1)
    {
        if (SoundData == null)
            LoadSounds();

        var s = SoundData.Get(sound);
        if (s == null) return;
        var gm = new GameObject("Sound_" + sound);
        Object.DontDestroyOnLoad(gm);
        var src = gm.AddComponent<AudioSource>();
        src.pitch = pitch;
        if (fadeIn)
        {
            src.volume = 0;
            var fadeInComp = gm.AddComponent<FadeInAudio>();
            fadeInComp.FadeIn(volume * UserData.SFXVolume, false);
        }
        else
        {
            src.volume = volume * UserData.SFXVolume;
        }
        src.clip = s.Clip[UnityEngine.Random.Range(0, s.Clip.Length - 1)];
        src.Play();
        src.loop = loop;
        if (loop)
            playingSongs.Add(sound, new PlayingSound() { 
                InitialVolume = volume,
                Source = src
            });
        else
            Object.Destroy(gm, src.clip.length);
    }
}