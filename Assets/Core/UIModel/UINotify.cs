using huqiang.UI;
using System;
using System.Collections.Generic;

public class UINotify : UIBase
{
    public static ModelElement Root { get; set; }
    public static UINotify Instance { get; private set; }
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
    List<PopWindow> pops;
    public UINotify()
    {
        pops = new List<PopWindow>();
        Instance = this;
    }
    public virtual void Show(object dat = null)
    {
    }
    public override void ReSize() {
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
    public void HidePopWindow()
    {
        for (int i = 0; i < pops.Count; i++)
            pops[i].Hide();
    }
    /// <summary>
    /// 释放掉当前未激活的弹窗
    /// </summary>
    public void ReleasePopWindow()
    {
        int c = pops.Count-1;
        for(;c>=0;c--)
        {
            var p = pops[c];
            if (p.model != null)
            { p.Dispose(); pops.RemoveAt(c); }
            else if (!p.model.activeSelf)
            { p.Dispose();pops.RemoveAt(c); }
        }
    }
    protected T ShowPopWindow<T>(object obj = null) where T : PopWindow, new()
    {
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
            {
                pops[i].Show(obj);
                return pops[i] as T;
            }
        var t = new T();
        pops.Add(t);
        t.Initial(Root, this, obj);
        t.ReSize();
        return t;
    }
    public T GetPopWindow<T>() where T : PopWindow
    {
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
                return pops[i] as T;
        return null;
    }
    public override void Update(float time)
    {
       for(int i=0;i<pops.Count;i++)
        {
            var p = pops[i];
            if (p.model != null)
                if (p.model.activeSelf)
                    p.Update(time);
        }
    }
}
