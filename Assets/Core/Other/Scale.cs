using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ScaleType
{
    None,
    FillX,
    FillY,
    FillXY,
    Cover,
}
public enum SizeType
{
    None, Anchor, Margin,MarginRatio
}
public enum AnchorType
{
    Cneter,
    Left,
    Right,
    Top,
    Down,
    LeftDown,
    LeftTop,
    RightDown,
    RightTop
}
public enum ParentType
{
    Tranfrom,Screen, BangsScreen
}
[Serializable]
public struct Margin
{
    /// <summary>
    /// pivot.x 0-1
    /// </summary>
    public float left;
    /// <summary>
    /// pivot.y 0-1
    /// </summary>
    public float down;
    /// <summary>
    /// size.x
    /// </summary>
    public float right;
    /// <summary>
    /// size.y
    /// </summary>
    public float top;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <param name="d"></param>
    /// <param name="t"></param>
    public Margin(float l, float r, float t, float d) { left = l;right = r;down = d;top = t; }
    public Margin(Vector4 v) { left = v.x; right = v.z; top = v.w; down = v.y; }
    public Margin(Rect v) { left = v.x; right = v.width; top = v.height; down = v.y; }
}

public class Scale
{
    public static Vector2 Center = new Vector2(0.5f,0.5f);
    public static Vector2 Left = new Vector2(0,0.5f);
    public static Vector2 Right = new Vector2(1, 0.5f);
    public static Vector2 Top = new Vector2(0.5f, 1);
    public static Vector2 Down = new Vector2(0.5f,0);
    public static Vector2 LeftDown = new Vector2(0,0);
    public static Vector2 LeftTop = new Vector2(0, 1);
    public static Vector2 RightDown = new Vector2(1,0);
    public static Vector2 RightTop = new Vector2(1,1);

    public static float LayoutWidth=720;
    public static float LayoutHeight = 1280;
    public static float ScreenWidth=720;
    public static float ScreenHeight=1280;
    public static float ScreenCurrentWidth ;
    public static float ScreenCurrentHeight;
    /// <summary>
    /// 对屏幕进行等比例覆盖
    /// </summary>
    /// <returns></returns>
    public static Vector3 ScreenStretchRatio()
    {
        float w = Screen.width;
        float h = Screen.height;

        w /= LayoutWidth;
        h /= LayoutHeight;
      
        if (w > h)
            return new Vector3(w, w, w);
        else return new Vector3(h, h, h);
    }
    /// <summary>
    /// 对屏幕进行等比例填充
    /// </summary>
    /// <returns></returns>
    public static Vector3 ScreenFillRatio()
    {
        float w = Screen.width;
        float h = Screen.height;
    
        w /= LayoutWidth;
        h /= LayoutHeight;

        if (w < h)
        {
           // Debug.Log("w:"+w);
            return new Vector3(w, w, w);
        }

        else
        {
           // Debug.Log("h:"+h);
            return new Vector3(h, h, h);
        }

    }
   
    /// <summary>
    /// 将目标填充到固定框中
    /// </summary>
    /// <param name="fill"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static Vector3 FillRatio(Vector2 fill, Vector2 border)
    {

        float w = border.x / fill.x;
        float h = border.y / fill.y;

        if (w < h)
            return new Vector3(w, w, w);
        else return new Vector3(h, h, h);
    }
   
    /// <summary>
    /// 按屏幕宽度比例缩放
    /// </summary>
    /// <returns></returns>
    public static Vector3 ScreenFillHorizontal()
    {
        float w = Screen.width;
        w /= LayoutWidth;

        return new Vector3(w, w, w);
    }
   
