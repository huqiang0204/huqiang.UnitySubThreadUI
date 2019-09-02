using System;
using huqiang.Data;
using huqiang.UIEvent;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextInputHelper:UICompositeHelp
{
    public Color inputColor = Color.white;
    public Color tipColor = Color.gray;
    public Color pointColor=Color.white;
    public Color selectColor= new Color(0.65882f, 0.8078f, 1, 0.2f);
    public string InputString;
    public string TipString="请输入...";
    public unsafe override object ToBufferData(DataBuffer data)
    {
        FakeStruct fake = new FakeStruct(data, TextInputData.ElementSize);
        TextInputData* sp = (TextInputData*)fake.ip;
        sp->inputColor = inputColor;
        sp->tipColor = tipColor;
        sp->pointColor = pointColor;
        sp->selectColor = selectColor;
        sp->inputString = data.AddData(InputString);
        sp->tipString = data.AddData(TipString);
        return fake;
    }
    public void Refresh()
    {
        if(InputString==null|InputString=="")
        {
            var txt = GetComponent<Text>();
            txt.text = TipString;
            txt.color = tipColor;
        }
        else
        {
            var txt = GetComponent<Text>();
            txt.text = InputString;
            txt.color = inputColor;
        }
    }
}
