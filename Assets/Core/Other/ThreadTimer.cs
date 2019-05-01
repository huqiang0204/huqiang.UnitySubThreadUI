using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace huqiang
{
    public class ThreadTimer
    {
        bool run = false;
        Thread thread;
        AutoResetEvent auto;
        public Action<ThreadTimer,Int32> Tick;
        Int32 m_inter;
        public Int32 Interal { set { if (value < 1) value = 1;m_inter = value; } }
        public ThreadTimer(Int32 inter = 16)
        {
            Interal = inter;
            auto = new AutoResetEvent(true);
            run = true;
            thread = new Thread(() => {Run();});
            thread.Start();
        }
        void Run()
        {
            int tick = DateTime.Now.Millisecond;
            while (run)
            {
                auto.WaitOne(1);
                var t = DateTime.Now.Millisecond;
                int v = t - tick;
                if (v < 0)
                    v += 1000;
                if (v > m_inter)
                {
                    tick = t;
                    if (Tick != null)
                        Tick(this,v);
                }
            }
        }
        public void Dispose()
        {
            run = false;
        }
    }
}
