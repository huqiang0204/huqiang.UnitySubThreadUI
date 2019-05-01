using System;
using System.Runtime.InteropServices;


namespace huqiang.Data
{
    public class FakeStruct : ToBytes
    {
        public unsafe void ReadFromStruct(void* tar)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ptr;
            for (int i = 0; i < element; i++)
            {
                *p = *t;
                t++;
                p++;
            }
        }
        public unsafe void WitreToStruct(void* tar)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ptr;
            for (int i = 0; i < element; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        public unsafe void ReadFromStruct(void* tar, int start, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)(ptr);
            p += start;
            for (int i = 0; i < size; i++)
            {
                *p = *t;
                t++;
                p++;
            }
        }
        public unsafe void WitreToStruct(void* tar, int start, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ptr;
            p += start;
            for (int i = 0; i < size; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        IntPtr ptr;
        public unsafe byte* ip;
        int msize;
        int element;
        public DataBuffer buffer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="size">元素个数</param>
        public FakeStruct(DataBuffer db, int size)
        {
            element = size;
            msize = size * 4;
            ptr = Marshal.AllocHGlobal(msize);
            unsafe
            {
                ip = (byte*)ptr;
                Int32* p = (Int32*)ptr;
                for (int i = 0; i < size; i++)
                {
                    *p = 0;
                    p++;
                }
            }
            buffer = db;
        }
        public unsafe FakeStruct(DataBuffer db, int size, Int32* point)
        {
            element = size;
            msize = size * 4;
            ptr = Marshal.AllocHGlobal(msize);
            unsafe
            {
                ip = (byte*)ptr;
                Int32* p = (Int32*)ptr;
                for (int i = 0; i < size; i++)
                {
                    *p = *point;
                    point++;
                    p++;
                }
            }
            buffer = db;
        }
        ~FakeStruct()
        {
            Marshal.FreeHGlobal(ptr);
        }
        public unsafe Int32 this[int index]
        {
            set
            {
                if (index < 0)
                    return;
                if (index >= element)
                    return;
                int o = index * 4;
                *(Int32*)(ip + o) = value;
            }
            get
            {
                if (index < 0)
                    return 0;
                if (index >= element)
                    return 0;
                int o = index * 4;
                return *(Int32*)(ip + o);
            }
        }
        public unsafe void SetData(int index, object dat)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            Int32* a = (Int32*)(ip + o);
            buffer.RemoveData(*a);
            *a = buffer.AddData(dat);
        }
        public unsafe T GetData<T>(int index) where T : class
        {
            if (index < 0)
                return null;
            if (index >= element)
                return null;
            int o = index * 4;
            Int32* a = (Int32*)(ip + o);
            return buffer.GetData(*a) as T;
        }
        public unsafe object GetData(int index)
        {
            if (index < 0)
                return null;
            if (index >= element)
                return null;
            int o = index * 4;
            Int32* a = (Int32*)(ip + o);
            return buffer.GetData(*a);
        }
        public unsafe void SetData(int* addr, object dat)
        {
            int a = (int)addr - (int)ip;
            if (a < 0|a>=msize)//超过界限
                return;
            buffer.RemoveData(*addr);
            *addr = buffer.AddData(dat);
        }
        public unsafe T GetData<T>(int* addr) where T : class
        {
            int a = (int)addr - (int)ip;
            if (a < 0|a>=msize)//超过界限
                return null;
            return buffer.GetData(*addr) as T;
        }
        public unsafe void SetInt64(int index, Int64 value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(Int64*)(ip + o) = value;
        }
        public unsafe Int64 GetInt64(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(Int64*)(ip + o);
        }
        public unsafe void SetFloat(int index, float value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(float*)(ip + o) = value;
        }
        public unsafe float GetFloat(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(float*)(ip + o);
        }
        public unsafe void SetDouble(int index, double value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(double*)(ip + o) = value;
        }
        public unsafe double GetDouble(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(double*)(ip + o);
        }
        public byte[] ToBytes()
        {
            byte[] tmp = new byte[msize];
            Marshal.Copy(ptr, tmp, 0, msize);
            return tmp;
        }
        /// <summary>
        /// 进行元素扩展
        /// </summary>
        /// <param name="size"></param>
        public unsafe void Extend(int size)
        {
            int s = msize;
            element += size;
            msize = s + size * 4;
            var tp = Marshal.AllocHGlobal(msize);
            unsafe
            {
                ip = (byte*)ptr;
                Int32* p = (Int32*)ptr;
                Int32* t = (Int32*)tp;
                for (int i = 0; i < s; i++)
                {
                    *t = *p;
                    p++;
                    t++;
                }
            }
            Marshal.FreeHGlobal(ptr);
            ptr = tp;
        }
    }
}
