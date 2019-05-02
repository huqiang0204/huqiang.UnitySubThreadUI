using huqiang.UI;
using System;
using System.Collections.Generic;
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

        EmojiString emojiString =new EmojiString("");
        string m_TipString = "";
        public string InputString { get { return emojiString.FullString; }
            set {
                emojiString.FullString = value;
                UpdateText();
            } }
        public string TipString
        {
            get { return m_TipString; }
            set
            {
                m_TipString = value;
                var str = emojiString.FullString;
                if (str == null | str == "")
                    UpdateText();
            }
        }
        public bool ReadOnly;
        Color textColor=Color.black;
        Color m_tipColor = new Color(0, 0, 0, 0.8f);
        public Color TipColor { get { return m_tipColor; } set { m_tipColor = value;} }
        public Color CaretColor = new Color(1, 1, 1, 0.8f);
        public Color SelectionColor = new Color(0.65882f, 0.8078f, 1, 0.4f);
        public List<UIVertex> SelectVertex { get; private set; }
        public List<int> SelectTriAngle { get; private set; }
        public Func<TextInput, int, char, char> ValidateChar;
        public Action<TextInput> OnValueChanged;
        public Action<TextInput, UserAction> OnSelectChanged;
        public Action<TextInput, UserAction> OnSelectEnd;
        public Action<TextInput> OnSubmit;
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
            SelectVertex = new List<UIVertex>();
            SelectTriAngle = new List<int>();
            Click = OnClick;
            LostFocus = OnLostFocus;
        }
        public TextElement TextCom { get; private set; }
        Vector3 Point;
        public override void OnMouseDown(UserAction action)
        {
            if (TextCom != null)
            {
               //startSelect = GetPressIndex(TextCom, this, ref Point, action);
            }
            base.OnMouseDown(action);
            UpdateText();
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
                if (TextCom != null)
                {
                    if (entry)
                    {
                        if (action.Motion != Vector2.zero)
                        {
                            //InputCaret.CaretStyle = 2;
                            //Vector3 p = Vector3.zero;
                            //int end = endSelect;
                            //endSelect=GetPressIndex(TextCom, this, ref p, action);
                            //if (end != endSelect)
                            //{
                            //    Selected();
                            //    if (OnSelectChanged != null)
                            //        OnSelectChanged(this, action);
                            //}
                        }
                    }
                    else
                    {
                        if (action.Motion != Vector2.zero)
                        {
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
            if (emojiString.FullString == "")
                return;
            var p = Vector3.zero;
            //endSelect= GetPressIndex(TextCom, this, ref p, action);
            Selected();
            if (OnSelectEnd != null)
                OnSelectEnd(this, action);
        }
        static void OnEdit(TextInput input)
        {
            if (input.ReadOnly)
                return;
            InputEvent = input;
            bool pass = InputEvent.contentType == ContentType.Password ? true : false;
            Keyboard.OnInput(input.emojiString.FullString, InputEvent.touchType,InputEvent.multiLine,pass,input.CharacterLimit);
        }
        static void OnClick(EventCallBack eventCall, UserAction action)
        {
            TextInput input = eventCall as TextInput;
            if (input == null)
                return;
            //input.TextCom.color = input.textColor;
            //if (input.contentType == ContentType.Password)
            //{
            //    input.TextCom.text = new string('*', input.emojiString.Length);
            //}
            //else
            //{
            //    input.TextCom.text = input.emojiString.FullString;
            //}
            //InputEvent = input;
            //InputEvent.startSelect=GetPressIndex(input.TextCom, eventCall, ref input.Point, action);
            //input.ChangePoint(InputEvent.startSelect);
            //InputEvent.endSelect = -1;
            //InputCaret.CaretStyle = 1;
            OnEdit(input);
        }
        static void OnLostFocus(EventCallBack eventCall, UserAction action)
        {
            TextInput text = eventCall as TextInput;
            if (text == InputEvent)
            {
                if (InputEvent.OnSubmit != null)
                    InputEvent.OnSubmit(InputEvent);
                InputEvent = null;
                //InputCaret.CaretStyle = 0;
            }
            text.UpdateText();
        }
        public static void SetCurrentInput(TextInput input, UserAction action)
        {
            if (input == null)
                return;
            if (InputEvent == input)
                return;
            if (InputEvent != null)
                OnLostFocus(InputEvent, action);
            InputEvent = input;
            //InputEvent.startSelect= GetPressIndex(input.TextCom, input, ref input.Point, action);
            //InputEvent.endSelect=-1;
            //InputCaret.CaretStyle = 1;
            OnEdit(input);
        }
        void Selected()
        {
            SelectVertex.Clear();
            SelectTriAngle.Clear();
            //GetChoiceArea(TextCom, SelectVertex, SelectTriAngle, SelectionColor,startSelect,endSelect);
            //InputCaret.ChangeCaret(SelectVertex,SelectTriAngle);
            //InputCaret.Active();
            //InputCaret.CaretStyle = 2;
        }
        void UpdateText()
        {
            if (TextCom == null)
                return;
            string str = emojiString.FullString;
            if (str == null | str == "")
            {
                //TextCom.color = m_tipColor;
                TextCom.text = m_TipString;
            }
            else
            {
                //TextCom.color = textColor;
                if (contentType == ContentType.Password)
                {
                    TextCom.text = new string('*',emojiString.FilterString.Length);
                }
                else
                {
                    TextCom.text = emojiString.FullString;
                }
            }
        }

        static TextInput InputEvent;
        static TouchScreenKeyboard m_touch;
        static bool IsTouchKeyboard;
        internal void InputNewString(string con)
        {
            if (CharacterLimit > 0)
                if (emojiString != null)
                    if (emojiString.Length + con.Length > CharacterLimit)
                    {
                        int len = CharacterLimit - emojiString.Length;
                        if (len <= 0)
                            return;
                        con = con.Substring(0, len);
                    }
            //InsertString(con);
            RefreshText();
            //ChangePoint(startSelect);
        }
        void ApplyText(string text)
        {
            emojiString.FullString = text;
            UpdateText();
            //ChangePoint(startSelect);
        }
        void RefreshText()
        {
            UpdateText();
        }
        public void ChangePoint(int index)
        {
            if (TextCom != null)
            {
                //GetCaretPoint(TextCom, SelectVertex,SelectTriAngle,startSelect,CaretColor);
            }
            //InputCaret.CaretStyle = 1;
            //InputCaret.ChangeCaret(SelectVertex,SelectTriAngle);
        }
        bool openKeyboard;
        bool closeKeyboard;
        TextInfo textInfo = new TextInfo();
        /// <summary>
        /// MainThread
        /// </summary>
        void Apply()
        {
            if (Context == null)
                return;
            if (openKeyboard)
            {
                bool pass = InputEvent.contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(emojiString.FullString, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
            }else if(closeKeyboard)
            {
                Keyboard.EndInput();
            }
            TextElement txt = TextCom;
            if (txt == null)
            {
                txt = TextCom = Context.GetComponent<TextElement>();
            }
            if (txt == null)
                return;
            if (txt.text!=InputString)
            {
                if(!txt.IsChanged)
                {
                    var g = txt.Context.cachedTextGenerator;
                    if (textInfo == null)
                        textInfo = new TextInfo();
                    textInfo.vertex = g.verts;
                    textInfo.lines = g.lines;
                    textInfo.characterCount = g.characterCount;
                    textInfo.visibleCount = g.characterCountVisible;
                }
                TextCom.text = InputString;
            }
        }
    }
}