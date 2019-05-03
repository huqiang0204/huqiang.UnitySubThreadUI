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
                        if (m.waitAction != null)
                        {
                            m.action = m.waitAction;
                            m.waitAction = null;
                            MainMission.Enqueue(m);
                        }
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
        public void Dispose()
        {
            run = false;
        }
        static List<ThreadMission>threads=new List<ThreadMission>();
        static int point;
        public static void AddMission(Action<object> action, object dat, int index, Action<object> wait = null)
        {
            if (threads == null)
            {
                return;
            }
            if (index < 0 | index >= threads.Count)
            {
                threads[point].AddSubMission(action, dat);
                point++;
                if (point >= threads.Count)
                    point = 0;
            }
            else
            {
                threads[index].AddSubMission(action, dat);
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
                    break;
                }
            }
        }
        public static void ExtcuteMain()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                for (int j = 0; j < 2048; j++)
                {
                    var m = threads[i].MainMission.Dequeue();
                    if (m == null)
                        break;
                    else {
                        m.action(m.data);
                        if (m.waitAction != null)
                            threads[i].AddMainMission(m.waitAction,m.data);
                    }
                }
            }
        }
        public static void DisposeAll()
        {
            for (int i = 0; i < threads.Count; i++)
                threads[i].Dispose();
            threads.Clear();
        }
    }
}