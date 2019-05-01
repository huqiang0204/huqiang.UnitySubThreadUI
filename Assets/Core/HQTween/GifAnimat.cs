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
        public RawImage image;
        public GifAnimat(RawImage img)
        {
            image = img;
            AnimationManage.Manage.AddAnimat(this);
        }
        List<Texture2D> texture2s;
        public void Play(List<Texture2D> gif)
        {
            lifetime = 0;
            if (gif != null)
            {
                texture2s = gif;
                image.texture = texture2s[0];
                Playing = true;
            }
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
        public void Update(float time)
        {
            if (Playing)
            {
                lifetime += time;
                if (texture2s != null)
                {
                    int c = (int)lifetime / 66;
                    if (c >= texture2s.Count)
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
                        image.texture = texture2s[c];
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
