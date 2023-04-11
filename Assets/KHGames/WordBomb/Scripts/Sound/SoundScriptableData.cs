using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SoundScriptableData : ScriptableObject
{
    [SerializeField]
    private List<SoundData> Sounds = new List<SoundData>();
    public SoundData Get(Sounds sound) {
        return Sounds.Find(t=>t.Sound==sound);
    }
}