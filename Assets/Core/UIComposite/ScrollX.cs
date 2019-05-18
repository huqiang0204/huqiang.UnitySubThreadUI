using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class ScrollX:ScrollContent
    {
        static void CenterScroll(ScrollX scroll)
        {
            var eve = scroll.eventCall;
            var tar = scroll.eventCall.ScrollDistanceX;
            float v = scroll.Point + tar;
            float sx = scroll.ItemSize.x;
            float ox = v % sx;
            tar -= ox;
            if (ox > sx * 0.5f)
                tar += sx;
            scroll.eventCall.ScrollDistanceX = tar;
            v = scroll.Point + tar + scroll.ScrollView.data.sizeDelta.x * 0.5f;
            int i = (int)(v / sx);
            int c = scroll.DataLength;
            i %= c;
            if (i < 0)
                i += c - 1;
            scroll.PreDockindex = i;
        }
        public EventCallBack eventCall;//scrollx自己的按钮
        protected float height;
        int Column = 1;
        float m_point;
        /// <summarx>
        /// 滚动的当前位置，从0开始
        /// </summarx>
        public float Point { get { return m_point; } set { Refresh(0, value - m_point); } }
        float m_pos = 0;
        /// <summarx>
        /// 0-1之间
        /// </summarx>
        public float Pos
        {
            get { var p = m_point / (ActualSize.x - Size.x); if (p < 0) p = 0; else if (p > 1) p = 1; return p; }
            set
            {
                if (value < 0 | value > 1)
                    return;
                m_point = value * (ActualSize.x - Size.x);
                Order();
            }
        }
        public bool ItemDockCenter;
        public int PreDockindex { get; private set; }
        public Vector2 ContentSize { get; private set; }
        public ScrollX()
        {
        }

        public override void Initial(ModelElement model)
        {
            base.Initial(model);
            eventCall = EventCallBack.RegEvent<EventCallBack>(model);
            eventCall.Drag = Draging;
            eventCall.DragEnd = (o, e, s) =>
            {
                Scrolling(o, s);
                if (ItemDockCenter)
                    CenterScroll(this);
                if (ScrollStart != null)
                    ScrollStart(this);
                if (eventCall.VelocityX == 0)
                    OnScrollEnd(o);
            };
            eventCall.Scrolling = Scrolling;
            eventCall.ScrollEndX = OnScrollEnd;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            Size = ScrollView.data.sizeDelta;
            eventCall.CutRect = true;
            if (model != null)
            {
                var item = model.FindChild("Item");
                if (item != null)
                {
                    item.activeSelf = false;
                    ItemMod = item.ModData;
                    ItemSize = item.data.sizeDelta;
                }
            }
        }
        public Action<ScrollX, Vector2> Scroll;
        public Action<ScrollX> ScrollStart;
        public Action<ScrollX> ScrollEnd;
        public Action<ScrollX> ScrollToTop;
        public Action<ScrollX> ScrollToDown;
        void Draging(EventCallBack back, UserAction action, Vector2 v)
        {
            back.DecayRateX = 0.998f;
            Scrolling(back, v);
        }
        /// <summarx>
        /// 
        /// </summarx>
        /// <param name="back"></param>
        /// <param name="v">移动的实际像素位移</param>
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if (ScrollView == null)
                return;
            v.x /= eventCall.Context.data.localScale.x;
            float x = Limit(back, v.x);
            Order();
            if (x != 0)
            {
                if (Scroll != null)
                    Scroll(this, v);
            }
            else
            {
                if (ScrollEnd != null)
                    ScrollEnd(this);
            }
        }
        void OnScrollEnd(EventCallBack back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (m_point < 0)
                {
                    back.DecayRateX = 0.988f;
                    float d = 0.25f - m_point;
                    back.ScrollDistanceX = d * eventCall.Context.data.localScale.x;
                }
                else if (m_point + Size.x > ActualSize.x)
                {
                    back.DecayRateX = 0.988f;
                    float d = ActualSize.x - m_point - Size.x - 0.25f;
                    back.ScrollDistanceX = d * eventCall.Context.data.localScale.x;
                }
                else
                {
                    if (ScrollEnd != null)
                        ScrollEnd(this);
                }
            }
            else if (ScrollEnd != null)
                ScrollEnd(this);
        }
        public void Calcul()
        {
            float w = Size.x - ItemOffset.x;
            w /= ItemSize.x;
            Column = (int)w;
            if (Column < 1)
                Column = 1;
            int c = DataLength;
            int a = c % Column;
            c /= Column;
            if (a > 0)
                c++;
            height = c * ItemSize.x;
            if (height < Size.x)
                height = Size.x;
            ActualSize = new Vector2(Size.x, height);
        }
        public override void Refresh(float x = 0, float y = 0)
        {
            m_point = x;
            Size = ScrollView.data.sizeDelta;
            ActualSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].target.activeSelf = false;
                }
                return;
            }
            if (ItemMod == null)
            {
                return;
            }
            if (ItemSize.x == 0)
            {
                return;
            }
            Calcul();
            Order(true);
        }
        /// <summarx>
        /// 指定下标处的位置重排
        /// </summarx>
        /// <param name="_index"></param>
        public void ShowBxIndex(int _index)
        {
            Size = ScrollView.data.sizeDelta;
            ActualSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                    Items[i].target.activeSelf = false;
                return;
            }
            if (ItemMod == null)
            {
                return;
            }
            if (ItemSize.x == 0)
            {
                return;
            }
            float x = _index * ItemSize.x;
            m_point = x;
            Calcul();
            Order(true);
        }
        void Order(bool force = false)
        {
            int len = DataLength;
            float lx = ItemSize.x;
            int sr = (int)(m_point / lx);//起始索引
            int er = (int)((m_point + Size.x) / lx) + 1;
            sr *= Column;
            er *= Column;//结束索引
            int e = er - sr;//总计显示数据
            if (e > len)
                e = len;
            if (scrollType == ScrollType.Loop)
            {
                if (er >= len)
                {
                    er -= len;
                    RecycleInside(er, sr);
                }
                else
                {
                    RecycleOutside(sr, er);
                }
            }
            else
            {
                if (sr < 0)
                    sr = 0;
                if (er >= len)
                    er = len;
                e = er - sr;
                RecycleOutside(sr, er);
            }

            PushItems();//将未被回收的数据压入缓冲区
            int index = sr;
            float ox = 0;
            for (int i = 0; i < e; i++)
            {
                UpdateItem(index, ox, force);
                index++;
                if (index >= len)
                {
                    index = 0;
                    ox = ActualSize.x;
                }
            }
            RecycleRemain();
        }
        void UpdateItem(int index, float ox, bool force)
        {
            float lx = ItemSize.x;
            int row = index / Column;
            float dx = lx * row + ox;
            dx -= m_point;
            float ss = 0.5f * Size.x - 0.5f * lx;
            dx = ss - dx;
            float os = (index % Column) * ItemSize.x + ItemSize.x * 0.5f + ItemOffset.x - Size.x * 0.5f;
            var a = PopItem(index);
            a.target.data.localPosition = new Vector3(os, dx, 0);
            Items.Add(a);
            if (a.index < 0 | force)
            {
                var dat = GetData(index);
                a.datacontext = dat;
                a.index = index;
                if (ItemUpdate != null)
                {
                    if (a.obj == null)
                        ItemUpdate(a.target, dat, index);
                    else ItemUpdate(a.obj, dat, index);
                }
            }
        }
        public void SetSize(Vector2 size)
        {
            Size = size;
            ScrollView.data.sizeDelta = size;
            Refresh();
        }
        protected float Limit(EventCallBack callBack, float x)
        {
            var size = Size;
            switch (scrollType)
            {
                case ScrollType.None:
                    if (x == 0)
                        return 0;
                    float vx = m_point + x;
                    if (vx < 0)
                    {
                        m_point = 0;
                        eventCall.VelocityX = 0;
                        if (ScrollToTop != null)
                            ScrollToTop(this);
                        return 0;
                    }
                    else if (vx + size.x > ActualSize.x)
                    {
                        m_point = ActualSize.x - size.x;
                        eventCall.VelocityX = 0;
                        if (ScrollToDown != null)
                            ScrollToDown(this);
                        return 0;
                    }
                    m_point += x;
                    break;
                case ScrollType.Loop:
                    if (x == 0)
                        return 0;
                    m_point += x;
                    float ax = ActualSize.x;
                    if (m_point < 0)
                        m_point += ax;
                    else if (m_point > ax)
                        m_point %= ax;
                    break;
                case ScrollType.BounceBack:
                    m_point += x;
                    if (!callBack.Pressed)
                    {
                        if (m_point < 0)
                        {
                            if (x < 0)
                            {
                                if (eventCall.DecayRateX >= 0.99f)
                                {
                                    eventCall.DecayRateX = 0.9f;
                                    eventCall.VelocityX = eventCall.VelocityX;
                                }
                            }
                        }
                        else if (m_point + size.x > ActualSize.x)
                        {
                            if (x > 0)
                            {
                                if (eventCall.DecayRateX >= 0.99f)
                                {
                                    eventCall.DecayRateX = 0.9f;
                                    eventCall.VelocityX = eventCall.VelocityX;
                                }
                            }
                        }
                    }
                    break;
            }
            return x;
        }
    }
}
