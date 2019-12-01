using huqiang.Data;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace huqiang.Data2D
{
    public class PrefabAsset
    {
        public string name;
        public FakeStruct models;
    }
    public class ModelManager2D
    {
        public static GameobjectBuffer GameBuffer;
        public static void Initial(Transform buff)
        {
            GameBuffer = new GameobjectBuffer(buff);
            GameBuffer.RegComponent(new ComponentInfo<Transform>() { loader = new TransfromLoader() });
            GameBuffer.RegComponent(new ComponentInfo<SpriteRenderer>() { loader=new SpriteRenderLoader() });
            GameBuffer.RegComponent(new ComponentInfo<SpriteMask>() { loader = new SpriteMaskLoader() });
            GameBuffer.RegComponent(new ComponentInfo<BoxCollider2D>() { loader=new BoxColliderLoader()});
            GameBuffer.RegComponent(new ComponentInfo<CircleCollider2D>() { loader=new CircleColliderLoader()});
            GameBuffer.RegComponent(new ComponentInfo<PolygonCollider2D>() { loader=new PolygonColliderLoader()});
            GameBuffer.RegComponent(new ComponentInfo<EdgeCollider2D>() { loader=new EdgeColliderLoader()});
            GameBuffer.RegComponent(new ComponentInfo<CapsuleCollider2D>() { loader=new CapsuleColliderLoader()});
            GameBuffer.RegComponent(new ComponentInfo<CompositeCollider2D>() { loader = new CompositeColliderLoader() });
            GameBuffer.RegComponent(new ComponentInfo<Rigidbody2D>() { loader=new RigidbodyLoader()});

            GameBuffer.RegComponent(new ComponentInfo<AreaEffector2D>() { loader = new AreaEffectLoader() });
            GameBuffer.RegComponent(new ComponentInfo<BuoyancyEffector2D>() { loader = new BuoyancyLoader() });
            GameBuffer.RegComponent(new ComponentInfo<ConstantForce2D>() { loader = new ConstantForceLoader() });
            GameBuffer.RegComponent(new ComponentInfo<DistanceJoint2D>() { loader = new DistanceJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<FixedJoint2D>() { loader = new FixedJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<FrictionJoint2D>() { loader = new FrictionJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<HingeJoint2D>() { loader = new HingeJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<PlatformEffector2D>() { loader = new PlatformEffectorLoader() });
            GameBuffer.RegComponent(new ComponentInfo<PointEffector2D>() { loader = new PointEffectorLoader() });
            GameBuffer.RegComponent(new ComponentInfo<RelativeJoint2D>() { loader = new RelativeJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<SliderJoint2D>() { loader = new SliderJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<SpringJoint2D>() { loader = new SpringJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<SurfaceEffector2D>() { loader = new SurfaceEffectorLoader() });
            GameBuffer.RegComponent(new ComponentInfo<TargetJoint2D>() { loader = new TargetJointLoader() });
            GameBuffer.RegComponent(new ComponentInfo<WheelJoint2D>() { loader = new WheelJointLoader() });
        }
        /// <summary>
        /// 将场景内的对象保存到文件
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <param name="path"></param>
        public static void SavePrefab(Transform uiRoot, string path)
        {
            DataBuffer db = new DataBuffer(1024);
            db.fakeStruct = GameBuffer.GetDataLoader(0).LoadFromObject(uiRoot, db);
            File.WriteAllBytes(path, db.ToBytes());
        }
        static List<PrefabAsset> prefabAssets = new List<PrefabAsset>();
        public unsafe static PrefabAsset LoadModels(byte[] buff, string name)
        {
            DataBuffer db = new DataBuffer(buff);
            var asset = new PrefabAsset();
            asset.models = db.fakeStruct;
            asset.name = name;
            for (int i = 0; i < prefabAssets.Count; i++)
                if (prefabAssets[i].name == name)
                { prefabAssets.RemoveAt(i); break; }
            prefabAssets.Add(asset);
            return asset;
        }
    }
}
