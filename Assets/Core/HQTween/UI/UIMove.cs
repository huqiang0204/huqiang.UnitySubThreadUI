using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang
{
    public class UIMove : AnimatBase, AnimatInterface
    {
        public UIMove(ModelElement t)
        {
            Target = t;
            UIAnimation.Manage.AddAnimat(this);
        }
        public ModelElement Target;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Action<UIMove> PlayStart;
        public Action<UIMove> PlayOver;
        public void Update(float timeslice)
        {
            if (playing)
            {
                if (Delay > 0)
                {
                    Delay -= timeslice;
                    if (Delay <= 0)
                    {
                        if (PlayStart != null)
                            PlayStart(this);
                        c_time = -Delay;
                    }
                }
                else
                {
                    c_time += timeslice;
                    if (!Loop & c_time >= m_time)
                    {
                        playing = false;
                        Target.data.localPosition = EndPosition;
                        if (PlayOver != null)
                            PlayOver(this);
                        Target.IsChanged = true;
                    }
                    else
                    {
                        if (c_time >= m_time)
                            c_time -= m_time;
                        float r = c_time / m_time;
                        if (Linear != null)
                            r = Linear(this, r);
                        Vector3 v = EndPosition - StartPosition;
                        Target.data.localPosition = StartPosition + v * r;
                        Target.IsChanged = true;
                    }
                }
            }
        }
        public void Dispose()
        {
            if (AutoHide)
            {
                Target.activeSelf = false;
                Target.IsChanged = true;
            }
            UIAnimation.Manage.ReleaseAnimat(this);
        }
    }
}
