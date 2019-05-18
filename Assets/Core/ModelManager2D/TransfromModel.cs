using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using huqiang.Data;
using huqiang.UI;
using UnityEngine;

namespace huqiang.ModelManager2D
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
    public class TransfromModel : DataConversion
    {
        public TransfromData transfrom;
        public string name;
        public string tag;
        public List<DataConversion> components = new List<DataConversion>();
        public List<TransfromModel> child = new List<TransfromModel>();
        unsafe public override void Load(FakeStruct fake)
        {
             transfrom = *(TransfromData*)fake.ip;
             var buff = fake.buffer;
             Int16[] coms = buff.GetData(transfrom.coms) as Int16[];
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
                       var dc= ModelManager2D.Load(type);
                        if(dc!=null)
                        {
                            dc.Load(fs);
                            components.Add(dc);
                        }
                    }
                }
            }
            Int16[] chi = fake.buffer.GetData(transfrom.child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buff.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        TransfromModel model = new TransfromModel();
                        model.Load(fs);
                        child.Add(model);
                    }
                }
            name = buff.GetData(transfrom.name) as string;
            tag = buff.GetData(transfrom.tag) as string;
        }
        public override void LoadToObject(Component com)
        {
            for (int i = 0; i < components.Count; i++)
                if (components[i] != null)
                    components[i].LoadToObject(com);
            var trans = com as Transform;
            trans.localEulerAngles = transfrom.localEulerAngles;
            trans.localPosition = transfrom.localPosition;
            trans.localScale = transfrom.localScale;
            trans.name = name;
            trans.tag = tag;
        }
        public static  unsafe FakeStruct LoadFromObject(Component com,DataBuffer buffer)
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
            td->type = ModelManager2D.GetTypeIndex(coms);
            List<Int16> tmp = new List<short>();
            for(int i=0;i<coms.Length;i++)
            {
                if(!(coms[i] is Transform))
                {
                    Int16 type = 0;
                    var fs = ModelManager2D.LoadFromObject(coms[i],buffer, ref type);
                    tmp.Add((Int16)buffer.AddData(fs));
                    tmp.Add(type);
                }
            }
            td->coms = buffer.AddData(tmp.ToArray());
            int c = trans.childCount;
            if(c>0)
            {
                Int16[] buf = new short[c];
                for(int i=0;i<c;i++)
                {
                    var fs = LoadFromObject(trans.GetChild(i),buffer);
                    buf[i] =(Int16) buffer.AddData(fs);
                }
                td->child = buffer.AddData(buf);
            }
            return fake;
        }
    }
}