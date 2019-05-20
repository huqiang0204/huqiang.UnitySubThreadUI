using System;
using UnityEngine;

namespace huqiang.Other
{
    public class Palette
    {
        static Texture2D ht2d;
        public Palette()
        {
        }
        public Color[] buffer = new Color[256*256];
        public static Texture2D LoadHTemplate()
        {
            if(ht2d==null)
            {
                Color[] colors = new Color[360];
                ht2d = new Texture2D(1, 360, TextureFormat.ARGB32, false);
                ThreadMission.AddMission((o)=> {
                    HTemplate(colors);
                },null,-1,(o)=> {
                    ht2d.SetPixels(colors);
                    ht2d.Apply();
                });
            }
            return ht2d;
        }
        static Texture2D ct2d;
        public static Texture2D LoadCTemplate()
        {
            if (ct2d == null)
            {
                Color[] colors = new Color[640* 640];
                ct2d = new Texture2D(640, 640, TextureFormat.ARGB32, false);
                ThreadMission.AddMission((o) => {
                    CTemplate(colors);
                }, null, -1, 
                (o) => {
                    ct2d.SetPixels(colors);
                    ct2d.Apply();
                });
            }
            return ct2d;
        }
        Texture2D t2d;
        public Texture2D texture { get {
                if(t2d==null)
                  t2d=  new Texture2D(256, 256, TextureFormat.ARGB32, false);
                return t2d;
            }}
        public void LoadHSVTAsync(float h)
        {
            ThreadMission.AddMission(
                (o) => {
                    HSVTemplate(h, buffer);
                }, this, -1,
                (o) => {
                    if (t2d == null)
                        t2d = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                    t2d.SetPixels(buffer);
                    t2d.Apply();
                });
        }
        public void LoadHSVT(float h)
        {
            HSVTemplate(h, buffer);
            ThreadMission.InvokeToMain(
                (o) => {
                    if (t2d == null)
                        t2d = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                    t2d.SetPixels(buffer);
                    t2d.Apply();
                }, this);
        }
        public static void HSVTemplate(float H,Color[] buffer)
        {
            int index = 0;
            float V = 0;
            for (int d = 0; d < 256; d++)
            {
                float S = 0;
                for (int t = 0; t < 256; t++)
                {
                    buffer[index]= Color.HSVToRGB(H,S,V);
                    index++;
                    S += 0.0039216f;
                }
                V += 0.0039216f;
            }
        }
        static void HTemplate(Color[] buffer)
        {
            for (int t = 0; t < 360; t++)
            {
                buffer[t] = Color.HSVToRGB(t* 0.0027777f, 1, 1);
            }
        }
        const int MaxSize = 640;
        const int SMaxSize = MaxSize*MaxSize;
        const float Center = 320;
        const float Min = 260;
        const float Max = 310;
        const float SMin = Min*Min;
        const float SMax = Max*Max;
        static void CTemplate(Color[] buff)
        {
            Vector2 center = new Vector2(Center,Center);
            for (int t = 0; t < 3600; t++)
            {
                Color col = Color.HSVToRGB(t/3600f, 1, 1);
                var v = MathH.Tan2(t*0.1f);
                var len = v.Move(1);
                var d = len*0.5f;
                d.x = -d.x;
                var max = center + v.Move(Max);
                for(int i=0;i<60;i++)
                {
                    FillColor(max, buff, ref col);
                    Vector2 h = max - d;
                    for (int j=0;j<2;j++)
                    {
                        FillColor(h, buff, ref col);
                        h -= d;
                    }
                    h = max + d;
                    for (int j = 0; j < 2; j++)
                    {
                        FillColor(h, buff, ref col);
                        h += d;
                    }
                    max -= len;
                }
            }
        }
        static void FillColor(Vector2 pos,Color[] buff,ref Color col)
        {
            int x = (int)pos.x;
            int y = MaxSize - (int)pos.y;
            int index = y * MaxSize - x;
            if (index < 0)
                return;
            if (index >= SMaxSize)
                return;
            buff[index] = col;
        }
    }
}
