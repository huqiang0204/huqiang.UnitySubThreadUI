using huqiang.UI;
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
    public class LayoutAuxiliary
    {
        class HeadView
        {
            public EventCallBack Lable;
            public EventCallBack Close;
        }
        class HeadInfo
        {
            public ModelElement Head;
            public ModelElement Lable;
            public ModelElement Close;
            public TextElement txt;
            public EventCallBack eve;
            public EventCallBack clo;
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
        public ModelElement Item;
        public ModelElement Cover;
        ModelElement docker;
        public Vector3 contentPos;
        public Vector2 contentSize;
        public List<LayoutContent> contents = new List<LayoutContent>();
        List<HeadInfo> Items = new List<HeadInfo>();
        LayoutContent Current;
        float headHigh;
        public HeadDock headDock = HeadDock.Top;
        StackPanel headScroll;
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
            Item = model.Find("Item");
            Item.activeSelf = false;
            Cover = model.Find("Cover");
            Cover.activeSelf = false;
            headScroll =new StackPanel();
            headScroll.direction = Direction.Horizontal;
            headScroll.Initial(head);
            InitialDocker();
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
            InitialItem(name,content);
            UIAnimation.Manage.FrameToDo(2,SetTextSize,null);
            return content;
        }
        void InitialItem(string name,LayoutContent context)
        {
            ModelElement mod = new ModelElement();
            mod.Load(Item.ModData);
            mod.SetParent(head);

            var lable = mod.Find("Lable");
            var eve = EventCallBack.RegEvent<EventCallBack>(lable);
            eve.PointerDown = HeadPointDown;
            eve.Click = HeadClick;
            eve.Drag = HeadDrag;
            eve.DragEnd = HeadDragEnd;
            eve.DataContext = context;
            var txt = lable.GetComponent<TextElement>();
            txt.text = name;
            txt.UseTextSize = true;

            var close = mod.Find("Close");
            var clo = EventCallBack.RegEvent<EventCallBack>(close);
            clo.DataContext = context;
            clo.Click = CloseClick;
            HeadInfo info= new HeadInfo();
            info.Head = mod;
            info.Lable = lable;
            info.Close = close;
            info.txt = txt;
            info.eve = eve;
            info.clo = clo;
            Items.Add(info);
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
            headScroll.Order();
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
                        layoutArea.layout.ShowAllDocker();
                        ac = 2;
                    }
                }
            }else if(ac==2)
            {
                layoutArea.layout.Draging(action);
            }
        }
        void HeadDragEnd(EventCallBack eventCall, UserAction action, Vector2 v)
        {
            layoutArea.layout.HideAllDocker();
            layoutArea.layout.DragEnd(action);
        }
        void CloseClick(EventCallBack eventCall, UserAction action)
        {
            var context = eventCall.DataContext as LayoutContent;
            if (context != null)
                context.Close();
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
        }
        void SetTextSize(object obj)
        {
            for(int i=0;i<Items.Count;i++)
            {
                var it = Items[i];
                float w = it.Lable.data.sizeDelta.x;
                float fw = w + 40;
                it.Head.data.sizeDelta.x = fw;
                it.Close.data.localPosition.x = w * 0.5f+8;
                it.Close.IsChanged = true;
            }
            headScroll.Order();
        }
        public void Refresh()
        {
            headScroll.Order();
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
            eve.PointerUp = LeftPointUp;
            eve.PointerEntry = LeftPointEntry;
            eve.PointerLeave = PointLeave;

            mod = docker.Find("Top");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = TopPointUp;
            eve.PointerEntry = TopPointEntry;
            eve.PointerLeave = PointLeave;

            mod = docker.Find("Right");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = RightPointUp;
            eve.PointerEntry = RightPointEntry;
            eve.PointerLeave = PointLeave;

            mod = docker.Find("Down");
            eve = EventCallBack.RegEvent<EventCallBack>(mod);
            eve.PointerUp = DownPointUp;
            eve.PointerEntry = DownPointEntry;
            eve.PointerLeave = PointLeave;
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
        }
        void PointLeave(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
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
        void LeftPointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
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
        void TopPointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
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
        void RightPointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
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
        void DownPointUp(EventCallBack callBack, UserAction action)
        {
            Cover.activeSelf = false;
        }
    }
}
