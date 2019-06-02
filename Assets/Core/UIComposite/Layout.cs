using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.UIComposite
{
    public class LayotLine
    {
        Layout layout;
        public LayotLine(Layout lay)
        {
            layout = lay;
            layout.lines.Add(this);
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
        public override void Initial(ModelElement mod)
        {
            base.Initial(mod);
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
