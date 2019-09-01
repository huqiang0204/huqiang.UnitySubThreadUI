using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UIEvent
{
    public partial class TextInput
    {
        enum EditState
        {
            Done,
            Continue,
            NewLine,
            Finish
        }
        /// <summary>
        /// 每秒5次
        /// </summary>
        static float KeySpeed = 220;
        static float MaxSpeed = 30;
        static float KeyPressTime;
        static TextInput InputEvent;
        static EditState KeyPressed()
        {
            KeyPressTime -= UserAction.TimeSlice;
            if (Keyboard.GetKey(KeyCode.Backspace))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.Delete(-1);
                    }
                    KeySpeed *= 0.8f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.Delete))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.Delete(1);
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.LeftArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.SetSelectPoint(-1);
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.RightArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.SetSelectPoint(1);
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.UpArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.MoveUp();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.DownArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.MoveDown();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            KeySpeed = 220f;
            if (Keyboard.GetKeyDown(KeyCode.Home))
            {
                InputEvent.SetSelectPoint(0);
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.End))
            {
                InputEvent.SetSelectPoint(10000000);
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.A))
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        InputEvent.textInfo.startSelect = 0;
                        InputEvent.textInfo.endSelect = InputEvent.textInfo.text.Length;
                        InputEvent.Selected();
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.X))//剪切
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        string str = InputEvent.SelectString;
                        InputEvent.Delete(-1);
                        ThreadMission.InvokeToMain((o) => { GUIUtility.systemCopyBuffer = str; },null);
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.C))//复制
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        string str = InputEvent.SelectString;
                        ThreadMission.InvokeToMain((o) => { GUIUtility.systemCopyBuffer = str; }, null);
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.V))//粘贴
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        InputEvent.OnInputChanged(Keyboard.systemCopyBuffer);
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.Return) | Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (InputEvent.lineType == LineType.MultiLineNewline)
                {
                    if (Keyboard.GetKey(KeyCode.RightControl))
                        return EditState.Finish;
                    return EditState.NewLine;
                }
                else return EditState.Finish;
            }
            if (Keyboard.GetKeyDown(KeyCode.Escape))
            {
                return EditState.Finish;
            }
            return EditState.Continue;
        }
        internal static void SubDispatch()
        {
            if (InputEvent != null)
            {
                if (!InputEvent.ReadOnly)
                    if (!InputEvent.Pressed)
                    {
                        var state = KeyPressed();
                        if (state == EditState.Continue)
                        {
                            if (Keyboard.InputChanged)
                            {
                                if (Keyboard.InputString == "")
                                    return;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                                if (Keyboard.Nokey())
                                    InputEvent.OnInputChanged(IME.CurrentCompStr());
                                else
                                    InputEvent.OnInputChanged(Keyboard.InputString);
#else
                                   InputEvent.TouchInputChanged(Keyboard.InputString);
#endif
                            }
                        }else if(state==EditState.Finish)
                        {
                            if (InputEvent.OnSubmit != null)
                                InputEvent.OnSubmit(InputEvent);
                        }else if(state==EditState.NewLine)
                        {
                            InputEvent.OnInputChanged(Environment.NewLine);
                        }
                    }
                if(InputEvent.textInfo.LineChange>0)
                {
                    InputEvent.textInfo.LineChange = 0;
                    if (InputEvent.LineChanged != null)
                        InputEvent.LineChanged(InputEvent);
                }
            }
        }
        internal static void MainDispath()
        {
            var input = InputEvent;
            if(input!=null)
            {
                input.Update();
            }
        }
        public static bool GetIndexPoint(TextInfo info, int index, ref Vector3 point)
        {
            ///仅可见区域的顶点 0=左上1=右上2=右下3=左下
            UIVertex[] vertex = info.filterVertex;
            if (vertex == null)
                return false;
            var line = info.filterLines;
            int max = line.Length - 1;
            for(int i=max;i>=0;i--)
            {
                if(line[i].startCharIdx<=index)
                {
                    max = i;
                    break;
                }
            }
            index *= 4;
            if (index >= vertex.Length)
                index -= 2;
            point.x = vertex[index].position.x;
            point.y = line[max].topY - 0.5f * line[max].height;
            point.z = line[max].height;
            return true;
        }
        static void ChangePoint(TextInfo info,TextInput input)
        {
            int index = info.startSelect-info.StartIndex;
            if (index < 0)
                index = 0;
            var text = info.buffer.FilterString;
            if (text == null|text=="")
                index = 0;
            else if (index > text.Length)
                index = text.Length;
            Vector3 Point = Vector3.zero;
            var o = GetIndexPoint(info, index, ref Point);
            var vert = info.selectVertex;
            var tri = info.selectTri;
            vert.Clear();
            tri.Clear();
            if (o)
            {
                float left = Point.x - 1;
                float right = Point.x + 1;
                float h = Point.z;
                h *= 0.4f;
                float top = Point.y + h;
                float down = Point.y - h;
                var v = new UIVertex();
                v.position.x = left;
                v.position.y = down;
                v.color = info.caretColor;
                vert.Add(v);
                v.position.x = left;
                v.position.y = top;
                v.color = info.caretColor;
                vert.Add(v);
                v.position.x = right;
                v.position.y = down;
                v.color = info.caretColor;
                vert.Add(v);
                v.position.x = right;
                v.position.y = top;
                v.color = info.caretColor;
                vert.Add(v);
            }
            else
            {
                var v = new UIVertex();
                vert.Add(v);
                vert.Add(v);
                vert.Add(v);
                vert.Add(v);
            }
            tri.Add(0);
            tri.Add(1);
            tri.Add(2);
            tri.Add(2);
            tri.Add(1);
            tri.Add(3);
        }
        public static int GetPressIndex(TextInfo info, EventCallBack callBack, UserAction action,ref int dock)
        {
            dock = 0;
            if (info == null)
                return 0;
            if (info.text == "" | info.text == null)
                return 0;
            IList<UILineInfo> lines = info.filterLines;
            if (lines == null)
                return 0;
     
            IList<UIVertex> verts = info.filterVertex;
            float lx = verts[0].position.x;
            float ty = verts[0].position.y;
            float dy = verts[verts.Count - 1].position.y;

            var pos = callBack.GlobalPosition;
            var scale = callBack.GlobalScale;
            float mx = action.CanPosition.x - pos.x;
            mx *= scale.x;
            float my = action.CanPosition.y - pos.y;
            my *= scale.y;
            int r = 0;//行
            if(my<ty)
            {
                if(my<dy)
                {
                    r = lines.Count - 1;
                }
                else
                {
                    for (int i = lines.Count-1; i >=0 ; i--)
                    {
                        if (my < lines[i].topY)
                        {
                            r = i;
                            break;
                        }
                    }
                }
            }
            if (mx > lx)
            {
                int s = lines[r].startCharIdx;
                int index = s * 4;
                int end = verts.Count - index;

                float ox = verts[index].position.x;
                for (int i = 0; i < end; i += 4)
                {
                    float tx = verts[index].position.x;
                    if (tx < ox | tx > mx)
                    {
                        index -= 4;
                        goto lable;
                    }
                    ox = tx;
                    index += 4;
                }
                info.lineIndex = info.visibleCount - lines[lines.Count - 1].startCharIdx;
                return info.visibleCount;
            lable:;
                float ax = verts[index].position.x;
                float bx = verts[index + 2].position.x;
                float cx = ax + (bx - ax) * 0.5f;
                index /= 4;
                if (mx > cx)//靠右
                {
                    index++;
                    dock = 1;
                }
                info.lineIndex = index - lines[r].startCharIdx;
                return index;
            }
            else {
                info.lineIndex = 0;
                return lines[r].startCharIdx;
            }
        }

        static Vector2 GetLineRect(IList<UIVertex> vertex, int start, int end,bool warp)
        {
            if (vertex.Count == 0)
                return Vector2.zero;
            int s = start * 4;
            int e = end * 4;
            if (warp)
                e += 2;
            if (s >= vertex.Count)
                s = vertex.Count - 1;
            if (e >= vertex.Count)
                e = vertex.Count - 1;
            return new Vector2(vertex[s].position.x, vertex[e].position.x);
        }
        static int CommonArea(int s1, int e1, ref int s2, ref int e2)
        {
            if (s1 > e2)
                return 0;
            if (s2 > e1)
                return 2;
            if (s2 < s1)
                s2 = s1;
            if (e2 > e1)
                e2 = e1;
            return 1;
        }
        static readonly char[] Separators = { ' ', '.', ',', '\t', '\r', '\n' };
        const string EmailCharacters = "!#$%&'*+-/=?^_`{|}~";
        static char Validate(CharacterValidation validat, string text, int pos, char ch)
        {
            if (validat == CharacterValidation.None)
                return ch;
            if (validat == CharacterValidation.Integer)
            {
                if (ch == '-')
                {
                    if (text == "")
                        return ch;
                    if (text.Length > 0)
                        return (char)0;
                }
                if (ch < '0' | ch > '9')
                    return (char)0;
                return ch;
            }
            else if (validat == CharacterValidation.Decimal)
            {
                if (ch >= '0' && ch <= '9')
                {
                    if (ch == '.')
                        if (text.IndexOf('.') < 0)
                            return ch;
                    return (char)0;
                }
                return ch;
            }
            else if (validat == CharacterValidation.Alphanumeric)
            {
                // All alphanumeric characters
                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.numberAndName)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.Name)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
            }
            else if (validat == CharacterValidation.EmailAddress)
            {

                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
                if (ch == '@' && text.IndexOf('@') == -1) return ch;
                if (EmailCharacters.IndexOf(ch) != -1) return ch;
                if (ch == '.')
                {
                    char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
                    char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
                    if (lastChar != '.' && nextChar != '.')
                        return ch;
                }
            }
            return (char)0;
        }
        /// <summary>
        /// 删除当前被选中区域的文字
        /// </summary>
        /// <returns></returns>
        static bool DeleteSelected(TextInfo info)
        {
            if (info.endSelect < 0)
                return false;
            int s = info.startSelect;
            int e = info.endSelect;
            if (e < s)
            {
                int a = s;
                s = e;
                e = a;
            }
            if (s < 0)
                return false;
            if (s == e)
                e++;
            info.buffer.Remove(s,e-s);
            info.endSelect = -1;
            info.startSelect = s;
            info.CaretStyle = 1;
            return true;
        }
        static void GetPreferredHeight(TextElement text,TextInfo info)
        {
            string str = info.buffer.FilterString;
            TextGenerationSettings settings = new TextGenerationSettings();
            settings.resizeTextMinSize = 2;
            settings.resizeTextMaxSize = 40;
            settings.scaleFactor = 1;
            settings.textAnchor = TextAnchor.UpperLeft;
            settings.color = Color.white;
            settings.generationExtents = new Vector2(text.model.data.sizeDelta.x, 0);
            settings.pivot = new Vector2(0.5f, 0.5f);
            settings.richText = true;
            settings.font = text.Context.font;
            settings.fontSize = text.data.fontSize;
            settings.fontStyle = FontStyle.Normal;
            settings.alignByGeometry = false;
            settings.updateBounds = false;
            settings.lineSpacing = 1;
            settings.horizontalOverflow = HorizontalWrapMode.Wrap;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            TextGenerator generator = new TextGenerator();
            float h = generator.GetPreferredHeight(str, settings);
            info.HeightChange = info.PreferredHeight - h;
            info.PreferredHeight = h;
            info.fullVertex = generator.verts.ToArray();
            info.fullLines = generator.lines.ToArray();
            info.characterCount = generator.characterCount;
     
            int lc = info.fullLines.Length;
            info.LineChange = lc - info.LineCount;
            info.LineCount = lc;
        }
        static void FilterPopulate(TextElement txt,TextInfo info)
        {
            if (info.StartLine < 0)
                info.StartLine = 0;
            Vector2 size = txt.model.data.sizeDelta;
    
            var lines = info.fullLines;
            int max = lines.Length - 1;
            info.EndLine = max;
            float dd = lines[max].topY - lines[max].height;
            int a = 0;
            for (int i = max; i >= 0; i--)
            {
                if (lines[i].topY - dd > size.y)
                {
                    a = i + 1;//允许下滑的最大行数
                    break;
                }
            }
            if (info.StartLine > a)
                info.StartLine = a;
            float tt = lines[info.StartLine].topY;
            for (int i = info.StartLine + 1; i < lines.Length; i++)
            {
                if (tt - lines[i].topY + lines[i].height > size.y)
                {
                    info.EndLine = i - 1;//下滑的结束行
                    break;
                }
            }
            int startIndex = lines[info.StartLine].startCharIdx;
            int endIndex = info.buffer.FilterString.Length;
            if (info.EndLine != lines.Length - 1)
                endIndex = lines[info.EndLine + 1].startCharIdx;
            info.StartIndex = startIndex;
            info.EndIndex = endIndex;
            info.ShowString.FullString = info.buffer.SubString(startIndex,endIndex-startIndex);

            var text = txt.Context;
            var settings = text.GetGenerationSettings(size);
            var generator = text.cachedTextGenerator;
            generator.Populate(info.ShowString.FilterString, settings);
            info.filterVertex = generator.verts.ToArray();
            info.filterLines = generator.lines.ToArray();
            info.visibleCount = generator.characterCountVisible;
        }
        static void GetChoiceArea(TextInfo info, int startSelect, int endSelect)
        {
            if (info == null)
                return;
            IList<UILineInfo> lines = info.filterLines;
            if (lines == null)
                return;
            IList<UIVertex> vertex = info.filterVertex;
            if (vertex == null)
                return;
            float top = 0;
            float down = 0;
            int end = info.characterCount;
            int max = lines.Count;
            int se = max - 1;
            var vert = info.selectVertex;
            var color = info.areaColor;
            var tri = info.selectTri;
            int s = startSelect;
            int e = endSelect;
            if (e < s)
            {
                int t = s;
                s = e;
                e = t;
            }
            for (int i = 0; i < lines.Count; i++)
            {
                int start = lines[i].startCharIdx;
                if (i < se)
                {
                    end = lines[i + 1].startCharIdx - 1;
                }
                else
                {
                    end = info.visibleCount;
                }
                int state = CommonArea(s, e, ref start, ref end);
                if (state == 2)
                {
                    break;
                }
                else
                if (state == 1)
                {
                    top = lines[i].topY;
                    down = top - lines[i].height;
                    bool warp = end < e ? true : false;
                    var w = GetLineRect(vertex, start, end, warp);
                    int st = vert.Count;
                    var v = new UIVertex();
                    v.position.x = w.x;
                    v.position.y = down;
                    v.color = color;
                    vert.Add(v);
                    v.position.x = w.x;
                    v.position.y = top;
                    v.color = color;
                    vert.Add(v);
                    v.position.x = w.y;
                    v.position.y = down;
                    v.color = color;
                    vert.Add(v);
                    v.position.x = w.y;
                    v.position.y = top;
                    v.color = color;
                    vert.Add(v);
                    tri.Add(st);
                    tri.Add(st + 1);
                    tri.Add(st + 2);
                    tri.Add(st + 2);
                    tri.Add(st + 1);
                    tri.Add(st + 3);
                }
            }
        }
        static void FilterChoiceArea(TextElement txt,TextInfo info)
        {
            int len = info.ShowString.FilterString.Length;
            int start =info.startSelect - info.StartIndex;
            if (start < 0)
                start = 0;
            if (start > len)
                start = len;
            int end = info.endSelect - info.StartIndex;
            if (end < 0)
                end = 0;
            if (end > len)
                end = len;
            GetChoiceArea(info,start,end);
        }
    }
    public class TextInfo
    {
        public EmojiString buffer = new EmojiString();
        public string text;
        public float fontSize;
        public UILineInfo[] fullLines;
        public UIVertex[] fullVertex;
        public UILineInfo[] filterLines;
        public UIVertex[] filterVertex;
        public int characterCount;
        public int visibleCount;
        public int lineIndex;
        public int startSelect;
        public int startDock;//光标停靠的索引
        public int endSelect;
        public int endDock;//光标停靠的索引
        public List<UIVertex> selectVertex=new List<UIVertex>();
        public List<int> selectTri=new List<int>();
        public Color color;
        public int CaretStyle;
        public Color caretColor = new Color(1, 1, 1, 0.8f);
        public Color areaColor = new Color(0.65882f, 0.8078f, 1, 0.4f);
        public float PreferredHeight;
        public float HeightChange=0;
        public int LineCount;
        public int LineChange = 0;
        public int StartLine;
        public int EndLine;
        public int StartIndex;
        public int EndIndex;
        public EmojiString ShowString = new EmojiString();
    }
}
