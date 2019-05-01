using huqiang.Data;
using huqiang.Pool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public class PrefabAsset
    {
        public string name;
        public TransfromModel models;
    }
    public class ModelManager2D
    {
        struct TypeContext
        {
            public Type type;
            public Func<Component, bool> Compare;
            public Func<DataConversion> CreateConversion;
            public Func<Component, DataBuffer, FakeStruct> LoadFromObject;
        }
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
        public static void RegComponent(Type type,Func<Component, bool> Compare, Func<DataConversion> create, Func<Component, DataBuffer, FakeStruct> load)
        {
            if (point >= 63)
                return;
            for (int i = 0; i < point; i++)
                if (types[i].type == type)
                {
                    types[i].Compare = Compare;
                    types[i].CreateConversion = create;
                    types[i].LoadFromObject = load;
                    return;
                }
            types[point].type = type;
            types[point].Compare = Compare;
            types[point].CreateConversion = create;
            types[point].LoadFromObject = load;
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
                if(c!=null)
                a |= GetTypeIndex(c);
            }
            return a;
        }
        static int GetTypeIndex(Type type)
        {
            for (int i = 0; i < point; i++)
            {
                if (type == types[i].type)
                {
                    int a = 1 << i;
                    return a;
                }
            }
            return 1;
        }
        /// <summary>
        /// 根据所有类型生成一个id
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static int GetTypeIndex(Type[] typ)
        {
            if (typ == null)
                return -1;
            int a = 1;
            for (int i = 0; i < typ.Length; i++)
                a |= GetTypeIndex(typ[i]);
            return a;
        }

        public static void Initial()
        {
            RegComponent(typeof(Transform), (o) => { return o is Transform; }, () => { return new TransfromModel(); },TransfromModel.LoadFromObject);
            RegComponent(typeof(SpriteRenderer), (o) => { return o is SpriteRenderer; }, () => { return new SpriteRenderModel(); }, SpriteRenderModel.LoadFromObject);
            RegComponent(typeof(SpriteMask), (o) => { return o is SpriteMask; }, () => { return new SpriteMaskModel(); }, SpriteMaskModel.LoadFromObject);
            RegComponent(typeof(BoxCollider2D), (o) => { return o is BoxCollider2D; }, () => { return new BoxColliderModel(); }, BoxColliderModel.LoadFromObject);
            RegComponent(typeof(EdgeCollider2D), (o) => { return o is EdgeCollider2D; }, () => { return new EdgeColliderModel(); }, EdgeColliderModel.LoadFromObject);
            RegComponent(typeof(CircleCollider2D), (o) => { return o is CircleCollider2D; }, () => { return new CircleColliderModel(); }, CircleColliderModel.LoadFromObject);
            RegComponent(typeof(CapsuleCollider2D), (o) => { return o is CapsuleCollider2D; }, () => { return new CapsuleColliderModel(); }, CapsuleColliderModel.LoadFromObject);
            RegComponent(typeof(CompositeCollider2D), (o) => { return o is CompositeCollider2D; }, () => { return new CompositeColliderModel(); }, CompositeColliderModel.LoadFromObject);
            RegComponent(typeof(PolygonCollider2D), (o) => { return o is PolygonCollider2D; }, () => { return new PolygonColliderModel(); }, PolygonColliderModel.LoadFromObject);
            RegComponent(typeof(Rigidbody2D), (o) => { return o is Rigidbody2D; }, () => { return new RigidbodyModel(); }, RigidbodyModel.LoadFromObject);
            RegComponent(typeof(AreaEffector2D), (o) => { return o is AreaEffector2D; }, () => { return new AreaEffectModel(); }, AreaEffectModel.LoadFromObject);
            RegComponent(typeof(BuoyancyEffector2D), (o) => { return o is BuoyancyEffector2D; }, () => { return new BuoyancyModel(); }, BuoyancyModel.LoadFromObject);
            RegComponent(typeof(PlatformEffector2D), (o) => { return o is PlatformEffector2D; }, () => { return new PlatformEffectorModel(); }, PlatformEffectorModel.LoadFromObject);
            RegComponent(typeof(PointEffector2D), (o) => { return o is PointEffector2D; }, () => { return new PointEffectorModel(); }, PointEffectorModel.LoadFromObject);
            RegComponent(typeof(SurfaceEffector2D), (o) => { return o is SurfaceEffector2D; }, () => { return new SurfaceEffectorModel(); }, SurfaceEffectorModel.LoadFromObject);
            RegComponent(typeof(ConstantForce2D), (o) => { return o is ConstantForce2D; }, () => { return new ConstantForceModel(); }, ConstantForceModel.LoadFromObject);
            RegComponent(typeof(DistanceJoint2D), (o) => { return o is DistanceJoint2D; }, () => { return new DistanceJointModel(); }, DistanceJointModel.LoadFromObject);
            RegComponent(typeof(FrictionJoint2D), (o) => { return o is FrictionJoint2D; }, () => { return new FrictionJointModel(); }, FrictionJointModel.LoadFromObject);
            RegComponent(typeof(HingeJoint2D), (o) => { return o is HingeJoint2D; }, () => { return new HingeJointModel(); }, HingeJointModel.LoadFromObject);
            RegComponent(typeof(FixedJoint2D), (o) => { return o is FixedJoint2D; }, () => { return new FixedJointModel(); }, FixedJointModel.LoadFromObject);
            RegComponent(typeof(RelativeJoint2D), (o) => { return o is RelativeJoint2D; }, () => { return new RelativeJointModel(); }, RelativeJointModel.LoadFromObject);
            RegComponent(typeof(SliderJoint2D), (o) => { return o is SliderJoint2D; }, () => { return new SliderJointModel(); }, SliderJointModel.LoadFromObject);
            RegComponent(typeof(SpringJoint2D), (o) => { return o is SpringJoint2D; }, () => { return new SpringJointModel(); }, SpringJointModel.LoadFromObject);
            RegComponent(typeof(TargetJoint2D), (o) => { return o is TargetJoint2D; }, () => { return new TargetJointModel(); }, TargetJointModel.LoadFromObject);
            RegComponent(typeof(WheelJoint2D), (o) => { return o is WheelJoint2D; }, () => { return new WheelJointModel(); }, WheelJointModel.LoadFromObject);
        }
        static List<ModelBuffer> models=new List<ModelBuffer>();
        /// <summary>
        /// 注册一种模型的管理池
        /// </summary>
        /// <param name="reset">模型被重复利用时,进行重置,为空则不重置</param>
        /// <param name="buffsize">池子大小,建议32</param>
        /// <param name="types">所有的Component组件</param>
        public static void RegModel(Action<GameObject> reset, int buffsize, params Type[] types)
        {
           Int64 typ = GetTypeIndex(types);
            for (int i = 0; i < models.Count; i++)
            {
                if (typ == models[i].type)
                    return;
            }
            ModelBuffer model = new ModelBuffer(typ,buffsize,reset,types);
            models.Add(model);
        }
        public static DataConversion Load(int type)
        {
            if (type < 0 | type >= point)
                return null;
            if (types[type].CreateConversion != null)
                return types[type].CreateConversion();
            return null;
        }
        public static FakeStruct LoadFromObject(Component com,DataBuffer buffer,ref Int16 type)
        {
            for (int i = 0; i < point; i++)
                if (types[i].Compare(com))
                {
                    type = (Int16)i;
                    if (types[i].LoadFromObject != null)
                        return types[i].LoadFromObject(com, buffer);
                    return null;
                }
            return null;
        }
        /// <summary>
        /// 将场景内的对象保存到文件
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <param name="path"></param>
        public static void SavePrefab(Transform uiRoot, string path)
        {
            DataBuffer db = new DataBuffer(1024);
            db.fakeStruct = TransfromModel.LoadFromObject(uiRoot,db);
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
                    for(int j=0;j<ms.Count;j++)
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
        public static GameObject CreateNew(params Type[] types)
        {
            return CreateNew(GetTypeIndex(types));
        }
        public static GameObject CreateNew(Int64 type)
        {
            if (type == 0)
                return null;
            for (int i = 0; i < models.Count; i++)
                if (type ==models[i].type)
                    return models[i].CreateObject();
            return null;
        }
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
            var ts = game.GetComponents<Component>();
            Int64 type = GetTypeIndex(ts);
            if (type > 0)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    if (models[i].type == type)
                    {
                        models[i].ReCycle(game);
                        break;
                    }
                }
            }
            var p = game.transform;
            for (int i = p.childCount - 1; i >= 0; i--)
                RecycleGameObject(p.GetChild(i).gameObject);
            if (type > 0)
                p.SetParent(CycleBuffer);
            else GameObject.Destroy(game);

        }
        public static void RecycleSonObject(GameObject game)
        {
            if (game == null)
                return;
            var p = game.transform;
            for (int i = p.childCount - 1; i >= 0; i--)
                RecycleGameObject(p.GetChild(i).gameObject);
        }
        #endregion
    }
}
