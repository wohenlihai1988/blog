using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainEditor{
	[MenuItem("Terrain/Clone")]
	static void Clone(){
		var go = Selection.activeGameObject;
		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 2; j++) {
				var old = go.GetComponent<Terrain> ();
				var current = new GameObject ("Clone" + i + "_" + j).AddComponent<Terrain> ();
				var oldData = old.terrainData;
				current.terrainData = Object.Instantiate (oldData) as TerrainData;
				current.terrainData.heightmapResolution = oldData.heightmapResolution / 2 + 5;
				var size = oldData.heightmapResolution / 2;
				var pickSize = size + 5;
				var xstart = size * i;
				var ystart = size * i;
				if (xstart + pickSize >= oldData.heightmapResolution - 1 || ystart + pickSize >= oldData.heightmapResolution - 1) {
					pickSize = size - 1;
				}
				var heights = oldData.GetHeights (0, 0, 200, 200);
				heights = oldData.GetHeights (xstart, ystart, pickSize, pickSize);
				current.terrainData.SetHeights (0, 0, heights);
				var col = current.gameObject.AddComponent<TerrainCollider> ();
				col.terrainData = current.terrainData;
				current.transform.position = old.transform.position + new Vector3 ((size * i) * 2, 0, (size * j) * 2);
			}
		}
	}
}