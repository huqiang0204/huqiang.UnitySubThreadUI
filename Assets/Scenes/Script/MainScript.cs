using huqiang;
using huqiang.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public TextAsset baseUI;
    // Start is called before the first frame update
    void Start()
    {
        App.Initial(transform as RectTransform);
        ModelManagerUI.LoadModels(baseUI.bytes, "baseUI");
        UIPage.LoadPage<LoadingPage>();
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
