﻿using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class LayoutContent
    {
        public string name;
        public ModelElement model;
        public LayoutAuxiliary auxiliary;
        public LayoutContent(LayoutAuxiliary aux)
        {
            auxiliary = aux;
            model = new ModelElement();
            model.Load(aux.content.ModData);
            model.SetParent(aux.content);
            model.IsChanged = true;
        }
        public void Hide()
        {
            model.activeSelf = false;
        }
        public void Show()
        {
            model.activeSelf = true;
        }
        public void Dock(LayoutAuxiliary aux)
        {
            auxiliary = aux;
            model.SetParent(aux.content);
            model.data.localPosition = aux.contentPos;
            model.data.sizeDelta = aux.contentSize;
            model.IsChanged = true;
            ModelElement.ScaleSize(model);
        }
        public void Close()
        {
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
            auxiliary.contents.Remove(this);
        }
        public PopWindow window;
        public void LoadPopWindow<T>()where T:PopWindow,new()
        {
            if (window != null)
                window.Dispose();
            var t = new T();
            t.Initial(model,null);
            t.model.data.sizeDelta = model.data.sizeDelta;
            t.model.IsChanged = true;
            t.ReSize();
            window = t;
        }
    }
    public class AuxiliaryHead
    {
        public string name;
        public ModelElement model;
        public LayoutContent context;
        TextElement label;
        public ModelElement head;
        ModelElement close;
        LayoutAuxiliary auxiliary;
        public AuxiliaryHead(LayoutAuxiliary aux, ModelElement mod)
        {
            auxiliary = aux;
            model = new ModelElement();
            model.Load(mod.ModData);
            model.SetParent(aux.head);
            head= model.Find("Lable");
            label = head.GetComponent<TextElement>();
            head.RegEvent<EventCallBack>();
            var eve = head.baseEvent;
            eve.Click = HeadClick;
            eve.Drag = HeadDrag;
            eve.DragEnd = HeadDragEnd;
            close = head.Find("Close");
            close.RegEvent<EventCallBack>();
            close.baseEvent.Click = CloseClick;
        }
        void HeadClick(EventCallBack eventCall,UserAction action)
        {
            auxiliary.ShowContent(context);
        }
        void HeadDrag(EventCallBack eventCall,UserAction action,Vector2 v)
        {

        }
        void HeadDragEnd(EventCallBack eventCall,UserAction action,Vector2 v)
        {

        }
        void CloseClick(EventCallBack eventCall, UserAction action)
        {
            if(context!=null)
                context.Close();
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
        }
        /// <summary>
        /// 断开关联
        /// </summary>
        public void BreakContext()
        {
            context = null;
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
        }
        public void BindingContext(LayoutContent content)
        {
            context = content;
            label.text = content.name;
        }
    }
    public class LayoutAuxiliary
    {
        class HeadView
        {
            public EventCallBack Lable;
            public EventCallBack Close;
        }
        /// <summary>
        /// 选项头停靠方向
        /// </summary>
        public enum HeadDock
        {
            Top,Down
        }
        public LayoutArea layoutArea;
        public ModelElement model;
        public ModelElement content;
        public ModelElement head;
        ModelElement docker;
        public Vector3 contentPos;
        public Vector2 contentSize;
        public List<LayoutContent> contents = new List<LayoutContent>();
        public List<AuxiliaryHead> heads = new List<AuxiliaryHead>();
        LayoutContent Current;
        float headHigh;
        public HeadDock headDock = HeadDock.Top;
        ScrollX headScroll;
        public LayoutAuxiliary(LayoutArea area)
        {
            layoutArea = area;
            model = new ModelElement();
            model.Load(area.layout.Auxiliary.ModData);
            head = model.Find("Head");
            headHigh = head.data.sizeDelta.y;
            docker = model.Find("Docker");
            docker.activeSelf = false;
            content = model.Find("Content");
            model.SetParent(area.model);
            headScroll = new ScrollX();
            headScroll.Initial(head);
            headScroll.scrollType = ScrollType.None;
            headScroll.DynamicSize = false;
            headScroll.BindingData = contents;
            headScroll.ItemObject = typeof(HeadView);
            headScroll.ItemUpdate = ItemUpdate;
        }
        public LayoutContent AddContent(string name)
        {
            if (Current != null)
                Current.Hide();
            LayoutContent content = new LayoutContent(this);
            contents.Add(content);
            content.name = name;
            content.model.name = name;
            Current = content;
            return content;
        }
        public void RemoveContent(LayoutContent content)
        {
            contents.Remove(content);
        }
        public void ShowContent(LayoutContent con)
        {
            if (Current != null)
                Current.model.activeSelf = false;
            Current = con;
            if (Current != null)
                Current.model.activeSelf = true;
        }
        public void ShowDocker()
        {
            docker.activeSelf = true;
        }
        public void HideDocker()
        {
            docker.activeSelf = false;
        }
        public void SizeChanged()
        {
            var size = model.data.sizeDelta = layoutArea.model.data.sizeDelta;
            model.IsChanged = true;
            float y = size.y;
            float cy = y - headHigh;
            float hy = 0.5f * headHigh;
            if(headDock==HeadDock.Top)
            {
                content.data.sizeDelta = new Vector2(size.x,cy);
                content.data.localPosition = new Vector3(0,-hy);
                head.data.sizeDelta = new Vector2(size.x,headHigh);
                head.data.localPosition = new Vector3(0,0.5f*cy);
            }
            else
            {
                content.data.sizeDelta = new Vector2(size.x, cy);
                content.data.localPosition = new Vector3(0, hy);
                head.data.sizeDelta = new Vector2(size.x, headHigh);
                head.data.localPosition = new Vector3(0, -0.5f * cy);
            }
            content.IsChanged = true;
            head.IsChanged = true;
            for(int i=0;i<contents.Count;i++)
            {
                var mod = contents[i].model;
                mod.data.sizeDelta = content.data.sizeDelta;
                mod.IsChanged = true;
            }
            headScroll.Refresh(headScroll.Point, 0);
        }
        void ItemUpdate(object obj,object dat,int index)
        {
            var v = obj as HeadView;
            var c = dat as LayoutContent;
            v.Lable.DataContext = c;
            v.Lable.Context.GetComponent<TextElement>().text = c.name;
            v.Close.DataContext = c;
            v.Lable.PointerDown = HeadPointDown;
            v.Lable.Click = HeadClick;
            v.Lable.Drag = HeadDrag;
            v.Lable.DragEnd = HeadDragEnd;
            v.Close.Click = CloseClick;
        }
        int ac = 0;
        void HeadPointDown(EventCallBack eventCall, UserAction action)
        {
            ac = 0;
        }
        void HeadClick(EventCallBack eventCall, UserAction action)
        {
            ShowContent(eventCall.DataContext as LayoutContent);
        }
        void HeadDrag(EventCallBack eventCall, UserAction action, Vector2 v)
        {
            if(ac==0)
            {
                float x = action.CanPosition.x - eventCall.RawPosition.x;
                if (x < -30 | x > 30)
                    ac = 1;
                else
                {
                    float y = action.CanPosition.y - eventCall.RawPosition.y;
                    if (y < -30 | y > 30)
                    {
                        headScroll.eventCall.RemoveFocus();
                        layoutArea.layout.ShowAllDocker();
                        ac = 2;
                    }
                }
            }
        }
        void HeadDragEnd(EventCallBack eventCall, UserAction action, Vector2 v)
        {
            layoutArea.layout.HideAllDocker();
        }
        void CloseClick(EventCallBack eventCall, UserAction action)
        {
            var context = eventCall.DataContext as LayoutContent;
            if (context != null)
                context.Close();
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
        }
        public void Refresh()
        {
            headScroll.Refresh(headScroll.Point, 0);
        }
    }
}
