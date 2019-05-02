using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIComposite
{
    public enum ScrollType
    {
        None, Loop, BounceBack
    }
    public class ScrollContent: ModelInital
    {
        static Type me = typeof(ModelElement);
        public ScrollType scrollType=ScrollType.BounceBack;
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        public ModelElement ScrollView;
        public Vector2 Size;//scrollView的尺寸
        public Vector2 ActualSize { get; protected set; }//相当于Content的尺寸
        public Vector2 ItemSize;
        FakeStruct model;
        public Type ItemObject = me;
        public FakeStruct ItemMod
        {
            set
            {
                model = value;
                //var c = Items.Count;
                //if (c > 0)
                //{
                //    for (int i = 0; i < Items.Count; i++)
                //        ModelManagerUI.RecycleGameObject(Items[i].target);
                //    Items.Clear();
                //}
            }
            get { return model; }
        }
        public Action<object, object, int> ItemUpdate;
        IList dataList;
        Array array;
        FakeArray fakeStruct;
        /// <summary>
        /// 传入类型为IList
        /// </summary>
        public object BindingData
        {
            get
            {
                if (dataList != null)
                    return dataList;
                if (array != null)
                    return array;
                return fakeStruct;
            }
            set
            {
                if (value is IList)
                {
                    dataList = value as IList;
                    array = null;
                    fakeStruct = null;
                }
                else if (value is Array)
                {
                    dataList = null;
                    array = value as Array;
                    fakeStruct = null;
                }
                else if (value is FakeArray)
                {
                    dataList = null;
                    array = null;
                    fakeStruct = value as FakeArray;
                }
                else
                {
                    dataList = null;
                    array = null;
                    fakeStruct = null;
                }
            }
        }
        int m_len;
        public int DataLength
        {
            set { m_len = value; }
            get
            {
                if (dataList != null)
                    return dataList.Count;
                if (array != null)
                    return array.Length;
                if (fakeStruct != null)
                    return fakeStruct.Length;
                return m_len;
            }
        }
        protected object GetData(int index)
        {
            if (dataList != null)
                return dataList[index];
            if (array != null)
                return array.GetValue(index);
            return null;
        }
        public Vector2 ItemOffset = Vector2.zero;
        protected int max_count;
        public List<ScrollItem> Items=new List<ScrollItem>();
        List<ScrollItem> Buffer=new List<ScrollItem>();
        List<ScrollItem> Recycler = new List<ScrollItem>();
        public ScrollContent()
        {
        }
        /// <summary>
        /// 当无法使用跨域反射时，使用此委托进行间接反射
        /// </summary>
        public Action<ScrollItem, ModelElement> Reflection;
        /// <summary>
        /// 当某个ui超出Mask边界，被回收时调用
        /// </summary>
        public Action<ScrollItem> ItemRecycle;
        public override void Initial(ModelElement model)
        {
            ScrollView = model;
        }
        public virtual void Refresh(float x = 0, float y = 0)
        {
        }
        protected void Initialtems()
        {
            int x = (int)(Size.x / ItemSize.x) + 2;
            int y = (int)(Size.y / ItemSize.y) + 2;
            max_count = x * y;
        }
        protected ScrollItem CreateItem()
        {
            if (Recycler.Count> 0)
            {
                var it = Recycler[0];
                it.target.activeSelf = true;
                it.index = -1;
                Recycler.RemoveAt(0);
                return it;
            }
            object obj = null;
            if (ItemObject != null)
                if (ItemObject != typeof(ModelElement))
                    obj = Activator.CreateInstance(ItemObject);
            ModelElement uI = new ModelElement();
            uI.Load(model);
            uI.SetParent(ScrollView);
            ScrollItem a = new ScrollItem();
            a.target = uI;
            a.obj = obj;
            if (Reflection != null)
                Reflection(a, a.target);
            return a;
        }
        public virtual void Order(float os, bool force = false)
        {
        }
        public void Clear()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var g = Items[i];
                ModelManagerUI.RecycleElement(g.target);
            }
            for (int i = 0; i < Recycler.Count; i++)
            {
                var g = Recycler[i];
                ModelManagerUI.RecycleElement(g.target);
            }
            Items.Clear();
            Recycler.Clear();
        }
        protected void PushItems()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].target.activeSelf = false;
            Buffer.AddRange(Items);
            Items.Clear();
        }
        protected ScrollItem PopItem(int index)
        {
            for(int i=0;i<Buffer.Count;i++)
            {
                var t = Buffer[i];
                if(t.index==index)
                {
                    Buffer.RemoveAt(i);
                    t.target.activeSelf = true;
                    return t;
                }
            }
            var it = CreateItem();
            return it;
        }
        protected void RecycleInside(int down, int top)
        {
            int c = Items.Count - 1;
            for (; c >= 0; c--)
            {
                var it = Items[c];
                int index = Items[c].index;
                if (index >= down & index <= top)
                {
                    RecycleItem(it);
                    Items.RemoveAt(c);
                }
            }
        }
        /// <summary>
        /// 回收范围外的条目
        /// </summary>
        /// <param name="down"></param>
        /// <param name="top"></param>
        protected void RecycleOutside(int down, int top)
        {
            int c = Items.Count - 1;
            for (; c >= 0; c--)
            {
                var it = Items[c];
                int index = Items[c].index;
                if (index < down | index > top)
                {
                    RecycleItem(it);
                    Items.RemoveAt(c);
                }
            }
        }
        protected void RecycleRemain()
        {
            for (int i = 0; i < Buffer.Count; i++)
               Buffer[i].target.activeSelf = false;
            Recycler.AddRange(Buffer);
            Buffer.Clear();
        }
        protected void RecycleItem(ScrollItem it)
        {
            //Items.RemoveAt(index);
            it.target.activeSelf = false;
            Recycler.Add(it);
            if (ItemRecycle != null)
                ItemRecycle(it);
        }
        protected void RecycleItem(ScrollItem[] items)
        {
            Recycler.AddRange(items);
            for (int i = 0; i < items.Length; i++)
            {
                items[i].target.activeSelf = false;
            }
        }
    }
}
