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
        public const string GreyboxMaterialPath = "Assets/Yendor/Resources/YendorGreybox.mat";

        [MenuItem("Yendor/Create greybox render resources")]
        public static void CreateRenderResources()
        {
            Directory.CreateDirectory("Assets/Yendor/Resources");
            if (!AssetDatabase.LoadAssetAtPath<Material>(GreyboxMaterialPath))
            {
                var shader = Shader.Find("Standard");
                if (!shader) throw new Exception("The built-in Standard shader is unavailable.");
                AssetDatabase.CreateAsset(new Material(shader), GreyboxMaterialPath);
                AssetDatabase.SaveAssets();
            }
            Debug.Log("YENDOR_RENDER_RESOURCES_CREATED");
        }

        [MenuItem("Yendor/Create or reset Copper Wild scene")]
        public static void CreateScene()
        {
            CreateRenderResources();
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
            if (!File.Exists(ScenePath)) throw new Exception("Required scene is missing: " + ScenePath + ". Run Yendor/Create or reset Copper Wild scene explicitly.");
            if (!AssetDatabase.LoadAssetAtPath<Material>(GreyboxMaterialPath))
                throw new Exception("Required greybox material is missing. Run Yendor/Create greybox render resources explicitly.");
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
            Debug.Log("YENDOR_BUILD_GUID: " + report.summary.guid);
            Debug.Log("YENDOR_BUILD_OK");
        }

        public static void AuditAssets()
        {
            Validate();
            int count = 0;
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Yendor" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                foreach (var transform in prefab.GetComponentsInChildren<Transform>(true))
                {
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject) > 0)
                        throw new Exception("Missing script in " + path);
                    foreach (var renderer in transform.GetComponents<Renderer>())
                        foreach (var material in renderer.sharedMaterials)
                            if (!material) throw new Exception("Missing material in " + path);
                }
                count++;
            }
            Debug.Log("YENDOR_ASSET_AUDIT_OK: prefabs inspected=" + count + "; runtime-generated geometry requires rendered review.");
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
