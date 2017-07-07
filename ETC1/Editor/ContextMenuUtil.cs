using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ETC1Helper
{
    public class ContextMenuUtil 
    {
        [MenuItem("Tools/SplitAlpha")]
        [MenuItem("Assets/ETC1Helper/SplitAlpha", false, 0)]
        static void SplitAlpha()
        {
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            var go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (null == go)
            {
                EditorUtility.DisplayDialog("error ", "please select a atlas prefab ", "OK");
                return;
            }
            var atlas = go.GetComponent<UIAtlas>();
            if (null == atlas)
            {
                EditorUtility.DisplayDialog("error ", "please select a atlas prefab ", "OK");
                return;
            }
            ETC1Tools.GenerateAlphaResources(atlas);
        }

        static List<UIAtlas> CollectAllAtlas()
        {
            var instanceId = Selection.activeInstanceID;
            AssetDatabase.GetAssetPath(instanceId);
            var path = AssetDatabase.GetAssetPath(instanceId);
            var array = AssetDatabase.LoadAllAssetsAtPath(path);
            var items = new List<string>();
            var atlasList = new List<UIAtlas>();
            LoopAllItems(path, items);
            foreach (var item in items)
            {
                if (item.EndsWith(".prefab"))
                {
                    var go = AssetDatabase.LoadAssetAtPath(item, typeof(GameObject)) as GameObject;
                    if (null == go)
                    {
                        Debug.LogError("go is null !! at " + item);
                        continue;
                    }
                    var atlas = go.GetComponent<UIAtlas>();
                    if(null != atlas)
                    {
                        atlasList.Add(atlas);
                    }
                    else
                    {
                        Debug.LogError("can't find UIAtlas in " + item);
                    }
                }
            }
            return atlasList;
        }

        [MenuItem("Assets/ETC1Helper/SwitchToIOS", false, 0)]
        static void BuildIOS()
        {
            var list = CollectAllAtlas();
            foreach (var atlas in list)
            {
                var instanceID = atlas.gameObject.GetInstanceID();
                var path = AssetDatabase.GetAssetPath(instanceID);
                var normalMatPath = path.Replace(".prefab", ".mat");
                var texPath = path.Replace(".prefab", ".png");
                if (!File.Exists(normalMatPath))
                {
                    Debug.LogError("NANI ?? !!!");
                }
                else
                {
                    ETC1Tools.SetAtlasToIOSSetting(atlas);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/ETC1Helper/SwitchToAndroid", false, 0)]
        static void BuildAndroid()
        {
            var list = CollectAllAtlas();
            var i = 0;
            foreach(var atlas in list)
            {
                if(null == atlas)
                {
                    Debug.LogError("null atlas " + i); 
                    continue;
                }
                var instanceID = atlas.gameObject.GetInstanceID();
                var path = AssetDatabase.GetAssetPath(instanceID); 
                var alphaMatPath = path.Replace(".prefab", ETC1Tools.AlphaAttachString + ".mat");
                if (!File.Exists(alphaMatPath))
                {
                    ETC1Tools.GenerateAlphaResources(atlas);
                }
                else
                {
                    ETC1Tools.SetAtlasToAlphaSetting(atlas);
                }
                i++;
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        static void LoopAllItems(string dir_, List<string> items_)
        {
            var dirs = Directory.GetDirectories(dir_);
            foreach(var dir in dirs)
            {
                LoopAllItems(dir, items_);
            }
            var items = Directory.GetFiles(dir_);
            foreach(var item in items)
            {
                items_.Add(item); 
            }
        }
    }
}
