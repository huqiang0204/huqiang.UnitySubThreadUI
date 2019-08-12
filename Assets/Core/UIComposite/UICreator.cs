using huqiang.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UIComposite
{
    public class UICreator
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
        public static ScrollYS CreateScrollYS(string name)
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
            slider.data.sizeDelta = new Vector2(20, 400);
            slider.SetParent(mod);
            slider.data.localPosition = new Vector3(190, 0, 0);

            var img = slider.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = background;
            img.data.type = Image.Type.Sliced;
            img.color = img.data.color = new Color32(152, 152, 152, 255);

            var Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(slider);
            Nob.data.localPosition = new Vector3(0, 185, 0);
            img = Nob.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = circlesm;

            ScrollYS xS = new ScrollYS();
            xS.Initial(mod);
            return xS;
        }
        static public UIRocker CreateRocker(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(300, 300);
            var img = mod.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = circleol;

            var Item = ModelElement.CreateNew("Nob");
            Item.data.sizeDelta = new Vector2(100, 100);
            Item.SetParent(mod);
            img = Item.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = circlesm;

            UIRocker rocker = new UIRocker();
            rocker.Initial(mod);
            return rocker;
        }
        static public UIPalette CreatePalette(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(500, 500);
            mod.AddComponent<RawImageElement>();

            var Fill = ModelElement.CreateNew("HTemplate");
            Fill.data.sizeDelta = new Vector2(256, 256);
            Fill.SetParent(mod);
            Fill.AddComponent<RawImageElement>();

            var Nob = ModelElement.CreateNew("NobA");
            Nob.data.sizeDelta = new Vector2(44, 44);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(0, -220, 0);
            var img = Nob.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = Aim;

            Nob = ModelElement.CreateNew("NobB");
            Nob.data.sizeDelta = new Vector2(24, 24);
            Nob.SetParent(mod);
            Nob.data.localPosition = new Vector3(-128, 128, 0);
            img = Nob.AddComponent<ImageElement>();
            img.textureName = icons;
            img.spriteName = Aim;

            var Slider = ModelElement.CreateNew("Slider");
            Slider.data.sizeDelta = new Vector2(400, 20);
            Slider.SetParent(mod);
           Slider.data.localPosition = new Vector3(0, -285, 0);
            Slider.AddComponent<RawImageElement>();

            Nob = ModelElement.CreateNew("Nob");
            Nob.data.sizeDelta = new Vector2(30, 30);
            Nob.SetParent(Slider);
            Nob.data.localPosition = new Vector3(200, 0, 0);
            img = Nob.AddComponent<ImageElement>();
            img.color = img.data.color = new Color(1, 1, 1, 1f);
            img.textureName = icons;
            img.spriteName = Aim;
            UIPalette palette = new UIPalette();
            palette.Initial(mod);

            return palette;
        }
        static public UIDate CreateDate(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(360, 210);

            var now = DateTime.Now;
            var Year = ModelElement.CreateNew("Year");
            Year.data.sizeDelta = new Vector2(120, 210);
            Year.SetParent(mod);
            Year.data.localPosition = new Vector3(-120, 0, 0);
            Year.AddComponent<ImageElement>();
            Year.AddComponent<RectMaskElement>();

            var Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(120, 30);
            Item.SetParent(Year);
            var txt = Item.AddComponent<TextElement>();
            txt.data.alignment = TextAnchor.MiddleCenter;
            txt.text = now.Year + " Year";
            txt.data.fontSize = 24;

            var Month = ModelElement.CreateNew("Month");
            Month.data.sizeDelta = new Vector2(120, 210);
            Month.SetParent(mod);
            Month.AddComponent<RectMaskElement>();

            Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(120, 30);
            Item.SetParent(Month);
            txt = Item.AddComponent<TextElement>();
            txt.data.alignment = TextAnchor.MiddleCenter;
            txt.text = now.Month + " Month";
            txt.data.fontSize = 24;

            var Day = ModelElement.CreateNew("Day");
            Day.data.sizeDelta = new Vector2(120, 210);
            Day.SetParent(mod);
            Day.data.localPosition = new Vector3(120, 0, 0);
            Day.AddComponent<RectMaskElement>();

            Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(120, 30);
            Item.SetParent(Day);
            txt = Item.AddComponent<TextElement>();
            txt.data.alignment = TextAnchor.MiddleCenter;
            txt.text = now.Day + " Day";
            txt.data.fontSize = 24;

            UIDate date = new UIDate();
            date.Initial(mod);
            return date;
        }
        static public TreeView CreateTreeView(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 400);
            mod.AddComponent<RectMaskElement>();

            var Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(200, 40);
            Item.SetParent(mod);
            var txt = Item.AddComponent<TextElement>();
            txt.data.alignment = TextAnchor.MiddleLeft;
            txt.data.fontSize = 24;

            TreeView tree = new TreeView();
            tree.Initial(mod);
            return tree;
        }
        static public DockPanel CreateLayout(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(1920, 1080);
            mod.data.anchorType = AnchorType.Cneter;
            mod.data.sizeType = SizeType.Margin;
            mod.data.parentType = ParentType.Tranfrom;
            mod.data.DesignSize = new Vector2(1920, 1080);
            mod.data.SizeScale = true;

            var AreaLevel = ModelElement.CreateNew("AreaLevel");
            AreaLevel.SetParent(mod);
            var LineLevel = ModelElement.CreateNew("LineLevel");
            LineLevel.SetParent(mod);
            var Line = ModelElement.CreateNew("Line");
            Line.SetParent(mod);
            var img= Line.AddComponent<ImageElement>();
            img.color = img.data.color = new Color32(64, 64, 64, 255);
            var Area = ModelElement.CreateNew("Area");
            Area.SetParent(mod);
            img = Area.GetComponent<ImageElement>();
            img.color = img.data.color = Color.black;
            var Auxiliary = ModelElement.CreateNew("Auxiliary");
            Auxiliary.SetParent(mod);
            var Content = ModelElement.CreateNew("Content");
            Content.SetParent(Auxiliary);
            var Head = ModelElement.CreateNew("Head");
            Head.SetParent(Auxiliary);
            Head.data.sizeDelta = new Vector2(100, 60);
            Head.AddComponent<RectMaskElement>();
            var Cover = ModelElement.CreateNew("Cover");
            Cover.SetParent(Auxiliary);
            var raw = Cover.AddComponent<RawImageElement>();
            raw.color = raw.data.color = new Color32(128, 128, 128, 128);

            var Docker = ModelElement.CreateNew("Docker");
            Docker.SetParent(Auxiliary);

            var Center = ModelElement.CreateNew("Center");
            Center.SetParent(Docker);
            Center.data.sizeDelta = new Vector2(100, 100);
            img = Center.AddComponent<ImageElement>();
            img.color = img.data.color = new Color32(59, 87, 255, 128);
            img.textureName = icons;
            img.spriteName = background;

            var Left = ModelElement.CreateNew("Left");
            Left.SetParent(Docker);
            Left.data.localPosition = new Vector3(-90, 0, 0);
            Left.data.sizeDelta = new Vector2(60, 100);
            img = Left.AddComponent<ImageElement>();
            img.color = img.data.color = new Color32(59, 87, 255, 128);
            img.textureName = icons;
            img.spriteName = background;

            var Top = ModelElement.CreateNew("Top");
            Top.SetParent(Docker);
            Top.data.localPosition = new Vector3(0, 90, 0);
            Top.data.sizeDelta = new Vector2(100, 60);
            img = Top.AddComponent<ImageElement>();
            img.color = img.data.color = new Color32(59, 87, 255, 128);
            img.textureName = icons;
            img.spriteName = background;

            var Right = ModelElement.CreateNew("Right");
            Right.SetParent(Docker);
            Right.data.localPosition = new Vector3(90, 0, 0);
            Right.data.sizeDelta = new Vector2(60, 100);
            img = Right.AddComponent<ImageElement>();
            img.color= img.data.color = new Color32(59, 87, 255, 128);
            img.textureName = icons;
            img.spriteName = background;

            var Down = ModelElement.CreateNew("Down");
            Down.SetParent(Docker);
            Down.data.localPosition = new Vector3(0, -90, 0);
            Down.data.sizeDelta = new Vector2(100, 60);
            img = Down.AddComponent<ImageElement>();
            img.color = img.data.color = new Color32(59, 87, 255, 128);
            img.textureName = icons;
            img.spriteName = background;

            var Item = ModelElement.CreateNew("Item");
            Item.SetParent(Auxiliary);

            var Label = ModelElement.CreateNew("Label");
            Label.SetParent(Item);
            Label.data.localPosition = new Vector3(-20, 0, 0);
            Label.data.sizeDelta = new Vector2(200, 40);
            var txt = Label.AddComponent<TextElement>();
            txt.color = txt.data.color = Color.white;
            txt.data.fontSize = 32;
            txt.text = "Label";
            txt.data.alignment = TextAnchor.MiddleLeft;

            var Close = ModelElement.CreateNew("Close");
            Close.SetParent(Item);
            Close.data.localPosition = new Vector3(100, 0, 0);
            Close.data.sizeDelta = new Vector2(48, 48);
            img = Close.AddComponent<ImageElement>();
            img.color = img.data.color = Color.white;
            img.textureName = icons;
            img.spriteName = close;

            var Drag = ModelElement.CreateNew("Drag");
            Drag.SetParent(mod);
            Drag.data.sizeDelta = new Vector2(60, 60);
            img = Drag.AddComponent<ImageElement>();
            img.color = img.data.color = Color.green;
            img.textureName = icons;
            img.spriteName = file;

            DockPanel layout = new DockPanel();
            layout.Initial(mod);
            return layout;
        }
        static public DropdownEx AddDropdownEx(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(400, 60);
            var Label = ModelElement.CreateNew("Label");
            Label.SetParent(mod);
            Label.data.localPosition = new Vector3(-30, 0, 0);
            Label.data.sizeDelta = new Vector2(340, 40);
            var txt = Label.AddComponent<TextElement>();
            txt.color = txt.data.color = Color.white;
            txt.data.fontSize = 32;
            txt.text = "Label";
            txt.data.alignment = TextAnchor.MiddleCenter;

            var Close = ModelElement.CreateNew("Menu");
            Close.SetParent(mod);
            Close.data.localPosition = new Vector3(170, 0, 0);
            Close.data.sizeDelta = new Vector2(48, 36);
            var img = Close.GetComponent<ImageElement>();
            img.color = img.data.color = Color.white;
            img.textureName = icons;
            img.spriteName = list;

            var ss = ModelElement.CreateNew("Scroll");
            ss.data.sizeDelta = new Vector2(400, 400);
            ss.SetParent(mod);
            ss.data.localPosition = new Vector3(0, -230, 0);
            ss.AddComponent<RectMaskElement>();
            var Item = ModelElement.CreateNew("Item");
            Item.data.sizeDelta = new Vector2(400, 80);
            Item.SetParent(ss);

            Label = ModelElement.CreateNew("Label");
            Label.SetParent(Item);
            Label.data.localPosition = new Vector3(-30, 0, 0);
            Label.data.sizeDelta = new Vector2(400, 60);
            txt = Label.AddComponent<TextElement>();
            txt.color =txt.data.color = Color.white;
            txt.data.fontSize = 32;
            txt.text = "Label";
            txt.data.alignment = TextAnchor.MiddleCenter;

            DropdownEx drop = new DropdownEx();
            drop.Initial(mod);
            return drop;
        }
        static public ModelElement CreateEmojiText(string name)
        {
            var mod = ModelElement.CreateNew(name);
            mod.data.sizeDelta = new Vector2(200, 40);
            var et = mod.AddComponent<EmojiElement>();
            return mod;
        }
    }
}
