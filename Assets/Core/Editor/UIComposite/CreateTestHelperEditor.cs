using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
