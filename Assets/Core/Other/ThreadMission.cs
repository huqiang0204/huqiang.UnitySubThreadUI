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
#if UNITY_WSA
        System.Threading.Tasks.Task thread;
#else
         Thread thread;
#endif
        public string Tag;
        public int Id { get; private set; }
        AutoResetEvent are;
        bool run;
        bool subFree,mainFree;
        public ThreadMission(string tag)
        {
            Tag = tag;
            SubMission = new QueueBuffer<Mission>();
            MainMission = new QueueBuffer<Mission>();
#if UNITY_WSA
            thread = System.Threading.Tasks.Task.Run(Run);
            are = new AutoResetEvent(false);
            run = true;
            thread.Start();
            Id = thread.Id;
#else
               thread = new Thread(Run);
            are = new AutoResetEvent(false);
            run = true;
            thread.Start();
            Id = thread.ManagedThreadId;
#endif

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
                    {
                        subFree = true;
                        are.WaitOne(1);
                    }
                    else
                    {
                        subFree = false;
                        m.action(m.data);
                        if (m.waitAction != null)//如果有等待的任务
                        {
                            if (m.Id == MainID)//交给主线程
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
        public static void AddMission(Action<object> action, object dat, string tag = null, Action<object> wait = null)
        {
            if (threads == null)
            {
                return;
            }
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].Tag == tag)
                {
                    threads[i].AddSubMission(action, dat, wait);
                    return;
                }
            }
            var mis = new ThreadMission(null);
            mis.AddSubMission(action, dat, wait);
            threads.Add(mis);
        }
        public static void InvokeToMain(Action<object> action, object dat, Action<object> wait=null)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            if(id==MainID)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].Tag == null)
                    {
                        threads[i].AddMainMission(action, dat, wait);
                        return;
                    }
                }
                var mis = new ThreadMission(null);
                mis.AddMainMission(action, dat, wait);
                threads.Add(mis);
            }
            else
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].Id == id)
                    {
                        threads[i].AddMainMission(action, dat, wait);
                        return;
                    }
                }
            }
         
        }
        public static void ExtcuteMain()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                ExtcuteMain(threads[i]);
            }
        }
        static void ExtcuteMain(ThreadMission mission)
        {
            for (int j = 0; j < 2048; j++)
            {
                var m = mission.MainMission.Dequeue();
                if (m == null)
                {
                    mission.mainFree = true;
                    break;
                }
                else
                {
                    mission.mainFree = false;
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
        }
        public static void DisposeFree()
        {
            int c = threads.Count-1;
            lock (threads)
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].subFree & threads[i].mainFree)
                    {
                        threads[i].Dispose();
                        threads.RemoveAt(i);
                    }
                }
        }
        static int MainID;
        public static void SetMianId()
        {
            MainID = Thread.CurrentThread.ManagedThreadId;
        }
    }
}