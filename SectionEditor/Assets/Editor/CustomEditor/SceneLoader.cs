using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneLoader : MonoBehaviour {

	static SceneLoader _instance;
	public static SceneLoader Instance{
		get{
			return _instance;
		}
	}

	void Awake(){
		if (null != Instance) {
			Debug.LogError ("two many instances !!");
			return;
		}
		_instance = this;
	}

	public void StartLoad(string path){
		StartCoroutine (CoroutineLoadScene (path));
	}

	IEnumerator CoroutineLoadScene(string path){
		FileInfo info = new FileInfo (path);
		while (!info.Exists) {
			yield return new WaitForSeconds (1f);
		}
		WWW www = new WWW (path);
		while (!www.isDone) {
			yield return new WaitForSeconds (1f);
		}
		var ab = www.assetBundle;
		EditorSceneManager.OpenScene (path);
	}
}