    /// <summary>
    /// 按屏幕高度比例缩放
    /// </summary>
    /// <returns></returns>
    public static Vector3 ScreenFillVertical()
    {
        float h = Screen.height;
        h /= LayoutHeight;
        return new Vector3(h, h, h);
    }
    public static Vector2 GetScreenFillHorizontalSize()
    {
        float w = Screen.width;
        float h = Screen.height;
        h = h / w * LayoutWidth;
        return new Vector2(w, h);
    }
    /// <summary>
    /// 获取按屏幕宽度比例填充时的实际像素高度
    /// </summary>
    /// <returns></returns>
    public static float ScreenFillHorizontalHigh()
    {
        float w = Screen.width;
        float h = Screen.height;

        return h / w * LayoutWidth;
     
    }
    /// <summary>
    /// 按屏幕填充，比例会拉伸
    /// </summary>
    /// <returns></returns>
    public static Vector3 ScreenFill()
    {

        float w = Screen.width;
        float h = Screen.height;
        w /= LayoutWidth;
        h /= LayoutHeight;
        return new Vector3(w, h, 1);
    }
    /// <summary>
    /// 控件进行ScreenFillHorizontal函数后使用此函数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="high"></param>
    public static void DockUp(Transform target, float high)
    {
        float w = Screen.width;
        float h = Screen.height;
        float rw = w / LayoutWidth;
        float sh = high * rw;//缩放后实际高度
        target.localPosition = new Vector3(0, h * 0.5f - sh * 0.5f, 0);

    }
    /// <summary>
    /// 控件进行ScreenFillHorizontal函数后使用此函数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="high"></param>
    public static void DockDown(Transform target, float high)
    {
        float w = Screen.width;
        float h = Screen.height;
        float rw = w / LayoutWidth;
        float sh = high * rw;
        target.localPosition = new Vector3(0, -h * 0.5f + sh * 0.5f, 0);
    }
    public static void DockLeft(Transform target, float width)
    {
        float x = Screen.width;
        x *= 0.5f;
        target.localPosition = new Vector3(width - x, 0, 0);
    }
    public static void DockRight(Transform target, float width)
    {
        float x = Screen.width;
        x *= 0.5f;
        target.localPosition = new Vector3(x - width, 0, 0);
    }
    /// <summary>
    /// 控件进行ScreenFillHorizontal函数后使用此函数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="high"></param>
    public static void DockCenter(RectTransform target, float up, float down)
    {
        float w = Screen.width;
        float h = Screen.height;
        float rw = w / LayoutWidth;
        float sh = (up + down) * rw;
        float ch = h - sh;
        ch /= rw;
        float oh = (down - up) * rw * 0.5f;
        float x = target.sizeDelta.x;
        //if (w > 720)
        //    w = 720;
        target.sizeDelta = new Vector2(x, ch);
        //Debug.Log(w);
        x = target.localPosition.x;
        target.localPosition = new Vector3(x, oh, 0);
    }
    /// <summary>
    /// 控件进行ScreenFillHorizontal函数后使用此函数,有宽度要求的时候
    /// </summary>
    /// <param name="target"></param>
    /// <param name="high"></param>
    public static void DockCenter(RectTransform target, float up, float down,float _width=720)
    {
        float w = Screen.width;
        float h = Screen.height;
        float rw = w / LayoutWidth;
        float sh = (up + down) * rw;
        float ch = h - sh;
        ch /= rw;
        float oh = (down - up) * rw * 0.5f;
        target.sizeDelta = new Vector2(_width, ch);
        target.localPosition = new Vector3(0, oh, 0);
    }
    /// <summary>
    /// 控件进行ScreenFillHorizontal函数后使用此函数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="high"></param>
    public static void DockCenter(RectTransform target, float up, float down, float _width ,float _x)
    {
        float w = Screen.width;
        float h = Screen.height;
        float rw = w / LayoutWidth;
        float sh = (up + down) * rw;
        float ch = h - sh;
        ch /= rw;
        float oh = (down - up) * rw * 0.5f;
        target.sizeDelta = new Vector2(_width, ch);
        target.localPosition = new Vector3(_x, oh, 0);
    }
    /// <summary>
    /// 获取按水平缩放后实际像素高度
    /// </summary>
    public static float GetActulPixels(float h)
    {
        float w = Screen.width;
        return h * LayoutWidth / w;
    }
    public class ScaleContent
    {
        public float uh;
        public float dh;
        public RectTransform scale;
        public RectTransform up;
        public RectTransform center;
        public RectTransform down;
        public RectTransform background;
    }
    public static void StretchScreenRatio(ScaleContent sc)
    {
        float w = Screen.width;
        float h = Screen.height;
        float r = w / h;
        if (r == 0.5625f)
        {
            float rw = w / LayoutWidth;
            if (sc.scale != null)
                sc.scale.localScale = new Vector3(rw, rw, 1);
            if (sc.background != null)
                sc.background.sizeDelta = new Vector2(LayoutWidth, LayoutHeight);
            float ch = LayoutHeight - sc.uh - sc.dh;
            float oh = (sc.dh - sc.uh) * 0.5f;
            if (sc.center != null)
            {
                sc.center.sizeDelta = new Vector2(LayoutWidth, ch);
                sc.center.localPosition = new Vector3(0, oh, 0);
            }
            if (sc.up != null)
                sc.up.localPosition = new Vector3(0, LayoutHeight * 0.5f - sc.uh * 0.5f, 0);
            if (sc.down != null)
                sc.down.localPosition = new Vector3(0, LayoutHeight*-0.5f+ sc.dh * 0.5f, 0);
        }
        else
        {
            float rw = w / LayoutWidth;
            float rh = h / LayoutHeight;
            float sh = h / rw;//按720x1280进行缩放后的实际像素
            if (r < 0.5625f)
            {
                if (sc.scale != null)
                    sc.scale.localScale = new Vector3(rh, rh, 1);
                if (sc.background != null)
                    sc.background.sizeDelta = new Vector2(LayoutWidth, sh);
            }
            else
            {
                if (sc.scale != null)
                    sc.scale.localScale = new Vector3(rw, rw, 1);
                if (sc.background != null)
                    sc.background.sizeDelta = new Vector2(sh * 0.5625f, sh);
            }
            float ch = sh - sc.uh - sc.dh;
            float oh = (sc.dh - sc.uh) * 0.5f;
            if (sc.center != null)
            {
                sc.center.sizeDelta = new Vector2(LayoutWidth, ch);
                sc.center.localPosition = new Vector3(0, oh, 0);
            }
            if (sc.up != null)
                sc.up.localPosition = new Vector3(0, sh * 0.5f - sc.uh * 0.5f, 0);
            if (sc.down != null)
                sc.down.localPosition = new Vector3(0, -sh * 0.5f + sc.dh * 0.5f, 0);
        }
    }
    public static Vector3 FillCenterX(float up,float down,float max)
    {
        float w = Screen.width;
        w /= LayoutWidth;
        float am= max * w;
        float h = (up + down) * w;
        float r = Screen.height - h;//剩余实际像素
        if (r > am)
            r = am;
        r = r/max;
        if (r < w)
        {
            return new Vector3(r,r,r);
        }
        else
        {
            return new Vector3(w,w,w);
        }
    }

