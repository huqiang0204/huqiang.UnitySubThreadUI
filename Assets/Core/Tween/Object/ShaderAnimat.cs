using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 用于控制着色器浮点数的动画
    /// </summary>
    public class ShaderFloat
    {
        public float StartValue;
        public float EndValue;
        float delay;
        public float Delay;
        public float Time;
        public string ParameterName;
        float SurplusTime;
        internal Material Target;
        internal void Reset()
        {
            SurplusTime = Time;
            delay = Delay;
        }
        internal static void Update(ShaderFloat t, float timeslice)
        {
            if (t.delay > 0)
            {
                t.delay -= timeslice;
                if (t.delay <= 0)
                {
                    t.SurplusTime += t.delay;
                    t.Target.SetFloat(t.ParameterName, t.StartValue);
                }
            }
            else
            {
                t.SurplusTime -= timeslice;
                if (t.SurplusTime <= 0)
                {
                    t.Target.SetFloat(t.ParameterName, t.EndValue);
                    return;
                }
                float r = 1 - t.SurplusTime / t.Time;
                float d = t.EndValue - t.StartValue;
                d *= r;
                d += t.StartValue;
                t.Target.SetFloat(t.ParameterName, d);
            }
        }
    }
    /// <summary>
    /// 用于控制着色器四维向量的动画
    /// </summary>
    public class ShaderVector4
    {
        public Vector4 StartValue;
        public Vector4 EndValue;
        float delay;
        public float Delay;
        public float Time;
        float SurplusTime;
        public string ParameterName;
        internal Material Target;
        internal void Reset()
        {
            SurplusTime = Time;
            delay = Delay;
        }
        internal static void Update(ShaderVector4 t, float timeslice)
        {
            if (t.delay > 0)
            {
                t.delay -= timeslice;
                if (t.delay <= 0)
                {
                    t.SurplusTime += t.delay;
                    t.Target.SetVector(t.ParameterName, t.StartValue);
                }
            }
            else
            {
                t.SurplusTime -= timeslice;
                if (t.SurplusTime <= 0)
                {
                    t.Target.SetVector(t.ParameterName, t.EndValue);
                    return;
                }
                float r = 1 - t.SurplusTime / t.Time;
                Vector4 d = t.EndValue - t.StartValue;
                d *= r;
                d += t.StartValue;
                t.Target.SetVector(t.ParameterName, d);
            }
        }
    }
    /// <summary>
    /// 着色器动画基本类
    /// </summary>
    public class ShaderAnimat : AnimatBase, AnimatInterface
    {
        List<ShaderFloat> lsf;
        List<ShaderVector4> lsv;
        public Action<ShaderAnimat> PlayStart;
        public Action<ShaderAnimat> PlayOver;
        public ShaderFloat FindFloatShader(string name)
        {
            for (int i = 0; i < lsf.Count; i++)
                if (lsf[i].ParameterName == name)
                    return lsf[i];
            return null;
        }
        public ShaderVector4 FindVectorShader(string name)
        {
            for (int i = 0; i < lsv.Count; i++)
                if (lsv[i].ParameterName == name)
                    return lsv[i];
            return null;
        }
        public Material Target { get; private set; }
        public ShaderAnimat(Material m)
        {
            Target = m;
            lsf = new List<ShaderFloat>();
            lsv = new List<ShaderVector4>();
            AnimationManage.Manage.AddAnimat(this);
        }
        public void AddDelegate(ShaderFloat sf)
        {
            lsf.Add(sf);
            sf.Target = Target;
        }
        public void AddDelegate(ShaderVector4 sv)
        {
            lsv.Add(sv);
            sv.Target = Target;
        }
        public void RemoveDelegate(ShaderFloat sf)
        {
            lsf.Remove(sf);
        }
        public void RemoveDelegate(ShaderVector4 sv)
        {
            lsv.Remove(sv);
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
                        if (lsf != null)
                            for (int i = 0; i < lsf.Count; i++)
                                ShaderFloat.Update(lsf[i], timeslice);
                        if (lsv != null)
                            for (int i = 0; i < lsv.Count; i++)
                                ShaderVector4.Update(lsv[i], timeslice);
                    }
                }
                else
                {
                    c_time -= timeslice;
                    if (lsf != null)
                        for (int i = 0; i < lsf.Count; i++)
                            ShaderFloat.Update(lsf[i], timeslice);
                    if (lsv != null)
                        for (int i = 0; i < lsv.Count; i++)
                            ShaderVector4.Update(lsv[i], timeslice);
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
            if (lsf != null)
                for (int i = 0; i < lsf.Count; i++)
                    lsf[i].Reset();
            if (lsv != null)
                for (int i = 0; i < lsv.Count; i++)
                    lsv[i].Reset();
            playing = true;
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}
