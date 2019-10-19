using huqiang;
using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AniRecordHelper : UICompositeHelp
{
    public class RecordData
    {
        public float time;
        public byte[] data;
    }
    public List<RecordData> records = new List<RecordData>();
    public void AddRecord()
    {
        App.RegUI();
        DataBuffer db = new DataBuffer(1024);
        db.fakeStruct = ModelElement.LoadFromObject(transform as RectTransform, db);
        RecordData record = new RecordData();
        record.data = db.ToBytes();
        records.Add(record);
    }
    public void Remove(int index)
    {
        if (index < 0)
            return;
        else if (index >= records.Count)
            return;
        records.RemoveAt(index);
    }
    public void Apply(int index)
    {
        if (index < 0)
            return;
        else if (index >= records.Count)
            return;
        DataBuffer db = new DataBuffer(records[index].data);
        ModelElement mod = new ModelElement();
        mod.Load(db.fakeStruct);
        Apply(transform as RectTransform, mod);
    }
    void Apply(RectTransform transform, ModelElement model)
    {
        ModelElement.LoadToObject(transform,ref model.data,model);
        var sc = transform.GetComponent<ShareImageChild>();
        if(sc!=null)
        {
            var sce= model.GetComponent<ShareImageChildElement>();
            if(sce!=null)
            {
                sc.color = sce.data.color;
            }
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            var mod = model.Find(c.name);
            if (mod != null)
            {
                Apply(c as RectTransform, mod);
            }
        }
    }
}
