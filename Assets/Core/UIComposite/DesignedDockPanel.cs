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
    public class DesignedDockPanel:DockPanel
    {
        public ModelElement Auxiliary;
        public ModelElement Drag;
        public List<DesignedDockContent> contents=new List<DesignedDockContent>();
        public override void Initial(ModelElement mod)
        {
            contents.Clear();
            base.Initial(mod);
            Auxiliary = mod.Find("Auxiliary");
            Auxiliary.activeSelf = false;
            Drag = mod.Find("Drag");
            Drag.activeSelf = false;
            ModelElement au = new ModelElement();
            au.Load(Auxiliary.ModData);
            MainContent = new DesignedDockContent(this);
            MainContent.Initial(MainArea, au);
            contents.Add(MainContent);
        }
        public void ShowAllDocker()
        {
            for (int i = 0; i < contents.Count; i++)
                contents[i].ShowDocker();
        }
        public void HideAllDocker()
        {
            for (int i = 0; i < contents.Count; i++)
                contents[i].HideDocker();
            Drag.activeSelf = false;
        }
        public void Draging(UserAction action)
        {
            Drag.data.localPosition = Drag.parent.ScreenToLocal(action.CanPosition);
            Drag.activeSelf = true;
            Drag.IsChanged = true;
        }
        public void DragEnd(UserAction action)
        {
            Drag.activeSelf = false;
        }
        public DesignedDockContent.ItemContent DragContent;
        public DesignedDockContent DragAuxiliary;
        public DesignedDockContent MainContent;
    }
    public class DesignedDockContent
    {
        public class ItemContent : TabControl.TableContent
        {
            public ModelElement Close;
            public PopWindow window;
            public void LoadPopWindow<T>() where T : PopWindow, new()
            {
                if (window != null)
                    window.Dispose();
                var t = new T();
                t.Initial(Content, null);
                t.model.data.sizeDelta = Content.data.sizeDelta;
                t.model.IsChanged = true;
                t.ReSize();
                window = t;
            }
        }
        public DockpanelArea dockArea;
        public ModelElement model;
        public ModelElement docker;
        public ModelElement tab;
        public TabControl control;
        public ModelElement Cover;
        DesignedDockPanel layout;
        public DesignedDockContent(DesignedDockPanel panel)
        {
            layout = panel;
        }
        public void Initial(DockpanelArea area, ModelElement mod)
        {
            dockArea = area;
            model = mod;
            docker = model.Find("Docker");
            tab = model.Find("TabControl");
            Cover = model.Find("Cover");
            control = new TabControl();
            control.Initial(tab);
            mod.SetParent(area.model);
            Cover.activeSelf = false;
            docker.activeSelf = false;
            InitialDocker();
        }
        public void SetParent(DockpanelArea area)
        {
            dockArea = area;
            model.SetParent(area.model);
        }
        void InitialDocker()
        {
            var mod = docker.Find("Center");
            var eve = EventCallBack.RegEvent<EventCallBack>(mod);
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
        void CenterPointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
            if (control.ExistContent(layout.DragContent))
                return;
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            AddContent(layout.DragContent);
            layout.HideAllDocker();
            layout.Refresh();
            ModelElement.ScaleSize(layout.DragContent.Content);
        }
        void PointLeave(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
        }
        void PointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
            if (layout.DragAuxiliary == this)
            {
                if (control.contents.Count < 2)
                {
                    return;
                }
            }
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            var area = AddArea((DockpanelArea.Dock)callBack.DataContext);
            area.AddContent(layout.DragContent);
            area.dockArea.SizeChanged();
            layout.HideAllDocker();
            layout.Refresh();
            ModelElement.ScaleSize(layout.DragContent.Content);
        }
        void LeftPointEntry(EventCallBack callBack, UserAction action)
        {
            var size = model.data.sizeDelta;
            Cover.data.localPosition.x = size.x * -0.25f;
            Cover.data.localPosition.y = 0;
            Cover.data.sizeDelta.x = size.x * 0.5f;
            Cover.data.sizeDelta.y = size.y;
            Cover.activeSelf = true;
        }
        void TopPointEntry(EventCallBack callBack, UserAction action)
        {
            var size = model.data.sizeDelta;
            Cover.data.localPosition.x = 0;
            Cover.data.localPosition.y = size.y * 0.25f;
            Cover.data.sizeDelta.x = size.x;
            Cover.data.sizeDelta.y = size.y * 0.5f;
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
        int ac;
        void HeadPointDown(EventCallBack eventCall, UserAction action)
        {
            ac = 0;
        }
        void HeadDrag(EventCallBack eventCall, UserAction action, Vector2 v)
        {
            if (!layout.LockLayout)
            {
                if (ac == 0)
                {
                    float y = action.CanPosition.y - eventCall.RawPosition.y;
                    if (y < -30 | y > 30)
                    {
                        layout.ShowAllDocker();
                        ac = 2;
                        layout.DragAuxiliary = this;
                        layout.DragContent = eventCall.DataContext as ItemContent;
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
            if (!layout.LockLayout)
            {
                layout.HideAllDocker();
                layout.DragEnd(action);
            }
        }
        void CloseClick(EventCallBack eventCall, UserAction action)
        {
            ItemContent con = eventCall.DataContext as ItemContent;
            if(con!=null)
            {
                if (con.window != null)
                    con.window.Dispose();
                control.ReleseContent(con);
                if (control.contents.Count == 0)
                {
                    if (layout.contents.Count > 1)
                        Dispose();
                    else layout.MainContent = this;
                }
            }
        }
        public ItemContent AddContent(string name)
        {
            ModelElement item = new ModelElement();
            item.Load(control.Item.ModData);
            ItemContent con = new ItemContent();
            con.Parent = control;
            con.Item = item;
            item.RegEvent<EventCallBack>();
            item.baseEvent.PointerDown = HeadPointDown;
            item.baseEvent.Drag = HeadDrag;
            item.baseEvent.DragEnd = HeadDragEnd;
            item.baseEvent.DataContext = con;

            var t =  ModelElement.CreateNew(name);
            t.data.SizeScale = true;
            t.data.marginType = MarginType.Margin;
            con.Content = t;
            con.Back = item.Find("Back");

            con.Label = item.Find("Label");
            var txt = con.Label.GetComponent<TextElement>();
            txt.text = name;
            txt.UseTextSize = true;
            con.Close = item.Find("Close");
            if (con.Close != null)
            {
                con.Close.RegEvent<EventCallBack>();
                con.Close.baseEvent.Click = CloseClick;
                con.Close.baseEvent.DataContext = con;
            }
            control.AddContent(con);
            UIAnimation.Manage.FrameToDo(2, OrderHeadLabel, con);
            return con;
        }
        /// <summary>
        /// 标签页排列
        /// </summary>
        /// <param name="obj"></param>
        public void OrderHeadLabel(object obj)
        {
            var ic = obj as ItemContent;
            var w  = ic.Label.data.sizeDelta.x ;
            ic.Item.data.sizeDelta.x = w + 48;
            ic.Back.data.sizeDelta.x = w + 48;
            ic.Close.data.localPosition.x = w * 0.5f;
            ic.Item.IsChanged = true;
            ic.Back.IsChanged = true;
            ic.Close.IsChanged = true;
            if (control.panel != null)
                control.panel.IsChanged = true;
        }
        public void AddContent(ItemContent con)
        {
            var eve = con.Item.baseEvent;
            eve.PointerDown = HeadPointDown;
            eve.Drag = HeadDrag;
            eve.DragEnd = HeadDragEnd;
            control.AddContent(con);
        }
        public void RemoveContent(TabControl.TableContent con)
        {
            control.RemoveContent(con);
            con.Item.SetParent(null);
            con.Content.SetParent(null);
            if(control.contents.Count==0)
            {
                dockArea.Dispose();
                layout.contents.Remove(this);
                layout.Refresh();
            }
        }
        public void ShowContent(TabControl.TableContent con)
        {
            control.ShowContent(con);
        }
        public void ShowDocker()
        {
            docker.activeSelf = true;
        }
        public void HideDocker()
        {
            docker.activeSelf = false;
        }
        public DesignedDockContent AddArea(DockpanelArea.Dock dock, float r = 0.5f)
        {
            var area = dockArea.AddArea(dock,r);
            ModelElement au = new ModelElement();
            au.Load(layout.Auxiliary.ModData);
            var con = new DesignedDockContent(layout);
            con.Initial(area, au);
            layout.contents.Add(con);
            return con;
        }
        public void Dispose()
        {
            dockArea.Dispose();
            layout.contents.Remove(this);
            layout.Refresh();
        }
    }
}
