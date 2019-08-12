using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.UI;
using huqiang.Data;

public class ShareChild : MonoBehaviour
{
    public float fillAmountX = 1;
    public float fillAmountY = 1;
    public Color color = Color.white;
    UIVertex[] buff = new UIVertex[4];
    public Sprite sprite;
    Vector2[] uvs = new Vector2[4];
    public void SetSprite(Sprite sp)
    {
        if (sp == null)
            return;
        sprite = sp;
        float tx = sprite.texture.width;
        float ty = sprite.texture.height;
        float x = sprite.rect.x / tx;
        float y = sprite.rect.y / ty;
        float w = sprite.rect.width;
        float h = sprite.rect.height;
        float r = x + w / tx;
        float t = y + h / ty;
        uvs[0].x = x;
        uvs[0].y = y;
        uvs[1].x = x;
        uvs[1].y = t;
        uvs[2].x = r;
        uvs[2].y = t;
        uvs[3].x = r;
        uvs[3].y = y;
    }
    public void GetUVInfo(List<UIVertex> vertices, List<int> tri, Vector3 position, Quaternion quate, Vector3 scale)
    {
        SetSprite(sprite);
        var rect = transform as RectTransform;
        float w = rect.localScale.x * rect.sizeDelta.x;
        float h = rect.localScale.y * rect.sizeDelta.y;
        var pos = rect.localPosition;
        pos = quate * pos + position;
        float left = -rect.pivot.x * w;
        float right = left + w * fillAmountX;
        float down = -rect.pivot.y * h;
        float top = down + h*fillAmountY;
        Vector3 ls = rect.localScale;
        ls.x *= scale.x;
        ls.y *= scale.y;
        right *= ls.x;
        left *= ls.x;
        down *= ls.y;
        top *= ls.y;
        buff[0].color = color;
        buff[1].color = color;
        buff[2].color = color;
        buff[3].color = color;

        var q = rect.localRotation * quate;
        buff[0].position = q * new Vector3(left, down) + pos;
        buff[1].position = q * new Vector3(left, top) + pos;
        buff[2].position = q * new Vector3(right, top) + pos;
        buff[3].position = q * new Vector3(right, down) + pos;
        float uw = uvs[2].x - uvs[1].x;
        float ux = uvs[1].x + uw * fillAmountX;
        float uh= uvs[2].y - uvs[1].y;
        float uy = uvs[1].y + uh * fillAmountY;
        buff[0].uv0 = uvs[0];
        buff[1].uv0 = uvs[1];
        buff[1].uv0.y = uy;
        buff[2].uv0.y = uy;
        buff[2].uv0.x = ux;
        buff[3].uv0 = uvs[3];
        buff[3].uv0.x = ux;
        int s = vertices.Count;
        vertices.AddRange(buff);
        tri.Add(s);
        tri.Add(s + 1);
        tri.Add(s + 2);
        tri.Add(s + 2);
        tri.Add(s + 3);
        tri.Add(s);
        for (int i = 0; i < rect.childCount; i++)
        {
            var help = rect.GetChild(i).GetComponent<ShareChild>();
            if (help != null)
            {
                help.GetUVInfo(vertices, tri, pos, q, ls);
            }
        }
    }
    public void SetNactiveSize()
    {
        if (sprite != null)
        {
            float w = sprite.rect.width;
            float h = sprite.rect.height;
            (transform as RectTransform).sizeDelta = new Vector2(w, h);
        }
    }
    public void SetSpritePivot()
    {
        if(sprite!=null)
        {
            (transform as RectTransform).pivot = new Vector2(sprite.pivot.x/ sprite.rect.width,sprite.pivot.y/ sprite.rect.height);
        }
    }
}
