using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateTestHelper), true)]
[CanEditMultipleObjects]
public class TestHelperEditor : Editor
{
    protected virtual void OnEnable() {
        //(target as CreateTestHelper).Build();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh"))
        {
            (target as CreateTestHelper).Build();
        }
    }
}
