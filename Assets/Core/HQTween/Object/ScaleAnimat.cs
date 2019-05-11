using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    public class ScaleAnimat : AnimatBase, AnimatInterface
    {
        public ScaleAnimat(Transform t)
        {
            Target = t;
            AnimationManage.Manage.AddAnimat(this);
        }
        public Transform Target;
        public Vector3 StartScale;
        public Vector3 EndScale;
        public Action<ScaleAnimat> PlayStart;
        public Action<ScaleAnimat> PlayOver;
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
                    if (!Loop & c_time >= m_time)//不会循环且超过总时长
                    {
                        playing = false;
                        Target.localScale = EndScale;
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
                        Vector3 v = EndScale - StartScale;
                        Target.localScale = StartScale + v * r;
                    }
                }
            }
        }
        public void Dispose()
        {
            if (AutoHide)
                Target.gameObject.SetActive(false);
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
