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
            if (!fix)
            {
                callBack = EventCallBack.RegEvent<EventCallBack>(model);
                callBack.Drag = Drag;
                callBack.DragEnd = Drag;
                direction = dir;
            }
            else mod.activeSelf = false;
            mod.SetParent(layout.LineLevel);
        }
        void Drag(EventCallBack callBack,UserAction action,Vector2 v)
        {
            if(direction==Direction.Vertical)//竖线只能横向移动
            {
                if(v.x<0)
                    MoveLeft(v.x);
                else
                    MoveRight(v.x);
                for (int i = 0; i < Left.Count; i++)
                    Left[i].SizeChanged();
                for (int i = 0; i <Right.Count; i++)
                    Right[i].SizeChanged();
            }
            else//横线只能纵向移动
            {
                if(v.y<0)
                    MoveDown(v.y);
                else
                    MoveTop(v.y);
                for (int i = 0; i < Top.Count; i++)
                    Top[i].SizeChanged();
                for (int i = 0; i < Down.Count; i++)
                    Down[i].SizeChanged();
            }
            for (int i = 0; i < AdjacentLines.Count; i++)
                AdjacentLines[i].SizeChanged();
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
        }
        void MoveDown(float dis)
        {
            float y = model.data.localPosition.y;
            y += dis;
            for (int i = 0; i < Down.Count; i++)
            {
                var ty = Down[i].Down.model.data.localPosition.y;
                if (y <= ty)
                    y = ty - 1;
            }
            model.data.localPosition.y = y;
            model.IsChanged = true;
        }
        public void SetSize(Vector3 pos,Vector2 size)
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
        public LayoutLine LineStart;//起点相邻的线
        public LayoutLine LineEnd;//终点相邻的线
        public List<LayoutLine> AdjacentLines = new List<LayoutLine>();//所有与之相邻的线
        public void SizeChanged()
        {
            if(LineStart!=null&LineEnd!=null)
            {
                if(direction==Direction.Horizontal)
                {
                    float sx = LineStart.model.data.localPosition.x;
                    float ex = LineEnd.model.data.localPosition.x;
                    float w= ex - sx;
                    model.data.localPosition.x = sx + 0.5f * w;
                    if (w < 0)
                        w = -w;
                    model.data.sizeDelta.x = w;
                    model.IsChanged = true;
                }
                else
                {
                    float sx = LineStart.model.data.localPosition.y;
                    float ex = LineEnd.model.data.localPosition.y;
                    float w = ex - sx;
                    model.data.localPosition.y = sx + 0.5f * w;
                    if (w < 0)
                        w = -w;
                    model.data.sizeDelta.y = w;
                    model.IsChanged = true;
                }
                for (int i = 0; i < Left.Count; i++)
                    Left[i].SizeChanged();
                for (int i = 0; i < Right.Count; i++)
                    Right[i].SizeChanged();
                for (int i = 0; i < Top.Count; i++)
                    Top[i].SizeChanged();
                for (int i = 0; i < Down.Count; i++)
                    Down[i].SizeChanged();
            }
        }
    }
    public class LayoutArea
    {
        public enum Dock
        {
            Center,Left,Top,Right,Down
        }
        public ModelElement model;
        Layout layout;
        public LayoutArea(Layout lay)
        {
            layout = lay;
            model = new ModelElement();
            model.Load(layout.AreaMod.ModData);
            layout = lay;
            model.SetParent(layout.AreaLevel);
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
        public LayoutArea AddArea(Dock dock)
        {
            switch(dock)
            {
                case Dock.Left:
                    return AddLeftArea();
                case Dock.Top:
                    return AddTopArea();
                case Dock.Right:
                    return AddRightArea();
                case Dock.Down:
                    return AddDownArea();
                default:
                    AddCenterArea();
                    return this;
            }
        }
        void AddCenterArea()
        {
            
        }
        LayoutArea AddLeftArea()
        {
            LayoutArea area = new LayoutArea(layout);
            layout.areas.Add(area);
            var m = new ModelElement();
            m.Load(Left.model.ModData);

            float ex = Top.model.data.localPosition.y;
            float sx = Down.model.data.localPosition.y;
           float w = ex - sx;
            if (w < 0)
                w = -w;
            LayoutLine line = new LayoutLine(layout,m,LayoutLine.Direction.Vertical);
            line.SetSize(model.data.localPosition, new Vector2(Layout.LineWidth,w));

            area.Left = Left;
            area.Top = Top;
            area.Down = Down;
            area.Right = line;

            Left.Right.Remove(this);
            Left = line;
            line.Right.Add(this);
            line.Left.Add(area);
       
            Top.AdjacentLines.Add(line);
            Down.AdjacentLines.Add(line);
            line.LineStart = Top;
            line.LineEnd = Down;

            ModelElement.ScaleSize(model);
            return area;
        }
        LayoutArea AddRightArea()
        {
            LayoutArea area = new LayoutArea(layout);
            layout.areas.Add(area);
            var m = new ModelElement();
            m.Load(Right.model.ModData);

            float ex = Top.model.data.localPosition.y;
            float sx = Down.model.data.localPosition.y;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            LayoutLine line = new LayoutLine(layout, m, LayoutLine.Direction.Vertical);
            line.SetSize(model.data.localPosition, new Vector2(Layout.LineWidth, w));

            area.Left = line;
            area.Top = Top;
            area.Down = Down;
            area.Right = Right;

            Right.Left.Remove(this);
            Right = line;
            line.Left.Add(this);
            line.Right.Add(area);

            Top.AdjacentLines.Add(line);
            Down.AdjacentLines.Add(line);
            line.LineStart = Top;
            line.LineEnd = Down;

            ModelElement.ScaleSize(model);
            return area;
        }
        LayoutArea AddTopArea()
        {
            LayoutArea area = new LayoutArea(layout);
            layout.areas.Add(area);
            var m = new ModelElement();
            m.Load(Top.model.ModData);

            float ex = Right.model.data.localPosition.x;
            float sx = Left.model.data.localPosition.x;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            LayoutLine line = new LayoutLine(layout, m, LayoutLine.Direction.Horizontal);
            line.SetSize(model.data.localPosition, new Vector2(w, Layout.LineWidth));

            area.Left = Left;
            area.Top = Top;
            area.Down = line;
            area.Right = Right;

            Top.Down.Remove(this);
            Top= line;
            line.Down.Add(this);
            line.Top.Add(area);

            Left.AdjacentLines.Add(line);
            Right.AdjacentLines.Add(line);
            line.LineStart = Left;
            line.LineEnd = Right;

            ModelElement.ScaleSize(model);
            return area;
        }
        LayoutArea AddDownArea()
        {
            LayoutArea area = new LayoutArea(layout);
            layout.areas.Add(area);
            var m = new ModelElement();
            m.Load(Down.model.ModData);

            float ex = Right.model.data.localPosition.x;
            float sx = Left.model.data.localPosition.x;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            LayoutLine line = new LayoutLine(layout, m, LayoutLine.Direction.Horizontal);
            line.SetSize(model.data.localPosition, new Vector2(w, Layout.LineWidth));

            area.Left = Left;
            area.Top = line;
            area.Down = Down;
            area.Right = Right;

            Down.Top.Remove(this);
            Down = line;
            line.Top.Add(this);
            line.Down.Add(area);

            Left.AdjacentLines.Add(line);
            Right.AdjacentLines.Add(line);
            line.LineStart = Left;
            line.LineEnd = Right;

            ModelElement.ScaleSize(model);
            return area;
        }
    }
    public class Layout : ModelInital
    {
        public static float LineWidth = 12f;
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
        public LayoutArea MainArea { get; private set; }
        public override void Initial(ModelElement mod)
        {
            base.Initial(mod);
            model = mod;
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
            ModelElement m = new ModelElement();
            m.Load(LineMod.ModData);
            Left = new LayoutLine(this,m,LayoutLine.Direction.Vertical,true);

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Right = new LayoutLine(this, m, LayoutLine.Direction.Vertical, true);

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Top = new LayoutLine(this, m, LayoutLine.Direction.Vertical, true);

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Down = new LayoutLine(this, m, LayoutLine.Direction.Vertical, true);
        }
        void InitialArea()
        {
            ModelElement m = new ModelElement();
            m.Load(AreaMod.ModData);
            LayoutArea area = new LayoutArea(this);
            area.Left = Left;
            area.Right = Right;
            area.Top = Top;
            area.Down = Down;
            areas.Add(area);
            MainArea = area;
            area.SizeChanged();
        }
        void SizeChanged(ModelElement mod)
        {
            Vector2 size = model.data.sizeDelta;
            float rx = size.x * 0.5f;
            float ty = size.y*0.5f;

            Left.SetSize(new Vector2(-rx, 0), new Vector2(LineWidth, size.y));
            Right.SetSize(new Vector2(rx, 0), new Vector2(LineWidth, size.y));
            Top.SetSize(new Vector2(0, ty), new Vector2(size.x, LineWidth));
            Down.SetSize(new Vector2(0, -ty), new Vector2(size.x, LineWidth));
            for (int i = 0; i < lines.Count; i++)
                lines[i].SizeChanged();
            MainArea.SizeChanged();
        }
    }
}
