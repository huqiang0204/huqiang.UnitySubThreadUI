using huqiang.Data;
using huqiang.UI;
using System.IO;
using UGUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ElementCreate), true)]
[CanEditMultipleObjects]
public class ElementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        ElementCreate ele = target as ElementCreate;
        if (GUILayout.Button("Clear All AssetBundle"))
        {
            AssetBundle.UnloadAllAssetBundles(true);
            ElementAsset.bundles.Clear();
            EditorModelManager.Clear();
        }
        if (GUILayout.Button("Create"))
        {
            Create(ele.Assetname, ele.dicpath, ele.gameObject);
        }
        if (GUILayout.Button("Clone"))
        {
            if (ele.bytesUI != null)
                Clone(ele.CloneName, ele.bytesUI.bytes, ele.transform);
        }
        if (GUILayout.Button("CloneAll"))
        {
            if (ele.bytesUI != null)
                CloneAll(ele.bytesUI.bytes, ele.transform);
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
                    ElementAsset.bundles.Add(AssetBundle.LoadFromFile(bs[i]));
                }
            }
        }
    }
    static void Create(string Assetname, string dicpath, GameObject gameObject)
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
        dc += Assetname;
        InitialUI();
        ModelManagerUI.SavePrefab(gameObject, dc);
        Debug.Log("create done path:"+dc);
    }
    static void Clone(string CloneName, byte[] ui, Transform root)
    {
        if (ui != null)
        {
            if (CloneName != null)
                if (CloneName != "")
                {
                    LoadBundle();
                    InitialUI();
                    ModelManagerUI.LoadModels(ui, "assTest");
                    EditorModelManager.LoadToGame(CloneName, null, root, "");
                }
        }
    }
    static void CloneAll(byte[] ui, Transform root)
    {
        if (ui != null)
        {
            LoadBundle();
            InitialUI();
            var all = ModelManagerUI.LoadModels(ui, "assTest");
            ModelElement element = new ModelElement();
            element.Load(all.models.ModData);
            element.Apply();
        }
    }
    static void InitialUI()
    {
        ModelManagerUI.RegComponent(new ComponentType<RectTransform, ModelElement>(ModelElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Image, ImageElement>(ImageElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<EmojiText, EmojiElement>(TextElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Text, TextElement>(TextElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<CustomRawImage, RawImageElement>(RawImageElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<RawImage, RawImageElement>(RawImageElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<RectMask2D, RectMaskElement>(RectMaskElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Mask, MaskElement>(MaskElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Outline, OutLineElement>(OutLineElement.LoadFromObject));
    }
}