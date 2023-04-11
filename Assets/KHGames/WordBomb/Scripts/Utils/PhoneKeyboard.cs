
using System;
using TMPro;
using UnityEngine;

namespace ilasm.WordBomb
{
    public class PhoneKeyboard : MonoBehaviour
    {
        const string pluginName = "com.khgames.wordbomblibrary.WordBombJavaLibrary";

        public static AndroidJavaClass AndroidPluginClass;
        public static AndroidJavaObject AndroidPluginInstance;

        public TMP_Text Text;
        private void Awake()
        {
            try
            {
                var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentUnityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");


                AndroidPluginClass = new AndroidJavaClass(pluginName);
                AndroidPluginInstance = AndroidPluginClass.CallStatic<AndroidJavaObject>("getInstance");


                AndroidPluginInstance.Call("SetActivity", currentUnityActivity);
                AndroidPluginInstance.Call("ShowKeyboard");
                AndroidPluginInstance.Call("Toast", "Oldu aq gerçekten");
            }
            catch (Exception e)
            {
                Text.color = Color.red;
                Text.text = e.ToString();
            }
        }
    }
}
