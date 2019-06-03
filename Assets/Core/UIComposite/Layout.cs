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
    public class LayotLine
    {
        public enum Direction
        {
            Horizontal, Vertical
        }
        Layout layout;
        EventCallBack callBack;
        public Direction direction { get; private set; }
        public LayotLine(Layout lay,ModelElement mod,Direction dir,bool fix=false)
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
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        public LayotLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        public LayotLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        public LayotLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        public LayotLine Down;
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
        public List<LayotLine> lines = new List<LayotLine>();
        public List<LayoutArea> areas = new List<LayoutArea>();
        ModelElement model;
        /// <summary>
        /// 左边相邻的线
        /// </summary>
        public LayotLine Left;
        /// <summary>
        /// 右边相邻的线
        /// </summary>
        public LayotLine Right;
        /// <summary>
        /// 顶部相邻的线
        /// </summary>
        public LayotLine Top;
        /// <summary>
        /// 底部相邻的线
        /// </summary>
        public LayotLine Down;
        ModelElement LineMod;
        public override void Initial(ModelElement mod)
        {
            base.Initial(mod);
            LineMod = mod.Find("Line");
            model.SizeChanged = SizeChanged;
        }
        void SizeChanged(ModelElement mod)
        {

        }
        /// <summary>
        /// 添加一个区域
        /// </summary>
        public void AddArea()
        {

        }
    }
}
