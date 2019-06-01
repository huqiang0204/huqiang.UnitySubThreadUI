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
        public ModelElement View;
        public Vector2 ActualSize;
        public Action<ScrollYExtand, Vector2> Scroll;
        public override void Initial(ModelElement model)
        {
            View = model;
            eventCall = EventCallBack.RegEvent<EventCallBack>(model);
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            Size = View.data.sizeDelta;
            eventCall.CutRect = true;
            Titles = new List<Item>();
            Items = new List<Item>();
            Tails = new List<Item>();
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
            if (View == null)
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
        public Action<ModelElement, DataTemplate, int> TitleUpdate;
        public Action<ModelElement, DataTemplate, int> TailUpdate;
        public Action<ModelElement, object, int> ItemUpdate;
        public List<DataTemplate> BindingData;
        public Vector2 TitleOffset = Vector2.zero;
        public Vector2 TailOffset = Vector2.zero;
        public Vector2 ItemOffset = Vector2.zero;
        List<Item> Titles;
        List<Item> Tails;
        List<Item> Items;
        class Item
        {
            public ModelElement target;
            public object datacontext;
        }
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
            Initialtems();
            for (int i = 0; i < Titles.Count; i++)
                Titles[i].target.activeSelf = false;
            for (int i = 0; i < Tails.Count; i++)
                Tails[i].target.activeSelf = false;
            for (int i = 0; i < Items.Count; i++)
                Items[i].target.activeSelf = false;
            Order(y, true);
        }
        void Initialtems()
        {
            int c = max_count;
            for (int i = 0; i < c; i++)
            {
                if (i >= Items.Count)
                {
                    ModelElement mod = new ModelElement();
                    mod.Load(ItemMod.ModData);
                    mod.SetParent(View);
                    mod.data.localScale = new Vector3(1, 1, 1);
                    mod.IsChanged = true;
                    Item a = new Item();
                    a.target = mod;
                    Items.Add(a);
                }
            }
            for (int i = Items.Count - 1; i >= c; i--)
            {
                var g = Items[i];
                Items.RemoveAt(i);
                ModelManagerUI.RecycleElement(g.target);
            }
        }
        void Order(float y, bool force = false)
        {
            for (int i = 0; i < BindingData.Count; i++)
            {
                var dat = BindingData[i];
                if (UpdateTitle(y, dat, i, force))
                    break;
                if (Items.Count > 0)
                    if (!dat.Hide)
                        if (UpdateItem(y, dat, i))
                            break;
                if (TailMod != null)
                    if (!dat.HideTail)
                        if (UpdateTail(y, dat, i, force))
                            break;
            }
            Point = y;
        }
        Item GetTitle(int index)
        {
            while (index >= Titles.Count)
            {
                Item i = new Item();
                if (TitleMod != null)
                {
                    ModelElement mod = new ModelElement();
                    mod.Load(TitleMod.ModData);
                    mod.SetParent(View);
                    mod.data.localScale = new Vector3(1, 1, 1);
                    mod.IsChanged = true;
                }
                Titles.Add(i);
            }
            Titles[index].target.activeSelf = true;
            return Titles[index];
        }
        Item GetTail(int index)
        {
            while (index >= Tails.Count)
            {
                Item i = new Item();
                if (TailMod != null)
                {
                    ModelElement mod = new ModelElement();
                    mod.Load(TailMod.ModData);
                    mod.SetParent(View);
                    mod.data.localScale = new Vector3(1, 1, 1);
                    mod.IsChanged = true;
                }
                Tails.Add(i);
            }
            Tails[index].target.activeSelf = true;
            return Tails[index];
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
            View.data.sizeDelta = size;
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
            float ay = os + TitleSize.y * 0.5f;
            float st = Size.y * 0.5f;
            float ht = TitleSize.y * 0.5f;
            var t = GetTitle(index);

            t.target.data.localPosition = new Vector3(TitleOffset.x, ay + st - TitleOffset.y - ht, 0);
            t.target.activeSelf = true;
            if (force | t.datacontext != dt.Title)
            {
                if (TitleUpdate != null)
                    TitleUpdate(t.target, dt, index);
                t.datacontext = dt.Title;
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
            var t = GetTail(index);
            t.target.data.localPosition = new Vector3(TitleOffset.x, ay + st - TailOffset.y + ht, 0);
            t.target.activeSelf = true;
            if (force | t.datacontext != dt.Title)
            {
                if (TailUpdate != null)
                    TailUpdate(t.target, dt, index);
                t.datacontext = dt.Tail;
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
                int p = j + i;
                p %= Items.Count;
                var t = Items[p];
                t.target.activeSelf = true;

                t.target.data.localPosition = new Vector3(ItemOffset.x + ItemSize.x * c, os - oh + st, 0);
                var d = data[i];
                if (t.datacontext != d)
                {
                    if (ItemUpdate != null)
                        ItemUpdate(t.target, d, i);
                    t.datacontext = d;
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
            Titles.Clear();
            Items.Clear();
        }
    }
}
