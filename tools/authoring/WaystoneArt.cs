// Run in the existing Editor with Unity Pipeline run_script. No paid services.
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WaystoneArt
{
    const string ModelPath = "Assets/Yendor/Art/Meshy/Waystone/waystone-preview.fbx";
    const string ArtPath = "Assets/Yendor/Art/Waystone";
    const string PrefabPath = "Assets/Yendor/Resources/YendorWaystone.prefab";
    const string StudyMaterial = "Assets/Yendor/Art/Studies/WaystoneSandstone.mat";
    const string StudySound = "Assets/Yendor/Art/Studies/WaystoneActivation.wav";
    static string Evidence => Path.GetFullPath("../artifacts/pilots/yen-rgi/integration");

    public static string Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new Exception("Preserve the current play session; exit play mode before authoring.");
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        var floorStudy = AssetDatabase.LoadAssetAtPath<Material>(StudyMaterial);
        var cue = AssetDatabase.LoadAssetAtPath<AudioClip>(StudySound);
        if (!model || !floorStudy || !cue) throw new Exception("Required reviewed source assets are missing.");
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath))
            throw new Exception("Prefab already exists; review its source before rebuilding.");
        Directory.CreateDirectory(ArtPath);
        Directory.CreateDirectory("Assets/Yendor/Resources");
        Directory.CreateDirectory(Evidence);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var stone = MakeMaterial("Sandstone", new Color(.57f, .47f, .32f));
        var magic = MakeMaterial("Turquoise", new Color(.1f, 1, .85f), "Unlit/Color");
        var floor = new Material(floorStudy) { name = "Waystone paving" };
        floor.color = new Color(.83f, .83f, .83f);
        floor.SetFloat("_Glossiness", .1f);
        AssetDatabase.CreateAsset(floor, ArtPath + "/Paving.mat");

        var scene = EditorSceneManager.NewPreviewScene();
        var root = new GameObject("Copper Wild waystone");
        SceneManager.MoveGameObjectToScene(root, scene);
        try
        {
            // The root stays at the existing extraction position. The landmark sits
            // beside the platform, leaving spawn, camera and the shrine corridor clear.
            var marker = new GameObject("Waystone landmark");
            marker.transform.SetParent(root.transform, false);
            marker.transform.localPosition = new Vector3(-3.5f, 0, 0);
            var near = UnityEngine.Object.Instantiate(model, marker.transform);
            near.name = "Reviewed Meshy stone";
            var renderers = near.GetComponentsInChildren<Renderer>();
            var size = Normalize(near.transform, marker.transform);
            foreach (var r in renderers)
                r.sharedMaterials = Enumerable.Repeat(stone, r.sharedMaterials.Length).ToArray();
            var collider = marker.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, 1.5f, 0);
            collider.size = size;

            var distant = new GameObject("Distant stone silhouette");
            distant.transform.SetParent(marker.transform, false);
            Primitive(distant.transform, "Stem", PrimitiveType.Cube,
                new Vector3(0, .37f, 0), new Vector3(.5f, .74f, .4f), stone);
            Ring(distant.transform, "Core ring", new Vector3(0, 1.38f, 0), .56f, .15f, stone);
            Ring(distant.transform, "Upper ring", new Vector3(0, 2.43f, 0), .32f, .13f, stone);
            var group = marker.AddComponent<LODGroup>();
            group.SetLODs(new[] {
                new LOD(.035f, renderers),
                new LOD(.006f, distant.GetComponentsInChildren<Renderer>())
            });
            group.RecalculateBounds();
            // Both faces remain identifiable. These small cores stay outside the LOD
            // group so the color cue persists when the fine mesh becomes a proxy.
            foreach (float side in new[] { -1f, 1f })
                Primitive(marker.transform, "Turquoise core", PrimitiveType.Sphere,
                    new Vector3(0, 1.38f, side * .34f), new Vector3(.65f, .65f, .12f), magic);
            // Existing cyan extraction platform remains underneath as its rim and
            // original collision. This purely visual inset adds the paving study.
            Primitive(root.transform, "Sandstone platform inset", PrimitiveType.Cylinder,
                new Vector3(0, .315f, 0), new Vector3(4.65f, .012f, 4.65f), floor);
            var audio = root.AddComponent<AudioSource>();
            audio.clip = cue; audio.playOnAwake = false; audio.loop = false;
            audio.spatialBlend = 0; audio.volume = .25f;
            AssetDatabase.SaveAssets();
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            AssetDatabase.SaveAssets();
            return "Waystone prefab authored: 3m stone, base pivot, box collider, two LODs, paving inset and extraction cue.";
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    static Vector3 Normalize(Transform near, Transform marker)
    {
        // FBX axes are represented by the imported root rotation. Measure rendered
        // bounds with that rotation preserved; raw Mesh.bounds uses different axes.
        near.localPosition = Vector3.zero;
        near.localScale = Vector3.one;
        var renderers = near.GetComponentsInChildren<Renderer>();
        var bounds = renderers[0].bounds;
        foreach (var r in renderers.Skip(1)) bounds.Encapsulate(r.bounds);
        float scale = 3 / bounds.size.y;
        var center = marker.InverseTransformPoint(bounds.center);
        float bottom = marker.InverseTransformPoint(bounds.min).y;
        near.localScale = Vector3.one * scale;
        near.localPosition = new Vector3(-center.x * scale, -bottom * scale, -center.z * scale);
        var finalBounds = renderers[0].bounds;
        foreach (var r in renderers.Skip(1)) finalBounds.Encapsulate(r.bounds);
        if (Mathf.Abs(finalBounds.size.y - 3) > .001f || Mathf.Abs(finalBounds.min.y - marker.position.y) > .001f || Mathf.Abs(finalBounds.center.x - marker.position.x) > .001f)
            throw new Exception("Waystone normalization invariant failed.");
        return finalBounds.size;
    }

    public static string CorrectLocalPivot()
    {
        // Re-read the authored prefab, retaining its GUID and all existing references.
        var root = PrefabUtility.LoadPrefabContents(PrefabPath);
        try {
            var marker = root.transform.Find("Waystone landmark");
            var near = marker.Find("Reviewed Meshy stone");
            var size = Normalize(near, marker);
            marker.GetComponent<BoxCollider>().size = size;
            marker.GetComponent<LODGroup>().RecalculateBounds();
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            return "Normalized from actual rendered bounds preserving FBX axes; base, height and horizontal alignment invariants pass.";
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    public static string TuneCore()
    {
        var material = AssetDatabase.LoadAssetAtPath<Material>(ArtPath + "/Turquoise.mat");
        if (!material) throw new Exception("Required core material is missing.");
        material.shader = Shader.Find("Unlit/Color");
        material.color = new Color(.1f, 1, .85f);
        EditorUtility.SetDirty(material); AssetDatabase.SaveAssets();
        return "Self-lit turquoise core retains navigation color on the shadowed face.";
    }

    static Material MakeMaterial(string name, Color color, string shader = "Standard")
    {
        string path = ArtPath + "/" + name + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path)) throw new Exception("Existing material: " + path);
        var material = new Material(Shader.Find(shader)) { name = name, color = color };
        if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", .13f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    static GameObject Primitive(Transform parent, string name, PrimitiveType kind,
        Vector3 position, Vector3 scale, Material material)
    {
        var item = GameObject.CreatePrimitive(kind);
        item.name = name; item.transform.SetParent(parent, false);
        item.transform.localPosition = position; item.transform.localScale = scale;
        UnityEngine.Object.DestroyImmediate(item.GetComponent<Collider>());
        item.GetComponent<Renderer>().sharedMaterial = material;
        return item;
    }

    static void Ring(Transform parent, string name, Vector3 position, float radius,
        float tube, Material material)
    {
        const int segments = 16, sides = 6;
        var vertices = new Vector3[segments * sides];
        var normals = new Vector3[vertices.Length];
        var triangles = new int[segments * sides * 6];
        for (int i = 0; i < segments; i++)
            for (int j = 0; j < sides; j++)
            {
                float a = i * Mathf.PI * 2 / segments, b = j * Mathf.PI * 2 / sides;
                var radial = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
                int index = i * sides + j;
                normals[index] = radial * Mathf.Cos(b) + Vector3.forward * Mathf.Sin(b);
                vertices[index] = radial * radius + normals[index] * tube;
                int next = ((i + 1) % segments) * sides + j;
                int corner = i * sides + (j + 1) % sides;
                int diagonal = ((i + 1) % segments) * sides + (j + 1) % sides;
                int start = index * 6;
                triangles[start] = index; triangles[start + 1] = next; triangles[start + 2] = corner;
                triangles[start + 3] = corner; triangles[start + 4] = next; triangles[start + 5] = diagonal;
            }
        var mesh = new Mesh { name = name, vertices = vertices, normals = normals, triangles = triangles };
        mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh, ArtPath + "/" + name.Replace(" ", "") + ".asset");
        var item = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        item.transform.SetParent(parent, false); item.transform.localPosition = position;
        item.GetComponent<MeshFilter>().sharedMesh = mesh;
        item.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    public static string Review()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (!prefab) throw new Exception("Required authored prefab missing.");
        var scene = EditorSceneManager.NewPreviewScene();
        var root = UnityEngine.Object.Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(root, scene);
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SceneManager.MoveGameObjectToScene(ground, scene);
        ground.transform.position = new Vector3(0, -.15f, 0);
        ground.transform.localScale = new Vector3(100, .3f, 100);
        var groundMaterial = new Material(Shader.Find("Standard")) { color = new Color(.21f, .32f, .22f) };
        ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        var camera = new GameObject("Art review camera").AddComponent<Camera>();
        SceneManager.MoveGameObjectToScene(camera.gameObject, scene); camera.scene = scene;
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.15f, .26f, .28f);
        camera.fieldOfView = 78; camera.nearClipPlane = .05f; camera.farClipPlane = 220;
        var sun = new GameObject("Late afternoon review sun").AddComponent<Light>();
        SceneManager.MoveGameObjectToScene(sun.gameObject, scene);
        sun.type = LightType.Directional; sun.intensity = 1.3f; sun.color = new Color(1, .82f, .6f);
        sun.transform.rotation = Quaternion.Euler(35, -35, 0);
        try
        {
            var targets = new[] { new Vector3(-3.5f, 1.5f, 0), new Vector3(-3.5f, 1.5f, 0), new Vector3(-3.5f, 1.5f, 0), new Vector3(-3.5f, 1.5f, 0), new Vector3(-1.2f, .8f, 0), new Vector3(-1.5f, 1.2f, 0) };
            var positions = new[] { new Vector3(-3.5f, 2, 4), new Vector3(-3.5f, 2, -4), new Vector3(1, 2, 0), new Vector3(-8, 2, 0), new Vector3(5, 5, 7), new Vector3(0, 2, 40) };
            for (int i = 0; i < positions.Length; i++)
            {
                camera.transform.position = positions[i]; camera.transform.LookAt(targets[i]);
                Capture(camera, Evidence + "/prefab-view-" + i + ".png");
            }
            var marker = root.transform.Find("Waystone landmark");
            var collider = marker.GetComponent<BoxCollider>();
            var lods = marker.GetComponent<LODGroup>().GetLODs();
            var report = new {
                prefab = PrefabPath, model = ModelPath,
                markerHeight = collider.size.y,
                collider = new { center = new[] { collider.center.x, collider.center.y, collider.center.z }, size = new[] { collider.size.x, collider.size.y, collider.size.z } },
                triangleCounts = lods.Select(l=>l.renderers.Sum(r=>r.GetComponent<MeshFilter>().sharedMesh.triangles.Length/3)).ToArray(),
                lodThresholds = lods.Select(l=>l.screenRelativeTransitionHeight).ToArray(),
                audio = new { clip = AssetDatabase.GetAssetPath(root.GetComponent<AudioSource>().clip), root.GetComponent<AudioSource>().volume, root.GetComponent<AudioSource>().playOnAwake },
                missingMaterials = root.GetComponentsInChildren<Renderer>(true).Count(r=>r.sharedMaterials.Any(m=>!m)),
                missingScripts = root.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)),
                graphics = SystemInfo.graphicsDeviceName
            };
            File.WriteAllText(Evidence + "/prefab-review.json", JsonConvert.SerializeObject(report, Formatting.Indented));
            return "Six actual prefab views and native audit saved, including 40m at gameplay FOV.";
        }
        finally { UnityEngine.Object.DestroyImmediate(groundMaterial); EditorSceneManager.ClosePreviewScene(scene); }
    }

    static void Capture(Camera camera, string path)
    {
        var rt = RenderTexture.GetTemporary(1600, 900, 24);
        var old = RenderTexture.active;
        var texture = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        try {
            camera.targetTexture = rt; camera.Render(); RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
        }
        finally { camera.targetTexture = null; RenderTexture.active = old; RenderTexture.ReleaseTemporary(rt); UnityEngine.Object.DestroyImmediate(texture); }
    }
}
