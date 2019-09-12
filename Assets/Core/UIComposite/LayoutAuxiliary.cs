using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class LayoutContent
    {
        public ModelElement Head;
        public ModelElement label;
        public ModelElement close;
        public TextElement txt;
        public EventCallBack eve;
        public EventCallBack clo;
        public LayoutContent content;
        public string name;
        public ModelElement model;
        public LayoutAuxiliary auxiliary;
        DesignedDockPanel layout;
        public LayoutContent(LayoutAuxiliary aux,string nam)
        {
            auxiliary = aux;
            name = nam;
            layout = auxiliary.layoutArea.layout as DesignedDockPanel ;
            model = new ModelElement();
            model.Load(aux.content.ModData);
            model.SetParent(aux.content);
            model.IsChanged = true;
            InitialLabel();
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
        void InitialLabel()
        {
            Head = new ModelElement();
            Head.Load(auxiliary.Item.ModData);
            Head.SetParent(auxiliary.head);

            label = Head.Find("Label");
            var eve = EventCallBack.RegEvent<EventCallBack>(label);
            eve.PointerDown = HeadPointDown;
            eve.Click = HeadClick;
            eve.Drag = HeadDrag;
            eve.DragEnd = HeadDragEnd;
            eve.DataContext = this;
            var txt = label.GetComponent<TextElement>();
            txt.text = name;
            txt.UseTextSize = true;

            close = Head.Find("Close");
            if(close!=null)
            {
                clo = EventCallBack.RegEvent<EventCallBack>(close);
                clo.DataContext = this;
                clo.Click = CloseClick;
            }
        }
        int ac = 0;
        void HeadPointDown(EventCallBack eventCall, UserAction action)
        {
            ac = 0;
        }
        void HeadClick(EventCallBack eventCall, UserAction action)
        {
            auxiliary.ShowContent(this);
        }
        void HeadDrag(EventCallBack eventCall, UserAction action, Vector2 v)
        {
            if(!layout.LockLayout)
            {
                if (ac == 0)
                {
                    float y = action.CanPosition.y - eventCall.RawPosition.y;
                    if (y < -30 | y > 30)
                    {
                        layout.ShowAllDocker();
                        ac = 2;
                        //layout.DragAuxiliary = auxiliary;
                        //layout.DragContent = this;
                    }
                }
                else if (ac == 2)
                {
                    layout.Draging(action);
                }
            }
        }
        void HeadDragEnd(EventCallBack eventCall, UserAction action, Vector2 v)
        {
            if(!layout.LockLayout)
            {
                layout.HideAllDocker();
                layout.DragEnd(action);
            }
        }
        void CloseClick(EventCallBack eventCall, UserAction action)
        {
            Head.SetParent(null);
            ModelManagerUI.RecycleElement(Head);
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
            auxiliary.RemoveContent(this);
            auxiliary.panel.IsChanged = true;
        }
    }
    public class LayoutAuxiliary
    {
        /// <summary>
        /// 选项头停靠方向
        /// </summary>
        public enum HeadDock
        {
            Top,Down
        }
        public DockpanelArea layoutArea;
        public ModelElement model;
        public ModelElement content;
        public ModelElement head;
        public ModelElement Item;
        public ModelElement Cover;
        ModelElement docker;
        public Vector3 contentPos;
        public Vector2 contentSize;
        public List<LayoutContent> contents = new List<LayoutContent>();
        LayoutContent Current;
        float headHigh;
        public HeadDock headDock = HeadDock.Top;
        public LayoutElement panel;
        DesignedDockPanel layout;
        public LayoutAuxiliary(DockpanelArea area)
        {
            layoutArea = area;
            layout = area.layout as DesignedDockPanel;
            model = new ModelElement();
            model.Load(layout.Auxiliary.ModData);
            head = model.Find("Head");
            panel = head.GetComponent<LayoutElement>();
            headHigh = head.data.sizeDelta.y;
            docker = model.Find("Docker");
            docker.activeSelf = false;
            content = model.Find("Content");
            model.SetParent(area.model);
            Item = model.Find("Item");
            Item.activeSelf = false;
            Cover = model.Find("Cover");
            Cover.activeSelf = false;
            InitialDocker();
        }
        public LayoutContent AddContent(string name)
        {
            if (Current != null)
                Current.Hide();
            LayoutContent content = new LayoutContent(this,name);
            contents.Add(content);
            content.name = name;
            content.model.name = name;
            Current = content;
            UIAnimation.Manage.FrameToDo(2,SetTextSize,null);
            return content;
        }
        public void AddContent(LayoutContent con)
        {
            if (Current != null)
                Current.Hide();
            con.auxiliary = this;
            con.model.SetParent(content);
            con.Head.SetParent(head);
            con.Head.IsChanged = true;
            contents.Add(con);
            Current = con;
            panel.IsChanged = true;
            con.model.data.sizeDelta = content.data.sizeDelta;
            ModelElement.ScaleSize(con.model);
            con.model.IsChanged = true;
            con.model.activeSelf = true;
        }
        public void RemoveContent(LayoutContent con)
        {
            con.model.SetParent(null);
            con.Head.SetParent(null);
            contents.Remove(con);
            if (con==Current)
            {
                if(contents.Count==0)
                {
                    layoutArea.Dispose();
                    layout.Refresh();
                    Current = null;
                }
                else
                {
                    Current = contents[0];
                    Current.Show();
                }
            }
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
            panel.IsChanged = true;
        }
        void SetTextSize(object obj)
        {
            for(int i=0;i<contents.Count;i++)
            {
                var it = contents[i];
                float w = it.label.data.sizeDelta.x;
                float fw = w + 40;
                it.Head.data.sizeDelta.x = fw;
                if(it.close!=null)
                {
                    it.close.data.localPosition.x = w * 0.5f + 8;
                    it.close.IsChanged = true;
                }
            }
            panel.IsChanged = true;
        }
        public void Refresh()
        {
            panel.IsChanged = true;
        }
        void InitialDocker()
        {
            var mod = docker.Find("Center");
            var eve= EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = CenterPointUp;
            eve.PointerEntry = CenterPointEntry;
            eve.PointerLeave = PointLeave;

            mod = docker.Find("Left");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = PointUp;
            eve.PointerEntry = LeftPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Left;

            mod = docker.Find("Top");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = PointUp;
            eve.PointerEntry = TopPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Top;

            mod = docker.Find("Right");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = PointUp;
            eve.PointerEntry = RightPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Right;

            mod = docker.Find("Down");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = PointUp;
            eve.PointerEntry = DownPointEntry;
            eve.PointerLeave = PointLeave;
            eve.DataContext = DockpanelArea.Dock.Down;
        }
        void CenterPointEntry(EventCallBack callBack, UserAction action)
        {
            Cover.data.localPosition = Vector3.zero;
            Cover.data.sizeDelta = model.data.sizeDelta;
            Cover.activeSelf = true;
        }
        void CenterPointUp(EventCallBack callBack,UserAction action)
        {
            Cover.activeSelf = false;
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            //AddContent(layout.DragContent);
        }
        void PointLeave(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
        }
        void PointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
            //if(layout.DragAuxiliary==this)
            //{
            //    if(contents.Count<2)
            //    {
            //        return;
            //    }
            //}
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            var area = layoutArea.AddArea((DockpanelArea.Dock)callBack.DataContext);
            //area.auxiliary.AddContent(layout.DragContent);
            //area.SizeChanged();
            layout.Refresh();
        }
        void LeftPointEntry(EventCallBack callBack, UserAction action)
        {
            var size = model.data.sizeDelta;
            Cover.data.localPosition.x = size.x*-0.25f;
            Cover.data.localPosition.y = 0;
            Cover.data.sizeDelta.x = size.x*0.5f;
            Cover.data.sizeDelta.y = size.y;
            Cover.activeSelf = true;
        }
        void TopPointEntry(EventCallBack callBack, UserAction action)
        {
            var size = model.data.sizeDelta;
            Cover.data.localPosition.x = 0;
            Cover.data.localPosition.y = size.y*0.25f;
            Cover.data.sizeDelta.x = size.x ;
            Cover.data.sizeDelta.y = size.y*0.5f;
            Cover.activeSelf = true;
        }
        void RightPointEntry(EventCallBack callBack, UserAction action)
        {
            var size = model.data.sizeDelta;
            Cover.data.localPosition.x = size.x * 0.25f;
            Cover.data.localPosition.y = 0;
            Cover.data.sizeDelta.x = size.x * 0.5f;
            Cover.data.sizeDelta.y = size.y;
            Cover.activeSelf = true;
        }
        void DownPointEntry(EventCallBack callBack, UserAction action)
        {
            var size = model.data.sizeDelta;
            Cover.data.localPosition.x = 0;
            Cover.data.localPosition.y = size.y * -0.25f;
            Cover.data.sizeDelta.x = size.x;
            Cover.data.sizeDelta.y = size.y * 0.5f;
            Cover.activeSelf = true;
        }
    }
}
