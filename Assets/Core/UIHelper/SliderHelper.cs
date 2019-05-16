﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using huqiang.UI;
using huqiang.UIComposite;
using UnityEngine;
using static huqiang.UIComposite.UISlider;

public class SliderHelper : UICompositeHelp
{
    public Vector2 StartOffset;
    public Vector2 EndOffset;
    public float MinScale=1;
    public float MaxScale=1;
    public Direction direction;
    UISlider slider;
    public void Awake()
    {
        DataBuffer db = new DataBuffer(1024);
        db.fakeStruct = ModelElement.LoadFromObject(transform, db);
        var mod = new ModelElement();
        mod.Load(db.fakeStruct);
    }
    public unsafe override FakeStruct ToFakeStruct(DataBuffer buffer)
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