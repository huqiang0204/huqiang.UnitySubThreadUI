using huqiang.Other;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class PaletteHelper:UICompositeHelp
{
    Palette palette;
    public void Initial()
    {
        palette = new Palette();
        palette.LoadHSVT(1);
        var hc = transform.Find("HTemplate");
        if (hc != null)
        {
            var template = hc.GetComponent<RawImage>();
            if(template!=null)
            template.texture = palette.texture;
        }
        var htemp = transform.GetComponent<RawImage>();
        if(htemp!=null)
            htemp.texture = Palette.LoadCTemplate();
        var sli= transform.Find("Slider");
        if(sli!=null)
        {
            var slider= sli.GetComponent<RawImage>();
            if(slider!=null)
                slider.texture = Palette.AlphaTemplate();
        }
        
    }
}