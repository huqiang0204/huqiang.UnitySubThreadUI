using huqiang.UI;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class ScrollYPop:ScrollContent
    {
        public EventCallBack eventCall;
        protected float height;
        int wm;
        /// <summary>
        /// 滚动的当前位置，从0开始
        /// </summary>
        public float Point;

        public override void Initial(ModelElement mod)
        {
            ScrollView = mod;
            eventCall = EventCallBack.RegEvent<EventCallBack>(mod);
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            Size = ScrollView.data.sizeDelta;
            eventCall.CutRect = true;
        }
        public Action<ScrollYPop, Vector2> Scroll;
        public Action<ScrollYPop, Vector2> ScrollEnd;
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if (ScrollView == null)
                return;
            var size = Size;
            float y = Point + v.y;
            if (y < 0)
            {
                y = 0; back.VelocityY = 0;
            }
            if (y + size.y > height)
            {
                y = height - size.y;
                back.VelocityY = 0;
            }
            back.VelocityX = 0;
            Order(y);
            if (v.y != 0)
            {
                if (Scroll != null)
                    Scroll(this, v);
            }
            else
            {
                if (ScrollEnd != null)
                    ScrollEnd(this, v);
            }
        }
        public void Calcul()
        {
            float w = Size.x - ItemOffset.x;
            w /= ItemSize.x;
            wm = (int)w;
            if (wm < 1)
                wm = 1;
            int c = DataLength;
            int a = c % wm;
            c /= wm;
            if (a > 0)
                c++;
            height = c * ItemSize.y;
            if (showpop)
                height += PopHigh;
            if (height < Size.y)
                height = Size.y;
            ActualSize = new Vector2(Size.x, height);
            max_count = (int)(Size.y / ItemSize.y) * wm;
            int more = (int)(200 / ItemSize.y);
            if (more < 2)
                more = 2;
            max_count += more * wm;
        }
        public override void Refresh(float x = 0, float y = 0)
        {
            ActualSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                    Items[i].target.activeSelf = false;
#if DEBUG
                Debug.Log("没有绑定的数据");
#endif
                return;
            }
            if (ItemMod == null)
            {
#if DEBUG
                Debug.Log("没有绑定数据模型");
#endif
                return;
            }
            if (ItemSize.y == 0)
            {
#if DEBUG
                Debug.Log("模型的尺寸不正确");
#endif
                return;
            }
            Calcul();
            Initialtems();
            var pos = Vector2.zero;
            pos.y = height * -0.5f - y;
            Order(y, true);
        }
        public override void Order(float os, bool force = false)
        {
            int len = Items.Count;
            if (len == 0)
                return;
            float h = ItemSize.y;
            float offset = os - 2 * h;//起始偏移地址
            if (offset < 0)
                offset = 0;
            int index = (int)(offset / h);//起始数据索引
            index *= wm;
            int count = DataLength;
            for (; index < count; index++)
            {
                if (UpdateItem(os, index, force))
                    break;
            }
            Point = os;
            if (showpop)
            {
                int row = PopIndex / wm;
                float ih = ItemSize.y;
                float dy = row * ih + 0.5f * PopHigh;
                dy = Point - dy + 0.5f * Size.y;
                PopWindow.data.localPosition = new Vector3(0, dy, 0);
            }
        }
        bool UpdateItem(float dy, int index, bool force)
        {
            int row = index / wm;
            int col = index % wm;
            float ih = ItemSize.y;
            float ay = row * ih;
            float os = ay - dy;
            if (showpop)
                if (index >= PopIndex)
                    os += PopHigh;
            float oe = os + ih * 2f;
            if (oe < 0)
                return false;
            if (os > Size.y + 0.5f * ih)
                return true;
            os = Size.y * 0.5f - os + ItemOffset.y - ih * 0.5f;
            float ox = col * ItemSize.x + ItemSize.x * 0.5f + ItemOffset.x - Size.x * 0.5f;
            int c = index % Items.Count;
            var a = Items[c];
            var dat = GetData(index);
            a.target.data.localPosition = new Vector3(ox, os, 0);
            a.target.activeSelf = true;
            if (force | a.index != index)
            {
                if (ItemUpdate != null)
                {
                    if (a.obj == null)
                        ItemUpdate(a.target, dat, index);
                    else ItemUpdate(a.obj, dat, index);
                }
                a.datacontext = dat;
            }
            return false;
        }
        public void ShowPop()
        {
            showpop = true;
            PopWindow.activeSelf = true;
            PopWindow.SetSiblingIndex(10000);
            Refresh(0, Point);
        }
        public void HidePop()
        {
            showpop = false;
            PopWindow.activeSelf = false;
            Refresh(0, Point);
        }
        bool showpop = false;
        public void SetPopWindow(ModelElement obj)
        {
            PopWindow = obj;
            PopWindow.SetParent(ScrollView);
        }
        ModelElement PopWindow;
        public float PopHigh;
        public int PopIndex;
    }
}
