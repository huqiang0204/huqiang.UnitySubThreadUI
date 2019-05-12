using System;
using UnityEngine;

namespace huqiang.Pool
{
    public class ModelBuffer
    {
        Type[] types;
        Action<GameObject> Reset;
        GameObject[] buff;
        int point, size;
        public Int64 type;
        public ModelBuffer(Int64 type, int buffersize, Action<GameObject> reset,Type[] typ)
        {
            if (buffersize > 0)
                buff = new GameObject[buffersize];
            point = 0;
            size = buffersize;
            this.type = type;
            types = typ;
            Reset = reset;
        }
        /// <summary>
        /// 找回或创建一个新的实例
        /// </summary>
        /// <returns></returns>
        public GameObject CreateObject()
        {
            if (point > 0)
            {
                point--;
                buff[point].SetActive(true);
                if (Reset != null)
                    Reset(buff[point]);
                return buff[point];
            }
            GameObject g = new GameObject("", types);
            if (Reset != null)
                Reset(g);
            return g;
        }
        /// <summary>
        /// 回收一个实例
        /// </summary>
        /// <param name="obj"></param>
        public bool ReCycle(GameObject obj)
        {
            if (point >= size)
            {
                GameObject.Destroy(obj);
                return false;
            }
            else
            {
                if (Reset != null)
                    Reset(obj);
                for (int i = 0; i < point; i++)
                    if (buff[i] == obj)
                        return false;//防止重复回收
                obj.SetActive(false);
                obj.name = "buff";
                buff[point] = obj;
                point++;
                return true;
            }
        }
    }
}
