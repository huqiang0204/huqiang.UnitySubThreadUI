using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierPathDataCreater), true)]
[CanEditMultipleObjects]
public class BezierPathDataCreaterEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create"))
        {
            var be = target as BezierPathDataCreater;
            var fake = be.CreateData(null);
            fake.buffer.fakeStruct = fake;
            var dat = fake.buffer.ToBytes();
            string path = Application.dataPath + "/AssetsBundle/"+be.name+".bytes";
            File.WriteAllBytes(path, dat);
            Debug.Log("create done : " + path);
        }
    }
}
