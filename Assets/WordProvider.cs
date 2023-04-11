using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;



public class WordProvider : MonoBehaviour
{
    static HashSet<string> blackListWords = new HashSet<string>();
    private async void Start()
    {
        blackListWords = await TextAssetToList(Resources.Load<TextAsset>("Words/words_blacklist"));
    }

    public static string Censore(string text)
    {
        if (blackListWords.Contains(text))
        {
            return new string('*', text.Length);
        }

        return text;
    }

    private Task<HashSet<string>> TextAssetToList(TextAsset ta)
    {
        int selectedLang = LobbySettings.GetLobbySelectedLanguage();
        string[] arrayString = ta.text.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return Task.Factory.StartNew(() =>
        {
            HashSet<string> listToReturn = new HashSet<string>();
            for (int i = 0; i < arrayString.Length; i++)
            {
                listToReturn.Add(arrayString[i]);
            }
            return listToReturn;
        });
    }
}