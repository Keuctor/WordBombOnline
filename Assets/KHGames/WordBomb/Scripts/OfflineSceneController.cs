using ilasm.WordBomb.Initialization;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineSceneController : MonoBehaviour
{
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

    private void Start()
    {
        UserData.LoggedIn = false;
        CanvasUtilities.Instance.Toggle(true, Language.Get("CONNECTING"));
        Connect();
    }

    public IEnumerator OnConnectedCoroutine()
    {
        CanvasUtilities.Instance.Toggle(true, "ID...");
        var nextTime = Time.timeSinceLevelLoad + 10;
        while (WordBombNetworkManager.Instance.Id == -1)
        {
            if (Time.timeSinceLevelLoad > nextTime)
            {
                CanvasUtilities.Instance.Toggle(false);
                PopupManager.Instance.Show("ERROR#2929");
                yield return null;
            }
            yield return new WaitForSeconds(0.05f);
        }
        GameSetup.LocalPlayerId = WordBombNetworkManager.Instance.Id;
        LoadScene();
    }

    private static void LoadScene()
    {
        CanvasUtilities.Instance.Toggle(true, Language.Get("SCENE_LOADING"));
        AsyncOperation op = SceneManager.LoadSceneAsync("Menu");
        op.completed += (a) => { CanvasUtilities.Instance.Toggle(false); };
    }


    private void OnDisconnected()
    {
        QuestionPopup msg = new QuestionPopup(Language.Get("CANT_CONNECT_TO_SERVER"));
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
}
