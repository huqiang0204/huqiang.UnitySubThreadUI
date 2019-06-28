using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class ScrollY : ScrollContent
    {
        static void CenterScroll(ScrollY scroll)
        {
            var eve = scroll.eventCall;
            var tar = scroll.eventCall.ScrollDistanceY;
            float ty = scroll.Size.y * 0.5f;
            float v = scroll.Point + tar+ty;
            float sy = scroll.ctSize.y;
            float oy = v % sy;
            tar -= oy;
            if (oy > sy * 0.5f)
                tar += sy;
            tar += sy * 0.5f;
            scroll.eventCall.ScrollDistanceY = tar;
        }
        public EventCallBack eventCall;//scrollY自己的按钮
        protected float height;
        int Column = 1;
        float m_point;
        /// <summary>
        /// 滚动的当前位置，从0开始
        /// </summary>
        public float Point { get { return m_point; } set { Refresh(0,value - m_point); } }
        /// <summary>
        /// 0-1之间
        /// </summary>
        public float Pos
        {
            get { var p = m_point/(ActualSize.y - Size.y);if (p < 0) p = 0; else if (p > 1) p = 1; return p; }
            set
            {
                if (value < 0 | value > 1)
                    return;
                m_point = value * (ActualSize.y - Size.y);
                Order();
            }
        }
        public bool ItemDockCenter;
        public Vector2 ContentSize { get; private set; }
        public bool DynamicSize = true;
        Vector2 ctSize;
        float ctScale;
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
                if (eventCall.VelocityY == 0)
                    OnScrollEnd(o);
            };
            eventCall.Scrolling = Scrolling;
            eventCall.ScrollEndY = OnScrollEnd;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            Size = Model.data.sizeDelta;
            eventCall.CutRect = true;
            model.SizeChanged = (o) => {  Refresh(0,m_point); };
        }
        public Action<ScrollY, Vector2> Scroll;
        public Action<ScrollY> ScrollStart;
        public Action<ScrollY> ScrollEnd;
        void Draging(EventCallBack back, UserAction action, Vector2 v)
        {
            back.DecayRateY = 0.998f;
            Scrolling(back, v);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="back"></param>
        /// <param name="v">移动的实际像素位移</param>
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if (Model == null)
                return;
            v.y /= eventCall.Context.data.localScale.y;
            back.VelocityX = 0;
            v.x = 0;
            float x = 0;
            float y = 0;
            switch (scrollType)
            {
                case ScrollType.None:
                    y = ScrollNone(back, ref v, ref x, ref m_point).y;
                    break;
                case ScrollType.Loop:
                    y = ScrollLoop(back, ref v, ref x, ref m_point).y;
                    break;
                case ScrollType.BounceBack:
                    y = BounceBack(back, ref v, ref x, ref m_point).y;
                    break;
            }
            Order();
            if (y != 0)
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
                if (m_point < -Tolerance)
                {
                    back.DecayRateY = 0.988f;
                    float d = - m_point;
                    back.ScrollDistanceY = d * eventCall.Context.data.localScale.y;
                }
                else
                {
                    float max = ActualSize.y + Tolerance;
                    if (max < Size.y)
                        max = Size.y + Tolerance;
                    if (m_point + Size.y > max)
                    {
                        back.DecayRateY = 0.988f;
                        float d = ActualSize.y - m_point - Size.y;
                        back.ScrollDistanceY = d * eventCall.Context.data.localScale.y;
                    }
                    else
                    {
                        if (ScrollEnd != null)
                            ScrollEnd(this);
                    }
                }
            }
            else if (ScrollEnd != null)
                ScrollEnd(this);
        }
        public void Calcul()
        {
            float w = Size.x - ItemOffset.x;
            float dw = w / ItemSize.x;
            Column = (int)dw;
            if (Column < 1)
                Column = 1;
            if (DynamicSize)
            {
                float dx = w / Column;
                ctScale =dx / ItemSize.x;
                ctSize.x = dx;
                ctSize.y = ItemSize.y * ctScale;
            }
            else
            {
                ctSize = ItemSize;
                ctScale = 1;
            }
            int c = DataLength;
            int a = c % Column;
            c /= Column;
            if (a > 0)
                c++;
            height = c * ctSize.y;
            if (height < Size.y)
                height = Size.y;
            ActualSize = new Vector2(Size.x, height);
        }
        public override void Refresh(float x = 0, float y = 0)
        {
            m_point = y;
            Size = Model.data.sizeDelta;
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
            Calcul();
            Order(true);
        }
        /// <summary>
        /// 指定下标处的位置重排
        /// </summary>
        /// <param name="_index"></param>
        public void ShowByIndex(int _index)
        {
            Size = Model.data.sizeDelta;
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
            Calcul();
            float y = _index * ctSize.y;
            m_point = y;
            Order(true);
        }
        void Order(bool force=false)
        {
            int len = DataLength;
            float ly = ctSize.y;
            int sr = (int)(m_point /ly);//起始索引
            int er = (int)((m_point + Size.y) / ly)+1;
            sr *= Column;
            er *= Column;//结束索引
            int e = er - sr;//总计显示数据
            if (e > len)
                e = len;
            if(scrollType==ScrollType.Loop)
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
            float oy = 0;
            for (int i=0;i<e;i++)
            {
                UpdateItem(index,oy,force);
                index++;
                if (index >= len)
                {
                    index = 0;
                    oy = ActualSize.y;
                }
            }
            RecycleRemain();
        }
        void UpdateItem(int index,float oy,bool force)
        {
            float ly = ctSize.y;
            int row = index / Column;
            float dy = ly * row + oy;
            dy -= m_point;
            float ss = 0.5f * Size.y - 0.5f * ly;
            dy = ss - dy;
            float ox = (index%Column) * ctSize.x + ctSize.x * 0.5f + ItemOffset.x - Size.x * 0.5f;
            var a = PopItem(index);
            a.target.data.localPosition = new Vector3(ox, dy, 0);
            a.target.data.localScale = new Vector3(ctScale,ctScale,ctScale);
            Items.Add(a);
            if (a.index < 0 | force)
            {
                var dat = GetData(index);
                a.datacontext = dat;
                a.index = index;
                ItemUpdate(a.obj, dat, index);
            }
        }
        public void SetSize(Vector2 size)
        {
            Size = size;
            Model.data.sizeDelta = size;
            Refresh();
        }
        public static ScrollItem GetCenterItem(List<ScrollItem> items)
        {
            if (items.Count < 1)
                return null;
            float min = 100;
            ScrollItem item = items[0];
            for (int i = 1; i < items.Count; i++)
            {
                float y = items[i].target.data.localPosition.y;
                if (y < 0)
                    y = -y;
                if (y < min)
                {
                    min = y;
                    item = items[i];
                }
            }
            return item;
        }
    }
}
