using System.Collections;
using System.Collections.Generic;
using huqiang.UI;
using UnityEngine;


    public class SizeScaleEx : SizeScaling
    {

        private ScaleType lastScaleType;
        private SizeType lastSizeType;
        private AnchorType lastAnchorType;
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
            GetParentInfo();
            RectTransform rect = transform as RectTransform;
            if (DesignSize.x == 0)
                DesignSize.x = 1;
            if (DesignSize.y == 0)
                DesignSize.y = 1;
            //ModelElement.Docking(rect, scaleType, psize, DesignSize);
            //if (sizeType == SizeType.Anchor)
            //{
            //    ModelElement.AnchorEx(rect, anchorType, new Vector2(margin.left, margin.right), pp, psize);
            //}
            //else if (sizeType == SizeType.Margin)
            //{
            //    var mar = margin;
            //    if (parentType == ParentType.BangsScreen)
            //        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
            //            mar.top += 88;
            //    ModelElement.MarginEx(rect, mar, pp, psize);
            //}
            //else if (sizeType == SizeType.MarginRatio)
            //{
            //    var mar = new Margin();
            //    mar.left = margin.left * psize.x;
            //    mar.right = margin.right * psize.x;
            //    mar.top = margin.top * psize.y;
            //    if (parentType == ParentType.BangsScreen)
            //        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
            //            mar.top += 88;
            //    mar.down = margin.down * psize.y;
            //    ModelElement.MarginEx(rect, mar, pp, psize);
            //}
            //for (int i = 0; i < transform.childCount; i++)
            //{
            //    var ss = transform.GetChild(i).GetComponent<SizeScaleEx>();
            //    if (ss != null)
            //        ss.Resize();
            //}
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
        static void RefreshChild(Transform t)
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
        void AnchorTypeChange()
        {
            if (sizeType == SizeType.Anchor)
            {
                GetParentInfo();
                var au = ModelElement.AntiAnchorEx(transform.localPosition, anchorType, pp, psize);
                margin.left = au.x;
                margin.right = au.y;
            }
            lastAnchorType = anchorType;
        }
        public void EditorRefresh()
        {
            UIroot = transform.root;
            var cam = Camera.main;
            float x = cam.pixelWidth;
            float y = cam.pixelHeight;
            if (x != Width | y != Height)
            {
                Width = x;
                Height = y;
                Scale.ScreenWidth = x;
                Scale.ScreenHeight = y;
                RefreshAll();
            }
            else
            {
                if (sizeType == SizeType.Margin | sizeType == SizeType.MarginRatio)
                {
                    if (lastmargin.left != margin.left | lastmargin.right != margin.right |
                  lastmargin.top != margin.top | lastmargin.down != margin.down)
                    {
                        Resize();
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            var t = transform.GetChild(i);
                            var ss = t.GetComponent<SizeScaleEx>();
                            if (ss != null)
                                ss.EditorRefresh();
                        }
                    }
                }
                else if (sizeType == SizeType.Anchor)
                {
                    lastAnchorType = anchorType;
                    Resize();
                }
                else if (lastScaleType != scaleType)
                {
                    lastScaleType = scaleType;
                    Resize();
                }
            }
        }
    }


