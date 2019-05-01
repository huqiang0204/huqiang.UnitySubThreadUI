using huqiang.Data;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UI
{
    public class MaskElement : DataConversion
    {
        Mask Context;
        bool data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(bool*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            var a = game.GetComponent<Mask>();
            if (a == null)
                return;
            a.showMaskGraphic = data;
            Context = a;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var img = com as Mask;
            if (img == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, 1);
            *(bool*)fake.ip = img.showMaskGraphic;
            return fake;
        }
    }
}
