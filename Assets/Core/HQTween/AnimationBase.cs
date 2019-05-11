using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 动画接口
    /// </summary>
    public interface AnimatInterface
    {
        void Update(float time);
    }
    class ToDoEvent
    {
        public float time;
        public string key;
        public Action<object> DoEvent;
        public object parameter;
        public int level;
    }
    public class AnimationBase
    {
        static int Frames = 0;
        //public void Add
        List<AnimatInterface> Actions;
        /// <summary>
        /// 获取当前对象池中对象的数量
        /// </summary>
        public int Count { get { return Actions.Count; } }
        public AnimationBase()
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
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i] != null)
                    tmp[i].Update(timeslice);
            }
            DoEvent(timeslice);
            DoFrameEvent();
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
                    t.level--;
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
        public void RemoveEvent(string key)
        {
            for (int i = 0; i < key_events.Count; i++)
            {
                if (key_events[i].key == key)
                {
                    key_events.RemoveAt(i);
                    return;
                }
            }
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
            if (!droc.Contains(animat))
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
}
