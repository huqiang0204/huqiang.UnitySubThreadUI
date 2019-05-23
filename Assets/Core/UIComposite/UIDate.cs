using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class UIDate  : ModelInital
    {
        class ItemView
        {
            public TextElement Item;
        }
        ModelElement model;
        EventCallBack callBack;
        ScrollY Year;
        ScrollY Month;
        ScrollY Day;
        public int StartYear=1800;
        public int EndYear=2400;
        int[] ys;
        string[] ms;
        List<string> Days;
        string unitY=" Year";
        string unitM=" Month";
        string unitD=" Day";
        public string YearUnit { get { return unitY; } set {
                unitY = value;
                var its = Year.Items;
                for(int i=0;i<its.Count;i++)
                {
                    var it = its[i];
                    var ele = Year.Items[i].obj as TextElement;
                    if(ele!=null)
                    {
                        ele.text = it.datacontext.ToString() + value;
                        ele.IsChanged = true;
                    }
                }
            } }
        public string MonUnit { get { return unitM; } set {
                unitM = value;
                var its = Month.Items;
                for (int i = 0; i < its.Count; i++)
                {
                    var it = its[i];
                    var ele = Year.Items[i].obj as TextElement;
                    if (ele != null)
                    {
                        ele.text = it.datacontext.ToString() + value;
                        ele.IsChanged = true;
                    }
                }
            } }
        public string DayUnit { get { return unitD; } set {
                unitD = value;
                var its = Day.Items;
                for (int i = 0; i < its.Count; i++)
                {
                    var it = its[i];
                    var ele = Year.Items[i].obj as TextElement;
                    if (ele != null)
                    {
                        ele.text = it.datacontext as string + value;
                        ele.IsChanged = true;
                    }
                }
            } }
        public override void Initial(ModelElement mod)
        {
            model = mod;
            var y= model.Find("Year");
            Year = new ScrollY();
            Year.Initial(y);
            Year.ItemObject = typeof(ItemView);
            Year.ItemUpdate = YearItem;
            Year.Scroll = Scrolling;
            Year.ScrollEnd = YearScrollToEnd;
            Year.ItemDockCenter = true;
            Year.scrollType = ScrollType.Loop;
            Year.eventCall.boxSize = new Vector2(120,160);
            Year.eventCall.UseAssignSize = true;

            var m = model.Find("Month");
            Month = new ScrollY();
            Month.Initial(m);
            Month.ItemObject = typeof(ItemView);
            Month.ItemUpdate = MonthItem;
            Month.Scroll = Scrolling;
            Month.ScrollEnd = MonthScrollToEnd;
            Month.ItemDockCenter = true;
            Month.scrollType = ScrollType.Loop;
            Month.eventCall.boxSize = new Vector2(120, 160);
            Month.eventCall.UseAssignSize = true;

            var d = model.Find("Day");
            Day = new ScrollY();
            Day.Initial(d);
            Day.ItemObject = typeof(ItemView);
            Day.ItemUpdate = DayItem;
            Day.Scroll = Scrolling;
            Day.ScrollEnd = DayScrollToEnd;
            Day.ItemDockCenter = true;
            Day.ScrollEnd = DayScrollToEnd;
            Day.scrollType = ScrollType.Loop;
            Day.eventCall.boxSize = new Vector2(120, 160);
            Day.eventCall.UseAssignSize = true;

            var fs = mod.GetExtand();
            if(fs!=null)
            {
                StartYear = fs[0];
                EndYear = fs[1];
                if (EndYear < StartYear)
                    EndYear = StartYear;
            }
            year = StartYear;
            month = 1;
            day = 1;
            int len = EndYear - StartYear;
            ys = new int[len];
            int s = StartYear;
            for (int i = 0; i < len; i++)
            { ys[i] = s; s++; }
            Year.BindingData = ys;
            Year.Refresh();
            ms = new string[12];
            for (int i = 0; i < 12; i++)
                ms[i] = (i + 1).ToString();
            Month.BindingData = ms;
            Month.Refresh();
           Days= new List<string>();
            for (int i = 0; i < 31; i++)
                Days.Add((i+1).ToString());
            Day.BindingData = Days;
            Day.Refresh();
            UpdateItems(Year);
            UpdateItems(Month);
            UpdateItems(Day);
        }
        void Scrolling(ScrollY scroll, Vector2 vector)
        {
            UpdateItems(scroll);
        }
        void UpdateItems(ScrollY scroll)
        {
            var items = scroll.Items;
            for(int i=0;i<items.Count;i++)
            {
                var mod = items[i].target;
                float h = mod.data.sizeDelta.y;
                float y = mod.data.localPosition.y;
                float angle = y / h * 15f;
                mod.data.localRotation = Quaternion.Euler(angle, 0, 0);

                var v = MathH.Tan2(90 - angle);
                mod.data.localPosition.y = v.y * 100;
                mod.IsChanged = true;
                var txt = (items[i].obj as ItemView).Item;
                var col = txt.color;
                angle /= 45;
                if (angle < 0)
                    angle = -angle;
                col.a = 1 - angle;
                txt.color = col;
            }
        }
        void YearItem(object obj,object dat,int index)
        {
            TextElement txt = (obj as ItemView).Item;
            txt.text = dat.ToString() + unitY;
        }
        void MonthItem(object obj, object dat, int index)
        {
            TextElement txt = (obj as ItemView).Item;
            txt.text = dat.ToString() + unitM;
        }
        void DayItem(object obj, object dat, int index)
        {
            TextElement txt = (obj as ItemView).Item;
            txt.text = dat as string + unitD;
        }
        void YearScrollToEnd(ScrollY scroll)
        {
            var item = ScrollY.GetCenterItem(scroll.Items);
            if (item == null)
                return;
            year = ys[item.index];
            RefreshDay();
        }
        void MonthScrollToEnd(ScrollY scroll)
        {
            var item= ScrollY.GetCenterItem(scroll.Items);
            month =item.index+1;
            RefreshDay();
        }
        void DayScrollToEnd(ScrollY scroll)
        {
            var item =ScrollY.GetCenterItem(scroll.Items);
            day = item.index + 1;
        }
        public int year;
        public int month=1;
        public int day=1;
        static int[] daysTable = new int[] { 31,28,31,30,31,30,31,31,30,31,30,31};
        void RefreshDay()
        {
            int a = daysTable[month-1];
            if (a == 1)
                if (year % 4 == 0)//闰二月
                    a++;
            Days.Clear();
            for (int i = 0; i <a; i++)
                Days.Add((i + 1).ToString());
            Day.Refresh(0,Day.Point);
            UpdateItems(Day);
        }
    }
}
