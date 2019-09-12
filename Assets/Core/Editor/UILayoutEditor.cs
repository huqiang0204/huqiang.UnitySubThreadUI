using System;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UILayout), true)]
[CanEditMultipleObjects]
public class UILayoutEditor:Editor
{
    public void OnEnable()
    {
        (target as UILayout).Refresh();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
        {
            (target as UILayout).Refresh();
        }
    }
}
