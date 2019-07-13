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
    public class TabControl:ModelInital
    {
        /// <summary>
        /// 选项头停靠方向
        /// </summary>
        public enum HeadDock
        {
            Top, Down
        }
        public ModelElement Main;
        public ModelElement Head;
        public ModelElement Content;
        public ModelElement Item;
        public ModelElement curContent;
        public HeadDock headDock = HeadDock.Top;
        float headHigh = 0;
        StackPanel panel;
        public override void Initial(ModelElement mod)
        {
            Main = mod;
            Head = mod.Find("Head");
            if(Head!=null)
            {
                panel = new StackPanel();
                panel.direction = Direction.Horizontal;
                panel.Initial(Head);
                headHigh = Head.data.sizeDelta.y;
            }
            Item = mod.Find("Item");
            if (Item != null)
                Item.activeSelf = false;
            Content = mod.Find("Content");
            mod.SizeChanged = SizeChanged;
        }
        void SizeChanged(ModelElement model)
        {
            var size = model.data.sizeDelta;
            float y = size.y;
            float cy = y - headHigh;
            float hy = 0.5f * headHigh;
            if (headDock == HeadDock.Top)
            {
                Content.data.sizeDelta = new Vector2(size.x, cy);
                Content.data.localPosition = new Vector3(0, -hy);
                Head.data.sizeDelta = new Vector2(size.x, headHigh);
                Head.data.localPosition = new Vector3(0, 0.5f * cy);
            }
            else
            {
                Content.data.sizeDelta = new Vector2(size.x, cy);
                Content.data.localPosition = new Vector3(0, hy);
                Head.data.sizeDelta = new Vector2(size.x, headHigh);
                Head.data.localPosition = new Vector3(0, -0.5f * cy);
            }
            Content.IsChanged = true;
            Head.IsChanged = true;
            panel.Order();
        }
        public void AddContent(ModelElement model, string label)
        {
            if (Item == null)
                return;
            ModelElement mod = new ModelElement();
            mod.Load(Item.ModData);
            mod.SetParent(Head);
            var txt= mod.Find("Label").GetComponent<TextElement>();
            if (txt != null)
            {
                txt.text = label;
                UIAnimation.Manage.FrameToDo(2, SetTextSize, null);
            }
            mod.RegEvent<EventCallBack>();
            mod.baseEvent.Click = (o, e) => {
                if (curContent != null)
                    curContent.activeSelf = false;
                int index = Head.child.IndexOf(o.Context);
                curContent = Content.child[index];
                curContent.activeSelf = true;
            };
            if (curContent != null)
                curContent.activeSelf = false;
            model.SetParent(Content);
            curContent = model;
        }
        public void AddContent<T>(ModelElement content, T dat,Action<ModelElement, T>callback)
        {
            if (Item == null)
                return;
            ModelElement mod = new ModelElement();
            mod.Load(Item.ModData);
            mod.SetParent(Head);
            panel.Order();
            if (callback != null)
                callback(mod,dat);
            UIAnimation.Manage.FrameToDo(2, SetTextSize, null);
        }
        public void AddContent(ModelElement content, ModelElement mod)
        {
            if (Head == null)
                return;
            mod.SetParent(Head);
            panel.Order();
        }
        void SetTextSize(object obj)
        {
            panel.Order();
        }
    }
}
