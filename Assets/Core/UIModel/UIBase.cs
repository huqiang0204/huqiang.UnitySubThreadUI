using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UIBase
{
    static int point;
    static UIBase[] buff = new UIBase[1024];
    public static T GetUI<T>() where T : UIBase
    {
        for (int i = 0; i < point; i++)
            if (buff[i] is T)
                return buff[i] as T;
        return null;
    }
    public static List<T> GetUIs<T>() where T : UIBase
    {
        List<T> tmp = new List<T>();
        for (int i = 0; i < point; i++)
            if (buff[i] is T)
                tmp.Add(buff[i] as T);
        return tmp;
    }
    public static void ClearUI()
    {
        for (int i = 0; i < point; i++)
            buff[i] = null;
        point = 0;
    }
    int Index;
    public UIBase()
    {
        Index = point;
        buff[point] = this;
        point++;
    }
    public object DataContext;
    public ModelElement Parent { get; protected set; }
    public ModelElement model { get; protected set; }
    protected UIBase UIParent;
    public virtual void Initial(huqiang.UI.ModelElement parent, UIBase ui, object obj = null)
    {
        DataContext = obj;
        UIParent = ui;
        Parent = parent;
        if (parent != null)
            if (model != null)
                model.SetParent(parent);
    }
    public virtual void Dispose()
    {
        if (model != null)
            ModelManagerUI.RecycleElement(model);
        point--;
        if (buff[point] != null)
            buff[point].Index = Index;
        buff[Index] = buff[point];
        buff[point] = null;
    }
    public virtual void Save()
    {
    }
    public virtual void Recovery()
    {
    }
    public virtual object CollectionData()
    {
        return null;
    }
    public virtual void Cmd(string cmd, object dat)
    {
    }
    public virtual void Cmd(DataBuffer dat)
    {
    }
    public virtual void ReSize()
    {
        if (model != null)
            ModelElement.ScaleSize(model);
    }
    public virtual void Update(float time)
    {
    }
    public virtual void ChangeLanguage()
    {

    }
}
