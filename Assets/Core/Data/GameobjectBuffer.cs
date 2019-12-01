using huqiang;
using huqiang.Data;
using huqiang.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Data
{
    public abstract class TypeInfo
    {
        public int Index;
        public Type type;
        public string name;
        public DataLoader loader;
        /// <summary>
        /// 类型比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Compare(object obj)
        {
            return false;
        }
    }
    public class ComponentInfo<T> : TypeInfo where T : Component 
    {
        public ComponentInfo()
        {
            type = typeof(T);
            name = type.Name;
        }
        public override bool Compare(object obj)
        {
            return obj is T;
        }
    }
    public class GameobjectBuffer
    {
        Transform CycleBuffer;
        public GameobjectBuffer(Transform buffer)
        {
            CycleBuffer = buffer;
        }
        int point;
        TypeInfo[] types = new TypeInfo[63];
        List<ModelBuffer> models = new List<ModelBuffer>();
        Container<InstanceContext> container = new Container<InstanceContext>(4096);
        /// <summary>
        /// 注册一个组件
        /// </summary>
        /// <param name="info"></param>
        public void RegComponent(TypeInfo info)
        {
            if (point >= 63)
                return;
            if (info.loader != null)
                info.loader.gameobjectBuffer = this;
            var name = info.name;
            for (int i = 0; i < point; i++)
                if (types[i].name == name)
                {
                    types[i] = info;
                    return;
                }
            info.Index = point;
            types[point] = info;
            point++;
        }
        /// <summary>
        /// 获取组件的索引
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public Int32 GetTypeIndex(Component com)
        {
            for (int i = 0; i < point; i++)
            {
                if (types[i].Compare(com))
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取组件的ID
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public Int64 GetTypeID(Component com)
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
        /// <summary>
        /// 获取一组组件的ID
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public Int64 GetTypeID(Component[] com)
        {
            if (com == null)
                return 0;
            Int64 a = 1;
            for (int i = 0; i < com.Length; i++)
            {
                var c = com[i];
                if (c != null)
                    a |= GetTypeID(c);
            }
            return a;
        }
        /// <summary>
        /// 创建一个模型缓存
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ModelBuffer CreateModelBuffer(Int64 type, Int32 size = 64)
        {
            long t = type;
            List<Type> tmp = new List<Type>();
            for (int i = 1; i < 64; i++)
            {
                t >>= 1;
                if (t > 0)
                {
                    if ((t & 1) > 0)
                    {
                        tmp.Add(types[i].type);
                    }
                }
                else break;
            }
            ModelBuffer model = new ModelBuffer(type, size, tmp.ToArray(), container);
            models.Add(model);
            return model;
        }
        /// <summary>
        /// 通过类型创建一个对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject CreateNew(Int64 type)
        {
            if (type == 0)
                return null;
            for (int i = 0; i < models.Count; i++)
                if (type == models[i].type)
                    return models[i].CreateObject();
            return CreateModelBuffer(type).CreateObject();
        }
        /// <summary>
        /// 回收游戏对象
        /// </summary>
        /// <param name="game"></param>
        public void RecycleGameObject(GameObject game)
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
        public DataLoader FindDataLoader(Component com)
        {
            for(int i=0;i<types.Length;i++)
            {
                if (types[i].Compare(com))
                    return types[i].loader;
            }
            return null;
        }
        public DataLoader GetDataLoader(int Index)
        {
            return types[Index].loader;
        }
        /// <summary>
        /// 克隆一个预制体对象
        /// </summary>
        /// <param name="fake"></param>
        public GameObject Clone(FakeStruct fake)
        {
            long id = fake.GetInt64(0);
            var go = CreateNew(id);
            types[0].loader.LoadToObject(fake,go.transform);
            return go;
        }
        /// <summary>
        /// 查询transform的子物体
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public unsafe FakeStruct FindChild(FakeStruct fake,string childName)
        {
            var data = (TransfromData*)fake.ip;
            var buff = fake.buffer;
            Int16[] chi = fake.buffer.GetData(data->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var cd = (TransfromData*)fs.ip;
                        string name = buff.GetData(cd->name) as string;
                        if (name == childName)
                            return fs;
                    }
                }
            return null;
        }
    }
}
