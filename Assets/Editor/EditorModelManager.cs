using huqiang;
using huqiang.UI;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EditorModelManager
{
    static List<UnityEngine.Object[]> objects = new List<UnityEngine.Object[]>();
    public static void LoadAllTexture(string folder)
    {
        var path = Application.dataPath;
        if (folder != null)
            path += "/" + folder;

    }
    static UnityEngine.Object[] LoadSprite(string name)
    {
        string path = null;
        var fs = AssetDatabase.FindAssets(name);
        if (fs != null)
        {
            HashSet<string> hash = new HashSet<string>();
            for (int i = 0; i < fs.Length; i++)
                hash.Add(fs[i]);
            var list = hash.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                path = AssetDatabase.GUIDToAssetPath(list[i]);
                var ss = path.Split('/');
                var str = ss[ss.Length - 1];
                ss = str.Split('.');
                var sp = AssetDatabase.LoadAllAssetsAtPath(path);
                if (sp != null)
                    if (sp.Length > 0)
                    {
                        objects.Add(sp);
                        return sp;
                    }
            }
        }
        return null;
    }
    public static Texture FindTexture(string tname)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            var objs = objects[i];
            if (objs != null)
            {
                if (objs.Length > 0)
                    if (objs[0].name == tname)
                        return objs[0] as Texture;
            }
        }
        var os = LoadSprite(tname);
        if (os != null)
        {
            if (os.Length > 0)
                return os[0] as Texture;
        }
        return null;
    }
    public static Sprite FindSprite(string tname, string name)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            var objs = objects[i];
            if (objs != null)
            {
                if (objs.Length > 0)
                    if (objs[0] != null)
                        if (objs[0].name == tname)
                        {
                            for (int j = 1; j < objs.Length; j++)
                            {
                                if (objs[j] != null)
                                    if (objs[j].name == name)
                                        return objs[j] as Sprite;
                            }
                        }
            }
        }
        var os = LoadSprite(tname);
        if (os != null)
        {
            if (os.Length > 0)
            {
                for (int j = 1; j < os.Length; j++)
                {
                    if (os[j] != null)
                        if (os[j].name == name)
                            return os[j] as Sprite;
                }
            }
        }
        return null;

    }
    public static void Clear()
    {
        objects.Clear();
    }
    public static ModelElement LoadToGame(string mod, object o, Transform parent, string filter = "mod")
    {
        var m = ModelManagerUI.FindModel(mod);
        LoadToGame(m, o, parent, filter);
        return m;
    }
    public static GameObject LoadToGame(ModelElement mod, object o, Transform parent, string filter = "mod")
    {
        if (mod == null)
        {
            Debug.Log("Mod is null");
            return null;
        }
        if (mod.tag == filter)
        {
            return null;
        }
        var g = ModelManagerUI.CreateNew(mod.data.type);
        if (g == null)
        {
            Debug.Log("Name:" + mod.name + " is null");
            return null;
        }
        var t = g.transform;
        if (parent != null)
            t.SetParent(parent);
        mod.LoadToObject(t);
        var typ = mod.data.type;
        if ((typ & 0x2) > 0)
        {
            var child = mod.components;
            for (int j = 0; j < child.Count; j++)
            {
                ImageElement e = child[j] as ImageElement;
                if (e != null)
                {
                    mod.Context.GetComponent<Image>().sprite = FindSprite(e.textureName, e.spriteName);
                    break;
                }
            }
        }
        else if ((typ & 0x20) > 0)
        {
            var child = mod.components;
            for (int j = 0; j < child.Count; j++)
            {
                RawImageElement e = child[j] as RawImageElement;
                if (e != null)
                {
                    mod.Context.GetComponent<RawImage>().texture = FindTexture(e.textureName);
                    break;
                }
            }
        }
        mod.Main = g;
        mod.AddSizeScale();
        var c = mod.child;
        for (int i = 0; i < c.Count; i++)
            LoadToGame(c[i], o, t, filter);
        //if (o != null)
        //    GetObject(mod, o, mod);
        return g;
    }
    static void GetObject(ModelElement t, object o, ModelElement mod)
    {
        //var m = o.GetType().GetField(t.name);
        //if (m != null)
        //{
        //    if (m.FieldType == typeof(GameObject))
        //        m.SetValue(o, t.gameObject);
        //    else if (typeof(EventCallBack).IsAssignableFrom(m.FieldType))
        //        m.SetValue(o, EventCallBack.RegEventCallBack(t as RectTransform, m.FieldType));
        //    else if (typeof(UIComposite).IsAssignableFrom(m.FieldType))
        //    {
        //        var obj = Activator.CreateInstance(m.FieldType) as UIComposite;
        //        obj.Initial(mod);
        //        m.SetValue(o, obj);
        //    }
        //    else if (typeof(Component).IsAssignableFrom(m.FieldType))
        //        m.SetValue(o, t.GetComponent(m.FieldType));
        //}
    }
}

