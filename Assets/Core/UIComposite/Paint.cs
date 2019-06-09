using huqiang.UI;
using huqiang.UIEvent;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class Paint : ModelInital
    {
        ModelElement model;
        RawImageElement raw;
        Color[] buffer;
        public int Width;
        public int Height;
        int HalfW;
        int HalfH;
        public float BrushSize =2;
        public Color BrushColor=Color.white;
        GestureEvent gesture;
        Vector3 Origin;
        Vector3 LastPos;
        Vector3 CurPos;
        Texture2D texture;
        Vector2[] LastBox;
        Vector2 LastDirect;
        Vector2 CurDirect;
        public override void Initial(ModelElement mod)
        {
            base.Initial(mod);
            model = mod;
            raw = model.GetComponent<RawImageElement>();
            var size = model.data.sizeDelta;
            Width = (int)size.x;
            Height = (int)size.y;
            HalfW = Width/2;
            HalfH = Height/2;
            buffer = new Color[Width*Height];
            gesture = EventCallBack.RegEvent<GestureEvent>(model);
            gesture.PointerDown = PointDown;
            gesture.Drag = Drag;
            gesture.AutoColor = false;
            ThreadMission.InvokeToMain(Apply, null);
        }
        void PointDown(EventCallBack callBack, UserAction action)
        {
            Origin = callBack.ScreenToLocal(action.CanPosition);
            LastPos = Origin;
            LastBox = null;
        }
        void Drag(EventCallBack callBack, UserAction action,Vector2 v)
        {
            CurPos = callBack.ScreenToLocal(action.CanPosition);
            float hx = Width * 0.5f;
            float hy = Height * 0.5f;
            if (LastPos.x <= hx & LastPos.x >= -hx)
                if (LastPos.y <= hy & LastPos.y >= -hy)
                {
                    DrawLine(LastPos, CurPos);
                    ThreadMission.InvokeToMain(Apply,null);
                    LastPos = CurPos;
                }
        }
        void DrawLine(Vector2 start,Vector2 end)
        {
            Vector2[] box = CreateBox(ref start,ref end);
            Vector2[] back = null;
            Vector2[] forward = null;
            if(LastBox!=null)
            {
                back = ExtandBoxBack(LastBox);
                forward = ExtandBoxForward(box);
            }
            float minx, miny, maxx, maxy;
            minx = box[0].x;
            miny = box[0].y;
            maxx = box[0].x;
            maxy = box[0].y;
            for (int i = 1; i < box.Length; i++)
            {
                if (box[i].x < minx)
                    minx = box[i].x;
                else if (box[i].x > maxx)
                    maxx = box[i].x;
                if (box[i].y < miny)
                    miny = box[i].y;
                else if (box[i].y > maxy)
                    maxy = box[i].y;
            }
            var s = BrushSize * 0.5f;
            int h = (int)s;
            int sx = (int)minx;
            sx-=h;
            int ex = (int)maxx;
            ex+=h;
            int sy = (int)miny;
            sy-=h;
            int ey = (int)maxy;
            ey+=h;
            s *= s;
            var p = Vector2.zero;
            for (int i = sx; i < ex; i++)
            {
                p.x = i;
                for (int j = sy; j < ey; j++)
                {
                    p.y = j;
                    var c = p - start;
                    if(c.x*c.x+c.y*c.y<s)//在圆里面
                    {
                        if (LastBox != null)
                        {
                            if (Physics2D.DotToPolygon(back, p))
                                goto label;
                            else if (Physics2D.DotToPolygon(forward, p))
                                goto label;
                        }
                        FillColor(p);
                        goto label2;
                    }
                label:;
                    if (Physics2D.DotToPolygon(box, p))
                    {
                        if (LastBox != null)
                            if (Physics2D.DotToPolygon(LastBox, p))
                                goto label2;
                        FillColor(p);
                    }
                label2:;
                }
            }
            LastBox = box;
            LastDirect = CurDirect;
        }
        Vector2[] CreateBox(ref Vector2 start,ref Vector2 end)
        {
            var v = end - start;
            v.Normalize();
            CurDirect = v;
            var n = v.Rotate(90);
            float h = BrushSize * 0.5f;
            n = n.Move(h);
            Vector2[] box = new Vector2[4];
            box[0] = start-v + n;
            box[1] = start-v - n;
            box[2] = end +v  - n;
            box[3] = end +v + n;
            return box;
        }
        Vector2[] ExtandBoxBack(Vector2[] box)
        {
            Vector2[] tmp = new Vector2[4];
            for (int i = 0; i < 4; i++)
                tmp[i] = box[i];
            var v = tmp[1] - tmp[2];
            v.x *= 1000;
            v.y *= 1000;
            tmp[0] += v;
            tmp[1] += v;
            return tmp;
        }
        Vector2[] ExtandBoxForward(Vector2[] box)
        {
            Vector2[] tmp = new Vector2[4];
            for (int i = 0; i < 4; i++)
                tmp[i] = box[i];
            var v = tmp[2] - tmp[1];
            v.x *= 1000;
            v.y *= 1000;
            tmp[2] += v;
            tmp[3] += v;
            return tmp;
        }
        void FillColor(Vector2 p)
        {
            int x =(int)p.x;
            int y = (int)p.y;
            if (x > HalfW)
                return;
            if (x < -HalfW)
                return;
            if (y > HalfH)
                return;
            if (y < -HalfH)
                return;
            x += HalfW;
            y += HalfH;
            int index = y * Width+x;
            if (index >= 0 & index < buffer.Length)
            {
                BlendColor2(ref buffer[index],ref BrushColor);
            }
        }
        void BlendColor(ref Color A1, ref Color A2)
        {
            float a1 = A1.a;
            float a2 = A2.a - (a1 * A2.a) / 1;
            float a = a1 + a2;

            A1.r = (a1 * A1.r + a2 * A2.r) / a;
            A1.g = (a1 * A1.g + a2 * A2.g) / a;
            A1.b = (a1 * A1.b + a2 * A2.b) / a;
            A1.a = a;
        }
        void BlendColor2(ref Color A1, ref Color A2)
        {
            var backB = A1.b;
            var backG = A1.g;
            var backR = A1.r;
            var foreB = A2.b;
            var foreG = A2.g;
            var foreR = A2.r;
            float alpha = A2.a;

            A1.b = (foreB * alpha) + (backB * (1.0f - alpha));
            A1.g = (foreG * alpha) + (backG * (1.0f - alpha));
            A1.r = (foreR * alpha) + (backR * (1.0f - alpha));
            A1.a = 1;
        }
        void Apply(object obj)
        {
            if (texture == null)
            {
                texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
                texture.name = "draw";
            }
            else if (texture.width != Width | texture.height != Height)
            {
                texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
                texture.name = "draw";
            }
            texture.SetPixels(buffer);
            texture.Apply();
            raw.Context.texture = texture;
        }
        public void Resize()
        {

        }
    }
}
