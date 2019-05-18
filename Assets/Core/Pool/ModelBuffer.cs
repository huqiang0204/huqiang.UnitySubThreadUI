using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Pool
{
    public class ModelBuffer
    {
        Type[] types;
        GameObject[] buff;
        int point, size;
        public int Index;
        public Int64 type;
        Container<InstanceContext> container;
        public ModelBuffer(Int64 type, int buffersize, Type[] typ, Container<InstanceContext> contain)
        {
            if (buffersize > 0)
                buff = new GameObject[buffersize];
            point = 0;
            size = buffersize;
            this.type = type;
            types = typ;
            container = contain;
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
                return buff[point];
            }
            GameObject g = new GameObject("", types);
            if(container!=null)
            {
                InstanceContext context = new InstanceContext();
                context.Instance = g;
                context.Id = g.GetInstanceID();
                context.Type = type;
                context.buffer = this;
                container.Add(context);
            }
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
                if(container!=null)
                {
                    int id = obj.GetInstanceID();
                    container.Remove((o) => { return o.Id == id; });
                }
                GameObject.Destroy(obj);
                return false;
            }
            else
            {
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
