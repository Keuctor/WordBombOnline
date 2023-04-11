using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayStateEditor
{
    static PlayStateEditor()
    {
        EditorApplication.playModeStateChanged += ModeChanged;
    }
    static void ModeChanged(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.EnteredEditMode)
        {
            if (!string.IsNullOrEmpty(SceneWizard.Instance.ActiveScenePath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(SceneWizard.Instance.ActiveScenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
            }
        }
    }
}
