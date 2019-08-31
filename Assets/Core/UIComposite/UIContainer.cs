using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class LinkerMod
    {
        public int index = -1;
        public object UI;
        public ModelElement main;
    }
    public class Linker
    {
        protected List<LinkerMod> buffer = new List<LinkerMod>();
        public virtual LinkerMod CreateUI() { return null; }
        public virtual void RefreshItem(object t, object u, int index) { }
        public void RecycleItem(LinkerMod mod)
        {
            buffer.Add(mod);
            mod.main.activeSelf = false;
        }
        public LinkerMod PopItem(int index)
        {
            for(int i=0;i<buffer.Count;i++)
                if(buffer[i].index==index)
                {
                    var t = buffer[i];
                    buffer.RemoveAt(i);
                    return t;
                }
            return null;
        }
        public void ClearIndex()
        {
            for (int i = 0; i < buffer.Count; i++)
                buffer[i].index = -1;
        }
    }
    /// <summary>
    /// 泛型连接器
    /// </summary>
    /// <typeparam name="T">模型</typeparam>
    /// <typeparam name="U">数据</typeparam>
    public class UILinker<T, U> : Linker where T : class, new() where U : class, new()
    {
        ModelElement model;
        public Action<T, U, int> ItemUpdate;
        UIContainer con;
        public UILinker(UIContainer container, ModelElement mod)
        {
            con = container;
            model = mod;
            container.linkers.Add(this);
        }
        public void AddData(U dat, float high)
        {
            con.AddData(this, dat, high);
        }
        public override LinkerMod CreateUI()
        {
            for(int i=0;i<buffer.Count;i++)
                if(buffer[i].index<0)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            LinkerMod mod = new LinkerMod();
            ModelElement me = new ModelElement();
            me.Load(model.ModData);
            mod.main = me;
            mod.UI = me.ComponentReflection<T>();
            return mod;
        }
        public override void RefreshItem(object t, object u, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(t as T, u as U, index);
        }
    }
    /// <summary>
    /// 对象型连接器，用用于热更新块
    /// </summary>
    public class ObjectLinker : Linker
    {
        ModelElement model;
        public Action<object, object, int> ItemUpdate;
        public Action<ObjectLinker, LinkerMod> ItemCreate;
        UIContainer con;
        public ObjectLinker(UIContainer container, ModelElement mod)
        {
            con = container;
            model = mod;
            container.linkers.Add(this);
        }
        public void AddData(object dat, float high)
        {
            con.AddData(this, dat, high);
        }
        public override LinkerMod CreateUI()
        {
            LinkerMod mod = new LinkerMod();
            ModelElement me = new ModelElement();
            me.Load(model.ModData);
            mod.main = me;
            if (ItemCreate != null)
                ItemCreate(this, mod);
            return mod;
        }
        public override void RefreshItem(object t, object u, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(t, u, index);
        }
    }
    public class UIContainer:ModelInital
    {
        class Item
        {
            public Linker linker;
            public LinkerMod mod;
            public object UI;
            public object Data;
            public BindingData binding;
            public int Index = -1;
            public Vector3 pos;
            public Vector2 size;
            public ModelElement main;
            public float offset;
            public float high;
        }
        class BindingData
        {
            public float offset;
            public float high;
            public object Data;
            public Linker linker;
        }
        public EventCallBack eventCall;
        public ModelElement View;
        public Action<UIContainer, Vector2> Scroll;
        List<Item> items;
        List<BindingData> datas=new List<BindingData>();
        public ModelElement model;
        public List<Linker> linkers = new List<Linker>();
        public UIContainer()
        {
        }
        public override void Initial(ModelElement mod)
        {
            items = new List<Item>();
            View = mod;
            eventCall = EventCallBack.RegEvent<EventCallBack>(mod);
            eventCall.AutoColor = false;
            eventCall.ForceEvent = true;
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => {
                if (o.VelocityY == 0)
                    OnScrollEnd(o);
                else Scrolling(o,s);
            };
            eventCall.Scrolling = Scrolling;
            eventCall.ScrollEndY = OnScrollEnd;
            eventCall.PointerUp = (o, e) => {
                if (o.VelocityY == 0)
                    OnScrollEnd(o);
            };
            model = mod;
            for (int i = 0; i < model.child.Count; i++)
                model.child[i].activeSelf = false;
        }
        public UILinker<T, U> RegLinker<T,U>(string ItemName)  where T : class, new() where U : class, new()
        {
            if (model == null)
                return null;
            var mod = model.Find(ItemName);
            if (mod == null)
                return null;
            UILinker<T, U> link = new UILinker<T, U>(this,mod);
            return link;
        }
        public int DataCount { get { return datas.Count; } }
        public void AddData(Linker linker, object data, float high)
        {
            BindingData binding = new BindingData();
            binding.linker = linker;
            binding.Data = data;
            binding.high = high;
            //binding.offset = end;
            datas.Add(binding);
        }
        Item CreateItem(BindingData data, int index)
        {
            var ui = data.linker.CreateUI();
            ui.index = index;
            data.linker.RefreshItem(ui.UI, data.Data, index);
            Item item = new Item();
            item.mod = ui;
            item.linker = data.linker;
            item.Index = index;
            item.binding = data;
            item.Data = data.Data;
            item.UI = ui.UI;
            //item.offset = data.offset;
            item.high = data.high;
            item.main = ui.main;
            var son = ui.main;
            son.SetParent(View);
            son.data.localScale = Vector3.one;
            son.IsChanged = true;
            return item;
        }
        void Scrolling(EventCallBack scroll, Vector2 offset)
        {
            if (datas.Count > 0)
            {
                BounceBack(scroll, ref offset);
                Calcul(offset.y);
            }
        }
        float GetDownLenth(float max)
        {
            int i = pointIndex;
            float d = datas[i].high*pointOffsetRatio;
            i--;
            for (; i >= 0; i--)
            {
                d += datas[i].high;
                if (d > max)
                    return max;
            }
            return d;
        }
        void OnScrollEnd(EventCallBack back)
        {
            if (datas.Count == 0)
                return;
            if (OutDown())
            {
                back.DecayRateY = 0.988f;
                float d = -datas[pointIndex].high*pointOffsetRatio;
                back.ScrollDistanceY = d * eventCall.Context.data.localScale.y;
            }
            else
            {
                if(OutTop())
                {
                    back.DecayRateY = 0.988f;
                    var item = datas[maxIndex];
                    float f = View.data.sizeDelta.y*0.5f;
                    float d = f + item.offset - item.high ;
                    d = GetDownLenth(d);
                    if (d > 0.01f)
                        back.ScrollDistanceY = -d * eventCall.Context.data.localScale.y;
                    else back.VelocityY = 0;
                }
            }
        }
        int pointIndex=0;//指向第一个显示的ui
        int maxIndex = 0;
        float pointOffsetRatio=0;//指向第一个显示ui的偏移百分比
        public void Move(float y)
        {
            if(datas.Count==1)
            {
                Calcul(0);
                return;
            }
            var item = datas[maxIndex];
            float h = View.data.sizeDelta.y;
            float top =0.5f* h - item.offset;
            top += 0.5f * item.high;
            float tar = top + y;
            if (tar < h)
                Calcul(0);
            else
            {
                float os = tar - h;
                if (y > os)
                    y = os;
                Calcul(y);
            }
        }
        void Calcul(float y)
        {
            var item = datas[pointIndex];//当前指向的数据
            float os = item.high * pointOffsetRatio;
            os += y;
            if (os > item.high)
            {
                if (pointIndex < datas.Count - 1)
                {
                    os -= item.high;
                    pointIndex++;
                    float h = datas[pointIndex].high;
                    pointOffsetRatio = os / h;
                }
                else pointOffsetRatio = os / item.high;
            }
            else if (os < 0)
            {
                if (pointIndex > 0)
                {
                    pointIndex--;
                    float h = datas[pointIndex].high;
                    os += h;
                    pointOffsetRatio = os / h;
                }
                else pointOffsetRatio = os / item.high;
            }
            else pointOffsetRatio = os / item.high;
            Order();
        }
        List<Item> buffer = new List<Item>();
        void Order()
        {
            for(int i=0;i<items.Count;i++)
            {
                var item = items[i];
                item.linker.RecycleItem(item.mod);
            }
            buffer.AddRange(items);
            items.Clear();
            float start = - datas[pointIndex].high * pointOffsetRatio;
            maxIndex = datas.Count - 1;
            for(int i=pointIndex;i<datas.Count;i++)
            {
                var item = datas[i];
                UpdateItem(item,i,start);
                start += item.high;
                if (start > model.data.sizeDelta.y)
                {
                    maxIndex = i;
                    break;
                }
            }
            for (int i = 0; i < buffer.Count; i++)
                buffer[i].Index = -1;
            for (int i = 0; i < linkers.Count; i++)
                linkers[i].ClearIndex();
        }
        Item FindOrCreateItem(int index)
        {
            for(int i=0;i<buffer.Count;i++)
                if(buffer[i].Index==index)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            for (int i = 0; i < buffer.Count; i++)
                if (buffer[i].Index <0)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            return new Item();
        }
        void UpdateItem(BindingData data, int index,float start)
        {
            var mod = data.linker.PopItem(index);
            if (mod == null)
            {
                mod = data.linker.CreateUI();
                mod.main.SetParent(View);
            }
            mod.main.activeSelf = true;
            var item = FindOrCreateItem(index);
            if(item.Index<0)
            {
                mod.index = index;
                data.linker.RefreshItem(mod.UI, data.Data, index);
                item.mod = mod;
                item.linker = data.linker;
                item.Index = index;
                item.binding = data;
                item.Data = data.Data;
                item.UI = mod.UI;
                //item.offset = data.offset;
                item.high = data.high;
                item.main = mod.main;
                var son = mod.main;
                son.SetParent(View);
                son.data.localScale = Vector3.one;
                son.IsChanged = true;
            }
            else
            {
                mod.index = index;
                item.mod = mod;
            }
            items.Add(item);
            float os = model.data.sizeDelta.y * 0.5f;
            os -= start;
            os -= data.high * 0.5f;
            mod.main.data.localPosition.y = os;
            mod.main.IsChanged = true;
            data.offset = os;
        }
        bool OutDown()
        {
            if (pointIndex == 0)
                if (pointOffsetRatio < 0)
                    return true;
            return false;
        }
        bool OutTop()
        {
            if (pointIndex == 0)
                if (pointOffsetRatio <= 0)
                    return false;
            if (maxIndex == datas.Count - 1)
            {
                var item = datas[maxIndex];
                float y = View.data.sizeDelta.y;
                float top = item.offset - item.high;
                if (top > y * -0.5f)
                    return true;
            }
            return false;
        }
        protected void BounceBack(EventCallBack eventCall, ref Vector2 v)
        {
            var Size = View.data.sizeDelta;
            if (eventCall.Pressed)
            {
                if (v.y < 0)
                {
                    if (OutDown())
                    {
                        float d = -datas[pointIndex].high * pointOffsetRatio;
                        float hf = View.data.sizeDelta.y * 0.5f;
                        float r = d / hf;
                        if (r > 1)
                            r = 1;
                        r = 1 - r;
                        v.y *=  r;
                        eventCall.VelocityY = 0;
                    }
                }
                else if (v.y > 0)
                {
                    if (OutTop())
                    {
                        var item = datas[pointIndex];
                        float f = View.data.sizeDelta.y;
                        f = GetDownLenth(f);
                        float d = f - item.offset + item.high * 0.5f;
                        float hf = f * 0.5f;
                        d /= hf;
                        if (d > 1)
                            d = 1;
                        else if (d < 0)
                            d = 0;
                        v.y *= d;
                        eventCall.VelocityY = 0;
                    }
                }
            }
            else
            {
                if (v.y < 0)
                {
                    if (eventCall.DecayRateY >= 0.95f)
                    {
                            if (OutDown())
                            {
                                eventCall.DecayRateY = 0.9f;
                                eventCall.VelocityY = eventCall.VelocityY;
                            }
                    }
                }
                else if (v.y > 0)
                {
                    if (eventCall.DecayRateY >= 0.95f)
                    {
                        if (OutTop())
                        {
                            var f = GetDownLenth(eventCall.ScrollDistanceY);
                            if(f<0.01f)
                            {
                                eventCall.VelocityY = 0;
                            }
                            else
                            {
                                eventCall.DecayRateY = 0.9f;
                                eventCall.VelocityY = eventCall.VelocityY;
                            }
                        }
                    }
                }
            }
        }
    }
}
