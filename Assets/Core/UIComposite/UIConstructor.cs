using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UIComposite
{
    public class UIConstructor
    {
        private const string icons = "icons";
        private const string Aim = "Aim";
        private const string background = "Background2";
        private const string file = "Pinned-Notices";
        private const string close = "Close";
        private const string list = "list";
        private const string line = "Line";
        private const string leaves = "Leaves";
        private const string ufo = "Ufo";
        private const string circleol = "Circle-Outline";
        private const string circlesm = "Circle-Small";
        private const string magicstick = "Magic-Stick";
        static public UISlider CreateSliderH(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 20);

            var image = mod.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;

            var Fill = ModelElement.CreateNew("FillImage");
            Fill.data.sizeDelta = new Vector2(400, 20);
            Fill.SetParent(mod);
            image = Fill.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;
            image.data.fillMethod = Image.FillMethod.Horizontal;
            image.color =  image.data.color = new Color32(94, 137, 197, 255);

            var Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(200, 0, 0);
            image = Nob.AddComponent<ImageElement>();
            image.color = image.data.color = Color.green;
            image.textureName = icons;
            image.spriteName = leaves;

            var slider = new UISlider();
            slider.Initial(mod);
            slider.info.direction = UISlider.Direction.Horizontal;
            return slider;
        }
        static public UISlider CreateSliderV(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 20);

            var image = mod.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;

            var Fill = ModelElement.CreateNew("FillImage");
            Fill.data.sizeDelta = new Vector2(20, 400);
            Fill.SetParent(mod);
            image = Fill.AddComponent<ImageElement>();
            image.textureName = icons;
            image.spriteName = background;
            image.data.type = Image.Type.Sliced;
            image.data.fillMethod = Image.FillMethod.Horizontal;
            image.color = image.data.color = new Color32(94, 137, 197, 255);

            var Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(0, 200, 0);
            image = Nob.AddComponent<ImageElement>();
            image.color = image.data.color = Color.green;
            image.textureName = icons;
            image.spriteName = leaves;

            var slider = new UISlider();
            slider.Initial(mod);
            slider.info.direction = UISlider.Direction.Vertical;
            return slider;
        }
        static ModelElement CreateScroll(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 400);
            mod.AddComponent<MaskElement>();
            var Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(80, 80);
            Item.SetParent(mod);
            return mod;
        }
        public static ScrollX CreateSrollX(string name)
        {
            var mod = CreateScroll(name);
            var scroll = new ScrollX();
            scroll.Initial(mod);
            return scroll;
        }
        public static ScrollY CreateSrollY(string name)
        {
            var mod = CreateScroll(name);
            var scroll = new ScrollY();
            scroll.Initial(mod);
            return scroll;
        }
        public static ScrollXS CreateScrollXS(string name)
        {
            var mod = ModelElement.CreateNew(name);

            var scroll = ModelElement.CreateNew("Scroll");
            scroll.SetParent(mod);
            scroll.data.sizeDelta = new Vector2(400, 400);
            scroll.AddComponent<MaskElement>();

            var Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(80, 80);
            Item.SetParent(scroll);

            var slider = ModelElement.CreateNew("Slider");
            slider.data.sizeDelta = new Vector2(400, 20);
            slider.SetParent(mod);
            slider.data.localPosition = new Vector3(0, -190, 0);

            var img = slider.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = background;
            img.data.type = Image.Type.Sliced;
            img.color = img.data.color = new Color32(152, 152, 152, 255);

            var Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(slider);
            Nob.data.localPosition = new Vector3(-185, 0, 0);
            img = Nob.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = circlesm;

            ScrollXS xS = new ScrollXS();
            xS.Initial(mod);
            return xS;
        }
    }
}
