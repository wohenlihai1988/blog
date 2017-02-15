using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.IO;
using System;
using CustomEditor;

public class ThreadManager {
	static ThreadManager _instance = new ThreadManager();
	public static ThreadManager Instance{
		get{
			return _instance;
		}
	}

	public bool Active{
		get{ 
			return _started;
		}
	}

	bool _started;
	Thread _thread;
	// Use this for initialization
	public void Start () {
		_started = true;
		_thread = new Thread (WaitABInited);
		_thread.IsBackground = true;
		_thread.Start ();
	}

	string _path;
	public void SetResPath(string path){
		_path = path;
	}

	public void WaitABInited(){
		while (true) {
			while (string.IsNullOrEmpty(_path)) {
				Thread.Sleep (1000);
			}
			FileInfo info = new FileInfo (_path);
			while (!info.Exists) {
				Thread.Sleep (1000);
			}
		}
	}
				
}
