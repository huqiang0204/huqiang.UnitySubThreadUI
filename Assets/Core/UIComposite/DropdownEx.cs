using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DropdownEx : ModelInital
    {
        public class PopItemMod
        {
            public EventCallBack Item;
            public TextElement Label;
            public ModelElement Checked;
            [NonSerialized]
            public object data;
            [NonSerialized]
            public int Index;
        }
        public ModelElement main;
        ModelElement Label;
        ScrollY m_scroll;
        public ScrollY scrollY
        {
            get { return m_scroll; }
            set
            {
                m_scroll = value;
                if (value == null)
                    return;
                ItemSize = m_scroll.ItemSize;
                MaxHeight = m_scroll.Model.data.sizeDelta.y;
                m_scroll.Model.activeSelf = false;
            }
        }
        public ModelElement ItemMod;
        IList DataList;
        public object BindingData { get { return DataList; } set { DataList = value as IList; } }
        public bool down = true;
        public float MaxHeight = 300;
        public float PopOffset = 0;
        public Vector2 ItemSize;
        int s_index;
        public EventCallBack callBack;
        public int SelectIndex
        {
            get { return s_index; }
            set
            {
                if (BindingData == null)
                    return;
                if (value < 0)
                {
                    s_index = -1;
                    if (ShowLabel != null)
                        ShowLabel.text = "";
                    return;
                }
                if (value >= DataList.Count)
                    value = DataList.Count - 1;
                s_index = value;
                if (ShowLabel != null)
                {
                    var dat = DataList[s_index];
                    if (dat is string)
                        ShowLabel.text = dat as string;
                    else ShowLabel.text = dat.ToString();
                }
            }
        }
        public override void Initial(ModelElement mod)
        {
            main = mod;
            Label = mod.Find("Label");
            ShowLabel = Label.GetComponent<TextElement>();
            callBack = EventCallBack.RegEvent<EventCallBack>(mod);
            callBack.Click = Show;
            var scroll = mod.Find("Scroll");
            if(scroll!=null)
            {
                m_scroll = new ScrollY();
                m_scroll.Initial(scroll);
                scroll.activeSelf = false;
                ItemSize = m_scroll.ItemSize;
                MaxHeight = m_scroll.Model.data.sizeDelta.y;
            }
        }
        void Show(EventCallBack back, UserAction action)
        {
            if (m_scroll != null)
            {
                if (ItemMod != null)
                    m_scroll.ItemMod = ItemMod.ModData;
                m_scroll.BindingData = BindingData;
                m_scroll.SetItemUpdate<PopItemMod,object>(ItemUpdate);
                m_scroll.eventCall.LostFocus = LostFocus;
                m_scroll.eventCall.DataContext = this;

                main.SetSiblingIndex(10000);
                Dock();
                action.AddFocus(m_scroll.eventCall);
            }
        }
        void Dock()
        {
            if (BindingData == null)
                return;
            m_scroll.Model.activeSelf = true;
            float x = main.data.sizeDelta.x;
            int c = DataList.Count;
            float height = c * ItemSize.y;
            if (height > MaxHeight)
                height = MaxHeight;
            scrollY.Model.data.sizeDelta = new Vector2(x, height);

            var y = main.data.sizeDelta.y * 0.5f + height * 0.5f;
            var t = scrollY.Model;
            t.SetParent(main);
            if (down)
                t.data.localPosition = new Vector3(PopOffset, -y, 0);
            else t.data.localPosition = new Vector3(PopOffset, y, 0);
            ItemSize.x = x;
            scrollY.ItemSize = ItemSize;
            float h = ItemSize.y * SelectIndex;
            scrollY.Refresh(0, h);
        }
        public Action<DropdownEx, object> OnSelectChanged;
        public TextElement ShowLabel;

        void LostFocus(EventCallBack eve, UserAction action)
        {
            m_scroll.Model.activeSelf = false;
        }
        ModelElement Checked;
        void ItemUpdate(PopItemMod g,object o, int index)
        {
            PopItemMod button = g as PopItemMod;
            if (button == null)
                return;

            if (button.Item != null)
            {
                var m = button.Item.Context.child[0];
                m.data.sizeDelta = new Vector2(ItemSize.x - 20, ItemSize.y - 10);
                m.IsChanged = true;
            }
            if (button.Label != null)
            {
                var m = button.Label.model;
                m.data.sizeDelta = new Vector2(ItemSize.x - 20, ItemSize.y - 10);
                m.IsChanged = true;
            }
            button.Index = index;
            button.data = o;
            if (button.Item != null)
            {
                button.Item.DataContext = button;
                button.Item.Click = ItemClick;
            }
            if (button.Label != null)
            {
                if (o is string)
                    button.Label.text = o as string;
                else button.Label.text = o.ToString();
            }
            if (button.Checked != null)
            {
                if (index == SelectIndex)
                {
                    button.Checked.activeSelf = true;
                    Checked = button.Checked;
                }
                else button.Checked.activeSelf = false;
            }
        }
        void ItemClick(EventCallBack eventCall, UserAction action)
        {
            if (Checked != null)
                Checked.activeSelf = false;
            PopItemMod mod = eventCall.DataContext as PopItemMod;
            if (mod == null)
                return;
            if (mod.Checked != null)
                mod.Checked.activeSelf = true;
            SelectIndex = mod.Index;
            if (ShowLabel != null)
            {
                if (mod.data is string)
                    ShowLabel.text = mod.data as string;
                else ShowLabel.text = mod.data.ToString();
            }
            if (OnSelectChanged != null)
                OnSelectChanged(this, mod.data);
            scrollY.Model.activeSelf = false;
        }
    }
}