    public static void FillX(Transform rect)
    {
        float w =ScreenWidth/LayoutWidth;
        rect.localScale = new Vector3(w,w,w);
    }
    public static void FillY(Transform rect)
    {
        float h = ScreenHeight / LayoutHeight;
        rect.localScale = new Vector3(h, h, h);
    }
    public static void FillScreenWithScaleX(RectTransform rect)
    {
        float h= ScreenHeight* LayoutWidth / ScreenWidth;
        rect.sizeDelta = new Vector2(LayoutWidth,h);
    }
    public static void FilllScreenWithScaleY(RectTransform rect)
    {
        float w = ScreenWidth * LayoutHeight / ScreenHeight;
        rect.sizeDelta = new Vector2(w,LayoutHeight);
    }
    public static void ScaleCover(RectTransform rect,RectTransform parent = null)
    {
        float w = ScreenWidth;
        float h = ScreenHeight;
        if(parent!=null)
        {
            w = parent.sizeDelta.x;
            h = parent.sizeDelta.y;
        }
        w /= rect.sizeDelta.x;
        h /= rect.sizeDelta.y;

        if (w > h)
            rect.localScale =new Vector3(w, w, w);
        else rect.localScale= new Vector3(h, h, h);
    }
    public static void SizeCover(RectTransform rect, RectTransform parent = null)
    {
        float w = ScreenWidth;
        float h = ScreenHeight;
        if (parent != null)
        {
            w = parent.sizeDelta.x;
            h = parent.sizeDelta.y;
        }
        var s = rect.sizeDelta;
        w /= s.x;
        h /= s.y;

        if (w > h)
        {
            s.x *= w;
            s.y *= w;
            rect.sizeDelta = s;
        }
        else {
            s.x *= h;
            s.y *= h;
            rect.sizeDelta = s;
        }
    }
    public static void MarginEx(RectTransform rect, Margin margin)
    {
        var parent = rect.parent as RectTransform;
        if (parent != null)
            MarginEx(rect,margin,parent.pivot,parent.sizeDelta);
    }
    public static void MarginEx(RectTransform rect, Margin margin,RectTransform parent)
    {
        if (parent != null)
            MarginEx(rect, margin, parent.pivot, parent.sizeDelta);
    }
    public static void MarginEx(RectTransform rect,Margin margin,Vector2 parentPivot,Vector2 parentSize)
    {
        float w = parentSize.x - margin.left - margin.right;
        float h = parentSize.y - margin.top - margin.down;
        var m_pivot = rect.pivot;
        float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
        float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
        rect.sizeDelta = new Vector2(w, h);
        rect.localPosition = new Vector3(ox, oy, 0);
    }
    public static float ScreenHeightWithScaleX()
    {
        return ScreenHeight * LayoutWidth/ScreenWidth;
    }
    public static float ScreenHeightBangsX()
    {
        float h = 0;
        if (ScreenHeight / ScreenWidth > 2f)
            h = 88;
        return h * LayoutWidth / ScreenWidth;
    }
    public static float ScreenHeightFilterBangsX()
    {
        float h = ScreenHeight;
        if(ScreenHeight/ScreenWidth>2f)
            h -= 88;
        return h *  LayoutWidth/ ScreenWidth;
    }
    public static float NormalDpi = 96;
    public static float ScreenScale { get { float dpi = Screen.dpi; return dpi / NormalDpi; } }
    public static Vector2 ScreenSize { get {
            float dpi = Screen.dpi;
            float s =  NormalDpi/dpi;
            return new Vector2(Screen.width*s,Screen.height*s);
        } }
}