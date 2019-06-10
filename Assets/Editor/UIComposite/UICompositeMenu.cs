using huqiang.Data;
using huqiang.UIComposite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UICompositeMenu
{
    private const string icons = "icons";
    private const string background = "Background2";
    private const string file= "Pinned-Notices";
    private const string close = "Close";
    private const string list = "list";
    private const string line = "Line";
    private const string leaves = "Leaves";
    private const string ufo = "Ufo";
    private const string circleol = "Circle-Outline";
    private const string circlesm = "Circle-Small";
    [MenuItem("GameObject/UIComposite/UISliderH", false, 1)]
    static public void AddSliderH(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("SliderH", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 20);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        var help = ss.AddComponent<SliderHelper>();
        help.StartOffset.x = -15;
        help.EndOffset.x = -15;
        var image = ss.AddComponent<Image>();
        image.sprite = EditorModelManager.FindSprite(icons, background);
        image.type = Image.Type.Sliced;

        var Fill = new GameObject("FillImage", typeof(RectTransform));
        var fr = Fill.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 20);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        image = Fill.AddComponent<Image>();
        image.sprite = EditorModelManager.FindSprite(icons, background);
        image.type = Image.Type.Sliced;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.color = new Color32(94,137,197,255);

        var Nob = new GameObject("Nob", typeof(RectTransform));
        var fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(200, 0, 0);
        fn.localScale = Vector3.one;
        image = Nob.AddComponent<Image>();
        image.color = Color.green;
        image.sprite = EditorModelManager.FindSprite(icons,leaves);
    }
    [MenuItem("GameObject/UIComposite/UISliderV", false, 1)]
    static public void AddSliderV(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("SliderV", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(20,400);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        var help = ss.AddComponent<SliderHelper>();
        help.direction = UISlider.Direction.Vertical;
        help.StartOffset.y = -15;
        help.EndOffset.y = -15;
        var image = ss.AddComponent<Image>();
        image.sprite = EditorModelManager.FindSprite(icons, background);
        image.type = Image.Type.Sliced;

        var Fill = new GameObject("FillImage", typeof(RectTransform));
        var fr = Fill.transform as RectTransform;
        fr.sizeDelta = new Vector2(20, 400);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        image = Fill.AddComponent<Image>();
        image.sprite = EditorModelManager.FindSprite(icons, background);
        image.type = Image.Type.Sliced;
        image.fillMethod = Image.FillMethod.Vertical;
        image.color = new Color32(94, 137, 197, 255);

        var Nob = new GameObject("Nob", typeof(RectTransform));
        var fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, 200, 0);
        fn.localScale = Vector3.one;
        image = Nob.AddComponent<Image>();
        image.color = Color.green;
        image.sprite = EditorModelManager.FindSprite(icons, ufo);
    }
    [MenuItem("GameObject/UIComposite/Scroll", false, 2)]
    static public void AddScroll(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("Scroll", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 400);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();
        var Item = new GameObject("Item", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(80, 80);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
    }
    [MenuItem("GameObject/UIComposite/ScrollX", false, 3)]
    static public void AddScrollX(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var Scroll = new GameObject("ScrollX", typeof(RectTransform));
        if (parent != null)
            Scroll.transform.SetParent(parent.transform);
        Scroll.transform.localPosition = Vector3.zero;
        Scroll.transform.localScale = Vector3.one;

        var ss = new GameObject("Scroll", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.SetParent(Scroll.transform);
        rect.sizeDelta = new Vector2(400, 400);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();
        var Item = new GameObject("Item", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(80, 80);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;

        ss = new GameObject("Slider", typeof(RectTransform));
        var fn = ss.transform as RectTransform;
        fn.sizeDelta = new Vector2(400, 20);
        fn.SetParent(Scroll.transform);
        fn.localPosition = new Vector3(0, -190, 0);
        fn.localScale = Vector3.one;
        ss.AddComponent<Image>().type = Image.Type.Sliced;
        var help = ss.AddComponent<SliderHelper>();
        help.StartOffset.x = 15;
        help.EndOffset.x = 15;

        var Nob = new GameObject("Nob", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(ss.transform);
        fn.localPosition = new Vector3(-185, 0, 0);
        fn.localScale = Vector3.one;
        Nob.AddComponent<Image>();

    }
    [MenuItem("GameObject/UIComposite/ScrollY", false, 4)]
    static public void AddScrollY(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var Scroll = new GameObject("ScrollY", typeof(RectTransform));
        if (parent != null)
            Scroll.transform.SetParent(parent.transform);
        Scroll.transform.localPosition = Vector3.zero;
        Scroll.transform.localScale = Vector3.one;

        var ss = new GameObject("Scroll", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.SetParent(Scroll.transform);
        rect.sizeDelta = new Vector2(400, 400);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();
        var Item = new GameObject("Item", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(80, 80);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;

        ss = new GameObject("Slider", typeof(RectTransform));
        var fn = ss.transform as RectTransform;
        fn.sizeDelta = new Vector2(20, 400);
        fn.SetParent(Scroll.transform);
        fn.localPosition = new Vector3(190, 0, 0);
        fn.localScale = Vector3.one;
        ss.AddComponent<Image>().type = Image.Type.Sliced;
        var help = ss.AddComponent<SliderHelper>();
        help.direction = huqiang.UIComposite.UISlider.Direction.Vertical;
        help.StartOffset.y = 15;
        help.EndOffset.y = 15;

        var Nob = new GameObject("Nob", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(ss.transform);
        fn.localPosition = new Vector3(0, -185, 0);
        fn.localScale = Vector3.one;
        Nob.AddComponent<Image>();
    }
    [MenuItem("GameObject/UIComposite/UIRocker", false, 5)]
    static public void AddRocker(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("Rocker", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(300, 300);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        var img=  ss.AddComponent<Image>();
        img.sprite= EditorModelManager.FindSprite(icons, circleol);
        var Item = new GameObject("Nob", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(100, 100);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        img = Item.AddComponent<Image>();
        img.sprite = EditorModelManager.FindSprite(icons, circlesm);
    }
    [MenuItem("GameObject/UIComposite/UIPalette", false, 6)]
    static public void AddPalette(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var palette = new GameObject("Palette", typeof(RectTransform));
        RectTransform rect = palette.transform as RectTransform;
        rect.sizeDelta = new Vector2(500, 500);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        palette.AddComponent<RawImage>();

        var Fill = new GameObject("HTemplate", typeof(RectTransform));
        var fr = Fill.transform as RectTransform;
        fr.sizeDelta = new Vector2(256, 256);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        Fill.AddComponent<RawImage>();

        var Nob = new GameObject("NobA", typeof(RectTransform));
        var fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(44, 44);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -220, 0);
        fn.localScale = Vector3.one;
        Nob.AddComponent<Image>();

        Nob = new GameObject("NobB", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(24, 24);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(-128, 128, 0);
        fn.localScale = Vector3.one;
        Nob.AddComponent<Image>();

        var Slider = new GameObject("Slider", typeof(RectTransform));
        fn = Slider.transform as RectTransform;
        fn.sizeDelta = new Vector2(400, 20);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(0, -285, 0);
        fn.localScale = Vector3.one;
        Slider.AddComponent<Image>().type = Image.Type.Sliced;

        Nob = new GameObject("Nob", typeof(RectTransform));
        fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(Slider.transform);
        fn.localPosition = new Vector3(200, 0, 0);
        fn.localScale = Vector3.one;
        Nob.AddComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    }
    [MenuItem("GameObject/UIComposite/UIDate", false, 7)]
    static public void AddDate(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var date = new GameObject("Date", typeof(RectTransform));
        RectTransform rect = date.transform as RectTransform;
        rect.sizeDelta = new Vector2(360, 210);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        var now = DateTime.Now;
        var Year = new GameObject("Year", typeof(RectTransform));
        var fr = Year.transform as RectTransform;
        fr.sizeDelta = new Vector2(120, 210);
        fr.SetParent(rect);
        fr.localPosition = new Vector3(-120, 0, 0);
        fr.localScale = Vector3.one;
        Year.AddComponent<Image>();
        Year.AddComponent<Mask>().showMaskGraphic = false;

        var Item = new GameObject("Item", typeof(RectTransform));
        var fn = Item.transform as RectTransform;
        fn.sizeDelta = new Vector2(120, 30);
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        var txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = now.Year + " Year";
        txt.fontSize = 24;

        var Month = new GameObject("Month", typeof(RectTransform));
        fr = Month.transform as RectTransform;
        fr.sizeDelta = new Vector2(120, 210);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        Month.AddComponent<RectMask2D>();

        Item = new GameObject("Item", typeof(RectTransform));
        fn = Item.transform as RectTransform;
        fn.sizeDelta = new Vector2(120, 30);
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = now.Month + " Month";
        txt.fontSize = 24;

        var Day = new GameObject("Day", typeof(RectTransform));
        fr = Day.transform as RectTransform;
        fr.sizeDelta = new Vector2(120, 210);
        fr.SetParent(rect);
        fr.localPosition = new Vector3(120, 0, 0);
        fr.localScale = Vector3.one;
        Day.AddComponent<RectMask2D>();

        Item = new GameObject("Item", typeof(RectTransform));
        fn = Item.transform as RectTransform;
        fn.sizeDelta = new Vector2(120, 30);
        fn.SetParent(fr);
        fn.localPosition = Vector3.zero;
        fn.localScale = Vector3.one;
        txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = now.Day + " Day";
        txt.fontSize = 24;
    }
    [MenuItem("GameObject/UIComposite/TreeView", false, 8)]
    static public void AddTreeView(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("TreeView", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 400);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();
        var Item = new GameObject("Item", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(200, 40);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        var txt = Item.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleLeft;
        txt.fontSize = 24;
    }
    [MenuItem("GameObject/UIComposite/DropDownEx", false, 9)]
    static public void AddDropDown(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("DropDownEx", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 40);
        if (parent != null)
            rect.SetParent(parent.transform);

        var label = new GameObject("Label", typeof(RectTransform));
        var fr = label.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 40);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        var txt = label.AddComponent<Text>();
        txt.alignment = TextAnchor.MiddleLeft;
        txt.fontSize = 24;

        var Scroll = new GameObject("Scroll", typeof(RectTransform));
        RectTransform scr = Scroll.transform as RectTransform;
        scr.sizeDelta = new Vector2(400, 400);
        scr.SetParent(rect);
        scr.localPosition = new Vector3(0,-220,0);
        scr.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();

        var Item = new GameObject("Item", typeof(RectTransform));
        fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 40);
        fr.SetParent(scr);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
    }
    [MenuItem("GameObject/UIComposite/Layout", false, 10)]
    static public void AddLayout(MenuCommand menuCommand)
    {
        Sprite bk = EditorModelManager.FindSprite(icons,background);

        GameObject parent = menuCommand.context as GameObject;
        var layout = new GameObject("Layout", typeof(RectTransform));
        RectTransform rect = layout.transform as RectTransform;
        rect.sizeDelta = new Vector2(1920, 1080);
        if (parent != null)
            rect.SetParent(parent.transform);
        var sse= layout.AddComponent<SizeScaleEx>();
        sse.anchorType = AnchorType.Cneter;
        sse.sizeType = SizeType.Margin;
        sse.parentType = ParentType.Tranfrom;
        sse.DesignSize = new Vector2(1920,1080);

        var AreaLevel = new GameObject("AreaLevel", typeof(RectTransform));
        AreaLevel.transform.SetParent(rect);
        var LineLevel = new GameObject("LineLevel", typeof(RectTransform));
        LineLevel.transform.SetParent(rect);
        var Line= new GameObject("Line", typeof(RectTransform),typeof(Image));
        Line.transform.SetParent(rect);
        Line.GetComponent<Image>().color = new Color32(64,64,64,255);
        var Area = new GameObject("Area", typeof(RectTransform), typeof(Image));
        Area.transform.SetParent(rect);
        Area.GetComponent<Image>().color = Color.black;
        var Auxiliary = new GameObject("Auxiliary", typeof(RectTransform));
        Auxiliary.transform.SetParent(rect);
        var Content = new GameObject("Content", typeof(RectTransform));
        Content.transform.SetParent(Auxiliary.transform);
        var Head = new GameObject("Head", typeof(RectTransform),typeof(RectMask2D));
        Head.transform.SetParent(Auxiliary.transform);
        (Head.transform as RectTransform).sizeDelta = new Vector2(100,60);
        var Cover = new GameObject("Cover", typeof(RectTransform),typeof(RawImage));
        Cover.transform.SetParent(Auxiliary.transform);
        Cover.GetComponent<RawImage>().color = new Color32(128,128,128,128);

        var Docker = new GameObject("Docker",typeof(RectTransform));
        Docker.transform.SetParent(Auxiliary.transform);

        var Center = new GameObject("Center",typeof(RectTransform),typeof(Image));
        var st = Center.transform as RectTransform;
        st.SetParent(Docker.transform);
        st.localPosition = Vector3.zero;
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(100, 100);
        var img = Center.GetComponent<Image>();
        img.color = new Color32(59,87,255,128);
        img.sprite = bk;

        var Left = new GameObject("Left", typeof(RectTransform), typeof(Image));
        st = Left.transform as RectTransform;
        st.SetParent(Docker.transform);
        st.localPosition = new Vector3(-90,0,0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(60,100);
        img = Left.GetComponent<Image>();
        img.color = new Color32(59, 87, 255, 128);
        img.sprite = bk;

        var Top = new GameObject("Top", typeof(RectTransform), typeof(Image));
        st =Top.transform as RectTransform;
        st.SetParent(Docker.transform);
        st.localPosition = new Vector3(0,90,0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(100, 60);
        img = Top.GetComponent<Image>();
        img.color = new Color32(59, 87, 255, 128);
        img.sprite = bk;

        var Right = new GameObject("Right", typeof(RectTransform), typeof(Image));
        st = Right.transform as RectTransform;
        st.SetParent(Docker.transform);
        st.localPosition = new Vector3(90, 0, 0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(60, 100);
        img =Right.GetComponent<Image>();
        img.color = new Color32(59, 87, 255, 128);
        img.sprite =bk;

        var Down = new GameObject("Down", typeof(RectTransform), typeof(Image));
        st = Down.transform as RectTransform;
        st.SetParent(Docker.transform);
        st.localPosition = new Vector3(0, -90, 0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(100, 60);
        img = Down.GetComponent<Image>();
        img.color = new Color32(59, 87, 255, 128);
        img.sprite = bk;

        var Item = new GameObject("Item", typeof(RectTransform));
        Item.transform.SetParent(Auxiliary.transform);

        var Label = new GameObject("Label",typeof(RectTransform),typeof(Text));
        st = Label.transform as RectTransform;
        st.SetParent(Item.transform);
        st.localPosition = new Vector3(-20,0,0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(200,40);
        var txt= Label.GetComponent<Text>();
        txt.color = Color.white;
        txt.fontSize = 32;
        txt.text = "Label";
        txt.alignment = TextAnchor.MiddleLeft;

        var Close = new GameObject("Close", typeof(RectTransform), typeof(Image));
        st = Close.transform as RectTransform;
        st.SetParent(Item.transform);
        st.localPosition = new Vector3(100, 0, 0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(48, 48);
        img = Close.GetComponent<Image>();
        img.color = Color.white;
        img.sprite = EditorModelManager.FindSprite(icons, close);

        var Drag = new GameObject("Drag", typeof(RectTransform), typeof(Image));
        st = Drag.transform as RectTransform;
        st.SetParent(rect);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(60, 60);
        img = Drag.GetComponent<Image>();
        img.color = Color.green;
        img.sprite = EditorModelManager.FindSprite(icons,file);
        
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
    }
    [MenuItem("GameObject/UIComposite/DropdownEx", false, 11)]
    static public void AddDropdownEx(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var drop = new GameObject("DropdownEx", typeof(RectTransform));
        RectTransform dt =drop.transform as RectTransform;
        if (parent != null)
            dt.SetParent(parent.transform);
        dt.sizeDelta = new Vector2(400, 60);
        var Label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        var st = Label.transform as RectTransform;
        st.SetParent(dt);
        st.localPosition = new Vector3(-30, 0, 0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(340, 40);
        var txt = Label.GetComponent<Text>();
        txt.color = Color.white;
        txt.fontSize = 32;
        txt.text = "Label";
        txt.alignment = TextAnchor.MiddleCenter;

        var Close = new GameObject("Menu", typeof(RectTransform), typeof(Image));
        st = Close.transform as RectTransform;
        st.SetParent(dt);
        st.localPosition = new Vector3(170, 0, 0);
        st.localScale = new Vector3(1,1,1);
        st.sizeDelta = new Vector2(48, 36);
        var img = Close.GetComponent<Image>();
        img.color = Color.white;
        img.sprite = EditorModelManager.FindSprite(icons, list);

        var ss = new GameObject("Scroll", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 400);
        rect.SetParent(dt);
        rect.localPosition = new Vector3(0,-230,0);
        rect.localScale = Vector3.one;
        ss.AddComponent<RectMask2D>();
        var Item = new GameObject("Item", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 80);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;

        Label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        st = Label.transform as RectTransform;
        st.SetParent(fr);
        st.localPosition = new Vector3(-30, 0, 0);
        st.localScale = Vector3.one;
        st.sizeDelta = new Vector2(400, 60);
        txt = Label.GetComponent<Text>();
        txt.color = Color.white;
        txt.fontSize = 32;
        txt.text = "Label";
        txt.alignment = TextAnchor.MiddleCenter;

        dt.localPosition = Vector3.zero;
        dt.localScale = Vector3.one;
    }
}