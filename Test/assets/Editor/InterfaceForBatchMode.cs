using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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
		return array [0];
	}

	class SceneData{
		
	}

	static void GenerateGoTree(ref  TreeData<Transform> treeParent, Transform goParent){
		TreeData<Transform> data = new TreeData<Transform> (goParent);
		data.SetParent (treeParent);
		for (int i = 0; i < goParent.childCount; i++) {
			GenerateGoTree (ref data, goParent.GetChild (i));
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
		if (null == data.Data) {
			output += "";
		} else {
			output += (data.Data as Transform).name;
		}
		output += "{";
		for (int i = 0; i < data.childList.Count; i++) {
			Serialize<T> (ref output, data.childList [i]);
		}
		output += "}";
	}
}