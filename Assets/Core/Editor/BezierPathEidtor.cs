using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPath), true)]
[CanEditMultipleObjects]
public class BezierPathEidtor:Editor
{
    private void OnEnable()
    {
        (target as BezierPath).Initial();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Add Node"))
        {
            (target as BezierPath).InsertNode(99999);
        }
         //(target as BezierPath).ShowLine();
    }
}
