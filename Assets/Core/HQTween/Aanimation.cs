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
    public class AnimationManage
    {
        static AnimationManage am;
        static int Frames =0;
        /// <summary>
        /// 返回此类的唯一实例
        /// </summary>
        public static AnimationManage Manage { get { if (am == null) am = new AnimationManage(); return am; } }
        //public void Add
        List<AnimatInterface> Actions;
        /// <summary>
        /// 获取当前对象池中对象的数量
        /// </summary>
        public int Count { get { return Actions.Count; } }
        AnimationManage()
        {
            Actions = new List<AnimatInterface>();
            droc = new List<AnimatInterface>();
            key_events = new List<ToDoEvent>();
            time_events = new List<ToDoEvent>();
            frame_events = new List<ToDoEvent>();
        }
        /// <summary>
        /// 主更新函数，更新所有动画
        /// </summary>
        public void Update()
        {
            Frames++;
            float timeslice = Time.deltaTime * 1000;
            var tmp = Actions.ToArray();
            for (int i=0; i<tmp.Length; i++)
            {
                if (tmp[i] != null)
                    tmp[i].Update(timeslice);
            }
            DoEvent(timeslice);
            DoFrameEvent();
            //LeanTween.update();
        }
        /// <summary>
        /// 添加一个新动画，重复添加会造成多倍运行
        /// </summary>
        /// <param name="ani">动画接口</param>
        public void AddAnimat(AnimatInterface ani)
        {
            if (Actions.Contains(ani))
                return;
            Actions.Add(ani);
        }
        /// <summary>
        /// 删除动画
        /// </summary>
        /// <param name="ani">动画接口</param>
        public void ReleaseAnimat(AnimatInterface ani)
        {
            Actions.Remove(ani);
            droc.Remove(ani);
        }
        /// <summary>
        /// 释放所有动画
        /// </summary>
        public void ReleaseAll()
        {
            Actions.Clear();
            Actions.AddRange(droc);
        }
        List<ToDoEvent> key_events;
        List<ToDoEvent> time_events;
        List<ToDoEvent> frame_events;
        void DoEvent(float time)
        {
            if (time_events == null)
                return;
            int i = time_events.Count - 1;
            for (; i >= 0; i--)
            {
                var t = time_events[i];
                if (t != null)
                {
                    t.time -= time;
                    if (t.time <= 0)
                    {
                        if (t.DoEvent != null)
                            t.DoEvent(t.parameter);
                        time_events.RemoveAt(i);
                    }
                }
                else time_events.RemoveAt(i);
            }
        }
        void DoFrameEvent()
        {
            if (frame_events == null)
                return;
            int i = frame_events.Count - 1;
            for (; i >= 0; i--)
            {
                var t = frame_events[i];
                if (t != null)
                {
                    t.level --;
                    if (t.level <= 0)
                    {
                        if (t.DoEvent != null)
                            t.DoEvent(t.parameter);
                        frame_events.RemoveAt(i);
                    }
                }
                else frame_events.RemoveAt(i);
            }
        }
        public void ToDo(string key, Action<object> action, object parameter, int level = 0)
        {
            ToDoEvent to = new ToDoEvent();
            to.key = key;
            to.DoEvent = action;
            to.parameter = parameter;
            to.level = level;
            for (int i = 0; i < key_events.Count; i++)
                if (key_events[i].key == key)
                {
                    if (to.level > key_events[i].level)
                        key_events[i] = to;
                    return;
                }
            key_events.Add(to);
        }
        /// <summary>
        /// 委托某个事件在多少毫秒后执行
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="parameter"></param>
        public void ToDo(float time, Action<object> action, object parameter)
        {
            for (int i = 0; i < time_events.Count; i++)
            {
                if (action == time_events[i].DoEvent)
                {
                    time_events[i].time = time;
                    return;
                }
            }
            ToDoEvent to = new ToDoEvent();
            to.time = time;
            to.DoEvent = action;
            to.parameter = parameter;
            time_events.Add(to);
        }
        public void FrameToDo(int frames, Action<object> action, object parameter)
        {
            ToDoEvent to = new ToDoEvent();
            to.level = frames;
            to.DoEvent = action;
            to.parameter = parameter;
            frame_events.Add(to);
        }
        /// <summary>
        /// 执行某个事件
        /// </summary>
        /// <param name="key"></param>
        public bool DoEvent(string key)
        {
            if (key_events == null)
                return false;
            for (int i = 0; i < key_events.Count; i++)
                if (key == key_events[i].key)
                {
                    if (key_events[i].DoEvent != null)
                        key_events[i].DoEvent(key_events[i].parameter);
                    key_events.RemoveAt(i);
                    return true;
                }
            return false;
        }
        /// <summary>
        /// 清除所有事件
        /// </summary>
        public void ClearEvent()
        {
            key_events.Clear();
            time_events.Clear();
            frame_events.Clear();
        }
        List<AnimatInterface> droc;
        public void DontReleaseOnClear(AnimatInterface animat)
        {
            if(!droc.Contains(animat))
            droc.Add(animat);
        }
        public T FindAni<T>(Func<T, bool> equl) where T : class, AnimatInterface
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                var ani = Actions[i];
                if (ani is T)
                {
                    if (equl != null)
                    {
                        if (equl(ani as T))
                            return ani as T;
                    }
                    else return ani as T;
                }
            }
            return null;
        }
    }
    /// <summary>
    /// 动画接口
    /// </summary>
    public interface AnimatInterface
    {
        void Update(float time);
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
    class ToDoEvent
    {
        public float time;
        public string key;
        public Action<object> DoEvent;
        public object parameter;
        public int level;
    }
    /// <summary>
    /// 排队执行函数,如果上一个函数未执行,则会等待该函数执行完毕
    /// </summary>
    public class DoWaitQueue : AnimatInterface
    {
        void AnimatInterface.Update(float time)
        {
            if (time_events == null)
                return;
            if (time_events.Count > 0)
            {
                var t = time_events[0];
                t.time -= time;
                label:;
                if (t.time <= 0)
                {
                    time_events.RemoveAt(0);
                    if (time_events.Count > 0)
                    {
                        var a = t.time;
                        t = time_events[0];
                        t.time += a;
                        if (t.DoEvent != null)
                            t.DoEvent(t.parameter);
                        goto label;
                    }
                }
            }
        }
        List<ToDoEvent> time_events;
        public DoWaitQueue()
        {
            time_events = new List<ToDoEvent>();
            AnimationManage.Manage.AddAnimat(this);
        }
        public void DoWait(float time, Action<object> action, object parameter)
        {
            if (time_events.Count == 0)
                if (action != null)
                    action(parameter);
            ToDoEvent to = new ToDoEvent();
            to.time = time;
            to.DoEvent = action;
            to.parameter = parameter;
            time_events.Add(to);
        }
        public void Dispose()
        {
            AnimationManage.Manage.ReleaseAnimat(this);
        }
    }
}