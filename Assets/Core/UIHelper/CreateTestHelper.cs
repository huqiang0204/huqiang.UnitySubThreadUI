using huqiang;
using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using System.IO;
using UnityEngine;

public class CreateTestHelper: UICompositeHelp
{
    
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
    public void Initital()
    {
       LoadBundle();
       TestHelper.InitialUI();
        var c = transform.childCount;
        for (int i = 0; i < c; i++)
            GameObject.Destroy(transform.GetChild(i).gameObject);
        App.Initial(transform);
    }
    public void Build()
    {
        if(!Application.isPlaying)
        {
            if (App.uiroot != null)
            {
                App.uiroot.Dispose();
            }
            int c = transform.childCount - 1;
            for (; c >= 0; c--)
            {
                GameObject.DestroyImmediate(transform.GetChild(c).gameObject);
            }
            var caret = InputCaret.Caret;
            if (caret != null)
                GameObject.DestroyImmediate(caret.gameObject);
            Initital();
            CreateTestPage();
            App.Update();
        }
    }
    public virtual void CreateTestPage()
    {

    }
    private void OnDestroy()
    {
        App.Dispose();
        AssetBundle.UnloadAllAssetBundles(true);
        ElementAsset.bundles.Clear();
    }
}
