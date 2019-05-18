using huqiang;
using huqiang.Data;
using huqiang.UI;
using System.IO;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 挂载在Canvas下
/// </summary>
public class TestHelper:UICompositeHelp
{
    static void InitialUI()
    {
        ModelManagerUI.RegComponent(new ComponentType<RectTransform, ModelElement>(ModelElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Image, ImageElement>(ImageElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<EmojiText, EmojiElement>(TextElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Text, TextElement>(TextElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<CustomRawImage, RawImageElement>(RawImageElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<RawImage, RawImageElement>(RawImageElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Mask, MaskElement>(MaskElement.LoadFromObject));
        ModelManagerUI.RegComponent(new ComponentType<Outline, OutLineElement>(OutLineElement.LoadFromObject));
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
    private void Awake()
    {
        LoadBundle();
        InitialUI();
        DataBuffer db = new DataBuffer(1024);
        db.fakeStruct = ModelElement.LoadFromObject(transform,db);
        PrefabAsset asset = new PrefabAsset();
        asset.models = new ModelElement();
        asset.models.Load(db.fakeStruct);
        asset.name = "baseUI";
        ModelManagerUI.prefabs.Add(asset);
        var c = transform.childCount;
        for (int i = 0; i < c; i++)
            GameObject.Destroy(transform.GetChild(i).gameObject);
        App.Initial(transform);
        UIPage.LoadPage<LoadingPage>();
    }
    private void Update()
    {
        App.Update();
    }
    private void OnDestroy()
    {
        App.Dispose();
        AssetBundle.UnloadAllAssetBundles(true);
        ElementAsset.bundles.Clear();
    }
}