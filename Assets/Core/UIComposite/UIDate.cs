using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;

namespace huqiang.UIComposite
{
    public class UIDate  : ModelInital
    {
        ModelElement model;
        EventCallBack callBack;
        ScrollY Year;
        ScrollY Month;
        ScrollY Day;
        List<string> Days;
        public override void Initial(ModelElement mod)
        {
            model = mod;
            var y= model.Find("Year");
            Year = new ScrollY();
            Year.Initial(y);
            Year.ItemUpdate = YearItem;
            Year.scrollType = ScrollType.Loop;

            var m = model.Find("Month");
            Month = new ScrollY();
            Month.Initial(m);
            Month.ItemUpdate = MonthItem;
            Month.scrollType = ScrollType.Loop;

            var d = model.Find("Day");
            Day = new ScrollY();
            Day.Initial(d);
            Day.ItemUpdate = DayItem;
            Day.scrollType = ScrollType.Loop;

            int sy = 1800;
            int ey = 2400;
          
            int len = ey - sy;
            int[] ys = new int[len];
            for (int i = 0; i < len; i++)
            { ys[i] = sy; sy++; }
            Year.BindingData = ys;
            Year.Refresh();
            string[] ms = new string[12];
            for (int i = 0; i < 12; i++)
                ms[i] = (i + 1).ToString();
            Month.BindingData = ms;
            Month.Refresh();
           Days= new List<string>();
            for (int i = 0; i < 31; i++)
                Days.Add((i+1).ToString());
            Day.BindingData = Days;
            Day.Refresh();
        }
        void YearItem(object obj,object dat,int index)
        {

        }
        void MonthItem(object obj, object dat, int index)
        {

        }
        void DayItem(object obj, object dat, int index)
        {

        }
    }
}
