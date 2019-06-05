﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UICompositeMenu
{
    [MenuItem("GameObject/UIComposite/UISlider", false, 1)]
    static public void AddSlider(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        var ss = new GameObject("Slider", typeof(RectTransform));
        RectTransform rect = ss.transform as RectTransform;
        rect.sizeDelta = new Vector2(400, 20);
        if (parent != null)
            rect.SetParent(parent.transform);
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        var image = ss.AddComponent<Image>();
        var Fill = new GameObject("FillImage", typeof(RectTransform));
        var fr = Fill.transform as RectTransform;
        fr.sizeDelta = new Vector2(400, 20);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
        image = Fill.AddComponent<Image>();
        image.type = Image.Type.Sliced;
        var Nob = new GameObject("Nob", typeof(RectTransform));
        var fn = Nob.transform as RectTransform;
        fn.sizeDelta = new Vector2(30, 30);
        fn.SetParent(rect);
        fn.localPosition = new Vector3(-200, 0, 0);
        fn.localScale = Vector3.one;
        image = Nob.AddComponent<Image>();
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
    [MenuItem("GameObject/UIComposite/ScrollX", false, 2)]
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
    [MenuItem("GameObject/UIComposite/ScrollY", false, 3)]
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
    [MenuItem("GameObject/UIComposite/UIRocker", false, 4)]
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
        ss.AddComponent<Image>();
        var Item = new GameObject("Nob", typeof(RectTransform));
        var fr = Item.transform as RectTransform;
        fr.sizeDelta = new Vector2(100, 100);
        fr.SetParent(rect);
        fr.localPosition = Vector3.zero;
        fr.localScale = Vector3.one;
    }
    [MenuItem("GameObject/UIComposite/UIPalette", false, 5)]
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
        fn.localPosition = new Vector3(0, 220, 0);
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
    [MenuItem("GameObject/UIComposite/UIDate", false, 6)]
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
    [MenuItem("GameObject/UIComposite/TreeView", false, 7)]
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
    [MenuItem("GameObject/UIComposite/DropDownEx", false, 7)]
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
}