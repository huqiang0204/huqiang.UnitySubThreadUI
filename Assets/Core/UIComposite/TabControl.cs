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
        public class TableContent
        {
            public EventCallBack eventCall;
            public ModelElement Label;
            public ModelElement Back;
            public ModelElement Content;
        }
        /// <summary>
        /// 选项头停靠方向
        /// </summary>
        public enum HeadDock
        {
            Top, Down
        }
        public ModelElement Main;
        public ModelElement Head;
        public ModelElement Items;
        public ModelElement Content;
        public ModelElement Item;
        public TableContent curContent;
        public HeadDock headDock = HeadDock.Top;
        float headHigh = 0;
        StackPanel panel;
        public List<TableContent> contents;
        public override void Initial(ModelElement mod)
        {
            contents = new List<TableContent>();
            Main = mod;
            Head = mod.Find("Head");
            Items = Head.Find("Items");
            if(Items!=null)
            {
                panel = new StackPanel();
                panel.direction = Direction.Horizontal;
                panel.Initial(Items);
                headHigh = Items.data.sizeDelta.y;
            }
            Item = Head.Find("Item");
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
            TableContent content = new TableContent();
            ModelElement mod = new ModelElement();
            mod.Load(Item.ModData);
            mod.SetParent(Items);
            model.SetParent(Content);

            content.Label = mod.Find("Label");
            content.Back = mod.Find("Back");
            content.Content = model;

            var txt = mod.Find("Label").GetComponent<TextElement>();
            if (txt != null)
            {
                txt.text = label;
                txt.UseTextSize = true;
                UIAnimation.Manage.FrameToDo(2, SetTextSize, null);
            }
            mod.RegEvent<EventCallBack>();
            mod.baseEvent.Click = (o, e) => {
                if (curContent != null)
                {
                    curContent.Content.activeSelf = false;
                    if (curContent.Back != null)
                        curContent.Back.activeSelf = false;
                }
                curContent = o.DataContext as TableContent;
                curContent.Content.activeSelf = true;
                if (curContent.Back != null)
                    curContent.Back.activeSelf = true;
            };
            mod.baseEvent.PointerEntry = (o, e) => {
               var c =  o.DataContext as TableContent;
                if(c!=null)
                {
                    if (c.Back != null)
                        c.Back.activeSelf = true;
                }
            };
            mod.baseEvent.PointerLeave = (o, e) => {
                var c = o.DataContext as TableContent;
                if (c != null)
                {
                    if (c != curContent)
                        if (c.Back != null)
                            c.Back.activeSelf = false;
                }
            };
            content.eventCall = mod.baseEvent;
            content.eventCall.DataContext = content;
            if (curContent != null)
            {
                curContent.Content.activeSelf = false;
                if (curContent.Back != null)
                    curContent.Back.activeSelf = false;
            }
            model.SetParent(Content);
            curContent = content;
            panel.Order();
            contents.Add(curContent);
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
        public void RemoveContent(TableContent table)
        {
            contents.Remove(table);
        }
        public void AddTable(TableContent table)
        {
            table.Label.SetParent(Head);
            table.Content.SetParent(Content);
        }
    }
}
