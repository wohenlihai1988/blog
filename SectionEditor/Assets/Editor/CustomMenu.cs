using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomMenu {

	[MenuItem("Test/Lock")]
	static void LockPanel(){
		var type = typeof(EditorWindow).Assembly.GetType ("UnityEditor.InspectorWindow");
		var window = EditorWindow.GetWindow (type);
		var info = type.GetMethod ("FlipLocked", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		info.Invoke (window, null);
	}
}
