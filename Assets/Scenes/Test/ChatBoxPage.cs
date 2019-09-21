using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using UGUI;
using UnityEngine;

public class ChatBoxPage:UIPage
{
    class View
    {
        public UIContainer container;
        public ModelElement Left;
        public ModelElement Right;
        public ModelElement Center;
        public EventCallBack narrator;
        public EventCallBack hero;
        public EventCallBack supporter;
        public EventCallBack send;
        public TextInput input;
        public EventCallBack Last;
        public EventCallBack Next;
    }
    View view;
    class TextDialogue
    {
        public RawImageElement avatar;
        public TextElement name;
        public EmojiElement content;
        public ModelElement back;
    }
    class Narrator
    {
        public EmojiElement content;
    }
    class DialogueData
    {
        public string avatar;
        public string name;
        public string content;
        public Vector2 contentSize;
    }
    class NarratorData
    {
        public string content;
        public Vector2 contentSize;
    }
    UILinker<TextDialogue, DialogueData> LeftLink, RightLink;
    UILinker<Narrator, NarratorData> CenterLink;
    public override void Initial(ModelElement parent, object dat = null)
    {
        view = LoadUI<View>("baseUI", "con");
        base.Initial(parent, dat);
        InitialEvent();
        view.Left.activeSelf = false;
        view.Right.activeSelf = false;
        view.Center.activeSelf = false;
    }
    int index = 0;
    void InitialEvent()
    {
        LeftLink = new UILinker<TextDialogue, DialogueData>(view.container, view.Left);
        LeftLink.ItemUpdate = DialogueItemUpdate;
        RightLink = new UILinker<TextDialogue, DialogueData>(view.container, view.Right);
        RightLink.ItemUpdate = DialogueItemUpdate;
        CenterLink = new UILinker<Narrator, NarratorData>(view.container, view.Center);
        CenterLink.ItemUpdate = NarratorItemUpdate;
        view.send.Click = SendClick;
        view.narrator.Click = (o, e) => { index = 0; };
        view.hero.Click = (o, e) => { index = 1; };
        view.supporter.Click = (o, e) => { index = 2; };
        view.Last.Click = (o, e) => { LoadPage<ScrollExTestPage>(); };
        view.Next.Click = (o, e) => { LoadPage<AniTestPage>(); };
    }

    float y = 0;
    void InputValueChanged(TextInput input)
    {
        TextElement.AsyncGetSizeY("", new Vector2(800, 120), 28, new EmojiString(input.InputString), FontStyle.Normal,
            (o) => {
                var mod = input.TextCom.model;
                var h = o.y - mod.data.sizeDelta.y;
                if (o.y > y)
                {
                    if (h > 0)
                    {
                        mod.data.sizeDelta = o;
                        mod.data.localPosition.y += (o.y - y) / 2;
                        mod.IsChanged = true;
                    }
                    y = o.y;
                }
            });

    }
    void DialogueItemUpdate(TextDialogue txt, DialogueData dat, int index)
    {
        var model = txt.avatar.model.parent;
        model.data.sizeDelta.y = dat.contentSize.y + 70;
        txt.back.data.sizeDelta.x = dat.contentSize.x + 40;
        txt.back.data.sizeDelta.y = dat.contentSize.y + 20;
        txt.name.text = dat.name;
        txt.content.text = dat.content;
        ModelElement.ScaleSize(txt.avatar.model.parent);
    }
    void NarratorItemUpdate(Narrator nar, NarratorData dat, int index)
    {
        var mod = nar.content.model.parent;
        mod.data.sizeDelta.y = dat.contentSize.y + 80;
        nar.content.text = dat.content;
        ModelElement.ScaleSize(mod);
    }
    void SendClick(EventCallBack ele, UserAction action)
    {

        if (view.input.InputString == "")
            return;
        switch (index)
        {
            case 0:
                AddCenterData(view.input.InputString);
                break;
            case 1:
                AddRightData(view.input.InputString);
                break;
            case 2:
                AddLeftData(view.input.InputString);
                break;
        }
        view.input.InputString = "";

        if (view.input.InputString == "")
            return;
        switch (index)
        {
            case 0:
                AddCenterData(view.input.InputString);
                break;
            case 1:
                AddRightData(view.input.InputString);
                break;
            case 2:
                AddLeftData(view.input.InputString);
                break;
        }
        view.input.InputString = "";
        var mod = view.input.TextCom.model;
        mod.data.sizeDelta.y = 120;
        mod.data.localPosition.y = 0;
        y = 0;
        mod.IsChanged = true;

    }
    void AddLeftData(string content)
    {
        DialogueData dialogue = new DialogueData();
        dialogue.name = "配角";
        dialogue.content = content;
        TextElement.AsyncGetSizeY("", new Vector2(780, 70), 28, new EmojiString(dialogue.content), FontStyle.Normal,
            (o) => {
                dialogue.contentSize = o;
                float h = o.y + 70;
                if (h < 140)
                    h = 140;
                LeftLink.AddData(dialogue, h);
                view.container.Move(h);
            });
    }
    void AddRightData(string content)
    {
        DialogueData dialogue = new DialogueData();
        dialogue.name = "主角";
        dialogue.content = content;
        TextElement.AsyncGetSizeY("", new Vector2(780, 70), 28, new EmojiString(dialogue.content), FontStyle.Normal,
            (o) => {
                dialogue.contentSize = o;
                float h = o.y + 80;
                if (h < 140)
                    h = 140;
                RightLink.AddData(dialogue, h);
                view.container.Move(h);
            });
    }
    void AddCenterData(string content)
    {
        NarratorData dialogue = new NarratorData();
        dialogue.content = content;
        TextElement.AsyncGetSizeY("", new Vector2(780, 70), 28, new EmojiString(dialogue.content), FontStyle.Normal,
            (o) => {
                dialogue.contentSize = o;
                CenterLink.AddData(dialogue, o.y + 100);
                view.container.Move(o.y + 100);
            });
    }
}
