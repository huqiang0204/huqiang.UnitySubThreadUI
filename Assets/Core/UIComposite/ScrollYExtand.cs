using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 带有标题的,可以展开收缩的
    /// </summary>
    public class ScrollYExtand : ModelInital
    {
        EventCallBack eventCall;
        protected float height;
        int wm = 1;
        public float Point;
        public Vector2 ActualSize;
        public Action<ScrollYExtand, Vector2> Scroll;
        public override void Initial(ModelElement model)
        {
            Model = model;
            eventCall = EventCallBack.RegEvent<EventCallBack>(model);
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            Size = Model.data.sizeDelta;
            eventCall.CutRect = true;
            Titles = new List<ScrollItem>();
            Items = new List<ScrollItem>();
            Tails = new List<ScrollItem>();
            if (model != null)
            {
                TitleMod = model.Find("Title");
                if (TitleMod != null)
                {
                    TitleSize = TitleMod.data.sizeDelta;
                    TitleMod.activeSelf = false;
                }
                ItemMod = model.Find("Item");
                if (ItemMod != null)
                {
                    ItemSize = ItemMod.data.sizeDelta;
                    ItemMod.activeSelf = false;
                }
                TailMod = model.Find("Tail");
                if (TailMod != null)
                {
                    TailSize = TailMod.data.sizeDelta;
                    TailMod.activeSelf = false;
                }
            }
        }
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if ( Model== null)
                return;
            var size = Size;
            float y = Point + v.y;
            if (y < 0)
            {
                y = 0; back.VelocityY = 0;
            }
            if (y + size.y > ActualSize.y)
            {
                y = ActualSize.y - size.y;
                back.VelocityY = 0;
            }
            if (back.VelocityX != 0)
                back.VelocityX = 0;
            Order(y);
            if (Scroll != null)
                Scroll(this, v);
        }
        public float Space = 0;
        public Vector2 Size;
        public Vector2 TitleSize;
        public Vector2 ItemSize;
        public Vector2 TailSize;
        public ModelElement TitleMod;
        public ModelElement TailMod;
        public ModelElement ItemMod;
        public List<DataTemplate> BindingData;
        public Vector2 TitleOffset = Vector2.zero;
        public Vector2 TailOffset = Vector2.zero;
        public Vector2 ItemOffset = Vector2.zero;
        List<ScrollItem> Titles;
        List<ScrollItem> Tails;
        List<ScrollItem> Items;
        List<ScrollItem> TitleBuffer = new List<ScrollItem>();
        List<ScrollItem> ItemBuffer = new List<ScrollItem>();
        List<ScrollItem> TailBuffer = new List<ScrollItem>();
        List<ScrollItem> TitleRecycler = new List<ScrollItem>();
        List<ScrollItem> ItemRecycler = new List<ScrollItem>();
        List<ScrollItem> TailRecycler = new List<ScrollItem>();
        int max_count;
        /// <summary>
        /// 所有设置完毕或更新数据时刷新
        /// </summary>
        public void Refresh(float y = 0)
        {
            if (BindingData == null)
                return;
            if (ItemMod == null)
                return;
            if (ItemSize.y == 0)
                return;
            for (int i = 0; i < Items.Count; i++)
                Items[i].datacontext = null;
            float w = Size.x - ItemOffset.x;
            w /= ItemSize.x;
            wm = (int)w;
            if (wm < 1)
                wm = 1;
            CalculOffset();
            if (content_high < Size.y)
                content_high = Size.y;
            max_count = (int)(Size.y / ItemSize.y) * wm;
            int more = (int)(200 / ItemSize.y);
            if (more < 2)
                more = 2;
            max_count += more * wm;
            if (max_count > data_count)
                max_count = data_count;
            PushItems();
            Order(y, true);

        }
        void Order(float y, bool force = false)
        {
            for (int i = 0; i < BindingData.Count; i++)
            {
                var dat = BindingData[i];
                if (UpdateTitle(y, dat, i, force))
                    break;
                if (!dat.Hide)
                    if (UpdateItem(y, dat, i))
                        break;
                if (TailMod != null)
                    if (!dat.HideTail)
                        if (UpdateTail(y, dat, i, force))
                            break;
            }
            Point = y;
            RecycleRemain();
        }

        void CalculOffset()
        {
            float dy = 0;
            int s = 0;
            var count = BindingData.Count;
            for (int i = 0; i < count; i++)
            {
                var a = BindingData[i];
                var dat = a.Data;
                a.Start = dy;
                a.Index = s;
                float h = TitleSize.y;
                if (!a.HideTail)
                    h += TailSize.y;
                if (!a.Hide)
                    if (dat != null)
                    {
                        int c = dat.Count;
                        if (c > 0)
                        {
                            s += c;
                            int d = c / wm;
                            if (c % wm > 0)
                                d++;
                            h += d * ItemSize.y;
                        }
                    }
                a.Height = h;
                dy += h;
                a.End = dy;
                dy += Space;
            }
            if (count > 0)
                dy -= Space;
            data_count = s;
            content_high = dy;
        }
        int data_count;
        float content_high;
        public class DataTemplate
        {
            public object Title;
            public object Tail;
            public IList Data;
            public bool Hide;
            public bool HideTail;
            internal int Index;
            public float Height { internal set; get; }
            internal float Start;
            internal float End;
        }
        public void SetSize(Vector2 size)
        {
            Model.data.sizeDelta = size;
            Size = size;
        }
        bool UpdateTitle(float oy, DataTemplate dt, int index, bool force = false)
        {
            float os = dt.Start - oy;
            float oe = dt.Start + TitleSize.y;
            if (oe < 0)
                return false;
            if (os > Size.y + TitleSize.y)
                return true;
            float ay = os;
            float st = Size.y * 0.5f;
            float ht = TitleSize.y * 0.5f;
            var t = PopItem(TitleBuffer,index);
            if (t == null)
                t = CreateTitle();
            t.target.data.localPosition = new Vector3(TitleOffset.x, ay + st - TitleOffset.y - ht, 0);
            t.target.activeSelf = true;
            if (force | t.datacontext != dt.Title)
            {
                t.datacontext = dt.Title;
                ItemUpdate(t.obj, dt, index,TitleCreator);
            }
            return false;
        }
        bool UpdateTail(float oy, DataTemplate dt, int index, bool force = false)
        {
            float oe = dt.End - oy;
            float os = oe - TailSize.y;
            if (oe < 0)
                return false;
            if (os > Size.y + TailSize.y)
                return true;
            float ay = os + TailSize.y * 0.5f;
            float st = Size.y * 0.5f;
            float ht = TailSize.y * 0.5f;
            var t = PopItem(TailBuffer,index);
            if (t == null)
                t = CreateTail();
            t.target.data.localPosition = new Vector3(TitleOffset.x, ay + st - TailOffset.y + ht, 0);
            t.target.activeSelf = true;
            if (force | t.datacontext != dt.Title)
            {
                t.datacontext = dt.Tail;
                ItemUpdate(t.obj, dt, index,TailCreator);
            }
            return false;
        }
        bool UpdateItem(float oy, DataTemplate dt, int index)
        {
            var data = dt.Data;
            if (data == null)
                return false;
            float ay = dt.Start + TitleSize.y - oy;
            int len = data.Count;
            int c = 0;
            int r = 0;
            float ah = ItemSize.y;
            int j = dt.Index;
            float oh = ItemSize.y * 0.5f;
            float st = Size.y * 0.5f;

            for (int i = 0; i < len; i++)
            {
                float os = ay + r * ah;
                float oe = os + ah;
                if (oe < 0)
                    goto label;
                if (os > Size.y)
                    return true;
                var t = PopItem(ItemBuffer,index);
                if (t == null)
                    t = CreateItem();
                t.target.activeSelf = true;
                t.target.data.localPosition = new Vector3(ItemOffset.x + ItemSize.x * c, os - oh + st, 0);
                var d = data[i];
                if (t.datacontext != d)
                {
                    t.datacontext = d;
                    ItemUpdate(t.obj, d, i,ItemCreator);
                }
            label:;
                c++;
                if (c >= wm)
                { r++; c = 0; }
            }
            return false;
        }
        public void Dispose()
        {
            for (int i = 0; i < Titles.Count; i++)
                ModelManagerUI.RecycleElement(Titles[i].target);
            for (int i = 0; i < Items.Count; i++)
                ModelManagerUI.RecycleElement(Items[i].target);
            for (int i = 0; i < Tails.Count; i++)
                ModelManagerUI.RecycleElement(Tails[i].target);
            Titles.Clear();
            Items.Clear();
            Tails.Clear();
        }
        Constructor ItemCreator;
        Constructor TitleCreator;
        Constructor TailCreator;
        public void SetTitleUpdate(Action<object, object, int> action, Func<ModelElement, object> reflect)
        {
            for (int i = 0; i < Titles.Count; i++)
                ModelManagerUI.RecycleElement(Titles[i].target);
            Titles.Clear();
            var m = new Middleware<ModelElement, object>();
            m.Update = action;
            m.hotfix = true;
            m.reflect = reflect;
            TitleCreator = m;
        }
        public void SetTitleUpdate<T, U>(Action<T, U, int> action) where T : class, new()
        {
            for (int i = 0; i < Titles.Count; i++)
                ModelManagerUI.RecycleElement(Titles[i].target);
            Titles.Clear();
            var m = new Middleware<T, U>();
            m.Invoke = action;
            TitleCreator = m;
        }
        public void SetItemUpdate(Action<object, object, int> action, Func<ModelElement, object> reflect)
        {
            for (int i = 0; i < Items.Count; i++)
                ModelManagerUI.RecycleElement(Items[i].target);
            Items.Clear();
            var m = new Middleware<ModelElement, object>();
            m.Update = action;
            m.hotfix = true;
            m.reflect = reflect;
            ItemCreator = m;
        }
        public void SetItemUpdate<T, U>(Action<T, U, int> action) where T : class, new()
        {
            for (int i = 0; i < Items.Count; i++)
                ModelManagerUI.RecycleElement(Items[i].target);
            Items.Clear();
            var m = new Middleware<T, U>();
            m.Invoke = action;
            ItemCreator = m;
        }
        public void SetTailUpdate(Action<object, object, int> action, Func<ModelElement, object> reflect)
        {
            for (int i = 0; i < Items.Count; i++)
                ModelManagerUI.RecycleElement(Items[i].target);
            Items.Clear();
            var m = new Middleware<ModelElement, object>();
            m.Update = action;
            m.hotfix = true;
            m.reflect = reflect;
            TailCreator = m;
        }
        public void SetTailUpdate<T, U>(Action<T, U, int> action) where T : class, new()
        {
            for (int i = 0; i < Tails.Count; i++)
                ModelManagerUI.RecycleElement(Tails[i].target);
            Tails.Clear();
            var m = new Middleware<T, U>();
            m.Invoke = action;
            TailCreator = m;
        }
        protected void ItemUpdate(object obj, object dat, int index,Constructor con)
        {
            if (con != null)
            {
                if (con.hotfix)
                {
                    if (con.Update != null)
                        con.Update(obj, dat, index);
                }
                else
                {
                    con.Call(obj, dat, index);
                }
            }
        }
        protected ScrollItem CreateItem(List<ScrollItem> buffer, Constructor con, ModelElement mod)
        {
            if (buffer.Count > 0)
            {
                var it = buffer[0];
                it.target.activeSelf = true;
                it.index = -1;
                buffer.RemoveAt(0);
                return it;
            }
            ModelElement me = new ModelElement();
            me.Load(mod.ModData);
            me.SetParent(Model);
            ScrollItem a = new ScrollItem();
            a.target = me;
            if(con==null)
            {
                a.obj = me;
            }
            else
            {
                if (con.hotfix)
                {
                    if (con.reflect != null)
                        a.obj = con.reflect(me);
                    else a.obj = me;
                }
                else
                {
                    a.obj = con.Create();
                    ModelManagerUI.ComponentReflection(me, a.obj);
                }
            }
            return a;
        }
        ScrollItem CreateTitle()
        {
            return CreateItem(TitleRecycler,TitleCreator,TitleMod);
        }
        ScrollItem CreateItem()
        {
            return CreateItem(ItemRecycler,ItemCreator,ItemMod);
        }
        ScrollItem CreateTail()
        {
            return CreateItem(TailRecycler,TailCreator,TailMod);
        }
        protected void PushItems()
        {
            PushItems(TitleBuffer,Titles);
            PushItems(ItemBuffer,Items);
            PushItems(TailBuffer,Tails);
        }
        protected void PushItems(List<ScrollItem> tar, List<ScrollItem> src)
        {
            for (int i = 0; i < src.Count; i++)
                src[i].target.activeSelf = false;
            tar.AddRange(src);
            src.Clear();
        }
        protected ScrollItem PopItem(List<ScrollItem> tar, int index)
        {
            for (int i = 0; i < tar.Count; i++)
            {
                var t = tar[i];
                if (t.index == index)
                {
                    tar.RemoveAt(i);
                    t.target.activeSelf = true;
                    return t;
                }
            }
            return null;
        }
        protected void RecycleRemain()
        {
            PushItems(TitleRecycler,TitleBuffer);
            PushItems(ItemRecycler,ItemBuffer);
            PushItems(TailRecycler,TailBuffer);
        }
    }
}
