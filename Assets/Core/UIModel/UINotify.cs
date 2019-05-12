using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UINotify : UIBase
{
    public static ModelElement Root { get; set; }
    public static UINotify CurrentPage { get; private set; }
    public static void LoadPage<T>(object dat = null) where T : UINotify, new()
    {
        if (CurrentPage is T)
        {
            CurrentPage.Show(dat);
            return;
        }
        if (CurrentPage != null)
        {
            CurrentPage.Save();
            CurrentPage.Dispose();
        }
        var t = new T();
        t.Initial(Root, dat);
        t.ReSize();
        CurrentPage = t;
    }
    public static void UpdateData(string cmd, object obj)
    {
        if (CurrentPage != null)
            CurrentPage.Cmd(cmd, obj);
    }
    public static void Refresh(float time)
    {
        if (CurrentPage != null)
            CurrentPage.Update(time);
    }

    public UINotify()
    {
        pops = new List<PopWindow>();
    }
    protected Type BackPage;
    protected Type BackPop;
    protected object BackData;
    public PopWindow currentPop { get; private set; }
    public virtual void Initial(ModelElement parent, object dat = null)
    {
        Parent = parent;
        DataContext = dat;
        if (parent != null)
            if (model != null)
                model.SetParent(parent);
    }
    public virtual void Initial(ModelElement parent, object dat = null, Type back = null, Type pop = null, object backData = null)
    {
        Initial(parent, dat);
        BackPage = back;
        BackPop = pop;
        BackData = backData;
    }
    public virtual void Show(object dat = null)
    {
    }
    public override void ReSize() { base.ReSize(); if (currentPop != null) currentPop.ReSize(); }
    public override void Dispose()
    {
        if (pops != null)
            for (int i = 0; i < pops.Count; i++)
                pops[i].Dispose();
        pops.Clear();
        currentPop = null;
        ModelManagerUI.RecycleElement(model);
        ClearUI();
    }
    public void HidePopWindow()
    {
        if (currentPop != null)
        {
            currentPop.Hide();
        }
        currentPop = null;
    }
    List<PopWindow> pops;
    protected T ShowPopWindow<T>(object obj = null, ModelElement parent = null) where T : PopWindow, new()
    {
        if (currentPop != null)
        { currentPop.Hide(); currentPop = null; }
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
            {
                currentPop = pops[i];
                pops[i].Show(obj);
                return pops[i] as T;
            }
        var t = new T();
        pops.Add(t);
        currentPop = t;
        if (parent == null)
            t.Initial(Parent, this, obj);
        else t.Initial(parent, this, obj);
        t.ReSize();
        return t;
    }
    protected object ShowPopWindow(Type type, object obj = null, ModelElement parent = null)
    {
        if (currentPop != null)
        { currentPop.Hide(); currentPop = null; }
        for (int i = 0; i < pops.Count; i++)
            if (pops[i].GetType() == type)
            {
                currentPop = pops[i];
                pops[i].Show(obj);
                return pops[i];
            }
        var t = Activator.CreateInstance(type) as PopWindow;
        pops.Add(t);
        currentPop = t;
        if (parent == null)
            t.Initial(Parent, this, obj);
        else t.Initial(parent, this, obj);
        t.ReSize();
        return t;
    }
    public virtual T PopUpWindow<T>(object obj = null) where T : PopWindow, new()
    {
        return ShowPopWindow<T>(obj, null);
    }
    object PopUpWindow(Type type, object obj = null)
    {
        var pop = ShowPopWindow(type, obj, null) as PopWindow;
        pop.Recovery();
        return pop;
    }
    /// <summary>
    /// 释放掉当前未激活的弹窗
    /// </summary>
    public void ReleasePopWindow()
    {
        if (pops != null)
            for (int i = 0; i < pops.Count; i++)
                if (pops[i] != currentPop)
                    pops[i].Dispose();
        pops.Clear();
        if (currentPop != null)
            pops.Add(currentPop);
    }
    public void ReleasePopWindow(PopWindow window)
    {
        pops.Remove(window);
        if (currentPop == window)
        {
            currentPop = null;
        }
        window.Dispose();
    }
    public void ReleasePopWindow<T>()
    {
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
            {
                pops[i].Dispose();
                pops.RemoveAt(i);
                break;
            }
        if (currentPop is T)
        {
            currentPop = null;
        }
    }
    public T GetPopWindow<T>() where T : PopWindow
    {
        for (int i = 0; i < pops.Count; i++)
            if (pops[i] is T)
            {
                return pops[i] as T;
            }
        return null;
    }
    public override void Update(float time)
    {
        if (currentPop != null)
            currentPop.Update(time);
    }
}
