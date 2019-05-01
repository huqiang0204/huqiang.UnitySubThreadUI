using huqiang.Data;
using System;
using System.Threading;
using UnityEngine;

namespace huqiang
{
    class Mission
    {
        public Action<object> action;
        public object data;
    }
    class ThreadMission
    {
        public QueueBuffer<Mission> SubMission;
        public QueueBuffer<Mission> MainMission;
        Thread thread;
        public int Id;
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
                    else m.action(m.data);
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
        public void AddSubMission(Action<object> action, object dat)
        {
            Mission mission = new Mission();
            mission.action = action;
            mission.data = dat;
            SubMission.Enqueue(mission);
        }
        public void AddMainMission(Action<object> action, object dat)
        {
            Mission mission = new Mission();
            mission.action = action;
            mission.data = dat;
            MainMission.Enqueue(mission);
        }
        public void Dispose()
        {
            run = false;
        }
    }
    public class ThreadPool
    {
        static ThreadMission[] threads;
        static int size;
        public static void Initial(int buffsize = 4)
        {
            if (threads != null)
                Dispose();
            size = buffsize;
            threads = new ThreadMission[size];
            for (int i = 0; i < buffsize; i++)
                threads[i] = new ThreadMission();
        }
        static int point;
        public static void AddMission(Action<object> action, object dat,int index=-1)
        {
            if (threads == null)
            {
                return;
            }
            if(index<0|index>=size)
            {
                threads[point].AddSubMission(action, dat);
                point++;
                if (point >= size)
                    point = 0;
            }
            else
            {
                threads[index].AddSubMission(action, dat);
            }
        }
        public static void InvokeToMain(Action<object> action, object dat)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            for(int i=0;i<size;i++)
            {
                if(threads[i].Id==id)
                {
                    threads[i].AddMainMission(action,dat);
                    break;
                }
            }
        }
        public static void ExtcuteMain()
        {
             for(int i=0;i<size;i++)
            {
                for(int j=0;j<2048;j++)
                {
                    var m = threads[i].MainMission.Dequeue();
                    if (m == null)
                        break;
                    else m.action(m.data);
                }
            }
        }
        public static void Dispose()
        {
            if (threads == null)
                return;
            for (int i = 0; i < threads.Length; i++)
                threads[i].Dispose();
        }
    }
}