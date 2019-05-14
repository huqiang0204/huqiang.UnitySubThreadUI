using huqiang;
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
       
        //ElementAsset.LoadAssetsAsync("picture.unity3d").PlayOver = (o, e) =>
        //{
        //    UIPage.LoadPage<LoadingPage>();
        //    // ShowPopWindow<PropKeyWin>();
        //};
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
