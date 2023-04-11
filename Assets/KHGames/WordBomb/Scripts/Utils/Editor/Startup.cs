#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private static void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        if (EditorSceneManager.GetActiveScene().buildIndex != 0)
        {
            //if (EditorSceneManager.GetActiveScene().name.ToLower().Contains("test"))
            //{ return; }

            //if (obj == PlayModeStateChange.EnteredPlayMode)
            //{
            //    EditorSceneManager.LoadScene(0);
            //}
        }
    }
}
#endif