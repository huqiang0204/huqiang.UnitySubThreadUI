using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang
{
    public class NetworkLink
    {
        public int ip;
        public int port;
        public int Index;
        public int buffIndex;
        public Int64 id;
        public IPEndPoint endpPoint;
        public virtual void Recive(long now)
        {
        }
    }
}
