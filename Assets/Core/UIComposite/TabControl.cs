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
            public ModelElement Item;
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
        /// <summary>
        /// 头部停靠位置
        /// </summary>
        public HeadDock headDock = HeadDock.Top;
        float headHigh = 0;
        StackPanel panel;
        public List<TableContent> contents;
        /// <summary>
        /// 当前被选中项的背景色
        /// </summary>
        public Color SelectColor = 0x2656FFff.ToColor();
        /// <summary>
        /// 鼠标停靠时的背景色
        /// </summary>
        public Color HoverColor = 0x5379FFff.ToColor();
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
        /// <summary>
        /// 使用默认标签页
        /// </summary>
        /// <param name="model"></param>
        /// <param name="label"></param>
        public void AddContent(ModelElement model, string label)
        {
            if (Item == null)
                return;
            TableContent content = new TableContent();
            ModelElement mod = new ModelElement();
            mod.Load(Item.ModData);
            mod.SetParent(Items);
            model.SetParent(Content);

            content.Item = mod;
            content.Label = mod.Find("Label");
            content.Back = mod.Find("Back");
            content.Content = model;

            var txt = mod.Find("Label").GetComponent<TextElement>();
            if (txt != null)
            {
                txt.text = label;
                txt.UseTextSize = true;
                UIAnimation.Manage.FrameToDo(2, OrderHeadLabel, null);
            }
            mod.RegEvent<EventCallBack>();
            mod.baseEvent.Click = (o, e) => {
                ShowContent(o.DataContext as TableContent);
            };
            mod.baseEvent.PointerEntry = (o, e) => {
               var c =  o.DataContext as TableContent;
                if (c == curContent)
                    return;
                if(c!=null)
                {
                    if (c.Back != null)
                    {
                        c.Back.GetComponent<ImageElement>().color = HoverColor;
                        c.Back.activeSelf = true;
                    }
                }
            };
            mod.baseEvent.PointerLeave = (o, e) => {
                var c = o.DataContext as TableContent;
                if (c == curContent)
                    return;
                if (c != null)
                {
                    if (c != curContent)
                        if (c.Back != null)
                        {
                            c.Back.activeSelf = false;
                        }
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
        /// <summary>
        /// 使用自定义标签页,标签模型自行管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="dat"></param>
        /// <param name="callback"></param>
        public void AddContent<T>(ModelElement content, T dat,Func<ModelElement,T, TableContent> callback)
        {
            if (Item == null)
                return;
            ModelElement mod = new ModelElement();
            mod.Load(Item.ModData);
            mod.SetParent(Head);
            panel.Order();
            if (callback != null)
            { 
               contents.Add(callback(mod, dat));
            }
            else
            {
                TableContent table = new TableContent();
                table.Item = mod;
                table.Content = content;
                contents.Add(table);
            }
        }
        /// <summary>
        /// 添加外部标签页
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mod"></param>
        public void AddContent(ModelElement content, ModelElement mod)
        {
            if (Head == null)
                return;
            mod.SetParent(Head);
            panel.Order();
        }
        /// <summary>
        /// 标签页排列
        /// </summary>
        /// <param name="obj"></param>
        public void OrderHeadLabel(object obj)
        {
            panel.Order();
        }
        /// <summary>
        /// 移除某个标签和其内容
        /// </summary>
        /// <param name="table"></param>
        public void RemoveContent(TableContent table)
        {
            contents.Remove(table);
        }
        /// <summary>
        /// 释放某个标签和其内容,其对象会被回收
        /// </summary>
        /// <param name="table"></param>
        public void ReleseContent(TableContent table)
        {
            contents.Remove(table);
            table.Content.SetParent(null);
            table.Item.SetParent(null);
            ModelManagerUI.RecycleElement(table.Content);
            ModelManagerUI.RecycleElement(table.Item);
        }
        public void AddTable(TableContent table)
        {
            table.Label.SetParent(Head);
            table.Content.SetParent(Content);
        }
        public void ShowContent(TableContent content)
        {
            if (curContent != null)
            {
                curContent.Content.activeSelf = false;
                curContent.Back.activeSelf = false;
            }
            curContent = content;
            curContent.Content.activeSelf = true;
            if (curContent.Back != null)
            {
                curContent.Back.GetComponent<ImageElement>().color = SelectColor;
                curContent.Back.activeSelf = true;
            }
        }
    }
}