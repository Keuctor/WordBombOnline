using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class SceneWizard : EditorWindow
{
    SceneWizardConfig config;

    Vector2 scrollView;
    public string ActiveScenePath;
    public static SceneWizard Instance { get; private set; }


    [MenuItem("Window/EMD Tools/Scene Wizard")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SceneWizard window = (SceneWizard)EditorWindow.GetWindow(typeof(SceneWizard));
        window.titleContent = new GUIContent("Scene Wizard", new GUIContent(EditorGUIUtility.IconContent("d_UnityLogo")).image);
        window.Show();
    }

    void RefreshConfig()
    {
        if (config == null)
        {
            bool sucessFindingConfig = false;

            if (!File.Exists(Application.dataPath + "/SceneWizard_Config.asset"))
            {
                SceneWizardConfig newConfig = new SceneWizardConfig();
	            AssetDatabase.CreateAsset(newConfig, "Assets/SceneWizard_Config.asset");
                
                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();
            }

            var foundAssets = AssetDatabase.FindAssets("SceneWizard_Config", new[] { "Assets/" });
            if (foundAssets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
	            config = AssetDatabase.LoadAssetAtPath<SceneWizardConfig>(path);
            }

            if(sucessFindingConfig)
                EditorUtility.SetDirty(config);
        }
    }

    void ReloadScenes()
    {
        config.scenes = new List<SceneConfigSetup>();
	    LoadFromPath(config.folderPath);
    }

    void LoadFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        string[] files = Directory.GetFiles(path);
        foreach (var fp in files)
        {
            if (fp.Contains(".unity"))
            {
                var assetPath = "Assets" + fp.Split("Assets")[1];

                var sceneLoaded = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

                if (sceneLoaded != null)
                {
                    var pathSplit = assetPath.Replace("\\", "/").Split("/");

                    SceneConfigSetup scs = new SceneConfigSetup()
                    {
                        name = sceneLoaded.name,
                        path = assetPath,
                        parentFolder = pathSplit[pathSplit.Length - 2]
                    };

                    config.scenes.Add(scs);
                }
            }
        }


        if (config.allowSubfolders)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                LoadFromPath(dir);
            }
        }

        EditorUtility.SetDirty(config);
    }

    private void OnEnable()
    {
        Instance = this;
        RefreshConfig();
        ReloadScenes();
    }
  
    private void OnFocus()
    {
        RefreshConfig();
        ReloadScenes();
    }


    private void OnGUI()
    {
        RefreshConfig();
        ReloadScenes();
        GUILayout.Label(ActiveScenePath);

        var prevAlignment = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        if(EditorApplication.isPlaying){
            if(GUILayout.Button("STOP")){
                EditorApplication.isPlaying=false;
            }
        }
        else
        if(GUILayout.Button("START")){
            foreach(var scene in config.scenes){
                if(scene.name=="Intro"){
                    ActiveScenePath = EditorSceneManager.GetActiveScene().path;
                    UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Single);
                    EditorApplication.isPlaying = true;
                    break;
                }
            }
        }

        if (config.scenes != null && config.scenes.Count > 0)
        {
            string lastFolderName = "";
            EditorGUILayout.BeginVertical(GUI.skin.box);

            foreach (var scene in config.scenes)
            {
                if (scene.parentFolder != lastFolderName)
                {
                    if (lastFolderName != "")
                        GUILayout.Space(4);

                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label(scene.parentFolder, EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();
                    lastFolderName = scene.parentFolder;
                    EditorGUI.indentLevel++;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(scene.name))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Single);
                }
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndVertical();
        }

        GUI.skin.button.alignment = prevAlignment;
    }
}
