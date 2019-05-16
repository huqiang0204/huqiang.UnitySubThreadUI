using huqiang.Data;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UI
{
    public unsafe struct OutLineData
    {
        public Color effectColor;
        public Vector2 effectDistance;
        public bool useGraphicAlpha;
        public static int Size = sizeof(OutLineData);
        public static int ElementSize = Size / 4;
    }
    public class OutLineElement : DataConversion
    {
        public Outline Context;
        public OutLineData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(OutLineData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref OutLineData dat)
        {
            var a = game.GetComponent<Outline>();
            if (a == null)
                return;
            a.effectColor = dat.effectColor;
            a.effectDistance = dat.effectDistance;
            a.useGraphicAlpha = dat.useGraphicAlpha;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var dat = com as Outline;
            if (dat == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, OutLineData.ElementSize);
            OutLineData* data = (OutLineData*)fake.ip;
            data->effectColor = dat.effectColor;
            data->effectDistance = dat.effectDistance;
            data->useGraphicAlpha = dat.useGraphicAlpha;
            return fake;
        }
    }
}
