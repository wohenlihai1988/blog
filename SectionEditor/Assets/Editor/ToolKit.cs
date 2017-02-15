using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ToolKit{
	public static string GetName(string path){
		var end = path.LastIndexOf ("/" [0]);
		return path.Substring (end, path.Length - end);
	}
}