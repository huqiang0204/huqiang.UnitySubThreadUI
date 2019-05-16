using huqiang.Data;
using huqiang.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompositeHelp : MonoBehaviour
{
    public enum CompositeType
    {
        Toggle,
        UISlider,
        DropDown,
        DragContent,
        ScrollX,
        ScrollY,
        GridScroll,
        DataTable
    }
    public CompositeType compositeType;
    public ModelElement CreateModel()
    {
        DataBuffer db = new DataBuffer(1024);
        db.fakeStruct = ModelElement.LoadFromObject(transform, db);
        var mod = new ModelElement();
        mod.Load(db.fakeStruct);
        return mod;
    }
    // Start is called before the first frame update
    public virtual FakeStruct ToFakeStruct(DataBuffer data)
    {
        return null;
    }
}
