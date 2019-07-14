using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public enum Direction
    {
        Horizontal, Vertical
    }
    public class DockPanelLine
    {
        public DockPanel layout;
        EventCallBack callBack;
        public Direction direction { get; private set; }
        /// <summary>
        /// 需要绘制线
        /// </summary>
        public bool realLine { get; private set; }
        public DockPanelLine(DockPanel lay,ModelElement mod,Direction dir,bool real=true)
        {
            realLine= real;
            layout = lay;
            layout.lines.Add(this);
            model = mod;
            if (real)
            {
                callBack = EventCallBack.RegEvent<EventCallBack>(model);
                callBack.Drag = Drag;
              
                direction = dir;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                //callBack.PointerEntry = (o, e) => {
                //    ThreadMission.InvokeToMain((y) => {
                //        Cursor.SetCursor(UnityEngine.Resources.Load<Texture2D>("emoji"),Vector2.zero,CursorMode.Auto);
                //    },null);
                //};
                //callBack.DragEnd = (o, e, v) => {
                //    ThreadMission.InvokeToMain((y) =>
                //    {
                //        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                //    }, null);
                //};
#endif
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
        public List<DockpanelArea> Left = new List<DockpanelArea>();
        /// <summary>
        /// 右边相邻的所有区域
        /// </summary>
        public List<DockpanelArea> Right = new List<DockpanelArea>();
        /// <summary>
        /// 顶部相邻的区域
        /// </summary>
        public List<DockpanelArea> Top = new List<DockpanelArea>();
        /// <summary>
        /// 底部相邻的区域
        /// </summary>
        public List<DockpanelArea> Down = new List<DockpanelArea>();
        DockPanelLine LineStart;//起点相邻的线
        DockPanelLine LineEnd;//终点相邻的线
        List<DockPanelLine> AdjacentLines = new List<DockPanelLine>();//所有与之相邻的线
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
        public void SetLineStart(DockPanelLine line)
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            LineStart = line;
            LineStart.AdjacentLines.Add(this);
        }
        public void SetLineEnd(DockPanelLine line)
        {
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            LineEnd = line;
            LineEnd.AdjacentLines.Add(this);
        }
        public void Dispose()
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            layout.lines.Remove(this);
            model.SetParent(null);
            ModelManagerUI.RecycleElement(model);
        }
        void Release()
        {
            if (LineStart != null)
                LineStart.AdjacentLines.Remove(this);
            if (LineEnd != null)
                LineEnd.AdjacentLines.Remove(this);
            LineStart = null;
            LineEnd = null;
        }
        public void MergeLeft(DockPanelLine line)
        {
            line.Release();
            Left.AddRange(line.Left);
            var areas = line.Left;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Right = this;
            model.data.localPosition.y = line.model.data.localPosition.y;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineEnd(this);
            }
        }
        public void MergeRight(DockPanelLine line)
        {
            line.Release();
            Right.AddRange(line.Right);
            var areas = line.Right;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Left = this;
            model.data.localPosition.y = line.model.data.localPosition.y;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineStart(this);
            }
        }
        public void MergeTop(DockPanelLine line)
        {
            line.Release();
            Top.AddRange(line.Top);
            var areas = line.Top;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Down = this;
            model.data.localPosition.x = line.model.data.localPosition.x;
            var al = line.AdjacentLines;
            int c = al.Count - 1;
            for (; c >= 0; c--)
            {
                var l = al[c];
                l.SetLineStart(this);
            }
        }
        public void MergeDown(DockPanelLine line)
        {
            line.Release();
            Down.AddRange(line.Down);
            var areas = line.Down;
            for (int i = 0; i < areas.Count; i++)
                areas[i].Top = this;
            model.data.localPosition.x = line.model.data.localPosition.x;
            var al = line.AdjacentLines;
            int c = al.Count-1;
            for (; c>=0;c--)
            {
                var l = al[c];
                l.SetLineEnd(this);
            }
        }
    }
    public class DockpanelArea
    {
        public enum Dock
        {
            Left, Top, Right, Down
        }
        public ModelElement model;
        public DockPanel layout;
        public DockpanelArea(DockPanel lay)
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
        internal DockPanelLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        internal DockPanelLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        internal DockPanelLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        internal DockPanelLine Down;
        public void SetLeftLine(DockPanelLine line)
        {
            if (Left != null)
                Left.Right.Remove(this);
            Left = line;
            line.Right.Add(this);
        }
        public void SetRightLine(DockPanelLine line)
        {
            if (Right != null)
                Right.Left.Remove(this);
            Right = line;
            line.Left.Add(this);
        }
        public void SetTopLine(DockPanelLine line)
        {
            if (Top != null)
                Top.Down.Remove(this);
            Top = line;
            line.Down.Add(this);
        }
        public void SetDownLine(DockPanelLine line)
        {
            if (Down != null)
                Down.Top.Remove(this);
            Down = line;
            line.Top.Add(this);
        }
        public void SizeChanged()
        {
            float hl = DockPanel.LineWidth * 0.5f;
            float rx = Right.model.data.localPosition.x;
            if (Right.realLine)
                rx -= hl;
            float lx = Left.model.data.localPosition.x;
            if (Left.realLine)
                lx += hl;
            float ty = Top.model.data.localPosition.y;
            if (Top.realLine)
                ty -= hl;
            float dy = Down.model.data.localPosition.y;
            if (Down.realLine)
                dy += hl;
            float w = rx - lx;
            float h = ty - dy;
            bool c = false;
            if (model.data.sizeDelta.x != w)
                c = true;
            model.data.sizeDelta.x = w;
            if (model.data.sizeDelta.y != h)
                c = true;
            model.data.sizeDelta.y = h;
            model.data.localPosition.x = lx + w * 0.5f;
            model.data.localPosition.y = dy + h * 0.5f;
            model.IsChanged = true;
            if (c)
            {
                ModelElement.ScaleSize(model);//触发SizeChange事件
            }
        }
        public DockpanelArea AddArea(Dock dock, float r = 0.5f)
        {
            switch (dock)
            {
                case Dock.Left:
                    return AddLeftArea(r);
                case Dock.Top:
                    return AddTopArea(r);
                case Dock.Right:
                    return AddRightArea(r);
                case Dock.Down:
                    return AddDownArea(r);
            }
            return this;
        }
        DockPanelLine AddHorizontalLine(float r)
        {
            var m = new ModelElement();
            m.Load(Top.model.ModData);
            float ex = Right.model.data.localPosition.x;
            float sx = Left.model.data.localPosition.x;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            DockPanelLine line = new DockPanelLine(layout, m, Direction.Horizontal);
            var pos = model.data.localPosition;
            float dy = Down.model.data.localPosition.y;
            pos.y = Top.model.data.localPosition.y - dy;
            pos.y *= r;
            pos.y += dy;
            line.SetSize(pos, new Vector2(w, DockPanel.LineWidth));
            line.SetLineStart(Left);
            line.SetLineEnd(Right);
            return line;
        }
        DockPanelLine AddVerticalLine(float r)
        {
            var m = new ModelElement();
            m.Load(Left.model.ModData);
            float ex = Top.model.data.localPosition.y;
            float sx = Down.model.data.localPosition.y;
            float w = ex - sx;
            if (w < 0)
                w = -w;
            DockPanelLine line = new DockPanelLine(layout, m, Direction.Vertical);
            var pos = model.data.localPosition;
            float dx = Left.model.data.localPosition.x;
            pos.x = Right.model.data.localPosition.x - dx;
            pos.x *= r;
            pos.x += dx;
            line.SetSize(pos, new Vector2(DockPanel.LineWidth, w));
            line.SetLineStart(Down);
            line.SetLineEnd(Top);
            return line;
        }
        DockpanelArea AddLeftArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddVerticalLine(r);
            area.SetLeftLine(Left);
            area.SetRightLine(line);
            area.SetTopLine(Top);
            area.SetDownLine(Down);
            SetLeftLine(line);
            ModelElement.ScaleSize(model);
            return area;
        }
        DockpanelArea AddRightArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddVerticalLine(1 - r);
            area.SetLeftLine(line);
            area.SetRightLine(Right);
            area.SetTopLine(Top);
            area.SetDownLine(Down);
            SetRightLine(line);
            ModelElement.ScaleSize(model);
            return area;
        }
        DockpanelArea AddTopArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddHorizontalLine(1 - r);
            area.SetLeftLine(Left);
            area.SetRightLine(Right);
            area.SetTopLine(Top);
            area.SetDownLine(line);
            SetTopLine(line);
            ModelElement.ScaleSize(model);
            return area;
        }
        DockpanelArea AddDownArea(float r)
        {
            DockpanelArea area = new DockpanelArea(layout);
            layout.areas.Add(area);
            var line = AddHorizontalLine(r);
            area.SetLeftLine(Left);
            area.SetRightLine(Right);
            area.SetTopLine(line);
            area.SetDownLine(Down);
            SetDownLine(line);
            ModelElement.ScaleSize(model);
            return area;
        }
        public void Dispose()
        {
            if (Left.realLine | Right.realLine | Top.realLine | Down.realLine)
            {
                Left.Right.Remove(this);
                Right.Left.Remove(this);
                Top.Down.Remove(this);
                Down.Top.Remove(this);
                model.SetParent(null);
                MergeArea();
                ModelManagerUI.RecycleElement(model);
                layout.areas.Remove(this);
            }
        }
        void MergeArea()
        {
            if (Left.realLine)
            {
                if (Left.Right.Count < 1)
                {
                    Right.MergeLeft(Left);
                    Left.Dispose();
                    return;
                }
            }
            if (Right.realLine)
            {
                if (Right.Left.Count < 1)
                {
                    Left.MergeRight(Right);
                    Right.Dispose();
                    return;
                }
            }
            if (Top.realLine)
            {
                if (Top.Down.Count < 1)
                {
                    Down.MergeTop(Top);
                    Top.Dispose();
                    return;
                }
            }
            if (Down.realLine)
            {
                if (Down.Top.Count < 1)
                {
                    Top.MergeDown(Down);
                    Down.Dispose();
                    return;
                }
            }
        }
    }
    public class DockPanel : ModelInital
    {
        public static float LineWidth = 12f;
        public List<DockPanelLine> lines = new List<DockPanelLine>();
        public List<DockpanelArea> areas = new List<DockpanelArea>();
        ModelElement model;
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        public DockPanelLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        public DockPanelLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        public DockPanelLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        public DockPanelLine Down;
        public ModelElement LineMod;
        public ModelElement AreaMod;
        public ModelElement LineLevel;
        public ModelElement AreaLevel;
  
        public DockpanelArea MainArea { get; private set; }
        public override void Initial(ModelElement mod)
        {
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
            Left = new DockPanelLine(this,m,Direction.Vertical,false);

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Right = new DockPanelLine(this, m, Direction.Vertical, false);

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Top = new DockPanelLine(this, m, Direction.Vertical, false);

            m = new ModelElement();
            m.Load(LineMod.ModData);
            Down = new DockPanelLine(this, m, Direction.Vertical, false);

            Vector2 size = model.data.sizeDelta;
            float rx = size.x * 0.5f;
            float ty = size.y * 0.5f;

            Left.SetSize(new Vector2(-rx, 0), new Vector2(LineWidth, size.y));
            Right.SetSize(new Vector2(rx, 0), new Vector2(LineWidth, size.y));
            Top.SetSize(new Vector2(0, ty), new Vector2(size.x, LineWidth));
            Down.SetSize(new Vector2(0, -ty), new Vector2(size.x, LineWidth));
        }
        void InitialArea()
        {
            ModelElement m = new ModelElement();
            m.Load(AreaMod.ModData);
            DockpanelArea area = new DockpanelArea(this);
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
        }
        public void Refresh()
        {
            for (int i = 0; i < lines.Count; i++)
                lines[i].SizeChanged();
            for (int i = 0; i < areas.Count; i++)
                areas[i].SizeChanged();
        }
        /// <summary>
        /// 锁定布局
        /// </summary>
        public bool LockLayout;
    }
}
