using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextInputHelper), true)]
[CanEditMultipleObjects]
public class TextInputEditor : Editor
{
    private void OnEnable()
    {
        (target as TextInputHelper).Refresh();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUI.changed)
        {
            (target as TextInputHelper).Refresh();
        }
    }
}
