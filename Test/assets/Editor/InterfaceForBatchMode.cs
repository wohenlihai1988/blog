using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class InterfaceForBatchMode {

	[MenuItem("Custom/GetAllAssets")]
	public static string GetAllAssets(){
		var id = Selection.activeInstanceID;
		var array = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(id));
		for (int i = 0; i < array.Length; i++) {
			Debug.LogError (array [i]);
		}

		var scene = EditorSceneManager.OpenScene (AssetDatabase.GetAssetPath(id), OpenSceneMode.Single);
		var goArray = scene.GetRootGameObjects ();
		var root = new GameObject ("SceneRoot");
		TreeData<Transform> data = new TreeData<Transform>(root.transform);
		for (int i = 0; i < goArray.Length; i++) {
			GenerateGoTree (ref data, goArray [i].transform);
		}
		string output = "";
		Serialize<Transform> (ref output, data);
		Debug.LogError (output);
		GameObject.DestroyImmediate(root);
		return array [0];
	}

	class SceneData{
		
	}

	static void GenerateGoTree(ref TreeData<Transform> treeParent, Transform current){
		TreeData<Transform> data = new TreeData<Transform> (current);
		data.SetParent (treeParent);
		for (int i = 0; i < current.childCount; i++) {
			GenerateGoTree (ref data, current.GetChild(i));
		}
	}

	[System.Serializable]
	class TreeData<T>{
		public TreeData<T> _parent;
		[SerializeField]
		public List<TreeData<T>> childList = new List<TreeData<T>>();
		public T _data;

		public T Data{
			get{ 
				return _data;
			}
			set{ 
				_data = value;
			}
		}

		public TreeData(T data){
			_data = data;
		}

		public void SetParent(TreeData<T> parent){
			_parent = parent;
			parent.AddChild (this);
		}

		public void AddChild(TreeData<T> data){
			childList.Add (data);
			data._parent = this;
		}

		public TreeData<T> GetChild(int index){
			return childList [index];
		}
	}

	static void Serialize<T> (ref string output, TreeData<T> data){
		output += "{";
		LoopSerialize<T>(ref output, data);
		output += "}";
	}

	static void LoopSerialize<T> (ref string output, TreeData<T> data){
		var trans = data.Data as Transform;
		output += "\"" + trans.name + "\":{";
		if(trans.localPosition != Vector3.zero){
			output += "\"p\":\"" + trans.localPosition.ToString() + "\"";
		}
		if(trans.localEulerAngles != Vector3.zero){
			output += ",\"r\":\"" + trans.localEulerAngles.ToString() + "\"";
		}
		if(trans.localScale != Vector3.one){
			output += ",\"s\":\"" + trans.localScale.ToString() + "\"";
		}
		if(data.childList.Count > 0){
			output += ",\"childs\":{";
			for (int i = 0; i < data.childList.Count; i++) {
				LoopSerialize<T> (ref output, data.childList [i]);
			}
			output += "}";
		}
		output += "}";
	}

	#region assetbundle test

	static void OutputAB(){
		var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeInstanceID));
		importer.assetBundleName = "tmp";
		var manifest = BuildPipeline.BuildAssetBundles(Application.dataPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneOSXIntel64);
		var array = manifest.GetAllAssetBundles();
		for(int i = 0; i < array.Length; i++){
			Debug.LogError(array[i]);
		}
		importer.assetBundleName = "";
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[MenuItem("Test/LoadAB")]
	static void LoadAB(){
		AssetBundle ab = new AssetBundle();
		try{
			ab = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "tmp"));
			var array = ab.GetAllAssetNames();
			for(int i = 0; i < array.Length; i++){
				Debug.LogError(array);
			}
		}finally{
			ab.Unload(true);
		}
	}

	#endregion
}