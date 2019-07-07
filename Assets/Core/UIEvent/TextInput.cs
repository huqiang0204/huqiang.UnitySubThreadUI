using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UIEvent
{
    public partial class TextInput:EventCallBack
    {
        public enum ContentType
        {
            Standard,
            Autocorrected,
            IntegerNumber,
            DecimalNumber,
            Alphanumeric,
            Name,
            NumberAndName,
            EmailAddress,
            Password,
            Pin,
            Custom
        }
        public enum InputType
        {
            Standard,
            AutoCorrect,
            Password,
        }
        public enum LineType
        {
            SingleLine,
            MultiLineSubmit,
            MultiLineNewline
        }
        public enum CharacterValidation
        {
            None,
            Integer,
            Decimal,
            Alphanumeric,
            Name,
            numberAndName,
            EmailAddress,
            Custom
        }

        string m_TipString = "";
        public string InputString { get { return textInfo.text; }
            set {
                value = ValidateString(value);
                textInfo.buffer.FullString = value;
                textInfo.text = value;
                SetShowText();
            } }
        public string TipString
        {
            get { return m_TipString; }
            set
            {
                m_TipString = value;
                SetShowText();
            }
        }
        void SetShowText()
        {
            if (textInfo.text == ""|textInfo.text==null)
            {
                TextCom.text = m_TipString;
                TextCom.data.color = TipColor;
            }
            else
            {
                TextCom.text = textInfo.text;
                TextCom.data.color = textColor;
            }
        }
        public bool ReadOnly;
        Color textColor=Color.black;
        Color m_tipColor = new Color(0, 0, 0, 0.8f);
        public Color TipColor { get { return m_tipColor; } set { m_tipColor = value;} }
        public Color SelectionColor = new Color(0.65882f, 0.8078f, 1, 0.4f);
        public Func<TextInput, int, char, char> ValidateChar;
        public Action<TextInput> OnValueChanged;
        public Action<TextInput> OnSubmit;
        public Action<TextInput, UserAction> OnSelectChanged;
        public Action<TextInput, UserAction> OnSelectEnd;
        public InputType inputType = InputType.Standard;
        public LineType lineType = LineType.MultiLineNewline;
        ContentType m_ctpye;
        bool multiLine = true;
        public ContentType contentType
        {
            get { return m_ctpye; }
            set
            {
                m_ctpye = value;
                switch (value)
                {
                    case ContentType.Standard:
                        {
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.Autocorrected:
                        {
                            inputType = InputType.AutoCorrect;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.IntegerNumber:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NumberPad;
                            characterValidation = CharacterValidation.Integer;
                            break;
                        }
                    case ContentType.DecimalNumber:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NumbersAndPunctuation;
                            characterValidation = CharacterValidation.Decimal;
                            break;
                        }
                    case ContentType.Alphanumeric:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.ASCIICapable;
                            characterValidation = CharacterValidation.Alphanumeric;
                            break;
                        }
                    case ContentType.Name:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NamePhonePad;
                            characterValidation = CharacterValidation.Name;
                            break;
                        }
                    case ContentType.NumberAndName:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NamePhonePad;
                            characterValidation = CharacterValidation.numberAndName;
                            break;
                        }
                    case ContentType.EmailAddress:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.EmailAddress;
                            characterValidation = CharacterValidation.EmailAddress;
                            break;
                        }
                    case ContentType.Password:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Password;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.Pin:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Password;
                            touchType = TouchScreenKeyboardType.NumberPad;
                            characterValidation = CharacterValidation.Integer;
                            break;
                        }
                    default:
                        {
                            // Includes Custom type. Nothing should be enforced.
                            break;
                        }
                }
            }
        }
        public CharacterValidation characterValidation = CharacterValidation.None;
        public TouchScreenKeyboardType touchType = TouchScreenKeyboardType.Default;
        public int CharacterLimit = 0;
        public TextInput()
        {
            Click = OnClick;
            LostFocus = OnLostFocus;
        }
        protected override void Initial()
        {
            var txt= TextCom = Context.GetComponent<TextElement>();
            InputString = txt.text;
            textColor = txt.data.color;
        }
        public TextElement TextCom { get; private set; }
        public override void OnMouseDown(UserAction action)
        {
            if (TextCom != null)
            {
                textInfo.startSelect = GetPressIndex(textInfo, this, action,ref textInfo.startDock);
                Editing = true;
            }
            base.OnMouseDown(action);
        }
        int a = 0;
        protected override void OnDrag(UserAction action)
        {
            a++;
            if (a < 2)
                return;
            a = 0;
            if (Pressed)
                if (TextCom != null)
                {
                    if (entry)
                    {
                        if (action.Motion != Vector2.zero)
                        {
                            textInfo.CaretStyle = 2;
                            int end = textInfo.endSelect;
                            textInfo.endSelect = GetPressIndex(textInfo, this,  action,ref textInfo.endDock);
                            if (end !=textInfo.endSelect)
                            {
                                Selected();
                                if (OnSelectChanged != null)
                                    OnSelectChanged(this, action);
                                ThreadMission.InvokeToMain((o)=> { InputCaret.ChangeCaret(textInfo); },null);
                            }
                        }
                    }
                }
        }
        internal override void OnDragEnd(UserAction action)
        {
            long r = action.EventTicks - pressTime;
            if (r <= ClickTime)
            {
                float x = action.CanPosition.x;
                float y = action.CanPosition.y;
                x -= RawPosition.x;
                x *= x;
                y -= RawPosition.y;
                y *= y;
                x += y;
                if (x < ClickArea)
                    return;
            }
            textInfo.endSelect= GetPressIndex(textInfo, this,action,ref textInfo.endDock);
            Selected();
            if (OnSelectEnd != null)
                OnSelectEnd(this, action);
            ThreadMission.InvokeToMain((o) => { InputCaret.ChangeCaret(textInfo); }, null);
        }
        string ValidateString(string input)
        {
            if (CharacterLimit > 0)
                if (input.Length > CharacterLimit)
                    input = input.Substring(0,CharacterLimit);
            if (characterValidation == CharacterValidation.None)
                return input;
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<input.Length;i++)
            {
                if (Validate(characterValidation, sb.ToString(), i, input[i]) != 0)
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }
        string OnInputChanged(string input)
        {
            if (input == "")
                return "";
            EmojiString es = new EmojiString(input);
            string str = textInfo.buffer.FilterString;
            if (CharacterLimit > 0)
            {
                string fs = es.FilterString;
                if (fs.Length + str.Length > CharacterLimit)
                {
                    int len = CharacterLimit - str.Length;
                    if (len <= 0)
                        return "";
                    es.Remove(fs.Length - len, len);
                }
            }
            str = es.FullString;
            if (Validate(characterValidation, textInfo.text, textInfo.startSelect, str[0]) == 0)
                return "";
            if (ValidateChar != null)
                if (ValidateChar(this, textInfo.startSelect, str[0]) == 0)
                    return "";
            DeleteSelected(textInfo);
            textInfo.buffer.Insert(textInfo.startSelect,es);
            textInfo.startSelect += es.FilterString.Length;
            if (OnValueChanged != null)
                OnValueChanged(this);
            textInfo.text = textInfo.buffer.FullString;
            SetShowText();
            textInfo.CaretStyle = 1;
            ThreadMission.InvokeToMain(TextChanged, textInfo, ChangeApplyed);
            return input;
        }
        string TouchInputChanged(string input)
        {
            if (input == "")
                return "";
            textInfo.buffer= new EmojiString(input);
            if (OnValueChanged != null)
                OnValueChanged(this);
            textInfo.text = textInfo.buffer.FullString;
            SetShowText();
            textInfo.CaretStyle = 1;
            ThreadMission.InvokeToMain(TextChanged, textInfo, ChangeApplyed);
            return input;
        }
        void OnClick(EventCallBack eventCall, UserAction action)
        {
            TextInput input = eventCall as TextInput;
            if (input == null)
                return;
            InputEvent = input;
            textInfo.startSelect = GetPressIndex(textInfo, this, action,ref textInfo.startDock);
            textInfo.endSelect = -1;
            textInfo.CaretStyle = 1;
            ChangePoint(textInfo);
            ThreadMission.InvokeToMain((o) => {
                bool pass = InputEvent.contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(textInfo.text, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
                InputCaret.SetParent(Context.Context);
                InputCaret.ChangeCaret(textInfo);
            },null);
        }
        public bool Editing;
        void OnLostFocus(EventCallBack eventCall, UserAction action)
        {
            TextInput text = eventCall as TextInput;
            if (text == InputEvent)
            {
                if (InputEvent.OnSubmit != null)
                    InputEvent.OnSubmit(InputEvent);
                InputEvent = null;
            }
            Editing = false;
            SetShowText();
            ThreadMission.InvokeToMain((o)=> { Keyboard.EndInput(); },null);
        }
        public static void SetCurrentInput(TextInput input, UserAction action)
        {
            if (input == null)
                return;
            if (InputEvent == input)
                return;
            if (InputEvent != null)
               InputEvent.LostFocus(InputEvent, action);
            InputEvent = input;
            InputEvent.Editing = true;
        }

        void Selected()
        {
            textInfo.selectVertex.Clear();
            textInfo.selectTri.Clear();
            textInfo.CaretStyle = 2;
            GetChoiceArea(textInfo);
        }
        TextInfo textInfo = new TextInfo();
        void SetSelectPoint(int index)
        {
            if (index != 0)
            {
                index += textInfo.startSelect;
                if (index < 0)
                    index = 0;
                if (index > textInfo.text.Length)
                    index = textInfo.text.Length;
            }
            textInfo.startSelect = index;
            textInfo.endSelect = -1;
            textInfo.CaretStyle = 1;
            ChangePoint(textInfo);
            ThreadMission.InvokeToMain((o) => { InputCaret.ChangeCaret(textInfo); }, null);
        }
        void Delete(int dir)
        {
            if (DeleteSelected(textInfo))
                goto label;
            if (dir<0)
            {
                int index = textInfo.startSelect;
                if(index>0)
                {
                    index--;
                    textInfo.buffer.Remove(index,1);
                    textInfo.startSelect = index;
                }
            }
            else
            {
                int index = textInfo.startSelect;
                if(index<textInfo.buffer.Length)
                {
                    textInfo.buffer.Remove(index, 1);
                }
            }
            label:;
            textInfo.text = textInfo.buffer.FullString;
            textInfo.CaretStyle = 1;
            ThreadMission.InvokeToMain(TextChanged, textInfo, ChangeApplyed);
        }
        void TextChanged(object obj)
        {
            var te = TextCom;
            if (te.Context != null)
            {
                var text = te.Context;
                text.text = textInfo.text;
                SetShowText();
                string str = textInfo.buffer.FilterString;
                Populate(text, str);
                var g = text.cachedTextGenerator;
                UIVertex[] vert = new UIVertex[g.verts.Count];
                for (int i = 0; i < vert.Length; i++)
                    vert[i] = g.verts[i];
                textInfo.vertex = vert;
                UILineInfo[] us = new UILineInfo[g.lines.Count];
                for (int i = 0; i < us.Length; i++)
                    us[i] = g.lines[i];
                textInfo.lines = us;
                textInfo.characterCount = g.characterCount;
                textInfo.visibleCount = g.characterCountVisible;
            }
        }
        void ChangeApplyed(object obj)
        {
            ChangePoint(textInfo);
            ThreadMission.InvokeToMain((o) => {
                InputCaret.ChangeCaret(textInfo);
            }, null);
        }
    }
}