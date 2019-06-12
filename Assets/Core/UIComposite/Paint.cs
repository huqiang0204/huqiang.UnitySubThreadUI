using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class Paint : ModelInital
    {
        public enum DrawModel
        {
            Brush,Scale,Rotate
        }
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
        Vector2 LastDirect;
        Vector2 CurDirect;
        LoopBuffer<DrawArea> loopBuffer=new LoopBuffer<DrawArea>(2);
        public DrawModel drawModel;
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
            gesture.DragEnd = DragEnd;
            gesture.AutoColor = false;
            gesture.TowFingerMove = TowFingerMove;
            ThreadMission.InvokeToMain(Apply, null);
        }
        void PointDown(EventCallBack callBack, UserAction action)
        {
            Origin = callBack.ScreenToLocal(action.CanPosition);
            LastPos = Origin;
            loopBuffer.Clear();
        }
        void Drag(EventCallBack callBack, UserAction action,Vector2 v)
        {
            if(drawModel==DrawModel.Brush)
            {
                CurPos = callBack.ScreenToLocal(action.CanPosition);
                float hx = Width * 0.5f;
                float hy = Height * 0.5f;
                if (LastPos.x <= hx & LastPos.x >= -hx)
                    if (LastPos.y <= hy & LastPos.y >= -hy)
                    {
                        DrawLine(LastPos, CurPos);
                        ThreadMission.InvokeToMain(Apply, null);
                        LastPos = CurPos;
                    }
            }
        }
        void DragEnd(EventCallBack callBack, UserAction action, Vector2 v)
        {

        }
        void TowFingerMove(GestureEvent ges)
        {
            if(drawModel==DrawModel.Scale)
            {
                float s = ges.CurScale;
                model.data.localScale.x = s;
                model.data.localScale.y = s;
                model.data.localScale.y = s;
                model.IsChanged = true;
            }else if(drawModel==DrawModel.Rotate)
            {
                float a = ges.DeltaAngle;
                model.data.localRotation*= Quaternion.Euler(0,0,a);
                model.IsChanged = true;
            }
        }
        void DrawLine(Vector2 start, Vector2 end)
        {
            DrawArea area = new DrawArea(start, end, BrushSize);
            loopBuffer.Push(area);
            int h = (int)area.hw;
            int sx = (int)area.minx;
            sx -= h;
            int ex = (int)area.maxx;
            ex += h;
            int sy = (int)area.miny;
            sy -= h;
            int ey = (int)area.maxy;
            ey += h;
            var p = Vector2.zero;
            for (int i = sx; i < ex; i++)
            {
                p.x = i;
                for (int j = sy; j < ey; j++)
                {
                    var pos = new Vector2(i,j);
                    if(area.CheckPix(pos))
                    {
                        var a = loopBuffer[0];
                        if (a != null)
                            if (a.CheckPix(pos))
                                goto label;
                        FillColor(pos);
                    }
                label:;
                }
            }

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
            A1.a +=alpha ;
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
    public class DrawArea
    {
        public float hw;
        public Vector2 Start;
        public Vector2 End;
        public Vector2[] box;
        public float minx, miny, maxx, maxy;
        float sqr;
        public DrawArea(Vector2 s,Vector2 e,float w)
        {
            Start = s;
            End = e;
            hw = w*0.5f;
            sqr = hw * hw;
            if(s!=e)
            {
                CreateBox();
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
                minx -= hw;
                maxx += hw;
                miny -= hw;
                maxy +=  hw;
            }
            else
            {
                minx = Start.x - hw-1;
                maxx = Start.x + hw+1;
                miny = Start.y - hw-1;
                maxy = Start.y + hw+1;
            }
        }
        void CreateBox()
        {
            var v = End - Start;
            v.Normalize();
            var n = v.Rotate(90);
            float h = hw;
            n = n.Move(h);
            box = new Vector2[4];
            box[0] = Start - v + n;
            box[1] = Start - v - n;
            box[2] = End + v - n;
            box[3] = End + v + n;
        }
        public bool CheckPix(Vector2 pos)
        {
            var dx = pos.x - Start.x;
            var dy = pos.y - Start.y;
            if (dx * dx + dy * dy < sqr)
                return true;
            dx = pos.x - End.x;
            dy = pos.y - End.y;
            if (dx * dx + dy * dy < sqr)
                return true;
            if (box != null)
                return Physics2D.DotToPolygon(box, pos);
            return false;

        }
    }
}