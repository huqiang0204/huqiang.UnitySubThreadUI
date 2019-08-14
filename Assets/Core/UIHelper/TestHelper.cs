using huqiang;
using huqiang.Communication;
using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using System.IO;
using UnityEngine;


/// <summary>
/// 挂载在Canvas下
/// </summary>
public class TestHelper:UICompositeHelp
{
    public static void InitialUI()
    {
        App.RegUI();
    }
    public virtual void LoadBundle()
    {
#if UNITY_EDITOR
        if (ElementAsset.bundles.Count == 0)
        {
            //var dic = Application.dataPath + "/StreamingAssets";
            var dic = Application.streamingAssetsPath;
            if (Directory.Exists(dic))
            {
                var bs = Directory.GetFiles(dic, "*.unity3d");
                for (int i = 0; i < bs.Length; i++)
                {
                    ElementAsset.bundles.Add(AssetBundle.LoadFromFile(bs[i]));
                }
            }
        } 
#endif
    }
    public string AssetName = "baseUI";
    private void Awake()
    {
        Initital();
        LoadTestPage();
    }
    public void Initital()
    {
        LoadBundle();
        InitialUI();
        DataBuffer db = new DataBuffer(1024);
        db.fakeStruct = ModelElement.LoadFromObject(transform, db);
        PrefabAsset asset = new PrefabAsset();
        asset.models = new ModelElement();
        asset.models.Load(db.fakeStruct);
        asset.name = AssetName;
        ModelManagerUI.prefabs.Add(asset);
        var c = transform.childCount;
        for (int i = 0; i < c; i++)
            GameObject.Destroy(transform.GetChild(i).gameObject);
        App.Initial(transform);
    }
    
    public virtual void LoadTestPage()
    {
    }

    private void Update()
    {
        App.Update();
        OnUpdate();
    }
    public virtual void OnUpdate()
    {
    }
    private void OnDestroy()
    {
        App.Dispose();
        AssetBundle.UnloadAllAssetBundles(true);
        ElementAsset.bundles.Clear();
    }
}