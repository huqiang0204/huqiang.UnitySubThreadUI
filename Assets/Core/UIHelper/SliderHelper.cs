using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using huqiang.UI;
using huqiang.UIComposite;
using UnityEngine;

public class SliderHelper : UICompositeHelp
{
    public Vector2 StartOffset;
    public Vector2 EndOffset;
    public float MinScale=1;
    public float MaxScale=1;
    public UISlider.Direction direction;
    UISlider slider;
    public unsafe override object ToBufferData(DataBuffer buffer)
    {
        FakeStruct fake = new FakeStruct(buffer, SliderInfo.ElementSize);
        SliderInfo* data = (SliderInfo*)fake.ip;
        data->StartOffset = StartOffset;
        data->EndOffset = EndOffset;
        data->MinScale = MinScale;
        data->MaxScale = MaxScale;
        data->direction = direction;
        return fake;
    }
}