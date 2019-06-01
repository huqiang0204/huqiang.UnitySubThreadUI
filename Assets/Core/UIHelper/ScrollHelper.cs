using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using huqiang.UIComposite;
using UnityEngine;

public class ScrollHelper: UICompositeHelp
{
    public Vector2 minBox;
    public unsafe override FakeStruct ToFakeStruct(DataBuffer data)
    {
        FakeStruct fake = new FakeStruct(data, ScrollInfo.ElementSize);
        ScrollInfo* info = (ScrollInfo*)fake.ip;
        info->minBox = minBox;
        return base.ToFakeStruct(data);
    }
}
