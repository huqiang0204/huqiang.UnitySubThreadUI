
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleCreate : Editor {

    [MenuItem("Custom Editor/Create Scene")]
    static void CreateSceneALL()
    {
        //清空一下缓存  
        Caching.ClearCache();
        string Path = Application.dataPath + "/MyScene.unity3d";
        string[] levels = { "Assets/Level.unity" };
        //打包场景  
        BuildPipeline.BuildPlayer(levels, Path, BuildTarget.StandaloneWindows, BuildOptions.BuildAdditionalStreamedScenes);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ExportBundlesForWin")]
    static void BuildAssetBundlesForWIn()
    {
        BuildAllAssetBundles(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Assets/ExportBundlesForAndroid")]
    static void BuildAssetBundlesForAndroid()
    {
        BuildAllAssetBundles(BuildTarget.Android);
    }
    [MenuItem("Assets/ExportBundlesForIOS")]
    static void BuildAssetBundlesForIos()
    {
        BuildAllAssetBundles(BuildTarget.iOS);
    }
    static void BuildAllAssetBundlesFolder(BuildTarget target)
    {
        // 打开保存面板，获得用户选择的路径  
        string i_path = EditorUtility.OpenFolderPanel("Assets", "Assets", "Resources");
        if (i_path.Contains(Application.dataPath))
        {
            if (i_path.Length != 0)
            {
                char[] buff = i_path.ToCharArray();
                string i_folder = new string(CopyCharArry(buff, Application.dataPath.Length + 1,
                i_path.Length - Application.dataPath.Length - 1));
                string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
                buff = o_path.ToCharArray();
                int s = FallFindChar(buff, '/');
                string o_folder = new string(CopyCharArry(buff, 0, s));
                string o_file = new string(CopyCharArry(buff, s + 1, buff.Length - s - 1));
                if (o_path.Length != 0)
                {
                    var di = new System.IO.DirectoryInfo(i_path);
                    var fi = di.GetFiles("*.*");//这里可以自己选择过滤
                    List<string> names = new List<string>();
                    for (int i = 0; i < fi.Length; i++)
                    {
                        if (fi[i].Name.Split('.').Length < 3)
                        {
                            names.Add("Assets/" + i_folder + "/" + fi[i].Name);
                        }
                    }
                    AssetBundleBuild[] abb = new AssetBundleBuild[1];
                    abb[0].assetBundleName = o_file;
                    abb[0].assetNames = names.ToArray();
                    BuildPipeline.BuildAssetBundles(o_folder, abb, BuildAssetBundleOptions.None, target);
                }
            }
        }
        else Debug.Log("请选择 " + Application.dataPath + "里面的文件夹");
    }
    static void BuildAllAssetBundles(BuildTarget target)
    {
        string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/StreamingAssets", "Assets", "unity3d");
        char[] buff = o_path.ToCharArray();
        int s = FallFindChar(buff, '/');
        string o_folder = new string(CopyCharArry(buff, 0, s));
        string o_file = new string(CopyCharArry(buff, s + 1, buff.Length - s - 1));
        if (o_path.Length != 0)
        {
            List<string> names = new List<string>();
            var o = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            foreach (Object obj in o)
            {
                string filePath = AssetDatabase.GetAssetPath(obj);
                if (filePath.IndexOf('.') > 0)
                    names.Add(filePath);
            }
            if (names.Count == 0)
                return;
            AssetBundleBuild[] abb = new AssetBundleBuild[1];
            abb[0].assetBundleName = o_file;
            abb[0].assetNames = names.ToArray();
            BuildPipeline.BuildAssetBundles(o_folder, abb, BuildAssetBundleOptions.ChunkBasedCompression, target);
            Debug.Log("打包完成");
        }
    }
    static int FallFindChar(char[] c_buff, char t)
    {
        int c = c_buff.Length - 1;
        for (int i = c; i > -1; i--)
        {
            if (c_buff[i] == t)
                return i;
        }
        return -1;
    }
    static char[] CopyCharArry(char[] c_buff, int s, int l)
    {
        char[] temp = new char[l];
        for (int i = 0; i < l; i++)
        {
            temp[i] = c_buff[s];
            s++;
        }
        return temp;
    }
}

