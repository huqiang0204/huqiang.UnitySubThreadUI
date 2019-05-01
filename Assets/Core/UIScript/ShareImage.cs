using huqiang;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UGUI;
using UnityEngine;

public class ShareComponent
{
    public void SetNativeSize()
    {
        if (sprite == null)
            return;
        SizeDelta.x = sprite.rect.width;
        SizeDelta.y = sprite.rect.height;
        Changed = true;
    }
    public Vector2 Pivot=new Vector2(0.5f,0.5f);
    public Vector2 SizeDelta;
    public Vector3 LocalPosition;
    public Vector2 LocalScale = Vector2.one;
    public Vector3 Angle { set { LocalQuaternion = Quaternion.Euler(value); } }
    public Quaternion LocalQuaternion = Quaternion.identity;
    public Color color = Color.white;
    public Sprite sprite;
    UIVertex[] buff = new UIVertex[4];
    static void ReCalcul(ShareComponent image)
    {
        float w = image.LocalScale.x * image.SizeDelta.x;
        float h = image.LocalScale.y * image.SizeDelta.y;
        var Pivot = image.Pivot;
        var pos = image.LocalPosition;
        float px = pos.x;
        float left = px - Pivot.x * w;
        float right = px + (1 - Pivot.x) * w;
        float py = pos.y;      
        float down = py - Pivot.y * h;
        float top = py + (1 - Pivot.y) * h;
        var buff = image.buff;
        var color = image.color;
        buff[0].color = color;
        buff[1].color = color;
        buff[2].color = color;
        buff[3].color = color;
        var sprite = image.sprite;
        if (sprite != null)
        {
            var t = sprite.uv;
            buff[0].uv0 = t[3];
            buff[1].uv0 = t[0];
            buff[2].uv0 = t[2];
            buff[3].uv0 = t[1];
        }
        else
        {
            buff[0].uv0 = Vector2.zero;
            buff[1].uv0 = new Vector2(0, 1);
            buff[2].uv0 = new Vector2(1, 1);
            buff[3].uv0 = new Vector2(1, 0);
        }
        var q = image.LocalQuaternion;
        buff[0].position = q * new Vector3(left, down) + pos;
        buff[1].position = q * new Vector3(left, top) + pos;
        buff[2].position = q * new Vector3(right, top) + pos;
        buff[3].position = q * new Vector3(right, down) + pos;
    }
    public bool Changed;
    public UIVertex[] GetUVInfo()
    {
        if (Changed)
        { ReCalcul(this); Changed = false; }
        return buff;
    }
}
public class ShareImage
{
    public List<ShareComponent> buff = new List<ShareComponent>();
   // static int[] triangles = new int[] { 0, 1, 2, 2, 3, 0 };
    List<UIVertex> vertices;
    List<int> tri;
    public Texture MainTexture;
    public CustomRawImage rawImage { get; private set; }
    public EventCallBack callBack;
    /// <summary>
    /// 开启此项将，事件派发到子ui中
    /// </summary>
    public bool OpenEvent;
    public ShareImage(CustomRawImage image)
    {
        vertices = new List<UIVertex>();
        tri = new List<int>();
        rawImage = image;
        //callBack = EventCallBack.RegEvent<EventCallBack>(image.rectTransform);
    }
    public void ReCalcul()
    {
        int s = 0;
        for(int i=0;i<buff.Count;i++)
        {
            vertices.AddRange(buff[i].GetUVInfo());
            tri.Add(s);
            tri.Add(s +1);
            tri.Add(s + 2);
            tri.Add(s +2);
            tri.Add(s + 3);
            tri.Add(s );
            s += 4;
        }
        rawImage.uIVertices = vertices;
        rawImage.triangle = tri;
        rawImage.Refresh();
    }
}
