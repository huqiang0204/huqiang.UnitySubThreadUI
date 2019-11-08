using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Data;

public class TransformTree
{
    unsafe struct LocalCoordinates
    {
        public Vector3 Postion;
        public Quaternion Rotate;
        public Vector3 Scale;
        public Int32 Name;
        public Int32 Child;
        public static int Size = sizeof(LocalCoordinates);
        public static int ElementSize = Size / 4;
    }
    public static unsafe byte[] Save(Transform root)
    {
        DataBuffer db = new DataBuffer(1024);
        FakeStruct fake = new FakeStruct(db, LocalCoordinates.ElementSize);
        LocalCoordinates* lc = (LocalCoordinates*)fake.ip;
        lc->Postion = root.localPosition;
        lc->Rotate = root.localRotation;
        lc->Scale = root.localScale;
        lc->Name = db.AddData(root.name);
        lc->Child = db.AddData(SaveChild(root, db));
        db.fakeStruct = fake;
        return db.ToBytes();
    }
    static unsafe FakeStructArray SaveChild(Transform root, DataBuffer db)
    {
        int c = root.childCount;
        if (c > 0)
        {
            FakeStructArray array = new FakeStructArray(db, LocalCoordinates.ElementSize, c);
            Int32[] cc = new int[c];
            for (int i = 0; i < c; i++)
            {
                var a = root.GetChild(i);
                LocalCoordinates* lc = (LocalCoordinates*)array[i];
                lc->Postion = a.localPosition;
                lc->Rotate = a.localRotation;
                lc->Scale = a.localScale;
                lc->Name = db.AddData(a.name);
                lc->Child = db.AddData(SaveChild(a, db));
            }
            return array;
        }
        return null;
    }
    public static unsafe void Load(Transform root, byte[] data)
    {
        DataBuffer db = new DataBuffer(data);
        FakeStruct fake = db.fakeStruct;
        LocalCoordinates* lc = (LocalCoordinates*)fake.ip;
        root.localPosition = lc->Postion;
        root.localRotation = lc->Rotate;
        root.localScale = lc->Scale;
        var array = db.GetData(lc->Child) as FakeStructArray;
        if (array != null)
        {
            LoadChild(root, db, array);
        }
    }
    unsafe static void LoadChild(Transform root, DataBuffer db, FakeStructArray array)
    {
        var c = root.childCount;
        for (int i = 0; i < c; i++)
        {
            var t = root.GetChild(c);
            var name = t.name;
            int l = array.Length;
            for (int j = 0; j < l; j++)
            {
                LocalCoordinates* lc = (LocalCoordinates*)array[j];
                string str = db.GetData(lc->Name) as string;
                if (str == name)
                {
                    t.localPosition = lc->Postion;
                    t.localRotation = lc->Rotate;
                    t.localScale = lc->Scale;
                    var arr = db.GetData(lc->Child) as FakeStructArray;
                    if (arr != null)
                    {
                        LoadChild(t, db, arr);
                    }
                }
            }
        }
    }
}