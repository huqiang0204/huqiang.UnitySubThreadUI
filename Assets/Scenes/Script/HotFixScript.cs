using DataControll;
using HotFix;
using huqiang;
using huqiang.Data;
using huqiang.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotFixScript : MonoBehaviour
{
    public static HotFixScript Instance;
    public TextAsset baseUI;
    public TextAsset hotfix;
    byte[] hotData;
    bool loadOver;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        KcpDataControll.Instance.Connection("127.0.0.1",8886);
        App.Initial(transform as RectTransform);
        ModelManagerUI.LoadModels(baseUI.bytes, "baseUI");
#if UNITY_EDITOR
        AssetBundle.UnloadAllAssetBundles(true);
#endif
        ElementAsset.LoadAssetsAsync("base.unity3d").PlayOver = (o, e) =>
        {
            loadOver = true;
            if (hotData != null)
                UIPage.LoadPage<HotFixEntry>(hotData);
           // UIPage.LoadPage<HotFixEntry>(hotfix);
        };
    }

    // Update is called once per frame
    void Update()
    {
        App.Update();
        KcpDataControll.Instance.DispatchMessage();
    }
    private void OnApplicationQuit()
    {
        App.Dispose();
    }
    public void Cmd(DataBuffer buffer)
    {
        var fake = buffer.fakeStruct;
        if(fake!=null)
        {
            switch (fake[Req.Cmd])
            {
                case 1:
                    hotData = fake.GetData<byte[]>(Req.Args);
                    if(loadOver)
                        UIPage.LoadPage<HotFixEntry>(hotData);
                    break;
            }

        }
    }
}
