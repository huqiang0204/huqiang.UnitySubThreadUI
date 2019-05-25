using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class TreeViewNode
    {
        public bool extand;
        public string content;
        public Vector2 offset;
        public List<TreeViewNode> child = new List<TreeViewNode>();
    }
    public class TreeViewItem
    {
        public ModelElement target;
        public TextElement text;
        public EventCallBack callBack;
        public TreeViewNode node;
    }
    public class TreeView : ModelInital
    {
        public ModelElement View;
        public Vector2 Size;//scrollView的尺寸
        Vector2 aSize;
        public Vector2 ItemSize;
        ModelElement model;
        public TreeViewNode nodes;
        public float ItemHigh = 16;
        public EventCallBack eventCall;//scrollY自己的按钮
        public ModelElement ItemMod;
        float m_point;
        public SwapBuffer<TreeViewItem, TreeViewNode> swap;
        QueueBuffer<TreeViewItem> queue;
        public TreeView()
        {
            swap = new SwapBuffer<TreeViewItem, TreeViewNode>(512);
            queue = new QueueBuffer<TreeViewItem>(256);
        }
        public override void Initial(ModelElement mod)
        {
            View = mod;
            eventCall = EventCallBack.RegEvent<EventCallBack>(mod);
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            Size = View.data.sizeDelta;
            eventCall.CutRect = true;
            if (mod != null)
            {
                ItemMod = mod.Find("Item");
                if (ItemMod != null)
                {
                    ItemSize = ItemMod.data.sizeDelta;
                    ItemHigh = ItemSize.y;
                }
            }
        }
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
            if (View == null)
                return;
            v.y /= eventCall.Context.data.localScale.y;
            Limit(back, v.y);
            Refresh();
        }
        void OnScrollEnd(EventCallBack back)
        {

        }
        float hy;
        public void Refresh()
        {
            if (nodes == null)
                return;
            hy = Size.y * 0.5f;
            aSize.y = CalculHigh(nodes, 0, 0);
            RecycleItem();
        }
        protected void RecycleItem()
        {
            int len = swap.Length;
            for (int i = 0; i < len; i++)
            {
                var it = swap.Pop();
                it.target.activeSelf = false;
                queue.Enqueue(it);
            }
            swap.Done();
        }

        float CalculHigh(TreeViewNode node, int level, float high)
        {
            node.offset.x = level * ItemHigh;
            node.offset.y = high;
            UpdateItem(node);
            level++;
            high += ItemHigh;
            if (node.extand)
                for (int i = 0; i < node.child.Count; i++)
                    high = CalculHigh(node.child[i], level, high);
            return high;
        }
        void UpdateItem(TreeViewNode node)
        {
            float dy = node.offset.y - m_point;
            if (dy <= Size.y)
                if (dy + ItemHigh > 0)
                {
                    var item = swap.Exchange((o, e) => { return o.node == e; }, node);
                    if (item == null)
                    {
                        item = CreateItem();
                        swap.Push(item);
                        item.node = node;
                        if (item.text != null)
                        {
                            if (node.child.Count > 0)
                                item.text.text = "▷ " + node.content;
                            else item.text.text = node.content;
                        }

                    }
                    var m = item.callBack.Context;
                    m.data.localPosition = new Vector3(node.offset.x, hy - dy - ItemHigh * 0.5f, 0);
                    m.IsChanged = true;
                }
        }
        protected TreeViewItem CreateItem()
        {
            TreeViewItem it = queue.Dequeue();
            if (it != null)
            {
                it.target.activeSelf = true;
                return it;
            }
            ModelElement mod = new ModelElement();
            mod.Load(ItemMod.ModData);
            mod.SetParent(View);
            mod.data.localPosition = new Vector3(10000, 10000);
            mod.data.localScale = Vector3.one;
            mod.IsChanged = true;
            TreeViewItem a = new TreeViewItem();
            a.target = mod;
            a.text = mod.GetComponent<TextElement>();
            a.callBack = EventCallBack.RegEvent<EventCallBack>(mod);
            a.callBack.Click = (o, e) => {
                var item = o.DataContext as TreeViewItem;
                if (item.node != null)
                {
                    item.node.extand = !item.node.extand;
                    Refresh();
                }
            };
            a.callBack.DataContext = a;
            return a;
        }
        protected void Limit(EventCallBack callBack, float y)
        {
            var size = Size;
            if (size.y > aSize.y)
            {
                m_point = 0;
                return;
            }
            if (y == 0)
                return;
            float vy = m_point + y;
            if (vy < 0)
            {
                m_point = 0;
                eventCall.VelocityY = 0;
                return;
            }
            else if (vy + size.y > aSize.y)
            {
                m_point = aSize.y - size.y;
                eventCall.VelocityY = 0;
                return;
            }
            m_point += y;
        }
    }
}
