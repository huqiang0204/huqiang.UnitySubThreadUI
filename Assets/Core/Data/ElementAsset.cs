using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class ElementAsset
    {
        public static Progress LoadAssetsAsync(string name,Action<Progress,AssetBundleCreateRequest> callback=null)
        {
            Progress pro = new Progress();
            pro.Play(LoadAssets(name));
            pro.PlayOver = callback;
            return pro;
        }
        public static AssetBundleCreateRequest LoadAssets(string name)
        {
            string path = Application.streamingAssetsPath + "/" + name;  // 其他平台
            return AssetBundle.LoadFromFileAsync(path);
        }
        public static List<AssetBundle> bundles = new List<AssetBundle>();
        public static Texture FindTexture(string bundle, string tname)
        {
            if (bundle == null)
            {
                return UnityEngine.Resources.Load<Texture>(tname);
            }
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
            {
                var tmp = bundles[i];
                if (bundle == tmp.name)
                {
                    return tmp.LoadAsset<Texture>(tname);
                }
            }
            return null;
        }
        public static Sprite FindSprite(string bundle, string tname, string name)
        {
            if(bundle==null)
            {
                var ss= UnityEngine.Resources.LoadAll<Sprite>(tname);
                if(ss!=null)
                {
                    for (int i = 0; i < ss.Length; i++)
                        if (ss[i].name == name)
                            return ss[i];
                }
                return null;
            }
            if (bundles == null)
                return null;
            for(int i=0;i<bundles.Count;i++)
            {
                var tmp = bundles[i];
                if(bundle==tmp.name)
                {
                    var sp = tmp.LoadAssetWithSubAssets<Sprite>(tname);
                    for(int j = 0; j < sp.Length; j++)
                    {
                        if (sp[j].name == name)
                            return sp[j];
                    }
                    break;
                }
            }
            return null;
        }
        public static string TxtureFormAsset(string name)
        {
            if (bundles == null)
                return null;
            for(int i=0;i<bundles.Count;i++)
            {
                if (bundles[i].LoadAsset<Texture>(name) != null)
                    return bundles[i].name;
            }
            return null;
        }
        public static AssetBundle FindBundle(string name)
        {
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
                if (bundles[i].name == name)
                    return bundles[i];
            return null;
        }
        public static Sprite[] FindSprites(string bundle, string tname, string[] names = null)
        {
            var bun = FindBundle(bundle);
            if (bun == null)
                return null;
            var sp = bun.LoadAssetWithSubAssets<Sprite>(tname);
            if (sp == null)
                return null;
            if (names == null)
                return sp;
            int len = names.Length;
            Sprite[] sprites = new Sprite[len];
            int c = 0;
            for (int i = 0; i < sp.Length; i++)
            {
                var s = sp[i];
                for (int j = 0; j < len; j++)
                {
                    if (s.name == names[j])
                    {
                        sprites[j] = s;
                        c++;
                        if (c >= len)
                            return sprites;
                        break;
                    }
                }
            }
            return sprites;
        }
        public static Sprite[][] FindSprites(string bundle, string tname, string[][] names)
        {
            var bun = FindBundle(bundle);
            if (bun == null)
                return null;
            var sp = bun.LoadAssetWithSubAssets<Sprite>(tname);
            if (sp == null)
                return null;
            if (names == null)
                return null;
            int len = names.Length;
            Sprite[][] sprites = new Sprite[len][];
            for(int k=0;k<len;k++)
            {
                var t = names[k];
                if(t!=null)
                {
                    Sprite[] ss = new Sprite[t.Length];
                    sprites[k] = ss;
                    for (int i = 0; i < ss.Length; i++)
                    {
                        var s = t[i];
                        for (int j = 0; j < len; j++)
                        {
                            if (s== sp[j].name)
                            {
                                ss[i] = sp[j];
                                break;
                            }
                        }
                    }
                }
            }
            return sprites;
        }
        public static void FindSpriteAsync(string bundle, string tname, string name, Action<Sprite> callBack)
        {
            Sprite result=null;
            ThreadMission.InvokeToMain(
                (o)=> { result = FindSprite(bundle,tname,name); },null,
                (e)=> {if(callBack!=null) callBack(result); });
        }
        public static void FindSpritesAsync(string bundle, string tname, string[] names,Action<Sprite[]> callBack)
        {
            Sprite[] result = null;
            ThreadMission.InvokeToMain(
                (o) => { result = FindSprites(bundle, tname, names); }, null,
                (e) => { if (callBack != null) callBack(result); });
        }
        public static void FindSpritesAsync(string bundle, string tname, string[][] names, Action<Sprite[][]> callBack)
        {
            Sprite[][] result = null;
            ThreadMission.InvokeToMain(
                (o) => { result = FindSprites(bundle, tname, names); }, null,
                (e) => { if (callBack != null) callBack(result); });
        }
        class SpriteData
        {
            public string name;
            public DataBuffer buffer;
        }
        static List<SpriteData> SpriteDatas=new List<SpriteData>();
        public static void AddSpriteData(string name, byte[] dat)
        {
            if (dat == null)
                return;
            RemoveSpriteData(name);
            DataBuffer db = new DataBuffer(dat);
            SpriteData data = new SpriteData();
            data.name = name;
            data.buffer = db;
            SpriteDatas.Add(data);
        }
        public static void RemoveSpriteData(string name)
        {
            for(int i=0;i<SpriteDatas.Count;i++)
            {
                if (SpriteDatas[i].name == name)
                {
                    SpriteDatas.RemoveAt(i);
                    return;
                }
            }
        }
        public static void ClearSpriteData()
        {
            SpriteDatas.Clear();
        }
        public static void FindSpriteUV(string tName, string sName,ref Rect rect, ref Vector2 txtSize,ref Vector2 pivot)
        {
            for(int k=0;k<SpriteDatas.Count;k++)
            {
                var fs = SpriteDatas[k].buffer.fakeStruct;
                if(fs!=null)
                {
                    var fsa = fs.GetData<FakeStructArray>(1);
                    if (fsa != null)
                    {
                        for (int i = 0; i < fsa.Length; i++)
                        {
                            if (fsa.GetData(i, 0) as string == tName)
                            {
                                fsa = fsa.GetData(i, 1) as FakeStructArray;
                                if (fsa != null)
                                {
                                    for (int j = 0; j < fsa.Length; j++)
                                    {
                                        if (fsa.GetData(j, 0) as string == sName)
                                        {
                                            unsafe
                                            {
                                                Data.SpriteData.SpriteDataS* sp = (Data.SpriteData.SpriteDataS*)fsa[j];
                                                txtSize = sp->txtSize;
                                                rect= sp->rect;
                                                pivot = sp->pivot;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static T LoadAssets<T>(string bundle, string name) where T : UnityEngine.Object
        {
            if (bundle == null)
            {
                return UnityEngine.Resources.Load<T>(name);
            }
            if (bundles == null)
                return null;
            for (int i = 0; i < bundles.Count; i++)
            {
                var tmp = bundles[i];
                if (bundle == tmp.name)
                {
                    return tmp.LoadAsset<T>(name);
                }
            }
            return null;
        }
    }
}