using huqiang.Data;
using huqiang.UIComposite;
using UnityEngine;

public class StackPanelHelper : UICompositeHelp
{
    public Direction direction;
    public Vector2 minSize;
    public bool autoSize = true;
    public void Refresh()
    {
        if (direction == Direction.Horizontal)
            OrderHorizontal();
        else OrderVertical();
    }
    void OrderHorizontal()
    {
        int c =  transform.childCount;
        float w = 0;
        for(int i=0;i<c;i++)
        {
            var rect = transform.GetChild(i)as RectTransform;
            w += rect.sizeDelta.x;
        }
        if (w < minSize.x)
            w = minSize.x;
        var v = (transform as RectTransform).sizeDelta;
        if (autoSize)
            v.x = w;
        else w = v.x;
        float sx = -0.5f * w;
        float y = v.y;
        (transform as RectTransform).sizeDelta = v;
        for (int i = 0; i < c; i++)
        {
            var rect = transform.GetChild(i) as RectTransform;
            v = rect.sizeDelta;
            v.y = y;
            rect.localPosition = new Vector3( sx + v.x * 0.5f,0,0);
            rect.sizeDelta = v;
            sx += v.x;
        }
    }
    void OrderVertical()
    {
        int c = transform.childCount;
        float h = 0;
        for (int i = 0; i < c; i++)
        {
            var rect = transform.GetChild(i) as RectTransform;
            h += rect.sizeDelta.y;
        }
        if (h < minSize.y)
            h = minSize.y;
        var v = (transform as RectTransform).sizeDelta;
        if (autoSize)
            v.y = h;
        else h = v.y;
        float sy = 0.5f * h;
        float x = v.x;
        (transform as RectTransform).sizeDelta = v;
        for (int i = 0; i < c; i++)
        {
            var rect = transform.GetChild(i) as RectTransform;
            v = rect.sizeDelta;
            v.x = x;
            rect.localPosition = new Vector3( 0, sy - v.y * 0.5f, 0);
            rect.sizeDelta = v;
            sy -= v.y;
        }
    }
    public unsafe override object ToBufferData(DataBuffer data)
    {
        FakeStruct fake = new FakeStruct(data, StackPanelData.ElementSize);
        StackPanelData* sp = (StackPanelData*)fake.ip;
        sp->direction = direction;
        sp->minSize = minSize;
        return fake;
    }
    public override void ReSize()
    {
        minSize = (transform as RectTransform).sizeDelta;
        Refresh();
    }
}
