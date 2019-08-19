using huqiang.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu: UIBase
{
    public static ModelElement Root { get; set; }
    public static UIMenu Instance { get; set; }
    public static void UpdateData(string cmd, object obj)
    {
        if (Instance != null)
            Instance.Cmd(cmd, obj);
    }
    public static void Refresh(float time)
    {
        if (Instance != null)
            Instance.Update(time);
    }
    List<MenuWindow> pops;
    public MenuWindow currentPop { get; private set; }
    public UIMenu()
    {
        pops = new List<MenuWindow>();
        Instance = this;
    }
    public override void ReSize()
    {
        for (int i = 0; i < pops.Count; i++)
        {
            var p = pops[i];
            if (p.model != null)
                p.ReSize();
        }
    }
    public override void Dispose()
    {
        if (pops != null)
            for (int i = 0; i < pops.Count; i++)
                pops[i].Dispose();
        pops.Clear();
        ModelManagerUI.RecycleElement(model);
        ClearUI();
    }
    public void HideMenu()
    {
        for (int i = 0; i < pops.Count; i++)
            pops[i].Hide();
    }
    /// <summary>
    /// 释放掉当前未激活的弹窗
    /// </summary>
    public void ReleaseMune()
    {
        int c = pops.Count - 1;
        for (; c >= 0; c--)
        {
            var p = pops[c];
            if (p.model != null)
            { p.Dispose(); pops.RemoveAt(c); }
            else if (!p.model.activeSelf)
            { p.Dispose(); pops.RemoveAt(c); }
        }
    }
    public T ShowMenu<T>(UIBase context, Vector2 pos, object obj = null) where T : MenuWindow, new()
    {
        if (currentPop != null)
        { currentPop.Hide(); currentPop = null; }
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
            {
                currentPop = pops[i];
                pops[i].Show(context, pos,obj);
                return pops[i] as T;
            }
        var t = new T();
        pops.Add(t);
        currentPop = t;
        t.Initial(Root, context, obj);
        t.Show(context,pos,obj);
        t.ReSize();
        return t;
    }
    public T GetMenu<T>() where T : PopWindow
    {
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
                return pops[i] as T;
        return null;
    }
    public override void Update(float time)
    {
        for (int i = 0; i < pops.Count; i++)
        {
            var p = pops[i];
            if (p.model != null)
                if (p.model.activeSelf)
                    p.Update(time);
        }
    }
}
