using System;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShareText), true)]
[CanEditMultipleObjects]
public class ShareTextEditor:Editor
{
    public void OnEnable()
    {
        (target as ShareText).Refresh();
    }
}
