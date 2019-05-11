using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang
{
    /// <summary>
    /// 线性变化值，参数范围为0-1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public delegate float LinearTransformation(AnimatBase sender, float ratio);
    /// <summary>
    /// 动画管理类，将所有动画添加至此类，进行统一更新
    /// </summary>
    public class AnimationManage:AnimationBase
    {
        static AnimationManage am;
        /// <summary>
        /// 返回此类的唯一实例
        /// </summary>
        public static AnimationManage Manage { get { if (am == null) am = new AnimationManage(); return am; } }
        AnimationManage() { }
    }

    /// <summary>
    /// 定时器
    /// </summary>
    public class Timer : AnimatBase, AnimatInterface
    {
        public Action<Timer> PlayStart;
        public Action<Timer> PlayOver;
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
                    }
                }
                else
                {
                    c_time -= timeslice;
                    if (c_time <= 0)
                    {
                        if (!Loop)
                            playing = false;
                        else c_time += m_time;
                        if (PlayOver != null)
                        {
                            PlayOver(this);
                        }
                    }
                }
            }
        }
        public Timer()
        {
            AnimationManage.Manage.AddAnimat(this);
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
 
}