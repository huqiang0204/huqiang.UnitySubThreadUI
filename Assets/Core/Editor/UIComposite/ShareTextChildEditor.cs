using System;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShareTextChild), true)]
[CanEditMultipleObjects]
public class ShareTextChildEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var tar = target as ShareTextChild;
        if(tar!=null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Font Size");
            tar.FontSize = EditorGUILayout.IntField(tar.FontSize);
            EditorGUILayout.EndHorizontal();
            tar.text = GUILayout.TextArea(tar.text);
            if(GUI.changed)
            {
                Refresh(tar.transform);
            }
        }
    }
    static void Refresh(Transform transform)
    {
        var t = transform.parent;
        if (t == null)
            return;
        var st =  t.GetComponent<ShareText>();
        if (st != null)
        {
            st.Refresh();
        }
        else Refresh(t);
    }
}