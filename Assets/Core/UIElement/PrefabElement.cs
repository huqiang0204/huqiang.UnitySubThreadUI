using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using UnityEngine;

namespace huqiang.UI
{
    public unsafe struct PrefabData
    {
        public int AssetsName;
        public int PrefabName;
        public static int Size = sizeof(PrefabData);
        public static int ElementSize = Size / 4;
    }
    public class PrefabScript
    {
        public virtual void Start()
        {
        }
        public virtual void Update()
        {
        }
    }
    public class PrefabElement:DataConversion
    {
        public PrefabScript script;
        public string AssetsName;
        public string PrefabName;
        PrefabData data;
        public GameObject Context;
        public unsafe override void Load(FakeStruct fake)
        {
            model.Entity = false;
            model.AutoRecycle = false;
            data = *(PrefabData*)fake.ip;
            AssetsName = fake.buffer.GetData(data.AssetsName) as string;
            PrefabName = fake.buffer.GetData(data.PrefabName) as string;
        }
        public override void Apply()
        {
            if(IsChanged)
            {
                IsChanged = false;
                if (model.Context == null)
                {
                    var ins = ElementAsset.LoadAssets<GameObject>(PrefabName, AssetsName);
                    if (ins != null)
                    {
                        Context = GameObject.Instantiate(ins);
                        model.Context = Context.transform as RectTransform;
                        if (script != null)
                            script.Start();
                    }
                }
            }else if(script!=null)
            {
                script.Update();
            }
        }
    }
}
