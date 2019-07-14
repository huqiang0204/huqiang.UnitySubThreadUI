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
        public override void Initial(ModelElement mod)
        {
            base.Initial(mod);
            Auxiliary = mod.Find("Auxiliary");
            Auxiliary.activeSelf = false;
            Drag = mod.Find("Drag");
            Drag.activeSelf = false;
        }
        public void ShowAllDocker()
        {
            //for (int i = 0; i < areas.Count; i++)
            //    areas[i].ShowAuxiliaryDocker();
        }
        public void HideAllDocker()
        {
            //for (int i = 0; i < areas.Count; i++)
            //    areas[i].HideAuxiliaryDocker();
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
        public LayoutContent DragContent;
        public LayoutAuxiliary DragAuxiliary;
    }
    public class DesignedDockContent
    {
        public ModelElement model;
        public ModelElement docker;
        public ModelElement tab;
        public TabControl control;
        public ModelElement Cover;
        DesignedDockPanel layout;
        public void Initial(ModelElement mod)
        {
            model = mod;
            docker = model.Find("Docker");
            tab = model.Find("Tab");
            Cover = model.Find("Cover");
            control = new TabControl();
            control.Initial(tab);
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
            //if (layout.DragAuxiliary == this)
            //{
            //    if (contents.Count < 2)
            //    {
            //        return;
            //    }
            //}
            layout.DragAuxiliary.RemoveContent(layout.DragContent);
            //var area = layoutArea.AddArea((DockpanelArea.Dock)callBack.DataContext);
            //area.auxiliary.AddContent(layout.DragContent);
            //area.SizeChanged();
            layout.Refresh();
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

        public void AddContent( ModelElement model,string name)
        {
            control.AddContent(model,name);
        }
        public void RemoveContent(TabControl.TableContent con)
        {
            control.RemoveContent(con);
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
    }
}
