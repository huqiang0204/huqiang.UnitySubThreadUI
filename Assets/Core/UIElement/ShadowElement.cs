using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UI
{
    public unsafe struct ShadowData
    {
        public Color effectColor;
        public Vector2 effectDistance;
        public bool useGraphicAlpha;
        public static int Size = sizeof(ShadowData);
        public static int ElementSize = Size / 4;
    }
    public class ShadowElement:DataConversion
    {
        public Shadow Context;
        ShadowData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(ShadowData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            Context = game.GetComponent<Shadow>();
            if (Context == null)
                return;
            Context.effectColor = data.effectColor;
            Context.effectDistance = data.effectDistance;
            Context.useGraphicAlpha = data.useGraphicAlpha;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var img = com as Shadow;
            if (img == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, ShadowData.ElementSize);
            ShadowData* data = (ShadowData*)fake.ip;
            data->effectColor = img.effectColor;
            data->effectDistance = img.effectDistance;
            data->useGraphicAlpha = img.useGraphicAlpha;
            return fake;
        }
        public override void Apply()
        {
            if(IsChanged)
            {
                IsChanged = false;
                if(Context!=null)
                {
                    Context.effectColor = data.effectColor;
                    Context.effectDistance = data.effectDistance;
                    Context.useGraphicAlpha = data.useGraphicAlpha;
                }
            }
        }
    }
}
