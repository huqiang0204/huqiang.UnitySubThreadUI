using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using UnityEngine;

namespace huqiang.UI
{
    public unsafe struct ShareChildData
    {
        public Color color;
        public Vector2 txtSize;
        public Rect rect;
        public Vector2 pivot;
        public float fillAmountX;
        public float fillAmountY;
        public int spriteName;
        public static int Size = sizeof(ShareChildData);
        public static int ElementSize = Size / 4;
    }
    public class ShareChildElement:DataConversion,Coloring
    {
        public ShareChildData data;
        public string spriteName;

        public Color color { get => data.color; set => data.color = value; }
        UIVertex[] buff = new UIVertex[4];
        Vector2[] uvs = new Vector2[4];
        public override void Reset()
        {
            model.Entity = false;
            data.fillAmountX = 1;
            data.fillAmountY = 1;
            data.color = Color.white;
            model.ColorController = this;
        }
        public override unsafe void Load(FakeStruct fake)
        {
            data = *(ShareChildData*)fake.ip;
            model.color = data.color;
            spriteName= fake.buffer.GetData(data.spriteName) as string;
            model.Entity = false;
            float tx = data.txtSize.x;
            float ty = data.txtSize.y;
            float x = data.rect.x / tx;
            float y = data.rect.y / ty;
            float w = data.rect.width;
            float h = data.rect.height;
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
            var rect = model;
            float w = rect.data.localScale.x * rect.data.sizeDelta.x;
            float h = rect.data.localScale.y * rect.data.sizeDelta.y;
            var pos = rect.data.localPosition;
            pos = quate * pos + position;
            float left = -rect.data.pivot.x * w;
            float right = left + w * data.fillAmountX;
            float down = -rect.data.pivot.y * h;
            float top = down + h;
            Vector3 ls = rect.data.localScale;
            ls.x *= scale.x;
            ls.y *= scale.y;
            right *= ls.x;
            left *= ls.x;
            down *= ls.y;
            top *= ls.y;
            buff[0].color = data.color;
            buff[1].color = data.color;
            buff[2].color = data.color;
            buff[3].color = data.color;

            var q = rect.data.localRotation * quate;
            if (spriteName!=null)
            {
                buff[0].position = q * new Vector3(left, down) + pos;
                buff[1].position = q * new Vector3(left, top) + pos;
                buff[2].position = q * new Vector3(right, top) + pos;
                buff[3].position = q * new Vector3(right, down) + pos;
                float tx = data.txtSize.x;
                float ty = data.txtSize.y;
                float l = data.rect.x / tx;
                float d = data.rect.y / ty;
                float r = l + data.rect.width / tx * data.fillAmountX;
                float t = d + data.rect.height / ty;
                buff[0].uv0.x = l;
                buff[0].uv0.y = d;
                buff[1].uv0.x = l;
                buff[1].uv0.y = t;
                buff[2].uv0.x = r;
                buff[2].uv0.y = t;
                buff[3].uv0.x = r;
                buff[3].uv0.y = d;
                int s = vertices.Count;
                vertices.AddRange(buff);
                tri.Add(s);
                tri.Add(s + 1);
                tri.Add(s + 2);
                tri.Add(s + 2);
                tri.Add(s + 3);
                tri.Add(s);
            }
            for (int i = 0; i < rect.child.Count; i++)
            {
                var help = rect.child[i].GetComponent<ShareChildElement>();
                if (help != null)
                {
                    help.GetUVInfo(vertices, tri, pos, q, ls);
                }
            }
        }
        public void SetNactiveSize()
        {
            float w = data.rect.width;
            float h = data.rect.height;
            model.data.sizeDelta = new Vector2(w, h);
        }
        public void SetSpritePivot()
        {
           model.data.pivot = new Vector2(data.pivot.x / data.rect.width, data.pivot.y / data.rect.height);
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var share = com as ShareChild;
            if (share == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, ShareChildData.ElementSize);
            ShareChildData* sp = (ShareChildData*)fake.ip;
            sp->color =share.color;
            sp->fillAmountX = share.fillAmountX;
            sp->fillAmountY = share.fillAmountY;
            var sprite = share.sprite;
            if (sprite != null)
            {
                sp->rect = sprite.rect;
                sp->txtSize.x = sprite.texture.width;
                sp->txtSize.y = sprite.texture.height;
                sp->pivot = sprite.pivot;
                sp->spriteName = buffer.AddData(sprite.name);
            }
            return fake;
        }
    }
}
