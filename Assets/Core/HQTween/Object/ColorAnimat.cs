using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang
{
    public class ColorAnimat : AnimatBase, AnimatInterface
    {
        public Graphic Target { get; private set; }
        public ColorAnimat(Graphic img)
        {
            Target = img;
            AnimationManage.Manage.AddAnimat(this);
        }
        public override void Play()
        {
            lifetime = 0;
            playing = true;
        }
        public Action<ColorAnimat> PlayOver;
        float lifetime = 0;
        int index = 0;
        public float Interval = 100;
        public bool autoHide;
        public Color StartColor;
        public Color EndColor;
        public void Update(float time)
        {
            if (playing)
            {
                if (Delay > 0)
                {
                    Delay -= time;
                    if (Delay <= 0)
                    {
                        c_time = -Delay;
                    }
                }
                else
                {
                    c_time += time;
                    if (!Loop & c_time >= m_time)
                    {
                        playing = false;
                        Target.color = EndColor;
                        if (PlayOver != null)
                            PlayOver(this);
                    }
                    else
                    {
                        if (c_time >= m_time)
                            c_time -= m_time;
                        float r = c_time / m_time;
                        if (Linear != null)
                            r = Linear(this, r);
                        Color v = EndColor - StartColor;
                        Target.color = StartColor + v * r;
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
