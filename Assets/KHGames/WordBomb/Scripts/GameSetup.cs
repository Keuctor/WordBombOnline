using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;
using Assets.KHGames.WordBomb;
using Task = System.Threading.Tasks.Task;
using LiteNetLib;
using System.Collections;
using UnityEngine.Localization.Settings;

#if UNITY_EDITOR
using ParrelSync;
using UnityEditor;
#endif


namespace ilasm.WordBomb.Initialization
{
    public class GameSetup : MonoBehaviour
    {
        public static string Version = "[0.91v]";
        public static int LocalPlayerId { get;  set; }
        private static bool _isConfigLoaded;

        private void OnEnable()
        {
            WordBombNetworkManager.OnDisconnectedFromServer += OnDisconnected;
            WordBombNetworkManager.OnConnectedToServer += OnConnected;
        }

        private void OnDisable()
        {
            WordBombNetworkManager.OnDisconnectedFromServer -= OnDisconnected;
            WordBombNetworkManager.OnConnectedToServer -= OnConnected;
        }


        private void OnConnected(NetPeer obj)
        {
                 StartCoroutine(OnConnectedCoroutine());
        }

        public IEnumerator OnConnectedCoroutine() {
            CanvasUtilities.Instance.Toggle(true, "ID...");
            var nextTime = Time.timeSinceLevelLoad + 10;
            while (WordBombNetworkManager.Instance.Id == -1)
            {
                if (Time.timeSinceLevelLoad > nextTime)
                {
                    CanvasUtilities.Instance.Toggle(false);
                    PopupManager.Instance.Show(Language.Get("CANT_CONNECT_TO_SERVER"));
                    yield return null;
                }
                yield return new WaitForSeconds(0.05f);
            }
            LocalPlayerId = WordBombNetworkManager.Instance.Id;
            if (UserData.LoggedIn)
            {
                CanvasUtilities.Instance.ShowNotification(Language.Get("USER_WELCOME", UserData.User.UserName), 2);
            }
            CanvasUtilities.Instance.Toggle(false);
            LoadScene();
        }

        private void OnDisconnected()
        {
            QuestionPopup msg = new QuestionPopup(Language.Get("CANT_CONNECT_TO_SERVER_RETRY"));
            msg.OnSubmit += () =>
            {
                Connect();
            };
            msg.OnCancel += () =>
            {
                Application.Quit(4);
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            };
            PopupManager.Instance.Show(msg);
        }


        public void Connect()
        {
            WordBombNetworkManager.Instance.Connect();
        }

        private void Start()
        {
            CanvasUtilities.Instance.Toggle(true, Language.Get("CONNECTING"));
            Connect();
        }

        private static void LoadScene()
        {
            CanvasUtilities.Instance.Toggle(true, Language.Get("SCENE_LOADING"));
            AsyncOperation op = SceneManager.LoadSceneAsync("Menu");
            op.completed += (a) => { CanvasUtilities.Instance.Toggle(false); };
        }

        private static async Task Authenticate()
        {
            var options = new InitializationOptions();
            options.SetProfile(Guid.NewGuid().ToString());
#if UNITY_EDITOR
            options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif
            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }


    }
}