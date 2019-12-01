using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Data
{
    public unsafe struct TransfromData
    {
        public Int64 type;
        public Vector3 localEulerAngles;
        public Vector3 localPosition;
        public Vector3 localScale;
        public Int32 name;
        public Int32 tag;
        /// <summary>
        /// int32数组,高16位为索引,低16位为类型
        /// </summary>
        public Int32 coms;
        /// <summary>
        /// int16数组
        /// </summary>
        public Int32 child;
        public static int Size = sizeof(TransfromData);
        public static int ElementSize = Size / 4;
    }
    public class TransfromLoader : DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            var transfrom = (TransfromData*)fake.ip;
            var trans = com as Transform;
            var buff = fake.buffer;
            Int16[] coms = buff.GetData(transfrom->coms) as Int16[];
            if (coms != null)
            {
                for (int i = 0; i < coms.Length; i++)
                {
                    int index = coms[i];
                    i++;
                    int type = coms[i];
                    var fs = buff.GetData(index) as FakeStruct;
                    if (fs != null)
                    {
                        var loader = gameobjectBuffer.GetDataLoader(type);
                        if (loader != null)
                            loader.LoadToObject(fs, com);
                    }
                }
            }
            Int16[] chi = fake.buffer.GetData(transfrom->child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var go = gameobjectBuffer.CreateNew(fs.GetInt64(0));
                        if (go != null)
                        {
                            go.transform.SetParent(trans);
                            this.LoadToObject(fs, go.transform);
                        }
                    }
                }
            com.name = buff.GetData(transfrom->name) as string;
            com.tag = buff.GetData(transfrom->tag) as string;
            trans.localEulerAngles = transfrom->localEulerAngles;
            trans.localPosition = transfrom->localPosition;
            trans.localScale = transfrom->localScale;
        }
        public override unsafe FakeStruct LoadFromObject(Component com,DataBuffer buffer)
        {
            var trans =  com as Transform;
            if (trans == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, TransfromData.ElementSize);
            TransfromData* td = (TransfromData*)fake.ip;
            td->localEulerAngles = trans.localEulerAngles;
            td->localPosition = trans.localPosition;
            td->localScale = trans.localScale;
            td->name = buffer.AddData(trans.name);
            td->tag = buffer.AddData(trans.tag);
            var coms = com.GetComponents<Component>();
            td->type = gameobjectBuffer.GetTypeID(coms);
            List<Int16> tmp = new List<short>();
            for(int i=0;i<coms.Length;i++)
            {
                if(!(coms[i] is Transform))
                {
                    Int32 type = gameobjectBuffer.GetTypeIndex(coms[i]);
                    var loader = gameobjectBuffer.GetDataLoader(type);
                    if (loader != null)
                    {
                        var fs = loader.LoadFromObject(coms[i], buffer);
                        tmp.Add((Int16)buffer.AddData(fs));
                    }
                    else tmp.Add(0);
                    tmp.Add((Int16)type);
                }
            }
            td->coms = buffer.AddData(tmp.ToArray());
            int c = trans.childCount;
            if (c > 0)
            {
                Int16[] buf = new short[c];
                for (int i = 0; i < c; i++)
                {
                    var fs = LoadFromObject(trans.GetChild(i), buffer);
                    buf[i] = (Int16)buffer.AddData(fs);
                }
                td->child = buffer.AddData(buf);
            }
            return fake;
        }
    }
}