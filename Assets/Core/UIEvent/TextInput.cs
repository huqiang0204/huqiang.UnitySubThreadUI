using huqiang.Data;
using huqiang.UI;
using System;
using System.Text;
using UGUI;
using UnityEngine;

namespace huqiang.UIEvent
{
    public unsafe struct TextInputData
    {
        public Color inputColor;
        public Color tipColor;
        public Color pointColor;
        public Color selectColor;
        public Int32 inputString;
        public Int32 tipString;
        public static int Size = sizeof(TextInputData);
        public static int ElementSize = Size / 4;
    }
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
        public EmojiString emojiString { get { return textInfo.buffer; } }
        void SetShowText()
        {
            if (textInfo.text == ""|textInfo.text==null)
            {
                TextCom.color = TipColor;
                TextCom.text = m_TipString;
            }
            else
            {
                TextCom.color = textColor;
                TextCom.text = textInfo.ShowString.FullString;
            }
        }
        public bool ReadOnly;
        Color textColor=Color.black;
        Color m_tipColor = new Color(0, 0, 0, 0.8f);
        public Color TipColor { get { return m_tipColor; } set { m_tipColor = value;} }
        public Color PointColor = Color.white;
        public Color SelectionColor = new Color(0.65882f, 0.8078f, 1, 0.2f);
        public Func<TextInput, int, char, char> ValidateChar;
        public Action<TextInput> OnValueChanged;
        public Action<TextInput> OnSubmit;
        public Action<TextInput> OnDone;
        public Action<TextInput> LineChanged;
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
        float overDistance = 500;
        float overTime = 0;
        public TextInput()
        {
            Click = OnClick;
            LostFocus = OnLostFocus;
        }
        protected override void Initial()
        {
            var txt = TextCom = Context.GetComponent<TextElement>();
            InputString = txt.text;
            textColor = txt.data.color;
            var fake = txt.model.GetExtand() as FakeStruct;
            if(fake!=null)
            {
                unsafe
                {
                    TextInputData* tp = (TextInputData*)fake.ip;
                    textColor = tp->inputColor;
                    m_tipColor = tp->tipColor;
                    InputString =fake.buffer.GetData(tp->inputString)as string;
                    TipString = fake.buffer.GetData(tp->tipString)as string;
                    PointColor = tp->pointColor;
                    SelectionColor = tp->selectColor;
                }
            }
        }
        public TextElement TextCom { get; private set; }
        public override void OnMouseDown(UserAction action)
        {
            overTime = 0;
            if (TextCom != null)
            {
                textInfo.startSelect = GetPressIndex(textInfo, this, action,ref textInfo.startDock)+textInfo.StartIndex;
                Editing = true;
                selectChanged = true;
                textInfo.CaretStyle = 0;
            }
            base.OnMouseDown(action);
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
                if (TextCom != null)
                {
                    if (action.Motion != Vector2.zero)
                    {
                        textInfo.CaretStyle = 2;
                        int end = textInfo.endSelect;
                        textInfo.endSelect = GetPressIndex(textInfo, this, action, ref textInfo.endDock)+textInfo.StartIndex;
                        if (end != textInfo.endSelect)
                        {
                            Selected();
                            if (OnSelectChanged != null)
                                OnSelectChanged(this, action);
                            selectChanged = true;
                        }
                    }else if(!entry)
                    {
                        float oy = action.CanPosition.y - GlobalPosition.y;
                        float py = GlobalScale.y * TextCom.model.data.sizeDelta.y * 0.5f;
                        if (oy > 0)
                            oy -= py;
                        else oy += py;
                        if (oy > overDistance)
                            oy = overDistance;
                        float per = 50000 / oy;
                        if (per < 0)
                            per = -per;
                        overTime += UserAction.TimeSlice;
                        if (overTime >= per)
                        {
                            overTime -= per;
                            if(oy>0)
                            {
                                textInfo.StartLine--;
                            }
                            else
                            {
                                textInfo.StartLine++;
                            }
                            int end = textInfo.endSelect;
                            textInfo.endSelect = GetPressIndex(textInfo, this, action, ref textInfo.endDock) + textInfo.StartIndex;
                            if (end != textInfo.endSelect)
                            {
                                Selected();
                                if (OnSelectChanged != null)
                                    OnSelectChanged(this, action);
                                selectChanged = true;
                            }
                            lineChanged = true;
                            if (LineChanged != null)
                                LineChanged(this);
                        }
                    }
                }
            base.OnDrag(action);
        }
        internal override void OnMouseWheel(UserAction action)
        {
            float oy = action.MouseWheelDelta;
            if (oy > 0)
            {
                textInfo.StartLine--;
            }
            else
            {
                textInfo.StartLine++;
            }
            lineChanged = true;
            if (LineChanged != null)
                LineChanged(this);
            base.OnMouseWheel(action);
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
            textInfo.endSelect = GetPressIndex(textInfo, this, action, ref textInfo.endDock)+textInfo.StartIndex;
            Selected();
            selectChanged = true;
            if (OnSelectEnd != null)
                OnSelectEnd(this, action);
            base.OnDragEnd(action);
        }
        void OnClick(EventCallBack eventCall, UserAction action)
        {
            TextInput input = eventCall as TextInput;
            if (input == null)
                return;
            InputEvent = input;
            textInfo.startSelect = GetPressIndex(textInfo, this, action, ref textInfo.startDock)+textInfo.StartIndex;
            textInfo.endSelect = -1;
            textInfo.CaretStyle = 1;
            selectChanged = true;
            ThreadMission.InvokeToMain((o) => {
                bool pass = InputEvent.contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(textInfo.text, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
                InputCaret.SetParent(Context.Context);
                InputCaret.ChangeCaret(textInfo);
            }, null);
        }
        void OnLostFocus(EventCallBack eventCall, UserAction action)
        {
            TextInput text = eventCall as TextInput;
            if (text == InputEvent)
            {
                if (InputEvent.OnDone != null)
                    InputEvent.OnDone(InputEvent);
                InputEvent = null;
            }
            Editing = false;
            SetShowText();
            ThreadMission.InvokeToMain((o) => { Keyboard.EndInput(); }, null);
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
            selectChanged = true;
            textChanged = true;
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
            //ChangePoint(textInfo);
            selectChanged = true;
            textChanged = true;
            return input;
        }
        public bool Editing;
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
            selectChanged = true;
        }
        bool textChanged;
        bool selectChanged;
        bool lineChanged;
        public string SelectString { get  {
                if (textInfo.endSelect == -1)
                    return "";
                int s = textInfo.startSelect;
                int e = textInfo.endSelect;
                if(s>e)
                {
                    var t = e;
                    e = s;
                    s = t;
                }
                int c = e - s;
                return textInfo.buffer.SubString(s,c);
            } }
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
            if (index < textInfo.StartIndex)
            {
                textInfo.StartLine--;
                lineChanged = true;
                if (LineChanged != null)
                    LineChanged(this);
            }
            else if(index > textInfo.EndIndex)
            {
                textInfo.StartLine++;
                lineChanged = true;
                if (LineChanged != null)
                    LineChanged(this);
            }
            selectChanged = true;
            var lines = textInfo.fullLines;
            if(lines!=null)
            {
                int i = lines.Length - 1;
                int start = textInfo.startSelect;
                for (; i >= 0; i--)
                {
                    int t = lines[i].startCharIdx;
                    if (t <= start)
                    {
                        textInfo.lineIndex = start - t;
                        break;
                    }
                }
            }
        }
        void MoveUp()
        {
            var lines = textInfo.fullLines;
            if (lines != null)
            {
                int start = textInfo.startSelect;
                int i = lines.Length - 1;
                for (; i >=1; i--)
                {
                    int t = lines[i].startCharIdx;
                    if (t<=start)
                    {
                        if (i > 0)
                        {
                            int a = lines[i - 1].startCharIdx + textInfo.lineIndex;
                            if (a >= t)
                                a = t - 1;
                            if (a < textInfo.StartIndex)
                            {
                                textInfo.StartLine--;
                                lineChanged = true;
                                if (LineChanged != null)
                                    LineChanged(this);
                            }
                            textInfo.startSelect = a;
                            selectChanged = true;
                        }
                        break;
                    }
                }
            }
        }
        void MoveDown()
        {
            var lines = textInfo.fullLines;
            if (lines != null)
            {
                int start = textInfo.startSelect;
                int c= lines.Length - 1;
                for (int i=c-1; i >= 0; i--)
                {
                    int t = lines[i].startCharIdx;
                    if (t <= start)
                    {
                        if (i <c)
                        {
                            int a = lines[i + 1].startCharIdx + textInfo.lineIndex;
                            if (a >= textInfo.buffer.Length)
                                a = textInfo.buffer.Length;
                            else if (i < c - 1)
                            {
                                int s = lines[i + 2].startCharIdx;
                                if (a >= s)
                                    a = s - 1;
                            }
                            if (a > textInfo.EndIndex)
                            {
                                textInfo.StartLine++;
                                lineChanged = true;
                                if (LineChanged != null)
                                    LineChanged(this);
                            }
                            textInfo.startSelect = a;
                            selectChanged = true;
                        }
                        else
                        {
                            textInfo.StartLine++;
                            lineChanged = true;
                            textInfo.startSelect = textInfo.buffer.Length;
                            selectChanged = true;
                        }
                        break;
                    }
                }
            }
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
            if (OnValueChanged != null)
                OnValueChanged(this);
            textInfo.text = textInfo.buffer.FullString;
            textInfo.CaretStyle = 1;
            textChanged = true;
            //ChangePoint(textInfo);
            selectChanged = true;
        }
        void Update()
        {
            var te = TextCom;
            if (te.Context != null)
            {
                if(textChanged)
                {
                    textInfo.fontSize = TextCom.data.fontSize;
                    textChanged = false;
                    GetPreferredHeight(TextCom,textInfo);
                    textInfo.StartLine += textInfo.LineChange;
                    textInfo.EndLine += textInfo.LineChange;
                    lineChanged = true;
                    var lines = textInfo.fullLines;
                    if (lines != null)
                    {
                        int i = lines.Length - 1;
                        int start = textInfo.startSelect;
                        for (; i >= 0; i--)
                        {
                            int t = lines[i].startCharIdx;
                            if (t <= start)
                            {
                                textInfo.lineIndex = start - t;
                                break;
                            }
                        }
                    }
                }
                if(lineChanged)
                {
                    lineChanged = false;
                    FilterPopulate(TextCom, textInfo);
                    TextCom.text =
                    TextCom.Context.text = textInfo.ShowString.FullString;
                }
                if(selectChanged)
                {
                    textInfo.fontSize = TextCom.data.fontSize;
                    textInfo.caretColor = PointColor;
                    textInfo.areaColor = SelectionColor;
                    selectChanged = false;
                    if (textInfo.CaretStyle == 2)
                        FilterChoiceArea(TextCom, textInfo);
                    else ChangePoint(textInfo,this);
                    InputCaret.ChangeCaret(textInfo);
                }
            }
        }
        public void SizeChanged()
        {
            textChanged = true;
            selectChanged = true;
        }
        public float Percentage { get {
                float a = textInfo.LineCount;
                float b = textInfo.EndLine - textInfo.StartLine;
                float c = a - b;
                return b / c; }
            set {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                float a = textInfo.LineCount;
                float b = textInfo.EndLine - textInfo.StartLine;
                float c = a - b;
                if (c < 0)
                    c = 0;
                int i = (int)(c * value);
                textInfo.StartLine = i;
                if(textInfo.StartLine!=i)
                {
                    lineChanged = true;
                }
            } }
    }
}