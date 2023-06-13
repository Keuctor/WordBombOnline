using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PerkData {
    public GamePerk PerkType;
    public Sprite Icon;
    public string TitleLocaleId;
    public string DescriptionLocaleId;
    public int Price;
}
[CreateAssetMenu]
public class PerkScriptableObject : ScriptableObject
{
    public List<PerkData> DefaultPerks = new List<PerkData>();
    public List<PerkData> GoldenPerks = new List<PerkData>();
}
