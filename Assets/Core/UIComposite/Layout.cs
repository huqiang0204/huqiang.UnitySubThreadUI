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
    public class LayoutLine
    {
        public enum Direction
        {
            Horizontal, Vertical
        }
        Layout layout;
        EventCallBack callBack;
        public Direction direction { get; private set; }
        public LayoutLine(Layout lay,ModelElement mod,Direction dir,bool fix=false)
        {
            layout = lay;
            layout.lines.Add(this);
            model = mod;
            if(!fix)
            {
                callBack = EventCallBack.RegEvent<EventCallBack>(model);
                callBack.Drag = Drag;
                callBack.DragEnd = Drag;
                direction = dir;
            }
            mod.SetParent(layout.LineLevel);
        }
        void Drag(EventCallBack callBack,UserAction action,Vector2 v)
        {
            if(direction==Direction.Horizontal)
            {
                if(v.x<0)
                    MoveLeft(v.x);
                else
                    MoveRight(v.x);
            }
            else
            {
                if(v.y<0)
                    MoveDown(v.y);
                else
                    MoveTop(v.y);
            }
        }
        void MoveLeft(float dis)
        {
            float x = model.data.localPosition.x;
            x += dis;
            for(int i=0;i<Left.Count;i++)
            {
                var lx = Left[i].Left.model.data.localPosition.x;
                if (x <= lx)
                    x = lx + 1;
            }
            model.data.localPosition.x = x;
            model.IsChanged = true;
            for(int i=0;i<Left.Count;i++)
            {
                Left[i].SizeChanged();
            }
        }
        void MoveTop(float dis)
        {
            float y = model.data.localPosition.y;
            y += dis;
            for (int i = 0; i < Top.Count; i++)
            {
                var ty = Top[i].Top.model.data.localPosition.y;
                if (y >= ty)
                    y = ty - 1;
            }
            model.data.localPosition.y = y;
            model.IsChanged = true;
            for (int i = 0; i < Top.Count; i++)
            {
                Top[i].SizeChanged();
            }
        }
        void MoveRight(float dis)
        {
            float x = model.data.localPosition.x;
            x += dis;
            for (int i = 0; i < Right.Count; i++)
            {
                var rx = Right[i].Right.model.data.localPosition.x;
                if (x >= rx)
                    x = rx - 1;
            }
            model.data.localPosition.x = x;
            model.IsChanged = true;
            for (int i = 0; i < Right.Count; i++)
            {
                Right[i].SizeChanged();
            }
        }
        void MoveDown(float dis)
        {
            float y = model.data.localPosition.y;
            y += dis;
            for (int i = 0; i < Down.Count; i++)
            {
                var ty = Down[i].Down.model.data.localPosition.y;
                if (y >= ty)
                    y = ty - 1;
            }
            model.data.localPosition.y = y;
            model.IsChanged = true;
            for (int i = 0; i < Down.Count; i++)
            {
                Down[i].SizeChanged();
            }
        }
        public void SetSize(Vector2 pos,Vector2 size)
        {
            model.data.localPosition = pos;
            model.data.sizeDelta = size;
            model.IsChanged = true;
        }
        public ModelElement model;
        /// <summary>
        /// 左边相邻的所有区域
        /// </summary>
        public List<LayoutArea> Left = new List<LayoutArea>();
        /// <summary>
        /// 右边相邻的所有区域
        /// </summary>
        public List<LayoutArea> Right = new List<LayoutArea>();
        /// <summary>
        /// 顶部相邻的区域
        /// </summary>
        public List<LayoutArea> Top = new List<LayoutArea>();
        /// <summary>
        /// 底部相邻的区域
        /// </summary>
        public List<LayoutArea> Down = new List<LayoutArea>();
    }
    public class LayoutArea
    {
        public enum Dock
        {
            Center,Left,Top,Right,Down
        }
        public ModelElement model;
        Layout layout;
        public LayoutArea(Layout lay,ModelElement mod)
        {
            layout = lay;
            model = mod;
            mod.SetParent(layout.AreaLevel);
            layout.areas.Add(this);
        }
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        public LayoutLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        public LayoutLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        public LayoutLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        public LayoutLine Down;
        public void SizeChanged()
        {
            float rx = Right.model.data.localPosition.x;
            float lx= Left.model.data.localPosition.x;
            float ty = Top.model.data.localPosition.y;
            float dy = Down.model.data.localPosition.y;
            float w = rx - lx;
            float h = ty - dy;
            model.data.sizeDelta.x = w;
            model.data.sizeDelta.y = h;
            model.data.localPosition.x = lx + w * 0.5f;
            model.data.localPosition.y = dy + h * 0.5f;
            model.IsChanged = true;
            ModelElement.ScaleSize(model);//触发SizeChange事件
        }
        public void AddArea(Dock dock)
        {
            switch(dock)
            {
                case Dock.Left:
                    AddLeftArea();
                    break;
                case Dock.Top:
                    AddTopArea();
                    break;
                case Dock.Right:
                    AddRightArea();
                    break;
                case Dock.Down:
                    AddDownArea();
                    break;
                default:
                    AddCenterArea();
                    break;
            }
        }
        void AddCenterArea()
        {

        }
        void AddLeftArea()
        {
            ModelElement m = new ModelElement();
            m.Load(layout.AreaMod.ModData);
            LayoutArea area = new LayoutArea(layout,m);
            layout.areas.Add(area);
            Vector2 size = model.data.sizeDelta;
            float px = model.data.localPosition.x;
            float py = model.data.localPosition.y;
            float w = size.x * 0.5f;
            float os = w * 0.5f;
            m.data.localPosition.x = px - os;
            m.data.localPosition.y = py;
            m.data.sizeDelta = new Vector2(w,size.y);
            m.IsChanged = true;

            model.data.localPosition.x = px + os;
            model.data.sizeDelta.x = w;
            model.IsChanged = true;

            m = new ModelElement();
            m.Load(Left.model.ModData);
            LayoutLine line = new LayoutLine(layout,m,LayoutLine.Direction.Vertical);
            m.data.localPosition = new Vector3(px,py,0);
            m.IsChanged = true;
            area.Right = line;
            Left = line;

            ModelElement.ScaleSize(model);
        }
        void AddRightArea()
        {

        }
        void AddTopArea()
        {

        }
        void AddDownArea()
        {

        }
    }
    public class Layout : ModelInital
    {
        public static float LineWidth = 4f;
        public List<LayoutLine> lines = new List<LayoutLine>();
        public List<LayoutArea> areas = new List<LayoutArea>();
        ModelElement model;
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        public LayoutLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        public LayoutLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        public LayoutLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        public LayoutLine Down;
        public ModelElement LineMod;
        public ModelElement AreaMod;
        public ModelElement LineLevel;
        public ModelElement AreaLevel;
        public override void Initial(ModelElement mod)
        {
            base.Initial(mod);
            LineLevel = mod.Find("LineLevel");
            AreaLevel= mod.Find("AreaLevel");
            LineMod = mod.Find("Line");
            LineMod.activeSelf = false;
            AreaMod = mod.Find("Area");
            AreaMod.activeSelf = false;
            model.SizeChanged = SizeChanged;
            InitialFixLine();
            InitialArea();
        }
        void InitialFixLine()
        {
            Vector2 size = model.data.sizeDelta;
            float rx = size.x*0.5f;
            float ty = size.y;

            ModelElement m = new ModelElement();
            m.Load(LineMod.ModData);
            Left = new LayoutLine(this,m,LayoutLine.Direction.Vertical,true);
            Left.SetSize(new Vector2(-rx,0),new Vector2(LineWidth,size.y));

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Right = new LayoutLine(this, m, LayoutLine.Direction.Vertical, true);
            Right.SetSize(new Vector2(rx, 0), new Vector2(LineWidth, size.y));

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Top = new LayoutLine(this, m, LayoutLine.Direction.Vertical, true);
            Top.SetSize(new Vector2(0, ty), new Vector2(size.x, LineWidth));

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Down = new LayoutLine(this, m, LayoutLine.Direction.Vertical, true);
            Down.SetSize(new Vector2(0, -ty), new Vector2(size.x, LineWidth));
        }
        void InitialArea()
        {
            ModelElement m = new ModelElement();
            m.Load(AreaMod.ModData);
            LayoutArea area = new LayoutArea(this,m);
            area.Left = Left;
            area.Right = Right;
            area.Top = Top;
            area.Down = Down;
        }
        void SizeChanged(ModelElement mod)
        {

        }
    }
}
