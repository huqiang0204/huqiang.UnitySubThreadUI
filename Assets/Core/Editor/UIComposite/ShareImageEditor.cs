using System;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShareImage), true)]
[CanEditMultipleObjects]
public class ShareImageEditor : Editor
{
    public void OnEnable()
    {
        (target as ShareImage).Refresh();
    }
}