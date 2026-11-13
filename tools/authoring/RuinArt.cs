// Existing-Editor authoring only. Never submits a paid request or changes a scene.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class RuinArt {
 const string Source="Assets/Yendor/Art/Meshy/BrokenArch/arch-preview.fbx";
 const string Art="Assets/Yendor/Art/Ruins";
 const string Prefab=Art+"/BrokenArchStudy.prefab";
 const string Study="Assets/Yendor/Art/Studies/RuinSandstone.mat";
 static string Evidence=>Path.GetFullPath("../artifacts/pilots/yen-8ji");
 static void Guard(){if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Preserve current Play session");}
 public static string Build(){
 Guard();if(File.Exists(Prefab))throw new Exception("Study already exists; re-read before editing");
 var source=AssetDatabase.LoadAssetAtPath<GameObject>(Source);if(!source)throw new Exception("Missing reviewed source");
 var scene=EditorSceneManager.NewPreviewScene();var original=UnityEngine.Object.Instantiate(source);SceneManager.MoveGameObjectToScene(original,scene);
 try {
 var filter=original.GetComponentsInChildren<MeshFilter>().Single();var mesh=filter.sharedMesh;
 var points=mesh.vertices.Select(v=>filter.transform.TransformPoint(v)).ToArray();var tris=mesh.triangles;
 var parents=Enumerable.Range(0,points.Length).ToArray();
 Func<int,int> find=null;find=i=>parents[i]==i?i:parents[i]=find(parents[i]);
 Action<int,int> join=(a,b)=>parents[find(b)]=find(a);
 var weld=new Dictionary<Vector3Int,int>();
 for(int i=0;i<points.Length;i++){var k=new Vector3Int(Mathf.RoundToInt(points[i].x*100000),Mathf.RoundToInt(points[i].y*100000),Mathf.RoundToInt(points[i].z*100000));if(weld.TryGetValue(k,out int previous))join(i,previous);else weld[k]=i;}
 for(int i=0;i<tris.Length;i+=3){join(tris[i],tris[i+1]);join(tris[i],tris[i+2]);}
 var groups=new Dictionary<int,List<int>>();for(int i=0;i<tris.Length;i+=3){int id=find(tris[i]);if(!groups.ContainsKey(id))groups[id]=new List<int>();groups[id].AddRange(new[]{tris[i],tris[i+1],tris[i+2]});}
 var components=groups.Values.Select(ids=>{var b=new Bounds(points[ids[0]],Vector3.zero);foreach(int i in ids)b.Encapsulate(points[i]);return new {ids,b};}).ToArray();
 // Native rendered orientation, not FBX raw axes. Only one component spans both feet and arch.
 var selected=components.Single(c=>c.b.size.y>1.5f && c.b.size.x>1.3f);
 var center=selected.b.center;float scale=4/selected.b.size.y;
 var positions=selected.ids.Select(i=>(points[i]-new Vector3(center.x,selected.b.min.y,center.z))*scale).ToArray();
 var normals=mesh.normals;var newNormals=selected.ids.Select(i=>filter.transform.TransformDirection(normals[i]).normalized).ToArray();
 // Per-face box UVs let a seamless wall study cover the extracted component instead of the source atlas.
 var uvs=new Vector2[positions.Length];
 for(int i=0;i<positions.Length;i+=3){var n=Vector3.Cross(positions[i+1]-positions[i],positions[i+2]-positions[i]).normalized;var a=new Vector3(Mathf.Abs(n.x),Mathf.Abs(n.y),Mathf.Abs(n.z));for(int j=i;j<i+3;j++){var p=positions[j];uvs[j]=a.z>=a.x&&a.z>=a.y?new Vector2(p.x,p.y):a.x>=a.y?new Vector2(p.z,p.y):new Vector2(p.x,p.z);}}
 var derived=new Mesh{name="Reviewed isolated arch",vertices=positions,normals=newNormals,uv=uvs,triangles=Enumerable.Range(0,positions.Length).ToArray()};derived.RecalculateBounds();derived.RecalculateTangents();
 Directory.CreateDirectory(Art);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
 AssetDatabase.CreateAsset(derived,Art+"/BrokenArch.asset");
 var material=new Material(Shader.Find("Standard")){name="Arch review sandstone",color=new Color(.65f,.53f,.36f)};material.SetFloat("_Glossiness",.1f);AssetDatabase.CreateAsset(material,Art+"/Sandstone.mat");
 var root=new GameObject("Broken arch study",typeof(MeshFilter),typeof(MeshRenderer));SceneManager.MoveGameObjectToScene(root,scene);root.GetComponent<MeshFilter>().sharedMesh=derived;root.GetComponent<MeshRenderer>().sharedMaterial=material;
 PrefabUtility.SaveAsPrefabAsset(root,Prefab);AssetDatabase.SaveAssets();
 var report=new {source=Source,prefab=Prefab,originalTriangles=tris.Length/3,components=components.Select(c=>new {triangles=c.ids.Count/3,size=new[]{c.b.size.x,c.b.size.y,c.b.size.z}}),selectedTriangles=positions.Length/3,height=derived.bounds.size.y,width=derived.bounds.size.x,depth=derived.bounds.size.z,removed="Interior columns, floating lintel and detached slivers",uv="1 metre per repeat, per-face box projection",collision="Unplaced review study; no collider until route review"};
 File.WriteAllText(Evidence+"/derived-arch.json",JsonConvert.SerializeObject(report,Formatting.Indented));return "Isolated native arch mesh and unplaced study prefab authored; original FBX unchanged.";
 } finally{EditorSceneManager.ClosePreviewScene(scene);}
 }
 public static string ReviewArch(){Guard();var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Prefab);if(!prefab)throw new Exception("Missing study prefab");var scene=EditorSceneManager.NewPreviewScene();var root=UnityEngine.Object.Instantiate(prefab);SceneManager.MoveGameObjectToScene(root,scene);try{Render(scene,root.GetComponent<Renderer>().bounds,"derived-arch",false);return "Four native derived arch views saved";}finally{EditorSceneManager.ClosePreviewScene(scene);}}
 public static string ReviewMaterial(){
 Guard();var material=AssetDatabase.LoadAssetAtPath<Material>(Study);if(!material || !material.mainTexture)throw new Exception("Generated texture not yet downloaded; blank placeholder is not acceptance");
 var scene=EditorSceneManager.NewPreviewScene();var wall=GameObject.CreatePrimitive(PrimitiveType.Cube);SceneManager.MoveGameObjectToScene(wall,scene);wall.transform.localScale=new Vector3(4,4,.3f);
 var review=new Material(material);review.mainTextureScale=new Vector2(2,2);wall.GetComponent<Renderer>().sharedMaterial=review;
 try{Render(scene,wall.GetComponent<Renderer>().bounds,"material-2x2",true);var report=new {material=Study,guid=AssetDatabase.AssetPathToGUID(Study),shader=material.shader.name,textures=material.GetTexturePropertyNames().Select(n=>new {name=n,path=AssetDatabase.GetAssetPath(material.GetTexture(n))}),width=material.mainTexture.width,height=material.mainTexture.height,graphics=SystemInfo.graphicsDeviceName};File.WriteAllText(Evidence+"/native-material-review.json",JsonConvert.SerializeObject(report,Formatting.Indented));return "Actual generated material rendered as 2x2 repeats; source unchanged";}finally{UnityEngine.Object.DestroyImmediate(review);EditorSceneManager.ClosePreviewScene(scene);}
 }
 static void Render(Scene scene,Bounds bounds,string prefix,bool material){
 var camera=new GameObject("Review camera").AddComponent<Camera>();SceneManager.MoveGameObjectToScene(camera.gameObject,scene);camera.scene=scene;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.07f,.105f,.12f);camera.fieldOfView=32;camera.nearClipPlane=.05f;camera.farClipPlane=100;
 var sun=new GameObject("Review key").AddComponent<Light>();SceneManager.MoveGameObjectToScene(sun.gameObject,scene);sun.type=LightType.Directional;sun.intensity=1.1f;sun.transform.rotation=Quaternion.Euler(35,-35,0);
 var fill=new GameObject("Review fill").AddComponent<Light>();SceneManager.MoveGameObjectToScene(fill.gameObject,scene);fill.type=LightType.Directional;fill.intensity=.45f;fill.transform.rotation=Quaternion.Euler(20,145,0);
 var dirs=material?new[]{Vector3.back}:new[]{Vector3.forward,Vector3.right,Vector3.back,Vector3.left};
 for(int i=0;i<dirs.Length;i++){camera.transform.position=bounds.center+dirs[i]*bounds.size.y*2.2f;camera.transform.LookAt(bounds.center);var rt=RenderTexture.GetTemporary(900,900,24);var old=RenderTexture.active;var texture=new Texture2D(900,900,TextureFormat.RGB24,false);try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;texture.ReadPixels(new Rect(0,0,900,900),0,0);texture.Apply();File.WriteAllBytes(Evidence+"/"+prefix+"-"+i+".png",texture.EncodeToPNG());}finally{camera.targetTexture=null;RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(texture);}}
 }
}
