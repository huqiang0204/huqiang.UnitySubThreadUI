using huqiang.Data;
using System.IO;
using UnityEngine;

namespace UGUI
{
    public class ElementCreate : MonoBehaviour
    {
        public TextAsset bytesUI;
        public void ClearAllAssetBundle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            ElementAsset.bundles.Clear();
        }
        public string dicpath;
        public string Assetname = "baseUI.bytes";
        public string CloneName;
    }
}
