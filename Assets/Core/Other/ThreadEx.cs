using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace huqiang
{
    public class ThreadEx
    {
#if UNITY_WSA
        Task thread;
#else
         Thread thread;
#endif
        AutoResetEvent are;
        
        public int Id;
        Action action;
        public ThreadEx(Action act)
        {
            action = act;
            are = new AutoResetEvent(false);
#if UNITY_WSA
            thread = Task.Run(Run);
            Id = thread.Id;
#else
            thread = new Thread(Run);
            Id = thread.ManagedThreadId;
#endif
        }
        public void Start()
        {
            thread.Start();
        }
        void Run()
        {
            if (action != null)
                action();
            are.Dispose();
        }
        public void WaitTime(int time)
        {
            are.WaitOne(time);
        }
        public static void Sleep(int time)
        {
#if UNITY_WSA
            Task.Delay(time);
#else
            Thread.Sleep(time);
#endif
        }
        public static int CurrentId { get {
#if UNITY_WSA
             return (int)System.Threading.Tasks.Task.CurrentId;
#else
             return Thread.CurrentThread.ManagedThreadId;
#endif
            }
        }

    }
}
