using huqiang.Data;
using huqiang.Pool;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;

namespace huqiang.ModelManager2D
{
    public class PrefabAsset
    {
        public string name;
        public TransfromModel models;
    }
    public class ModelManager2D
    {
    
        static int point;
        /// <summary>
        /// 0-62,第63为负数位
        /// </summary>
        static TypeContext[] types = new TypeContext[63];
        /// <summary>
        /// 注册一个组件
        /// </summary>
        /// <param name="context"></param>
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
        public static Int64 GetTypeIndex(Component com)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].Compare(com))
                {
                    Int64 a = 1 << i;
                    return a;
                }
            }
            return 1;
        }
        public static Int64 GetTypeIndex(Component[] com)
        {
            if (com == null)
                return -1;
            Int64 a = 1;
            for (int i = 0; i < com.Length; i++)
            {
                var c = com[i];
                if (c != null)
                    a |= GetTypeIndex(c);
            }
            return a;
        }
        public static void Initial()
        {
            RegComponent(new ComponentType<Transform,TransfromModel>( TransfromModel.LoadFromObject));
            RegComponent(new ComponentType<SpriteRenderer,SpriteRenderModel>( SpriteRenderModel.LoadFromObject));
            RegComponent(new ComponentType<SpriteMask, SpriteMaskModel>( SpriteMaskModel.LoadFromObject));
            RegComponent(new ComponentType<BoxCollider2D,BoxColliderModel>(BoxColliderModel.LoadFromObject));
            RegComponent(new ComponentType<EdgeCollider2D,EdgeColliderModel>(EdgeColliderModel.LoadFromObject));
            RegComponent(new ComponentType<CircleCollider2D,CircleColliderModel>( CircleColliderModel.LoadFromObject));
            RegComponent(new ComponentType<CapsuleCollider2D,CapsuleColliderModel>( CapsuleColliderModel.LoadFromObject));
            RegComponent(new ComponentType<CompositeCollider2D, CompositeColliderModel>(CompositeColliderModel.LoadFromObject));
            RegComponent(new ComponentType<PolygonCollider2D,PolygonColliderModel>(PolygonColliderModel.LoadFromObject));
            RegComponent(new ComponentType<Rigidbody2D, RigidbodyModel>(RigidbodyModel.LoadFromObject));
            RegComponent(new ComponentType<AreaEffector2D, AreaEffectModel>( AreaEffectModel.LoadFromObject));
            RegComponent(new ComponentType<BuoyancyEffector2D, BuoyancyModel>( BuoyancyModel.LoadFromObject));
            RegComponent(new ComponentType<PlatformEffector2D, PlatformEffectorModel>(PlatformEffectorModel.LoadFromObject));
            RegComponent(new ComponentType<PointEffector2D,PointEffectorModel>( PointEffectorModel.LoadFromObject));
            RegComponent(new ComponentType<SurfaceEffector2D, SurfaceEffectorModel>( SurfaceEffectorModel.LoadFromObject));
            RegComponent(new ComponentType<ConstantForce2D, ConstantForceModel>(ConstantForceModel.LoadFromObject));
            RegComponent(new ComponentType<DistanceJoint2D, DistanceJointModel>(DistanceJointModel.LoadFromObject));
            RegComponent(new ComponentType<FrictionJoint2D,FrictionJointModel>( FrictionJointModel.LoadFromObject));
            RegComponent(new ComponentType<HingeJoint2D,HingeJointModel>( HingeJointModel.LoadFromObject));
            RegComponent(new ComponentType<FixedJoint2D,FixedJointModel>(FixedJointModel.LoadFromObject));
            RegComponent(new ComponentType<RelativeJoint2D,RelativeJointModel>(RelativeJointModel.LoadFromObject));
            RegComponent(new ComponentType<SliderJoint2D, SliderJointModel>(SliderJointModel.LoadFromObject));
            RegComponent(new ComponentType<SpringJoint2D, SpringJointModel>( SpringJointModel.LoadFromObject));
            RegComponent(new ComponentType<TargetJoint2D,TargetJointModel>(TargetJointModel.LoadFromObject));
            RegComponent(new ComponentType<WheelJoint2D, WheelJointModel>(WheelJointModel.LoadFromObject));
        }
        static List<ModelBuffer> models = new List<ModelBuffer>();
        static Container<InstanceContext> container = new Container<InstanceContext>(4096);
        public static DataConversion Load(int type)
        {
            if (type < 0 | type >= point)
                return null;
           return types[type].Create();
    
        }
        public static FakeStruct LoadFromObject(Component com, DataBuffer buffer, ref Int16 type)
        {
            for (int i = 0; i < point; i++)
                if (types[i].Compare(com))
                {
                    type = (Int16)i;
                    if (types[i].Load != null)
                        return types[i].Load(com, buffer);
                    return null;
                }
            return null;
        }
        static ModelBuffer CreateModelBuffer(Int64 type, Int32 size = 64)
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
            ModelBuffer model = new ModelBuffer(type, size, tmp.ToArray(), container);
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
        /// <summary>
        /// 将场景内的对象保存到文件
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <param name="path"></param>
        public static void SavePrefab(Transform uiRoot, string path)
        {
            DataBuffer db = new DataBuffer(1024);
            db.fakeStruct = TransfromModel.LoadFromObject(uiRoot, db);
            File.WriteAllBytes(path, db.ToBytes());
        }
        static List<PrefabAsset> prefabAssets = new List<PrefabAsset>();
        public unsafe static PrefabAsset LoadModels(byte[] buff, string name)
        {
            DataBuffer db = new DataBuffer(buff);
            TransfromModel model = new TransfromModel();
            model.Load(db.fakeStruct);
            var asset = new PrefabAsset();
            asset.models = model;
            asset.name = name;
            for (int i = 0; i < prefabAssets.Count; i++)
                if (prefabAssets[i].name == name)
                { prefabAssets.RemoveAt(i); break; }
            prefabAssets.Add(asset);
            return asset;
        }
        /// <summary>
        /// 查询一个模型数据,并实例化对象
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="mod"></param>
        /// <param name="o"></param>
        /// <param name="parent"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static TransfromModel LoadToGame(string asset, string mod, object o, Transform parent, string filter = "mod")
        {
            for (int i = 0; i < prefabAssets.Count; i++)
            {
                if (asset == prefabAssets[i].name)
                {
                    var ms = prefabAssets[i].models.child;
                    for (int j = 0; j < ms.Count; j++)
                    {
                        if (ms[j].name == mod)
                        {
                            LoadToGame(ms[j], o, parent, filter);
                            return ms[j];
                        }
                    }
                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// 使用模型数据,实例化对象
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="o"></param>
        /// <param name="parent"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static GameObject LoadToGame(TransfromModel mod, object o, Transform parent, string filter = "mod")
        {
            if (mod == null)
            {
#if DEBUG
                Debug.Log("Mod is null");
#endif
                return null;
            }
            if (mod.tag == filter)
            {
                return null;
            }
            var g = CreateNew(mod.transfrom.type);
            if (g == null)
            {
#if DEBUG
                Debug.Log("Name:" + mod.name + " is null");
#endif
                return null;
            }
            var t = g.transform;
            if (parent != null)
                t.SetParent(parent);
            mod.LoadToObject(g.transform);
            mod.Main = g;
            var c = mod.child;
            for (int i = 0; i < c.Count; i++)
                LoadToGame(c[i], o, t, filter);
            if (o != null)
                ReflectionObject(t, o, mod);
            return g;
        }
        static void ReflectionObject(Transform t, object o, TransfromModel mod)
        {
            var m = o.GetType().GetField(t.name);
            if (m != null)
            {
                if (typeof(Component).IsAssignableFrom(m.FieldType))
                    m.SetValue(o, t.GetComponent(m.FieldType));
            }
        }

#region 创建和回收
        /// <summary>
        /// 挂载被回收得对象
        /// </summary>
        public static Transform CycleBuffer;
        /// <summary>
        /// 回收一个对象，包括子对象
        /// </summary>
        /// <param name="game"></param>
        public static void RecycleGameObject(GameObject game)
        {
            if (game == null)
                return;
            int id = game.GetInstanceID();
            InstanceContext ins = container.Find((o) => { return o.Id == id; });
            if (ins != null)
                ins.buffer.ReCycle(game);
            var p = game.transform;
            for (int i = p.childCount - 1; i >= 0; i--)
                RecycleGameObject(p.GetChild(i).gameObject);
            if (ins != null)
                p.SetParent(CycleBuffer);
            else GameObject.Destroy(game);
        }
#endregion
    }
}
