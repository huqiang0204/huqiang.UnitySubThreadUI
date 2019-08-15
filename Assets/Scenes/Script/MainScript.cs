using huqiang;
using huqiang.Data;
using huqiang.UI;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public TextAsset baseUI;
    // Start is called before the first frame update
    void Start()
    {
        App.Initial(transform as RectTransform);
        ModelManagerUI.LoadModels(baseUI.bytes, "baseUI");
#if UNITY_EDITOR
        AssetBundle.UnloadAllAssetBundles(true);
#endif
        ElementAsset.LoadAssetsAsync("base.unity3d").PlayOver = (o, e) =>
        {
            UIPage.LoadPage<TestPage>();
        };
    }

    // Update is called once per frame
    void Update()
    {
        App.Update();
    }
    private void OnApplicationQuit()
    {
        App.Dispose();
    }
}
