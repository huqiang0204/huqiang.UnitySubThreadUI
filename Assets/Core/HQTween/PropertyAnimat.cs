using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace huqiang
{
    /// <summary>
    /// 属性动画，用于更新某个类的某个属性的动画
    /// </summary>
    public class PropertyFloat
    {
        public PropertyFloat(object cla, string PropertyName)
        {
            Target = cla;
            info = cla.GetType().GetProperty(PropertyName);
        }
        public float StartValue;
        public float EndValue;
        float delay;
        public float Delay;
        public float Time;
        float SurplusTime;
        internal object Target;
        PropertyInfo info;
        internal void Reset()
        {
            SurplusTime = Time;
            delay = Delay;
            info.SetValue(Target, 0, null);
        }
        internal static void Update(PropertyFloat t, float timeslice)
        {
            if (t.delay > 0)
            {
                t.delay -= timeslice;
                if (t.delay <= 0)
                {
                    t.SurplusTime += t.delay;
                    t.info.SetValue(t.Target, t.StartValue, null);
                }
            }
            else
            {
                t.SurplusTime -= timeslice;
                if (t.SurplusTime <= 0)
                {
                    t.info.SetValue(t.Target, t.EndValue, null);
                    return;
                }
                float r = 1 - t.SurplusTime / t.Time;
                float d = t.EndValue - t.StartValue;
                d *= r;
                d += t.StartValue;
                t.info.SetValue(t.Target, d, null);
            }
        }
    }
    /// <summary>
    /// 属性动画基本类
    /// </summary>
    public class PropertyAnimat : AnimatBase, AnimatInterface
    {
        List<PropertyFloat> lpf;
        public Action<PropertyAnimat> PlayStart;
        public Action<PropertyAnimat> PlayOver;
        public void AddDelegate(PropertyFloat pf)
        {
            if (lpf == null)
                lpf = new List<PropertyFloat>();
            lpf.Add(pf);
        }
        public void RemoveDelegate(PropertyFloat pf)
        {
            if (lpf != null)
                lpf.Remove(pf);
        }
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
                        c_time += Delay;
                        timeslice += Delay;
                        if (lpf != null)
                            for (int i = 0; i < lpf.Count; i++)
                                PropertyFloat.Update(lpf[i], timeslice);
                    }
                }
                else
                {
                    c_time -= timeslice;
                    if (lpf != null)
                        for (int i = 0; i < lpf.Count; i++)
                            PropertyFloat.Update(lpf[i], timeslice);
                    if (!Loop & c_time <= 0)
                    {
                        playing = false;
                        if (PlayOver != null)
                        {
                            PlayOver(this);
                        }
                    }
                    else
                    {
                        if (c_time <= 0)
                            Play();
                    }
                }
            }
        }
        public override void Play()
        {
            c_time = m_time;
            if (lpf != null)
                for (int i = 0; i < lpf.Count; i++)
                    lpf[i].Reset();
            playing = true;
        }
        public PropertyAnimat()
        {
            AnimationManage.Manage.AddAnimat(this);
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
