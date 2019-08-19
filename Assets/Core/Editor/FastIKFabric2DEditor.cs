using Assets.Core.IK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FastIKFabric2D), true)]
[CanEditMultipleObjects]
public class FastIKFabric2DEditor:Editor
{
    Vector3 pos;
    Vector3 angle;
    Vector2 size;
    Vector2 pivot;
    private void OnEnable()
    {
        var trans = (target as FastIKFabric2D).transform as RectTransform;
        pos = trans.localPosition;
        angle = trans.localEulerAngles;
        size = trans.sizeDelta;
        pivot = trans.pivot;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var help = target as FastIKFabric2D;
        var trans = help.transform as RectTransform;
        bool changed = false;
        if (trans.localPosition != pos)
        {
            pos = trans.localPosition;
            changed = true;
        }
        if (trans.localEulerAngles != angle)
        {
            angle = trans.localEulerAngles;
            changed = true;
        }
        if (trans.sizeDelta != size)
        {
            size = trans.sizeDelta;
            changed = true;
        }
        if (trans.pivot != pivot)
        {
            pivot = trans.pivot;
            changed = true;
        }
        if (changed)
        {
            help.ResolveIK();
            if (help.shareImage != null)
                help.shareImage.Refresh();
        }
    }
}
