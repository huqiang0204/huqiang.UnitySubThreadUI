using huqiang;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace huqiang
{
    /// <summary>
    /// 客户端连接
    /// </summary>
    public class TcpLink:NetworkLink
    {
        //PlayerInfo playerInfo;
        TcpEnvelope envelope=new TcpEnvelope();
        internal Socket Link;
        byte[] buff;
        public void SetSocket(Socket soc,IPEndPoint end,PackType pack=PackType.All,int buffsize=4096)
        {
            Link = soc;
            envelope.type = pack;
            endpPoint = end;
            addr = end.Address;
            buff = new byte[buffsize];
            var buf = addr.GetAddressBytes();
            unsafe
            {
                fixed (byte* bp = &buf[0])
                    ip = *(int*)bp;
            }
            port = end.Port;
        }
        public void SetPackType(PackType pack)
        {
            envelope.type = pack;
        }
        //玩家登录ip
        public IPAddress addr;
        public int Send(byte[] data,byte type = EnvelopeType.Mate)
        {
            try
            {
                if (Link != null)
                    lock (Link)
                        if (Link.Connected)
                        {
                            var ss = envelope.Pack(data,type);
                            for (int i = 0; i < ss.Length; i++)
                                Link.Send(ss[i]);
                            return 1;
                        }
                        else return -1;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.StackTrace);
                return -1;
            }
            return 0;
        }
        public int Send(byte[][] data)
        {
            try
            {
                if (Link != null)
                    lock (Link)
                        if (Link.Connected)
                        {
                            for (int i = 0; i < data.Length; i++)
                                Link.Send(data[i]);
                            return 1;
                        }
                        else return -1;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.StackTrace);
                return -1;
            }
            return 0;
        }
        public int Send(string data)
        {
            return Send(Encoding.UTF8.GetBytes(data));
        }
        ~TcpLink()
        {
            if (Link != null)
                lock (Link)
                    Link.Close();
        }
        public virtual void Dispose()
        {
            if (Link != null)
                lock (Link)
                    Link.Close();
        }
        public override void Recive(long time)
        {
            try
            {
                int len = Link.Receive(buff, SocketFlags.Peek);
                if (len > 0)
                {
                    len = Link.Receive(buff);
                    var list= envelope.Unpack(buff,len);
                    try
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var dat = list[i];
                            Dispatch(dat.data, dat.type);
                        }
                    }catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public virtual void Dispatch( byte[] dat, byte tag)
        {
        }
    }
}
