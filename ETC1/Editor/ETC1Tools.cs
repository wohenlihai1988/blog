using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace ETC1Helper
{
    public class ETC1Tools 
    {
        public const string AlphaAttachString = "_alpha";

        public static void GenerateAlphaResources(UIAtlas atlas_)
        {
            try
            {
                var instanceID = atlas_.gameObject.GetInstanceID();
                var path = AssetDatabase.GetAssetPath(instanceID);
                var tex = SplitAlphaTexture(atlas_.spriteMaterial.mainTexture as Texture2D);
                CreateAlphaMaterial(atlas_.spriteMaterial, tex, path.Replace(".prefab", AlphaAttachString + ".mat"));
                SetAtlasToAlphaSetting(atlas_);

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("atlas " + atlas_.gameObject.name);
            }
        }

        public static void CreateAlphaMaterial(Material srcMat_, Texture alphaTex_, string path_)
        {
            var alphaMat = new Material(srcMat_);
            alphaMat.shader = Shader.Find("Unlit/Transparent Colored Split Alpha");
            alphaMat.SetTexture("_AlphaTex", alphaTex_);
            AssetDatabase.CreateAsset(alphaMat, path_);
        }

        public static void SetAtlasToAlphaSetting(UIAtlas atlas_)
        {
            var instanceID = atlas_.gameObject.GetInstanceID();
            var path = AssetDatabase.GetAssetPath(instanceID);
            var alphaMatPath = path.Replace(".prefab", AlphaAttachString + ".mat");
            var mainTexPath = path.Replace(".prefab", ".png");
            var alphaMat = AssetDatabase.LoadAssetAtPath(alphaMatPath, typeof(Material)) as Material;
            if(null == alphaMat)
            {
                Debug.LogError("can't find alpha texture at " + alphaMatPath);
                return;
            }
            atlas_.spriteMaterial = alphaMat;
            var mainTexImporter = (TextureImporter.GetAtPath(mainTexPath) as TextureImporter);
            ApplyAndroidTextSetting(mainTexImporter);
        }

        public static void SetAtlasToIOSSetting(UIAtlas atlas_)
        {
            var instanceID = atlas_.gameObject.GetInstanceID();
            var path = AssetDatabase.GetAssetPath(instanceID);
            var mormalMatPath = path.Replace(".prefab", ".mat");
            var mainTexPath = path.Replace(".prefab", ".png");
            var normalMat = AssetDatabase.LoadAssetAtPath(mormalMatPath, typeof(Material)) as Material;
            if (null == normalMat)
            {
                Debug.LogError("can't find alpha texture at " + mormalMatPath);
                return;
            }
            atlas_.spriteMaterial = normalMat;
            var mainTexImporter = (TextureImporter.GetAtPath(mainTexPath) as TextureImporter);
            ApplyIOSTexSetting(mainTexImporter);
        }

        public static Texture2D SplitAlphaTexture(Texture2D tex)
        {
            var path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
            (TextureImporter.GetAtPath(path) as TextureImporter).isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var colArray = tex.GetPixels();
            var alphaTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            var alphaArray = new Color[colArray.Length];
            for (int i = 0; i < alphaArray.Length; i++)
            {
                alphaArray[i].r = alphaArray[i].g = alphaArray[i].b = colArray[i].a;
                alphaArray[i].a = 1;
            }
            alphaTex.SetPixels(alphaArray);
            var bytes = alphaTex.EncodeToPNG();
            var srcimporter = (TextureImporter.GetAtPath(path) as TextureImporter);
            ApplyAndroidTextSetting(srcimporter);
            var alphaPath = path.Replace(".png", AlphaAttachString).Replace(".psd", AlphaAttachString).Replace(".jpg", AlphaAttachString);
            alphaPath += ".png";
            File.WriteAllBytes(alphaPath, bytes);
            AssetDatabase.Refresh();
            var importer = (TextureImporter.GetAtPath(alphaPath) as TextureImporter);
            ApplyAndroidTextSetting(importer);
            AssetDatabase.ImportAsset(alphaPath, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath(alphaPath, typeof(Texture2D)) as Texture2D;
        }

        public static void ApplyAndroidTextSetting(TextureImporter importer_)
        {
            if (null != importer_)
            {
                importer_.textureType = TextureImporterType.GUI;
                importer_.textureFormat = TextureImporterFormat.ETC_RGB4;
                importer_.mipmapEnabled = false;
                importer_.isReadable = false;

                importer_.SetPlatformTextureSettings("Android", 4096, importer_.textureFormat);
                AssetDatabase.ImportAsset(importer_.assetPath, ImportAssetOptions.ForceUpdate);
            }
        }

        public static void ApplyIOSTexSetting(TextureImporter importer_)
        {
            if (null != importer_)
            {
                importer_.textureType = TextureImporterType.GUI;
                importer_.textureFormat = TextureImporterFormat.ARGB32;
                importer_.mipmapEnabled = false;
                importer_.isReadable = false;

                importer_.SetPlatformTextureSettings("iPhone", 4096, importer_.textureFormat);
                AssetDatabase.ImportAsset(importer_.assetPath, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
