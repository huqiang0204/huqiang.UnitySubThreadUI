using System.Collections;
using System.Collections.Generic;
using huqiang.UI;
using UnityEngine;


public class SizeScaleEx : SizeScaling
{
    private ScaleType lastScaleType;
    private AnchorType lastSizeType;
    private AnchorPointType lastAnchorType;
    private Margin lastmargin;
    Vector2 psize;
    Vector2 pp;
    void GetParentInfo()
    {
        pp = ModelElement.Anchors[0];
        if (parentType == ParentType.Tranfrom)
        {
            var t = (transform.parent as RectTransform);
            psize = t.sizeDelta;
            pp = t.pivot;
        }
        else
        if (parentType == ParentType.Screen)
        {
            var t = transform.root as RectTransform;
            psize = t.sizeDelta;
        }
        else
        {
            var t = transform.root as RectTransform;
            psize = t.sizeDelta;
        }
    }
    public void Resize()
    {
        Vector3 loclpos = transform.localPosition;
        GetParentInfo();
        RectTransform rect = transform as RectTransform;
        if (DesignSize.x == 0)
            DesignSize.x = 1;
        if (DesignSize.y == 0)
            DesignSize.y = 1;
        Scaling(rect, scaleType, psize, DesignSize);
        switch(anchorType)
        {
            case AnchorType.None:
                break;
            case AnchorType.Anchor:
                AnchorEx(rect, anchorPointType, anchorOffset, pp, psize);
                break;
            case AnchorType.Alignment:
                AlignmentEx(rect, anchorPointType, anchorOffset, pp, psize);
                break;
        }
        switch(marginType)
        {
            case MarginType.None:
                break;
            case MarginType.Margin:
                var mar = margin;
                if (parentType == ParentType.BangsScreen)
                    if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                        mar.top += 88;
                MarginEx(rect, mar, pp, psize);
                break;
            case MarginType.MarginRatio:
                mar = new Margin();
                mar.left = margin.left * psize.x;
                mar.right = margin.right * psize.x;
                mar.top = margin.top * psize.y;
                if (parentType == ParentType.BangsScreen)
                    if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                        mar.top += 88;
                mar.down = margin.down * psize.y;
                MarginEx(rect, mar, pp, psize);
                break;
            case MarginType.MarginX:
                mar = margin;
                if (parentType == ParentType.BangsScreen)
                    if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                        mar.top += 88;
                MarginX(rect, mar, pp, psize);
                break;
            case MarginType.MarginY:
                mar = margin;
                if (parentType == ParentType.BangsScreen)
                    if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                        mar.top += 88;
                MarginY(rect, mar, pp, psize);
                break;
            case MarginType.MarginRatioX:
                break;
            case MarginType.MarginRatioY:
                break;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            var ss = transform.GetChild(i).GetComponent<SizeScaleEx>();
            if (ss != null)
                ss.Resize();
        }
        if (transform.parent == UIroot)
            transform.localPosition = loclpos;
        var help = GetComponent<UICompositeHelp>();
        if (help != null)
            help.ReSize();
    }
    static float Width = 720;
    static float Height = 1280;
    public static Transform UIroot;
    public static void RefreshAll()
    {
        if (UIroot != null)
        {
            for (int i = 0; i < UIroot.childCount; i++)
            {
                var c = UIroot.GetChild(i);
                RefreshChild(c);
            }
        }
    }
    public static void RefreshChild(Transform t)
    {
        var ss = t.GetComponent<SizeScaleEx>();
        if (ss != null)
            ss.Resize();
        for (int i = 0; i < t.childCount; i++)
        {
            var c = t.GetChild(i);
            RefreshChild(c);
        }
    }
    public void EditorRefreshAll()
    {
        UIroot = transform.root;
        var cam = Camera.main;
        float x = cam.pixelWidth;
        float y = cam.pixelHeight;
        Width = x;
        Height = y;
        Scale.ScreenWidth = x;
        Scale.ScreenHeight = y;
        RefreshAll();
    }
    public void EditorRefresh()
    {
        UIroot = transform.root;
        var cam = Camera.main;
        float x = cam.pixelWidth;
        float y = cam.pixelHeight;
    }

