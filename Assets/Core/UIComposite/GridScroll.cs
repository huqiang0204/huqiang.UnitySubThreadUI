using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class GridScroll:ScrollContent
    {
        public GridScroll()
        {
        }
        ModelElement model;

        public int Column = 1;
        public int Row = 0;
        /// <summary>
        /// 当前滚动的位置
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 事件
        /// </summary>
        public EventCallBack eventCall;
        /// <summary>
        /// 滚动事件
        /// </summary>
        public Action<GridScroll, Vector2> Scroll;
        /// <summary>
        /// 滚动结束事件
        /// </summary>
        public Action<GridScroll> ScrollEnd;
        void Calcul()
        {
            Size = ScrollView.data.sizeDelta;
            if (BindingData == null)
            {
                Row = 0;
                return;
            }
            int c = DataLength;
            Row = c / Column;
            if (c % Column > 0)
                Row++;
            ActualSize = new Vector2(Column * ItemSize.x, Row * ItemSize.y);
        }
        public override void Initial(ModelElement model)
        {
            base.Initial(model);
            eventCall = EventCallBack.RegEvent<EventCallBack>(model);
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            Size = ScrollView.data.sizeDelta;
            eventCall.CutRect = true;
            eventCall.ScrollEndX = OnScrollEndX;
            eventCall.ScrollEndY = OnScrollEndY;
            model.SizeChanged = (o) => {
                Refresh(Position);
            };
        }
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if (ScrollView == null)
                return;
            if (BindingData == null)
                return;
            v.x /= eventCall.Context.data.localScale.x;
            v.y /= eventCall.Context.data.localScale.y;
            switch (scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(back, ref v, ref Position.x, ref Position.y);
                    break;
                case ScrollType.Loop:
                    v = ScrollLoop(back, ref v, ref Position.x, ref Position.y);
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(back, ref v, ref Position.x, ref Position.y);
                    break;
            }
            Order();
            if (v != Vector2.zero)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">视口尺寸</param>
        /// <param name="pos">视口位置</param>
        public void Order(bool force = false)
        {
            float w = Size.x;
            float h = Size.y;
            float left = Position.x;
            float ls = left - ItemSize.x;
            float right = Position.x + w;
            float rs = right + ItemSize.x;
            float top = Position.y + h;//与unity坐标相反
            float ts = top + ItemSize.y;
            float down = Position.y;//与unity坐标相反
            float ds = down - ItemSize.y;
            RecycleOutside(left, right, down, top);
            int colStart = (int)(left / ItemSize.x);
            if (colStart < 0)
                colStart = 0;
            int colEnd = (int)(rs / ItemSize.x);
            if (colEnd > Column)
                colEnd = Column;
            int rowStart = (int)(down / ItemSize.y);
            if (rowStart < 0)
                rowStart = 0;
            int rowEnd = (int)(ts / ItemSize.y);
            if (rowEnd > Row)
                rowEnd = Row;
            for (; rowStart < rowEnd; rowStart++)
                UpdateRow(rowStart, colStart, colEnd, force);
        }
        void RecycleOutside(float left, float right, float down, float top)
        {
            int c = Items.Count - 1;
            for (; c >= 0; c--)
            {
                var it = Items[c];
                int index = Items[c].index;
                int r = index / Column;
                float y = (r + 1) * ItemSize.y;
                if (y < down | y > top)
                {
                    Items.RemoveAt(c);
                    RecycleItem(it);
                    if (ItemRecycle != null)
                        ItemRecycle(it);
                }
                else
                {
                    int col = index % Column;
                    float x = (col + 1) * ItemSize.x;
                    if (x < left | x > right)
                    {
                        Items.RemoveAt(c);
                        RecycleItem(it);
                        if (ItemRecycle != null)
                            ItemRecycle(it);
                    }
                }
            }
        }
        void UpdateRow(int row, int colStart, int colEnd, bool force)
        {
            int index = row * Column + colStart;
            int len = colEnd - colStart;
            int cou = DataLength;
            for (int i = 0; i < len; i++)
            {
                if (index >= cou)
                    return;
                UpdateItem(index, force);
                index++;
            }
        }
        void UpdateItem(int index, bool force)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (item.index == index)
                {
                    SetItemPostion(item);
                    if (force)
                        if (ItemUpdate != null)
                            ItemUpdate(item.obj, item.datacontext, index);
                    return;
                }
            }
            var it = CreateItem();
            Items.Add(it);
            it.index = index;
            it.datacontext = GetData(index);//dataList[index];
            SetItemPostion(it);
            if (ItemUpdate != null)
                ItemUpdate(it.obj, it.datacontext, index);
        }
        void SetItemPostion(ScrollItem item)
        {
            int r = item.index / Column;
            int c = item.index % Column;
            float x = (c + 0.5f) * ItemSize.x;
            float y = (r + 0.5f) * ItemSize.y;
            x -= Position.x;
            x -= Size.x * 0.5f;
            y = Position.y - y;
            y += Size.y * 0.5f;
            item.target.data.localPosition = new Vector3(x, y, 0);
            item.target.IsChanged = true;
        }
        /// <summary>
        /// 刷新到指定位置
        /// </summary>
        /// <param name="pos"></param>
        public void Refresh(Vector2 pos)
        {
            Position = pos;
            Calcul();
            Order();
        }
        /// <summary>
        /// 刷新到默认位置
        /// </summary>
        public void Refresh()
        {
            Calcul();
            Order(true);
        }
        protected Vector2 Limit(EventCallBack callBack, Vector2 v)
        {
            Vector2 v2 = Vector2.zero;
            var size = Size;
            switch (scrollType)
            {
                case ScrollType.None:
                    float vx = Position.x + v.x;
                    if (vx < 0)
                    {
                        Position.x = 0;
                        eventCall.VelocityX = 0;
                        v.x = 0;
                    }
                    else if (vx + size.x > ActualSize.x)
                    {
                        Position.x = ActualSize.x - size.x;
                        eventCall.VelocityX = 0;
                        v.x = 0;
                    }
                    else
                    {
                        Position.x += v.x;
                        v2.x = v.x;
                    }
                    float vy = Position.y + v.y;
                    if (vy < 0)
                    {
                        Position.y = 0;
                        eventCall.VelocityY = 0;
                        v.y = 0;
                    }
                    else if (vy + size.y > ActualSize.y)
                    {
                        Position.y = ActualSize.y - size.y;
                        eventCall.VelocityY = 0;
                        v.y = 0;
                    }
                    else
                    {
                        Position.y += v.y;
                        v2.y = v.y;
                    }
                    break;
                case ScrollType.BounceBack:
                    Position += v;
                    if (!callBack.Pressed)
                    {
                        if (Position.x < 0)
                        {
                            if (v.x < 0)
                                if (eventCall.DecayRateX >= 0.99f)
                                {
                                    eventCall.DecayRateX = 0.9f;
                                    eventCall.VelocityX = eventCall.VelocityX;
                                }
                        }
                        else if (Position.x + size.x > ActualSize.x)
                        {
                            if (v.x > 0)
                            {
                                if (eventCall.DecayRateX >= 0.99f)
                                {
                                    eventCall.DecayRateX = 0.9f;
                                    eventCall.VelocityX = eventCall.VelocityX;
                                }
                            }
                        }
                        if (Position.y < 0)
                        {
                            if (v.y < 0)
                                if (eventCall.DecayRateY >= 0.99f)
                                {
                                    eventCall.DecayRateY = 0.9f;
                                    eventCall.VelocityY = eventCall.VelocityY;
                                }
                        }
                        else if (Position.y + size.y > ActualSize.y)
                        {
                            if (v.y > 0)
                            {
                                if (eventCall.DecayRateY >= 0.99f)
                                {
                                    eventCall.DecayRateY = 0.9f;
                                    eventCall.VelocityY = eventCall.VelocityY;
                                }
                            }
                        }
                    }
                    return v;
            }
            return v2;
        }
        void OnScrollEndX(EventCallBack back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (Position.x < -Tolerance)
                {
                    back.DecayRateX = 0.988f;
                    float d =  - Position.x;
                    back.ScrollDistanceX = -d * eventCall.Context.data.localScale.x;
                }
                else
                {
                    float max = ActualSize.x+Tolerance;
                    if (max < Size.x)
                        max = Size.x + Tolerance;
                    if (Position.x + Size.x >max)
                    {
                        back.DecayRateX = 0.988f;
                        float d = ActualSize.x - Position.x - Size.x;
                        back.ScrollDistanceX = -d * eventCall.Context.data.localScale.x;
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
        void OnScrollEndY(EventCallBack back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (Position.y < -Tolerance)
                {
                    back.DecayRateY = 0.988f;
                    float d = - Position.y;
                    back.ScrollDistanceY = d * eventCall.Context.data.localScale.y;
                }
                else
                {
                    float max = ActualSize.y+Tolerance;
                    if (max < Size.y)
                        max = Size.y + Tolerance;
                    if (Position.y + Size.y > max)
                    {
                        back.DecayRateY = 0.988f;
                        float d = ActualSize.y - Position.y - Size.y ;
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
    }
}
