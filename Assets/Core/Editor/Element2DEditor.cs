using huqiang.Manager2D;
using huqiang.Data2D;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UGUI;
using UnityEditor;
using UnityEngine;
using huqiang.Data;

[CustomEditor(typeof(Element2DCreate), true)]
[CanEditMultipleObjects]
public class Element2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        Element2DCreate ele = target as Element2DCreate;
        if (GUILayout.Button("Clear All AssetBundle"))
        {
            AssetBundle.UnloadAllAssetBundles(true);
            ElementAsset.bundles.Clear();
        }
        if (GUILayout.Button("Create"))
        {
            Create(ele.Assetname, ele.dicpath, ele.gameObject,ele);
        }
        if (GUILayout.Button("Clone"))
        {
            if (ele.bytes != null)
                Clone(ele.CloneName, ele.bytes.bytes, ele.transform,ele);
        }
        serializedObject.ApplyModifiedProperties();
    }
    static void LoadBundle()
    {
        if (ElementAsset.bundles.Count == 0)
        {
            var dic = Application.dataPath + "/StreamingAssets";
            if (Directory.Exists(dic))
            {
                var bs = Directory.GetFiles(dic, "*.unity3d");
                for (int i = 0; i < bs.Length; i++)
                {
                    var ass = AssetBundle.LoadFromFile(bs[i]);
                    ElementAsset.AddBundle(ass);
                }
            }
        }

    }
    static void Create(string Assetname, string dicpath, GameObject gameObject, Element2DCreate ele)
    {
        if (Assetname == null)
            return;
        if (Assetname == "")
            return;
        LoadBundle();
        Assetname = Assetname.Replace(" ", "");
        
        var dc = dicpath;
        if (dc == null | dc == "")
        {
            dc = Application.dataPath + "/AssetsBundle/";
        }
        ModelManager2D.Initial(gameObject.transform);
        ele.RegExtendComponent();
        ModelManager2D.SavePrefab(gameObject.transform, dc + Assetname);
        Debug.Log("create done");
    }
    static void Clone(string CloneName, byte[] ui, Transform root, Element2DCreate ele)
    {
        if (ui != null)
        {
            if (CloneName != null)
                if (CloneName != "")
                {
                    LoadBundle();
                    ModelManager2D.Initial(root);
                    ele.RegExtendComponent();
                    var assets = ModelManager2D.LoadModels(ui, "assTest");
                    var fake = ModelManager2D.GameBuffer.FindChild(assets.models, CloneName);
                    var go = ModelManager2D.GameBuffer.Clone(fake);
                    go.transform.SetParent(root);
                }
        }
    }
    static void CloneAll(byte[] ui, Transform root)
    {
        if (ui != null)
        {
            LoadBundle();
            //ModelManager2D.Initial();
            //var all = ModelManager2D.LoadModels(ui, "assTest");
            //var models = all.models.child;
            //for (int i = 0; i < models.Count; i++)
            //    EditorModelManager2D.LoadToGame(models[i], null, root, "");
        }
    }
}
