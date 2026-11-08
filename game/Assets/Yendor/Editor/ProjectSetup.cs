using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Yendor.Editor
{
    public static class ProjectSetup
    {
        public const string ScenePath = "Assets/Yendor/Scenes/CopperWild.unity";

        [MenuItem("Yendor/Create or reset Copper Wild scene")]
        public static void CreateScene()
        {
            Directory.CreateDirectory("Assets/Yendor/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Expedition bootstrap").AddComponent<Expedition>();
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            PlayerSettings.companyName = "Yendor";
            PlayerSettings.productName = "Yendor";
            AssetDatabase.SaveAssets();
        }

        public static void Validate()
        {
            if (!File.Exists(ScenePath)) CreateScene();
            if (EditorUtility.scriptCompilationFailed) throw new Exception("Unity script compilation failed.");
            var scene = EditorSceneManager.OpenScene(ScenePath);
            if (UnityEngine.Object.FindObjectsByType<Expedition>(FindObjectsSortMode.None).Length != 1)
                throw new Exception("Expected one expedition bootstrap.");
            Debug.Log("YENDOR_VALIDATION_OK: scripts compiled and expedition scene is valid.");
        }

        public static void Build()
        {
            Validate();
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { ScenePath }, locationPathName = "Builds/Linux/Yendor.x86_64",
                target = BuildTarget.StandaloneLinux64, options = BuildOptions.Development
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Yendor build failed: " + report.summary.result);
            Debug.Log("YENDOR_BUILD_OK");
        }

        public static void Smoke()
        {
            Validate();
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--yendor-smoke") < 0)
                throw new Exception("Smoke requires --yendor-smoke on the Editor command line.");
            EditorApplication.EnterPlaymode();
        }
    }
}
