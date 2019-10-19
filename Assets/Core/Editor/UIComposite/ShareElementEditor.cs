using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShareImageChild), true)]
[CanEditMultipleObjects]
public class ShareElementEditor:Editor
{
    Vector3 pos;
    Vector3 angle;
    Vector2 size;
    Vector2 pivot;
    private void OnEnable()
    {
        var trans = (target as ShareImageChild).transform as RectTransform;
        pos = trans.localPosition;
        angle = trans.localEulerAngles;
        size = trans.sizeDelta;
        pivot = trans.pivot;
    }
    public override void OnInspectorGUI()
    {
        var help = target as ShareImageChild;
        if(help!=null)
        {
            var col = help.color;
            help.color= EditorGUILayout.ColorField(col);
            var o = EditorGUILayout.ObjectField(help.sprite, typeof(Sprite), true);
            help.SetSprite(o as Sprite);
            help.fillAmountX = EditorGUILayout.Slider(help.fillAmountX, 0, 1);
        }
        bool changed = GUI.changed;
        if (GUILayout.Button("SetNativeSize"))
        {
            changed = true;
            help.SetNactiveSize();
        }
        if (GUILayout.Button("SetSpritePivot"))
        {
            changed = true;
            help.SetSpritePivot();
        }

        var trans = help.transform as RectTransform;
        if(trans.localPosition!=pos)
        {
            pos = trans.localPosition;
            changed = true;
        }
        if(trans.localEulerAngles!=angle)
        {
            angle = trans.localEulerAngles;
            changed = true;
        }
        if(trans.sizeDelta!=size)
        {
            size = trans.sizeDelta;
            changed = true;
        }
        if(trans.pivot!=pivot)
        {
            pivot = trans.pivot;
            changed = true;
        }
        if (changed)
        {
            RefreshParent(help.transform.parent);
        }
    }
    bool RefreshParent(Transform trans)
    {
        var help = trans.GetComponent<ShareImageHelper>();
        if(help!=null)
        {
            help.Refresh();
            var rect = (help.transform as RectTransform);
             var s=rect.sizeDelta;
            if(s.x>100)
                s.x--;
            else s.x++;
            rect.sizeDelta = s;
            return true;
        }else if(trans.parent!=null)
        {
            return RefreshParent(trans.parent);
        }
        return false;
    }
}
