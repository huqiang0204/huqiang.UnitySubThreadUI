using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EmojiText), true)]
[CanEditMultipleObjects]
public class EmojiTextEditor:Editor
{
    private void OnEnable()
    {
        var emoji = target as EmojiText;
        if(emoji!=null)
        {
            if (emoji.material == null)
            {
                emoji.material = new Material(Shader.Find("Custom/UIEmoji"));
                emoji.material.SetTexture("_emoji", Resources.Load<Texture>("emoji"));
            }
            else if(emoji.material.shader.name== "UI/Default")
            {
                emoji.material = new Material(Shader.Find("Custom/UIEmoji"));
                emoji.material.SetTexture("_emoji", Resources.Load<Texture>("emoji"));
            }
        }
    }
}
