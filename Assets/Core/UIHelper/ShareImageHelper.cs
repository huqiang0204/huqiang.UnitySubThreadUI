using System;
using System.Collections.Generic;
using huqiang.Data;
using huqiang.UI;
using UGUI;
using UnityEngine;

[RequireComponent(typeof(ShareImage))]
public class ShareImageHelper : UICompositeHelp
{
    void VertexCalculation(CustomRawImage raw)
    {
        var vert = new List<UIVertex>();
        var tri = new List<int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var c= transform.GetChild(i);
            var help = c.GetComponent<ShareChild>();
            if (help != null)
                help.GetUVInfo(vert,tri,Vector3.zero,Quaternion.identity,Vector3.one);
        }
        raw.uIVertices = vert;
        raw.triangle = tri;
        raw.SetVerticesDirty();
    }
    public void Refresh()
    {
        var raw = GetComponent<CustomRawImage>();
        if (raw != null)
            VertexCalculation(raw);
    }
}
