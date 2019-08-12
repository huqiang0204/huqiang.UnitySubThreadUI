using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public interface ToBytes
    {
        byte[] ToBytes();
    }
    public interface FakeArray
    {
         Int32 Length { get; }
    }
    public class DataType
    {
        public const short String = 0;
        public const short FakeStruct = 1;
        public const short FakeStructArray = 2;
        public const short ByteArray = 3;
        public const short Int32Array = 4;
        public const short FloatArray = 5;
        public const short Int16Array = 6;
        public const short Int64Array = 7;
        public const short DoubleArray = 8;
        public const short FakeStringArray = 9;
        public const short Int = 10;
        public const short Float = 11;
        public const short Long = 12;
        public const short Double = 13;
    }
    //C#的这些类型不能被继承：System.ValueType, System.Enum, System.Delegate, System.Array, etc.
    public class DataBuffer
    {
        static Int32 GetType(object obj)
        {
            if (obj is string)
                return DataType.String;
            else if (obj is FakeStruct)
                return DataType.FakeStruct;
            else if (obj is FakeStructArray)
                return DataType.FakeStructArray;
            else if (obj is byte[])
                return DataType.ByteArray;
            else if (obj is Int32[])
                return DataType.Int32Array;
            else if (obj is Single[])
                return DataType.FloatArray;
            else if (obj is Int16[])
                return DataType.Int16Array;
            else if (obj is Int64[])
                return DataType.Int64Array;
            else if (obj is Double[])
                return DataType.DoubleArray;
            else if (obj is FakeStringArray)
                return DataType.FakeStringArray;
            return -1;
        }
        static int GetArraySize(Array array)
        {
            int size = 1;
            if (array is Int32[])
                size = 4;
            else if (array is float[])
                size = 4;
            else if (array is Int16[])
                size = 2;
            else if (array is Double[])
                size = 8;
            else if (array is Int64[])
                size = 8;
            return size;
        }
        public FakeStruct fakeStruct;
        static byte[] Zreo = new byte[4];
        struct ReferenceCount
        {
            /// <summary>
            /// Reference count
            /// </summary>
            public Int16 rc;
            /// <summary>
            ///byte[], String,FakeStruct,FakeStructArray,int[],float[],double[]
            /// </summary>
            public Int16 type;
            public Int32 size;
            public object obj;
        }
        /// <summary>
        /// 添加一个引用类型的数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="size">FakeStructArray 成员大小</param>
        /// <returns></returns>
        public int AddData(object obj)
        {
            return AddData(obj, GetType(obj));
        }
        internal int AddData(object obj, Int32 type)
        {
            if (obj is string[])
                return 0;
            int min = max;
            if(type==DataType.String)
            {
                string str = obj as string;
                for (int a = max - 1; a >= 0; a--)
                {
                    var c = buff[a].obj as string;
                    if (str == c)
                    {
                        buff[a].rc++;
                        return a;
                    }
                    if (buff[a].rc == 0)
                    {
                        min = a;
                    }
                }
            }
            else
            {
                for (int a = max - 1; a >= 0; a--)
                {
                    if (obj == buff[a].obj)
                    {
                        buff[a].rc++;
                        return a;
                    }
                    if (buff[a].rc == 0)
                    {
                        min = a;
                    }
                }
            }
            buff[min].rc = 1;
            buff[min].type = (Int16)type;
            buff[min].obj = obj;
            if (type == DataType.FakeStructArray)
                buff[min].size = (obj as FakeStructArray).m_size;
            if (min == max)
            {
                max++;
                if (max >= Count)
                {
                    Count *= 2;
                    ReferenceCount[] tmp = new ReferenceCount[Count];
                    Array.Copy(buff, tmp, min + 1);
                    buff = tmp;
                }
            }
            return min;
        }
        public object GetData(int index)
        {
            return buff[index].obj;
        }
        public object this[int index]
        {
            get {
                if (index < 0|index>buff.Length)
                    return null;
                    return buff[index].obj;
            }
        }
        public void RemoveData(int index)
        {
            if (index < 1)
                return;
            if (index < buff.Length)
                buff[index].rc--;
        }
        ReferenceCount[] buff;
        int max = 1;
        int Count = 256;
        /// <summary>
        /// 创建一个空白的缓存
        /// </summary>
        /// <param name="buffCount"></param>
        public DataBuffer(int buffCount = 256)
        {
            buff = new ReferenceCount[buffCount];
            buff[0].rc = 256;
            Count = buffCount;
        }
        byte[] temp;
        unsafe byte* tempStart;
        /// <summary>
        /// 从已有的数据进行恢复
        /// </summary>
        /// <param name="dat"></param>
        public unsafe DataBuffer(byte[] dat)
        {
            temp = dat;
            var src = Marshal.UnsafeAddrOfPinnedArrayElement(dat, 0);
            tempStart = (byte*)src;
            Int32* ip = (Int32*)src;
            ip = ReadHead(ip);
            if (fakeStruct == null)
                return;
            Int32 len = *ip;
            ip++;
            Int32* rs = ip + len * 3;
            int a = len;
            for (int i = 0; i < 32; i++)
            {
                a >>= 1;
                if (a == 0)
                {
                    a = 2 << i;
                    break;
                }
            }
            max = len;
            Count = a;
            buff = new ReferenceCount[a];
            byte* bp = (byte*)rs;
            for (int i = 0; i < len; i++)
            {
                GetTable(ip, i, bp);
                ip += 3;
            }
            temp = null;
        }
        unsafe Int32* ReadHead(Int32* p)
        {
            int len = *p;
            if (len > 0)
            {
                p++;
                if (AddressDetection((byte*)p, len))
                {
                    len /= 4;
                    fakeStruct = new FakeStruct(this, len, p);
                    p += len;
                }
            }
            else
            {
                p++;
            }
            return p;
        }
        unsafe void GetTable(Int32* p, int index, byte* rs)
        {
            if (!AddressDetection((byte*)p, 8))
                return;
            Int16* sp = (Int16*)p;
            buff[index].rc = *sp;
            sp++;
            buff[index].type = *sp;
            p++;
            buff[index].size = *p;
            p++;
            Int32 offset = *p;
            rs += offset;
            if (AddressDetection(rs, 4))//如果资源地址合法
                buff[index].obj = GetObject(rs, buff[index].type, buff[index].size);
        }
        unsafe object GetObject(byte* bp, short type, int size)
        {
            Int32* p = (Int32*)bp;
            int len = *p;
            if (len == 0)
                return null;
            p++;
            if (!AddressDetection((byte*)p, len))
                return null;
            if (type == DataType.String)
            {
                int offset = (int)((byte*)p - tempStart);
                return Encoding.UTF8.GetString(temp, offset, len);
            }
            else if (type == DataType.FakeStruct)
            {
                len /= 4;
                return new FakeStruct(this, len, p);
            }
            else if (type == DataType.FakeStructArray)
            {
                len /= size;
                len /= 4;
                return new FakeStructArray(this, size, len, p);
            }
            else if (type == DataType.ByteArray)
            {
                byte[] buf = new byte[len];
                bp += 4;
                for (int i = 0; i < len; i++)
                { buf[i] = *bp; bp++; }
                return buf;
            }
            else if (type == DataType.Int32Array)
            {
                len /= 4;
                Int32[] buf = new Int32[len];
                for (int i = 0; i < len; i++)
                { buf[i] = *p; p++; }
                return buf;
            }
            else if (type == DataType.FloatArray)
            {
                len /= 4;
                Single[] buf = new Single[len];
                for (int i = 0; i < len; i++)
                { buf[i] = *p; p++; }
                return buf;
            }
            else if (type == DataType.Int16Array)
            {
                len /= 2;
                Int16[] buf = new Int16[len];
                Int16* sp = (Int16*)p;
                for (int i = 0; i < len; i++)
                {
                    buf[i] = *sp; sp++;
                }
                return buf;
            }
            else if (type == DataType.Int64Array)
            {
                len /= 8;
                Int64[] buf = new long[len];
                for (int i = 0; i < len; i++)
                { buf[i] = *p; p++; }
                return buf;
            }
            else if (type == DataType.DoubleArray)
            {
                len /= 8;
                Double[] buf = new Double[len];
                for (int i = 0; i < len; i++)
                { buf[i] = *p; p++; }
                return buf;
            }
            else if (type == DataType.FakeStringArray)
            {
                len = *p;
                p++;
                return new FakeStringArray(this, p, len);
            }
            return null;
        }
        /// <summary>
        /// 检测地址是否合法
        /// </summary>
        /// <param name="bp"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public unsafe bool AddressDetection(byte* bp, int len)
        {
            var offset = bp - tempStart;
            if (offset + len > temp.Length)
                return false;
            return true;
        }
        static byte[] GetBytes(object obj)
        {
            if (obj is byte[])
                return obj as byte[];
            var array = obj as Array;
            if (array != null)
            {
                int size = GetArraySize(array);
                int len = array.Length;
                byte[] buf = new byte[len * size];
                var src = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
                Marshal.Copy(src, buf, 0, buf.Length);
                return buf;
            }
            else
            {
                var str = obj as string;
                if (str == null)
                {
                    var to = obj as ToBytes;
                    if (to != null)
                        return to.ToBytes();
                    return null;
                }
                return Encoding.UTF8.GetBytes(str);
            }
        }
        public byte[] GetBytes(int index)
        {
            return GetBytes(buff[index].obj);
        }
        public byte[] ToBytes()
        {
            MemoryStream table = new MemoryStream();
            if (fakeStruct == null)
            {
                table.Write(Zreo, 0, 4);
            }
            else
            {
                byte[] buf = fakeStruct.ToBytes();
                Int32 len = buf.Length;
                table.Write(len.ToBytes(), 0, 4);
                table.Write(buf, 0, len);
            }
            table.Write(max.ToBytes(), 0, 4);
            MemoryStream ms = new MemoryStream();
            Int32 offset = 0;
            for (int i = 0; i < max; i++)
            {
                var buf = GetBytes(buff[i].obj);
                table.Write(buff[i].rc.ToBytes(), 0, 2);//引用计数
                table.Write(buff[i].type.ToBytes(), 0, 2);//数据类型
                table.Write(buff[i].size.ToBytes(), 0, 4);//数据结构体长度
                table.Write(offset.ToBytes(), 0, 4);//数据偏移地址
                if (buf == null)
                {
                    ms.Write(Zreo, 0, 4);
                    offset += 4;
                }
                else
                {
                    Int32 len = buf.Length;
                    ms.Write(len.ToBytes(), 0, 4);
                    ms.Write(buf, 0, len);
                    offset += len + 4;
                }
            }
            byte[] tmp = ms.ToArray();
            table.Write(tmp, 0, tmp.Length);
            tmp = table.ToArray();
            ms.Dispose();
            table.Dispose();
            return tmp;
        }
        public int AddArray<T>(T[] obj) where T : unmanaged
        {
            if(obj is byte[])
               return AddData(obj,DataType.ByteArray);
            var dat = obj.ToBytes<T>();
            return AddData(dat,DataType.ByteArray);
        }
        public T[] GetArray<T>(int index) where T : unmanaged
        {
            var buf = buff[index].obj as byte[];
            if(buf!=null)
               return buf.ToArray<T>();
            return null;
        }
    }
}