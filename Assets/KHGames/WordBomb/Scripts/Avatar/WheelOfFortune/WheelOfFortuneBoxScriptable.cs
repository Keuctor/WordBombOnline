using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class WheelOfFortuneBoxScriptable : ScriptableObject
{
    public short Id;
    public string Name;
    public Sprite Icon;
    public List<Sprite> BoxContainer = new List<Sprite>();
    public int Price;
}
