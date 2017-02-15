using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace CustomEditor{
	public class Util {
		public static List<GameObject> _goList = new List<GameObject>();

		public static GameObject GetGameObject(string name, Vector3 pos){
			var go = new GameObject (name);
			go.transform.position = pos;
			_goList.Add (go);
			return go;
		}

		public static Vector3 ParseVector3(string posStr){
			var array = posStr.Split ("," [0]);
			if (array.Length < 3) {
				Debug.LogError ("pos string is illegal");
				return Vector3.zero;
			}
			var x = int.Parse (array [0]);
			var y = int.Parse (array [1]);
			var z = int.Parse (array [2]);
			return new Vector3 (x, y, z);
		}

		public static Vector2 ParseVector2(string posStr){
			var array = posStr.Split ("," [0]);
			if (array.Length < 2) {
				Debug.LogError ("pos string is illegal");
				return Vector3.zero;
			}
			var x = int.Parse (array [0]);
			var y = int.Parse (array [1]);
			return new Vector2 (x, y);
		}

		public static void ClearSceneObjs(){
			for (int i = 0; i < _goList.Count; i++) {
				GameObject.DestroyImmediate (_goList [i]);
			}
		}

		public static void SaveJsonFile<T>(T data, string name){
			var path = Const.jsonFilePath + name;
			var json = JsonUtility.ToJson (data);
			File.WriteAllText (path, json);
			AssetDatabase.Refresh ();
		}

		public static T LoadJsonFile<T>(string name){
			var txt = File.ReadAllText (Const.jsonFilePath + name);
			return JsonUtility.FromJson<T> (txt);
		}

		public static string GetAssetFolderDir(string path){
			var index = path.IndexOf ("Assets/");
			return path.Substring (0, index + "Assets/".Length);
		}

	}
}
