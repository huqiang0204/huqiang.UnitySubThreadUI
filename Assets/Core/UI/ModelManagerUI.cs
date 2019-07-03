using huqiang.Data;
using huqiang.Pool;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace huqiang.UI
{
    public class ModelManagerUI
    {
        static int point;
        /// <summary>
        /// 0-62,第63为负数位
        /// </summary>
        static TypeContext[] types = new TypeContext[63];
        /// <summary>
        /// 注册一个组件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Compare">不可空</param>
        /// <param name="create">可空</param>
        /// <param name="load">可空</param>
        public static void RegComponent(TypeContext context)
        {
            if (point >= 63)
                return;
            var name = context.name;
            for (int i = 0; i < point; i++)
                if (types[i].name == name)
                {
                    types[i] = context;
                    return;
                }
            types[point] = context;
            point++;
        }
        public static Int64 GetTypeIndex(DataConversion com)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].CompareData(com))
                {
                    Int64 a = 1 << i;
                    return a;
                }
            }
            return 0;
        }
        public static Int64 GetTypeIndex(Component com)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].CompareCom(com))
                {
                    Int64 a = 1 << i;
                    return a;
                }
            }
            return 0;
        }
        public static Int64 GetTypeIndex(Component[] com)
        {
            if (com == null)
                return 0;
            Int64 a = 0;
            for (int i = 0; i < com.Length; i++)
            {
                var c = com[i];
                if (c != null)
                    a |= GetTypeIndex(c);
            }
            return a;
        }
        static List<ModelBuffer> models = new List<ModelBuffer>();
        public static DataConversion Load(int type)
        {
            if (type < 0 | type >= point)
                return null;
            return types[type].Create();
        }
        public static FakeStruct LoadFromObject(Component com, DataBuffer buffer, ref Int16 type)
        {
            string name = com.GetType().Name;
            for (int i = 0; i < point; i++)
            {
                if (types[i].name==name)
                {
                    type = (Int16)i;
                    if (types[i].Load != null)
                        return types[i].Load(com, buffer);
                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// 将场景内的对象保存到文件
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <param name="path"></param>
        public static void SavePrefab(GameObject uiRoot, string path)
        {
            DataBuffer db = new DataBuffer(1024);
            db.fakeStruct = ModelElement.LoadFromObject(uiRoot.transform, db);
            File.WriteAllBytes(path, db.ToBytes());
        }
        public static List<PrefabAsset> prefabs = new List<PrefabAsset>();
        /// <summary>
        /// 载入模型数据
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public unsafe static PrefabAsset LoadModels(byte[] buff, string name)
        {
            DataBuffer db = new DataBuffer(buff);
            var asset = new PrefabAsset();
            asset.models = new ModelElement();
            asset.models.Load(db.fakeStruct);
            asset.name = name;
            for (int i = 0; i < prefabs.Count; i++)
                if (prefabs[i].name == name)
                {
                    prefabs.RemoveAt(i);
                    break;
                }
            prefabs.Add(asset);
            return asset;
        }
        public static ModelElement GetMod(ModelElement mod, string name)
        {
            if (mod.tag == "mod")
                if (mod.name == name)
                    return mod;
            var c = mod.child;
            for (int i = 0; i < c.Count; i++)
            {
                var m = GetMod(c[i], name);
                if (m != null)
                    return m;
            }
            return null;
        }
        public static ModelElement FindModel(string asset, string name)
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (asset == prefabs[i].name)
                {
                    var models = prefabs[i].models;
                    return models.Find(name);
                }
            }
            return null;
        }
        public static ModelElement CloneModel(string asset, string name)
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (asset == prefabs[i].name)
                {
                    var models = prefabs[i].models;
                    var mod= models.Find(name);
                    if(mod!=null)
                    {
                        ModelElement model = new ModelElement();
                        model.Load(mod.ModData);
                        return model;
                    }
                    return null;
                }
            }
            return null;
        }
        static ModelBuffer CreateModelBuffer(Int64 type, Int32 size = 32)
        {
            long t = type;
            List<Type> tmp = new List<Type>();
            for (int i = 0; i < 64; i++)
            {
                if (t > 0)
                {
                    if ((t & 1) > 0)
                    {
                        tmp.Add(types[i].type);
                    }
                    t >>= 1;
                }
                else break;
            }
            ModelBuffer model = new ModelBuffer(type, size, tmp.ToArray(),null);
            models.Add(model);
            return model;
        }
        public static GameObject CreateNew(Int64 type)
        {
            if (type == 0)
                return null;
            for (int i = 0; i < models.Count; i++)
                if (type == models[i].type)
                    return models[i].CreateObject();
            return CreateModelBuffer(type).CreateObject();
        }
        public static ModelElement FindModel(string str)
        {
            if (prefabs == null)
                return null;
            if (prefabs.Count > 0)
                return prefabs[0].models.Find(str);
            return null;
        }
        public static ModelElement LoadToGame(string asset, string mod, object o, string filter = "mod")
        {
            if (prefabs == null)
                return null;
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (asset == prefabs[i].name)
                {
                    var m = prefabs[i].models.Find(mod);
                    LoadToGame(m, o,  filter);
                    return m;
                }
            }
            return null;
        }
        public static void LoadToGame(ModelElement mod, object o,  string filter = "mod")
        {
            if (o != null)
            {
                var tmp = ObjectFelds(o);
                LoadToGameR(mod, tmp,  filter);
                ReflectionModel[] all = tmp.All;
                for (int i = 0; i < all.Length; i++)
                    all[i].field.SetValue(o, all[i].Value);
            }
            else
            {
                LoadToGameR(mod, null,  filter);
            }
        }
        public static TempReflection ObjectFelds(object obj)
        {
            var fs = obj.GetType().GetFields();
            TempReflection temp = new TempReflection();
            temp.Top = fs.Length;
            ReflectionModel[] reflections = new ReflectionModel[temp.Top];
            for (int i = 0; i < fs.Length; i++)
            {
                ReflectionModel r = new ReflectionModel();
                r.field = fs[i];
                r.FieldType = fs[i].FieldType;
                r.name = fs[i].Name;
                reflections[i] = r;
            }
            temp.All = reflections;
            return temp;
        }
        public static void LoadToGameR(ModelElement mod, TempReflection reflections, string filter = "mod")
        {
            if (mod == null)
            {
#if DEBUG
                Debug.Log("Mod is null");
#endif
                return;
            }
            if (mod.tag == filter)
                return;
            var c = mod.child;
            for (int i = 0; i < c.Count; i++)
                LoadToGameR(c[i], reflections, filter);
            if (reflections != null)
                GetObject(reflections, mod);
        }
        /// <summary>
        /// 对象反射
        /// </summary>
        /// <param name="t"></param>
        /// <param name="reflections"></param>
        /// <param name="mod"></param>
        public static void GetObject(TempReflection reflections, ModelElement mod)
        {
            for (int i = 0; i < reflections.Top; i++)
            {
                var m = reflections.All[i];
                if (m.name == mod.name)
                {
                    if (m.FieldType == typeof(ModelElement))
                        m.Value = mod;
                    else if (typeof(EventCallBack).IsAssignableFrom(m.FieldType))
                        m.Value = EventCallBack.RegEvent(mod,m.FieldType);
                    else if (typeof(ModelInital).IsAssignableFrom(m.FieldType))
                    {
                        var obj = Activator.CreateInstance(m.FieldType) as ModelInital;
                        obj.Initial( mod);
                        m.Value = obj;
                    }
                    else if (typeof(DataConversion).IsAssignableFrom(m.FieldType))
                        m.Value = mod.GetComponent(m.FieldType.Name);
                    reflections.Top--;
                    var j = reflections.Top;
                    var a = reflections.All[j];
                    reflections.All[i] = a;
                    reflections.All[j] = m;
                    break;
                }
            }
        }
        /// <summary>
        /// 挂载被回收得对象
        /// </summary>
        public static Transform CycleBuffer;
        static QueueBuffer<ModelElement> RecycleQueue = new QueueBuffer<ModelElement>();
        public static void RecycleElement(ModelElement model)
        {
            if (model == null)
                return;
            RecycleQueue.Enqueue(model);
            var child = model.child;
            for (int i = 0; i < child.Count; i++)
                RecycleElement(child[i]);
            child.Clear();
        }
        public static void RecycleGameObject()
        {
            int c = RecycleQueue.Count;
            for(int j=0;j<c;j++)
            {
                var mod = RecycleQueue.Dequeue();
                if(mod!=null)
                {
                    if(mod.Context!=null)
                    {
                        long type = mod.data.type;
                        for (int i = 0; i < models.Count; i++)
                        {
                            if (models[i].type == type)
                            {
                                var g = mod.Main;
                                if (models[i].ReCycle(g))
                                {
                                    g.SetActive(false);
                                    mod.Context.SetParent(CycleBuffer);
                                    mod.Context = null;
                                    mod.Main = null;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static void ComponentReflection(ModelElement mod, object obj)
        {
            var r = ObjectFelds(obj);
            LoadToGameR(mod, r, "");
            ReflectionModel[] all = r.All;
            for (int i = 0; i < all.Length; i++)
                all[i].field.SetValue(obj, all[i].Value);
        }
    }
    public class ReflectionModel
    {
        public string name;
        public FieldInfo field;
        public Type FieldType;
        public object Value;
    }
    public class TempReflection
    {
        public int Top;
        public ReflectionModel[] All;
    }
    public abstract class ModelInital
    {
        public ModelElement Model;
        public virtual void Initial(ModelElement mod) { }
    }
    public class TypeContext
    {
        public Type type;
        public Func<Component, DataBuffer, FakeStruct> Load;
        public string name;
        public Type dcType;
        public string dcName;
        public virtual bool CompareCom(object obj)
        {
            return false;
        }
        public virtual bool CompareData(object obj)
        {
            return false;
        }
        public virtual DataConversion Create()
        {
            return null;
        }
    }
    public class ComponentType<T, U> : TypeContext where T : Component where U : DataConversion, new()
    {
        public ComponentType(Func<Component, DataBuffer, FakeStruct> load)
        {
            Load = load;
            type = typeof(T);
            name = type.Name;
            dcType= typeof(U);
            dcName = dcType.Name;
        }
        public override DataConversion Create()
        {
            U u= new U();
            u.type = dcName;
            return u;
        }
        public override bool CompareCom(object obj)
        {
            return obj is T;
        }
        public override bool CompareData(object obj)
        {
            return obj is U;
        }
    }
    public class PrefabAsset
    {
        public string name;
        public ModelElement models;
    }
}
