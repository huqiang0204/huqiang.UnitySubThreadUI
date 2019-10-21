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
        var share = target as ShareText;
        if(share!=null)
        {
            if (share.material == null)
            {
                share.material = new Material(Shader.Find("Custom/UIEmoji"));
                share.material.SetTexture("_emoji", Resources.Load<Texture>("emoji"));
            }
            else if (share.material.shader.name == "UI/Default")
            {
                share.material = new Material(Shader.Find("Custom/UIEmoji"));
                share.material.SetTexture("_emoji", Resources.Load<Texture>("emoji"));
            }
            share.Refresh();
        }
    }
}
