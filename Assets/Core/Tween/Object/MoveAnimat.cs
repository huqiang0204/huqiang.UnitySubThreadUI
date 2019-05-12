using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    public class MoveAnimat : AnimatBase, AnimatInterface
    {
        public MoveAnimat(Transform t)
        {
            Target = t;
            AnimationManage.Manage.AddAnimat(this);
        }
        public Transform Target;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Action<MoveAnimat> PlayStart;
        public Action<MoveAnimat> PlayOver;
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
                        Target.localPosition = EndPosition;
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
                        Vector3 v = EndPosition - StartPosition;
                        Target.localPosition = StartPosition + v * r;
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
