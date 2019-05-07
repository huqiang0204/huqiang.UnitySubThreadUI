using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UGUI;
using System;
using huqiang;
using huqiang.Data;
using System.IO;
using System.Runtime.InteropServices;

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
            var o = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            foreach (UnityEngine.Object obj in o)
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
    [MenuItem("Assets/CreateEmojiInfo")]
    static void CreateEmojiInfo()
    {
        var o = Selection.activeObject;
        if(o is Texture2D)
        {
            string path = AssetDatabase.GetAssetPath(o);
            var ss = path.Split('/');
            var str = ss[ss.Length - 1];
            ss = str.Split('.');
            var sp = AssetDatabase.LoadAllAssetsAtPath(path);
            string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets/AssetsBundle", "Emoji", "bytes");
            CreateMapInfo(sp,o_path);
        }
    }
    class CharInfoA
    {
        public int len;
        public List<char> dat = new List<char>();
        public List<CharUV> uvs = new List<CharUV>();
    }
    static void CalculUV(Rect sr, float w, float h, ref CharUV uv)
    {
        float x = sr.x;
        float rx = sr.width + x;
        float y = sr.y;
        float ty = sr.height + y;
        x /= w;
        rx /= w;
        y /= h;
        ty /= h;
        uv.uv0.x = x;
        uv.uv0.y = ty;
        uv.uv1.x = rx;
        uv.uv1.y = ty;
        uv.uv2.x = rx;
        uv.uv2.y = y;
        uv.uv3.x = x;
        uv.uv3.y = y;
    }
    static int UnicodeToUtf16(string code)
    {
        int uni = int.Parse(code, System.Globalization.NumberStyles.HexNumber);
        if (uni > 0x10000)
        {
            uni = uni - 0x10000;
            int vh = (uni & 0xFFC00) >> 10;
            int vl = uni & 0x3ff;
            int h = 0xD800 | vh;
            int l = 0xDC00 | vl;
            int value = h << 16 | l;
            return value;
        }
        return uni;
    }
    static byte[] buff = new byte[16];
    private unsafe static int AddSpriteInfo(Sprite spr)
    {
        for (int i = 0; i < 16; i++)
        {
            buff[i] = 0;
        }
        string str = spr.name;
        int len = 0;
        var t = spr.uv;
        fixed (byte* bp = &buff[0])
        {
            UInt16* ip = (UInt16*)bp;
            string[] ss = str.Split('-');
            for (int j = 0; j < ss.Length; j++)
            {
                UInt32 uni = UInt32.Parse(ss[j], System.Globalization.NumberStyles.HexNumber);
                if (uni > 0x10000)
                {
                    uni = uni - 0x10000;
                    UInt32 vh = (uni & 0xFFC00) >> 10;
                    UInt32 vl = uni & 0x3ff;
                    UInt32 h = 0xD800 | vh;
                    UInt32 l = 0xDC00 | vl;
                    //int value = h << 16 | l;
                    *ip = (UInt16)h;
                    ip++;
                    *ip = (UInt16)l;
                    ip++;
                    len += 2;
                }
                else
                {
                    *ip = (UInt16)uni;
                    ip++;
                    len++;
                }
            }
        }
        return len;
    }

    public static void CreateMapInfo(UnityEngine.Object[] sprites, string savepath)
    {
        CharInfoA[] tmp = new CharInfoA[7];
        for (int i = 0; i < 7; i++)
        {
            tmp[i] = new CharInfoA();
            tmp[i].len = i + 1;
        }
        CharUV uv = new CharUV();
        unsafe
        {
            fixed (byte* bp = &buff[0])
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    var sp = sprites[i] as Sprite;
                    if(sp!=null)
                    {
                        int len = AddSpriteInfo(sp);
                        var dat = tmp[len].dat;
                        char* cp = (char*)bp;
                        for (int j = 0; j < len; j++)
                        {
                            dat.Add(*cp);
                            cp++;
                        }
                        CalculUV(sp.rect, sp.texture.width, sp.texture.height, ref uv);
                        tmp[len].uvs.Add(uv);
                    }
                }
            }
        }
        DataBuffer db = new DataBuffer();
        FakeStruct fake = new FakeStruct(db, 7);
        for (int i = 0; i < 7; i++)
        {
            FakeStruct fs = new FakeStruct(db, 3);
            fs[0] = tmp[i].len;
            if (tmp[i].dat.Count > 0)
            {
                fs[1] = db.AddArray<char>(tmp[i].dat.ToArray());
                fs[2] = db.AddArray<CharUV>(tmp[i].uvs.ToArray());
            }
            fake.SetData(i, fs);
        }
        byte[] data = db.ToBytes();
        File.WriteAllBytes(savepath, data);
        Debug.Log("emoji info create done");
    }
    static void WriteTable(Stream stream, Array array, Int32 structLen, Int32 charLen)
    {
        int len = array.Length * structLen;
        stream.Write(charLen.ToBytes(), 0, 4);
        stream.Write(len.ToBytes(), 0, 4);
        var tmp = new byte[len];
        Marshal.Copy(Marshal.UnsafeAddrOfPinnedArrayElement(array, 0), tmp, 0, len);
        stream.Write(tmp, 0, len);
    }
}

