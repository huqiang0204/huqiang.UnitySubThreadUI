using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AniRecordHelper), true)]
[CanEditMultipleObjects]
public class AniRecordEditor:Editor
{
    public override void OnInspectorGUI()
    {
        AniRecordHelper help = target as AniRecordHelper;

       for(int i=0;i<help.records.Count;i++)
        {
            GUILayout.BeginHorizontal();
            help.records[i].time = EditorGUILayout.FloatField(help.records[i].time);
            if (GUILayout.Button("Apply"))
            {
                help.Apply(i);
                var si = help.GetComponent<ShareImage>();
                if (si != null)
                    si.Refresh();
            }
            if (GUILayout.Button("Delete"))
            {
                help.Remove(i);
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add"))
        {
            help.AddRecord();
        }
    }
}
