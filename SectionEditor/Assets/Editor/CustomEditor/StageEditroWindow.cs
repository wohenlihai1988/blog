using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using OfficeOpenXml;
using System;
using Excelutil;
using ExcelClass;
using UnityEngine.SceneManagement;

namespace CustomEditor{

	public class DirCache{
		public string scenePath;
		public string stagePath;
	}

	public class StageEditroWindow : EditorWindow {
		[MenuItem("Editor/StageEditor")]
		static void Init(){
			EditorWindow.GetWindow<StageEditroWindow> (false, "StageEditor", true);
			SelectFolder ();
		}

		static void SelectFolder(bool force = false){
			FileInfo info = new FileInfo (Const.jsonFilePath + Const.dirCacheName);
			if (!info.Exists || force) {
				DirCache cache = new DirCache ();
				cache.scenePath = EditorUtility.OpenFolderPanel ("SelectScenePath", "", "");
				cache.stagePath = EditorUtility.OpenFolderPanel ("SelectStagePath", "", "");
				Util.SaveJsonFile (cache, Const.dirCacheName); 
			}
		}

		AssetBundle _lastab;
		GameObject _lastScene;

		readonly int _rowStart = 3;
		List<GameObject> _goList = new List<GameObject>();

		class CfgContainer{
			public List<SectionCfg> sectionCfgList;
			public List<ConfigCfg> configCfgList;
			public List<EventCfg> eventCfgList;
			public List<SpawnerCfg> spawnerCfgList;
		}
		List<StageCfg> _cfgList = new List<StageCfg>();
		List<SectionCfg> _sectionList = new List<SectionCfg>();
		List<ConfigCfg> _configList = new List<ConfigCfg>();
		List<EventCfg> _eventList = new List<EventCfg>();
		List<SpawnerCfg> _spawnerList = new List<SpawnerCfg>();
		SelectionItem _stageSelection = new SelectionItem();
		SelectionItem _sectionSelection = new SelectionItem();
		bool _panelLocked = false;
		string _selectedPath;
		string _stagePath;
		string _scenePath;
		string _selectedDir;

