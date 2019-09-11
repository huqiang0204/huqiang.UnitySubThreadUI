using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang
{
    public class GifAnimat : AnimatInterface
    {
        public RawImageElement image;
        public GifAnimat(RawImageElement img)
        {
            image = img;
            AnimationManage.Manage.AddAnimat(this);
        }
        List<Texture2D> texture2s;
        public int gifCount;
        public void Play(List<Texture2D> gif)
        {
            lifetime = 0;
            if (gif != null)
            {
                texture2s = gif;
                image.texture = texture2s[0];
                Playing = true;
            }
            gifCount = gif.Count;
        }
        public void Pause()
        {
            Playing = false;
        }
        public Action<GifAnimat> PlayOver;
        public bool Loop;
        bool Playing;
        float lifetime = 0;
        int index = 0;
        public int Interval = 66;
        public void Update(float time)
        {
            if (Playing)
            {
                float a = lifetime + time;
                if (texture2s != null)
                {
                    int c = (int)a / Interval;
                    if (c >= gifCount)
                    {
                        if (Loop)
                        {
                            lifetime = 0;
                            image.texture = texture2s[0];
                        }
                        else
                        {
                            Playing = false;
                            if (PlayOver != null)
                                PlayOver(this);
                        }
                    }
                    else
                    {
                        if (c < texture2s.Count )
                        {
                            image.texture = texture2s[c];
                            lifetime = a;
                        }
                    }
                }
            }
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
