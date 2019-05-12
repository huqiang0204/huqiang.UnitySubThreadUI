using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang
{
    public class UIAnimation:AnimationBase
    {
        static UIAnimation am;
        /// <summary>
        /// 返回此类的唯一实例
        /// </summary>
        public static UIAnimation Manage { get { if (am == null) am = new UIAnimation(); return am; } }
        UIAnimation() { }
    }
}
