using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace UGUI
{
    public class ShareImage : CustomRawImage
    {
#if UNITY_EDITOR

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var help = GetComponent<ShareImageHelper>();
            if (help != null)
                help.Refresh();
            base.OnPopulateMesh(vh);
        }
#endif
    }
}

