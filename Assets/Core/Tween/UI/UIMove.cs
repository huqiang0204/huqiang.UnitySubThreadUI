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
            EndPosition = StartPosition = t.data.localPosition;
            StartAngle = EndAngle = t.data.localRotation.eulerAngles;
            StartScale = EndScale = t.data.localScale;
            StartSize = EndSize = t.data.sizeDelta;
        }
        public ModelElement Target;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3 StartAngle;
        public Vector3 EndAngle;
        public Vector2 StartSize;
        public Vector2 EndSize;
        public Vector3 StartScale;
        public Vector3 EndScale;
        public Action<UIMove> PlayStart;
        public Action<UIMove> PlayOver;
        public Action<UIMove> Playing;
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
                        Target.data.localRotation = Quaternion.Euler(EndAngle);
                        Target.data.localScale = EndScale;
                        Target.data.sizeDelta = EndSize;
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

                        Vector3 a = EndAngle - StartAngle;
                        Target.data.localRotation = Quaternion.Euler(StartAngle + a * r);

                        Vector3 s = EndScale - StartScale;
                        Target.data.localScale = StartScale - s * r;

                        Vector2 d = EndSize - StartSize;
                        Target.data.sizeDelta = StartSize + d * r;

                        Target.IsChanged = true;
                        if (Playing != null)
                            Playing(this);
                    }
                }
            }
        }
        public void Dispose()
        {
            if (AutoHide)
            {
                Target.activeSelf = false;
            }
            UIAnimation.Manage.ReleaseAnimat(this);
        }
    }
}