    public static Vector2[] Anchors = new[] { new Vector2(0.5f, 0.5f), new Vector2(0, 0.5f),new Vector2(1, 0.5f),
        new Vector2(0.5f, 1),new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)};
    public static void Scaling(RectTransform rect, ScaleType type, Vector2 pSize, Vector2 ds)
    {
        switch (type)
        {
            case ScaleType.None:
                break;
            case ScaleType.FillX:
                float sx = pSize.x / ds.x;
                rect.localScale = new Vector3(sx, sx, sx);
                break;
            case ScaleType.FillY:
                float sy = pSize.y / ds.y;
                rect.localScale = new Vector3(sy, sy, sy);
                break;
            case ScaleType.FillXY:
                sx = pSize.x / ds.x;
                sy = pSize.y / ds.y;
                if (sx < sy)
                    rect.localScale = new Vector3(sx, sx, sx);
                else rect.localScale = new Vector3(sy, sy, sy);
                break;
            case ScaleType.Cover:
                sx = pSize.x / ds.x;
                sy = pSize.y / ds.y;
                if (sx < sy)
                    rect.localScale = new Vector3(sy, sy, sy);
                else rect.localScale = new Vector3(sx, sx, sx);
                break;
        }
    }
    public static void Anchor(RectTransform rect, Vector2 pivot, Vector2 offset)
    {
        Vector2 p;
        Vector2 pp = new Vector2(0.5f, 0.5f);
        if (rect.parent != null)
        {
            var t = rect.parent as RectTransform;
            p = t.sizeDelta;
            pp = t.pivot;
        }
        else { p = new Vector2(Screen.width, Screen.height); }
        rect.localScale = Vector3.one;
        float sx = p.x * (pivot.x - 0.5f);
        float sy = p.y * (pivot.y - 0.5f);
        float ox = sx + offset.x;
        float oy = sy + offset.y;
        rect.localPosition = new Vector3(ox, oy, 0);
    }
    public static void AnchorEx(RectTransform rect, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
    {
        Vector2 pivot = Anchors[(int)type];
        float ox = (p.x - 1) * psize.x;//原点x
        float oy = (p.y - 1) * psize.y;//原点y
        float tx = ox + pivot.x * psize.x;//锚点x
        float ty = oy + pivot.y * psize.y;//锚点y
        offset.x += tx;//偏移点x
        offset.y += ty;//偏移点y
        rect.localPosition = new Vector3(offset.x, offset.y, 0);
    }
    public static void AlignmentEx(RectTransform rect, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
    {
        Vector2 pivot = Anchors[(int)type];
        float ox = (p.x - 1) * psize.x;//原点x
        float oy = (p.y - 1) * psize.y;//原点y
        float tx = ox + pivot.x * psize.x;//锚点x
        float ty = oy + pivot.y * psize.y;//锚点y
        float x = offset.x + tx;
        float y = offset.y + ty;
        switch(type)
        {
            case AnchorPointType.Left:
                x += rect.sizeDelta.x * 0.5f;
                break;
            case AnchorPointType.Right:
                x -= rect.sizeDelta.x * 0.5f;
                break;
            case AnchorPointType.Top:
                y -= rect.sizeDelta.y * 0.5f;
                break;
            case AnchorPointType.Down:
                y += rect.sizeDelta.y * 0.5f;
                break;
            case AnchorPointType.LeftDown:
                x += rect.sizeDelta.x * 0.5f;
                y += rect.sizeDelta.y * 0.5f;
                break;
            case AnchorPointType.LeftTop:
                x += rect.sizeDelta.x * 0.5f;
                y -= rect.sizeDelta.y * 0.5f;
                break;
            case AnchorPointType.RightDown:
                x -= rect.sizeDelta.x * 0.5f;
                y += rect.sizeDelta.y * 0.5f;
                break;
            case AnchorPointType.RightTop:
                x -= rect.sizeDelta.x * 0.5f;
                y -= rect.sizeDelta.y * 0.5f;
                break;
        }
        rect.localPosition = new Vector3(x, y, 0);
    }
    public static void MarginEx(RectTransform rect, Margin margin, Vector2 parentPivot, Vector2 parentSize)
    {
        float w = parentSize.x - margin.left - margin.right;
        float h = parentSize.y - margin.top - margin.down;
        var m_pivot = rect.pivot;
        float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
        float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
        float sx = rect.localScale.x;
        float sy = rect.localScale.y;
        rect.sizeDelta = new Vector2(w / sx, h / sy);
        rect.localPosition = new Vector3(ox, oy, 0);
    }
    public static void MarginX(RectTransform rect, Margin margin, Vector2 parentPivot, Vector2 parentSize)
    {
        float w = parentSize.x - margin.left - margin.right;
        var m_pivot = rect.pivot;
        float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
        float sx = rect.localScale.x;
        float y = rect.sizeDelta.y;
        rect.sizeDelta = new Vector2(w / sx, y);
        float py = rect.localPosition.y;
        rect.localPosition = new Vector3(ox, py, 0);
    }
    public static void MarginY(RectTransform rect, Margin margin, Vector2 parentPivot, Vector2 parentSize)
    {
        float h = parentSize.y - margin.top - margin.down;
        var m_pivot = rect.pivot;
        float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
        float sy = rect.localScale.y;
        float x = rect.sizeDelta.x;
        rect.sizeDelta = new Vector2(x, h / sy);
        float px = rect.localPosition.x;
        rect.localPosition = new Vector3(px, oy, 0);
    }
}