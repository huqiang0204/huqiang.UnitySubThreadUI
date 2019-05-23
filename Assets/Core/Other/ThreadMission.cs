using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace huqiang
{
    public class ThreadMission
    {
        class Mission
        {
            public Action<object> action;
            public object data;
            public Action<object> waitAction;
            public int Id;
        }
        QueueBuffer<Mission> SubMission;
        QueueBuffer<Mission> MainMission;
        Thread thread;
        public int Id { get; private set; }
        AutoResetEvent are;
        bool run;
        public ThreadMission()
        {
            SubMission = new QueueBuffer<Mission>();
            MainMission = new QueueBuffer<Mission>();
            thread = new Thread(Run);
            are = new AutoResetEvent(false);
            run = true;
            thread.Start();
            Id = thread.ManagedThreadId;
            threads.Add(this);
        }
        void Run()
        {
            while (run)
            {
                try
                {
                    var m = SubMission.Dequeue();
                    if (m == null)
                        are.WaitOne(1);
                    else {
                        m.action(m.data);
                        if (m.waitAction != null)//如果有等待的任务
                        {
                            if(m.Id==MainID)//交给主线程
                            {
                                m.action = m.waitAction;
                                m.waitAction = null;
                                MainMission.Enqueue(m);
                                goto label;
                            }
                            for (int i = 0; i < threads.Count; i++)
                                if (m.Id == threads[i].Id)
                                {
                                    threads[i].AddMainMission(m.waitAction, m.data);//任务交给源线程
                                    goto label;
                                }
                            m.action = m.waitAction;
                            m.waitAction = null;
                            MainMission.Enqueue(m);//否则交给主线程
                        }
                    label:;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.LogError(ex.StackTrace);
#endif
                }
            }
            are.Dispose();
        }
        public void AddSubMission(Action<object> action, object dat,Action<object> wait = null)
        {
            Mission mission = new Mission();
            mission.action = action;
            mission.data = dat;
            mission.waitAction = wait;
            mission.Id = Thread.CurrentThread.ManagedThreadId;
            SubMission.Enqueue(mission);
        }
        public void AddMainMission(Action<object> action, object dat, Action<object> wait = null)
        {
            Mission mission = new Mission();
            mission.action = action;
            mission.data = dat;
            mission.waitAction = wait;
            MainMission.Enqueue(mission);
        }
        void Dispose()
        {
            run = false;
        }
        static List<ThreadMission>threads = new List<ThreadMission>();
        static ThreadMission FreeMission;
        public static void AddMission(Action<object> action, object dat, int index=-1, Action<object> wait = null)
        {
            if (threads == null)
            {
                return;
            }
            if (index < 0 | index >= threads.Count)
            {
                if (FreeMission == null)
                    FreeMission = new ThreadMission();
                FreeMission.AddSubMission(action, dat, wait);
            }
            else
            {
                threads[index].AddSubMission(action, dat, wait);
            }
        }
        public static void InvokeToMain(Action<object> action, object dat, Action<object> wait=null)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].Id == id)
                {
                    threads[i].AddMainMission(action, dat, wait);
                    return;
                }
            }
            if (FreeMission == null)
                FreeMission = new ThreadMission();
            FreeMission.AddMainMission(action, dat, wait);
        }
        public static void ExtcuteMain()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                ExtcuteMain(threads[i]);
            }
            if (FreeMission != null)
                ExtcuteMain(FreeMission);
        }
        static void ExtcuteMain(ThreadMission mission)
        {
            for (int j = 0; j < 2048; j++)
            {
                var m = mission.MainMission.Dequeue();
                if (m == null)
                    break;
                else
                {
                    m.action(m.data);
                    if (m.waitAction != null)
                        mission.AddSubMission(m.waitAction, m.data);
                }
            }
        }
        public static void DisposeAll()
        {
            for (int i = 0; i < threads.Count; i++)
                threads[i].Dispose();
            threads.Clear();
            if(FreeMission!=null)
            {
                FreeMission.Dispose();
                FreeMission = null;
            }
        }
        static int MainID;
        public static void SetMianId()
        {
            MainID = Thread.CurrentThread.ManagedThreadId;
        }
    }
}