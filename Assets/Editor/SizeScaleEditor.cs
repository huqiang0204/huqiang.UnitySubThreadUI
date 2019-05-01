using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SizeScaleEx), true)]
[CanEditMultipleObjects]
public class SizeScaleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        if (GUILayout.Button("Refresh"))
        {
            (target as SizeScaleEx).EditorRefresh();
        }
        serializedObject.ApplyModifiedProperties();
    }
}

