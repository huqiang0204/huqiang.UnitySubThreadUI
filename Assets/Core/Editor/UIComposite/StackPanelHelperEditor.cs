using System;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StackPanelHelper), true)]
[CanEditMultipleObjects]
public class StackPanelHelperEditor:Editor
{
    public void OnEnable()
    {
        (target as StackPanelHelper).Refresh();
    }
}
