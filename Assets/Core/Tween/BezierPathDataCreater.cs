using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BezierPathDataCreater : MonoBehaviour
{
    public string name = "Bezier";
    public FakeStruct CreateData(DataBuffer db)
    {
        if (db == null)
            db = new DataBuffer(1024);
        List<BezierPath> paths = new List<BezierPath>();
        var c = transform.childCount;
        for(int i=0;i<c;i++)
        {
            var path = transform.GetChild(i).GetComponent<BezierPath>();
            if (path != null)
                paths.Add(path);
        }
        var fake = new FakeStruct(db,2);
        fake[0] = db.AddData(name);
        FakeStructArray array = new FakeStructArray(db,2,paths.Count);
        for(int i=0;i<paths.Count;i++)
        {
            var path = paths[i];
            array[i, 0] = db.AddData(path.name);
            array[i, 1] = db.AddArray<Vector3>(GetPathData(path));
        }
        fake[1] = db.AddData(array);
        return fake;
    }
    Vector3[] GetPathData(BezierPath path)
    {
        var nodes = path.nodes;
        var ori = path.transform.position;
        Vector3[] tmp = new Vector3[nodes.Count*3];
        for(int i=0;i<nodes.Count;i++)
        {
            int s = i * 3;
            tmp[s] = nodes[i].transform.position - ori;
            s++;
            tmp[s] = nodes[i].LastDir.position - ori;
            s++;
            tmp[s] = nodes[i].NextDir.position - ori;
        }
        return tmp;
    }
}