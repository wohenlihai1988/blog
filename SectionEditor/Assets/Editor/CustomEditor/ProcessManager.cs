using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;

public class ProcessManager {

	static Process _process;

	// Use this for initialization
	public static void Start (string path) {
		var index = path.IndexOf ("Assets/");
		var projPath = path.Substring (0, index);
		var filePath = path.Substring (index, path.Length - index);
		_process = new Process ();
		_process.StartInfo.FileName = @"D:\Program Files\Unity5.5\Editor\Unity.exe";
//		-nographics
		_process.StartInfo.Arguments = "-projectPath " + projPath + " -batchmode -executeMethod InterfaceForBatchMode.CreateTmpAB " + filePath;
//		_process.StartInfo.Arguments = "-projectPath " + projPath + " -executeMethod InterfaceForBatchMode.CreateTmpAB " + filePath;
		_process.Start ();
	}

	public static void Kill(){
		if (null != _process) {
			_process.Kill ();
		}
	}
}