		public void OnGUI ()
		{
			GUILayout.BeginHorizontal ();
			if(GUILayout.Button("kill", GUILayout.Width(50))){
				ProcessManager.Kill();
			}
			if (GUILayout.Button ("SelectFolder", GUILayout.Width (100))) {
				SelectFolder (true);
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("Load", GUILayout.Width(50))) {
				if (!_panelLocked) {
					LockPanel ();
					_panelLocked = true;
				}
				Reset ();
				var cache = Util.LoadJsonFile<DirCache> (Const.dirCacheName);
				_scenePath = cache.scenePath;
				_selectedPath = EditorUtility.OpenFilePanel ("SelectFile", cache.stagePath, "");
				if (string.IsNullOrEmpty (_selectedPath)) {
					return;
				}
				var end = _selectedPath.LastIndexOf ("/"[0]);
				_selectedDir = _selectedPath.Substring (0, end);
				var cfg = ExcelUtilMain.Instance.Parse<StageCfg> (_selectedPath);
				// start form 3 ,avoid Name and Desc
				for (int i = _rowStart; i < cfg.Count; i++) {
					_stageSelection.SelectionList.Add (cfg [i].id);
					_cfgList.Add (cfg [i]);
				}
			}
			GUILayout.BeginVertical ();
			GUILayout.Label ("StageList");
			// stage list
			_stageSelection.CurIndex = GUILayout.SelectionGrid (_stageSelection.CurIndex, _stageSelection.SelectionList.ToArray(), 10);
			if(_stageSelection.Changed){
				// clear last stage info
				ResetStageInfo ();
				if (null != _lastScene) {
					GameObject.DestroyImmediate (_lastScene);
				}
				if (null != _lastab) {
					_lastab.Unload (true);
				}

				if (_stageSelection.CurIndex < _cfgList.Count) {
					_stagePath = _selectedDir + "/" + _cfgList[_stageSelection.CurIndex].stagePath;
					if (Directory.Exists (_stagePath)) {
						FileInfo info = new FileInfo (_stagePath + "/" + Const.SectionName);
						if (info.Exists) { 
							LoadExistFiles ();
						} else {
							CreateDefaultFiles ();
							LoadExistFiles ();
						}
					} else {
						Directory.CreateDirectory (_stagePath);
						CreateDefaultFiles ();
						LoadExistFiles ();
					}

					var scenePath = _scenePath + "/" + _cfgList [_stageSelection.CurIndex].scenePath;
					ProcessManager.Start (scenePath);
					_sceneLoaded = false;
				}
			}

			GUILayout.Box ("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			GUILayout.Label ("SectionList");
			//setcion list
			_sectionSelection.CurIndex = GUILayout.SelectionGrid(_sectionSelection.CurIndex, _sectionSelection.SelectionList.ToArray(), 10);
			if (_sectionSelection.Changed) {
				if (_sectionSelection.CurIndex < _cfgList.Count) {
					GenerateSceneObjs ();
				}
			}
			GUILayout.EndVertical ();
			UpdateSceneRes ();
		}


		bool _sceneLoaded;
		void UpdateSceneRes(){
			if (_sceneLoaded) {
				return;
			}
			if (string.IsNullOrEmpty (_scenePath)) {
				return;
			}
			if (_stageSelection.CurIndex > _cfgList.Count - 1) {
				return;
			}
			var path = Util.GetAssetFolderDir (_scenePath) + "Temp/" + Path.GetFileName (_cfgList [_stageSelection.CurIndex].scenePath);
			FileInfo info = new FileInfo (path);
			if (!info.Exists) {
				return;
			}
			var bytes = File.ReadAllBytes (path);
			var ab = AssetBundle.LoadFromMemory (bytes);
			if (null == ab) {
				Debug.LogError ("no assetbundle is loaded");
				_sceneLoaded = true;
				return;
			}
			try{
				_lastab = ab;
				InstantiateAB(ab);
			}finally{
				_sceneLoaded = true;
				ProcessManager.Kill ();
			}
		}

		void InstantiateAB(AssetBundle ab){
			var assetArray = ab.GetAllAssetNames ();
			for(int i = 0; i < assetArray.Length; i++){
				var obj = ab.LoadAsset(assetArray[i]);
				_lastScene = GameObject.Instantiate(obj) as GameObject;
			}
		}

		void Reset(){
			_cfgList.Clear ();
			_stageSelection.Reset ();
			ResetStageInfo ();
		}

		void ResetStageInfo(){
			_sectionList.Clear ();
			_eventList.Clear ();
			_configList.Clear ();
			_spawnerList.Clear ();
			_sectionSelection.Reset ();
		}

		// lock panel to avoid inspector change
		void LockPanel(){
			var type = typeof(EditorWindow).Assembly.GetType ("UnityEditor.InspectorWindow");
			var window = EditorWindow.GetWindow (type);
			var info = type.GetMethod ("FlipLocked", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			info.Invoke (window, null);
		}

		void CreateDefaultFiles(){
			var sectionCfg = new SectionCfg ();
			_sectionList.Add (sectionCfg);
			var cache = Resources.Load ("DefaultCache") as TextAsset;
			if (null == cache) {
				EditorUtility.DisplayDialog ("Error", "No Cache Exist", "Ok");
				return;
			}
			var container = JsonUtility.FromJson<CfgContainer> (cache.text);
			ExcelUtilMain.Instance.Create<SectionCfg> (_stagePath + "/" + Const.SectionName, container.sectionCfgList);
			ExcelUtilMain.Instance.Create<ConfigCfg> (_stagePath + "/" + Const.configName, container.configCfgList);
			ExcelUtilMain.Instance.Create<EventCfg> (_stagePath + "/" + Const.EventName, container.eventCfgList);
			ExcelUtilMain.Instance.Create<SpawnerCfg> (_stagePath + "/" + Const.SpawnerName, container.spawnerCfgList);
			AssetDatabase.Refresh ();
		}

		void LoadExistFiles(){
			_sectionList = ExcelUtilMain.Instance.Parse<SectionCfg> (_stagePath + "/" + Const.SectionName);
			for(int i = _rowStart; i < _sectionList.Count; i++){
				_sectionSelection.SelectionList.Add (_sectionList [i].sectionId);
			}
			_configList = ExcelUtilMain.Instance.Parse<ConfigCfg> (_stagePath + "/" + Const.configName);
			_eventList = ExcelUtilMain.Instance.Parse<EventCfg> (_stagePath + "/" + Const.EventName);
			_spawnerList = ExcelUtilMain.Instance.Parse <SpawnerCfg> (_stagePath + "/" + Const.SpawnerName);
			var cache = Resources.Load ("DefaultCache") as TextAsset;
			if (null == cache) {
				GenerateCacheFiles ();
			}
		}

		void GenerateCacheFiles(){
			CfgContainer container = new CfgContainer ();
			container.sectionCfgList = _sectionList;
			container.configCfgList = _configList;
			container.eventCfgList = _eventList;
			container.spawnerCfgList = _spawnerList;
			var cacheJson = JsonUtility.ToJson (container);
			File.WriteAllText(Application.dataPath + "/Resources/DefaultCache.json", cacheJson);
			AssetDatabase.Refresh ();
		}

		void GenerateSceneObjs(){
			Util.ClearSceneObjs ();
			for (int i = _rowStart; i < _sectionList.Count; i++) {
				var pos = _sectionList [i].sectionPoint;
				var size = _sectionList [i].sectionSize;
				var posV2 = Util.ParseVector2 (pos);
				var go = Util.GetGameObject (_sectionList [i].sectionId, new Vector3 (posV2.x, 0, posV2.y));
				var col = go.AddComponent<BoxCollider> ();
				var extent = Util.ParseVector2 (size);
				col.size = new Vector3(extent.x, 0, extent.y);
				Selection.activeGameObject = go;
			}
		}

	}
}