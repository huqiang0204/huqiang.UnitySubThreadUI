using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            else if (Keyboard.GetKeyDown(KeyCode.Return) | Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (InputEvent.lineType != LineType.MultiLineNewline)
                {
                    return EditState.Finish;
                }
                else return EditState.NewLine;
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
                        if (KeyPressed() == EditState.Continue)
                            if (Keyboard.InputChanged)
                            {
                                if (Keyboard.InputString == "")
                                    return;
                                InputEvent.OnInputChanged(Keyboard.InputString);
                            }
                    }
            }
        }
        public static bool GetIndexPoint(TextInfo info, int index, ref Vector3 point)
        {
            ///仅可见区域的顶点 0=左上1=右上2=右下3=左下
            IList<UIVertex> vertex = info.vertex;
            if (vertex == null)
                return false;
            ///仅可见区域的行数
            IList<UILineInfo> lines = info.lines;
            float top = lines[lines.Count - 1].topY;
            float high = lines[lines.Count - 1].height;
            for (int i = 0; i < lines.Count; i++)
            {
                int a = lines[i].startCharIdx;
                if ( a >= index)
                {
                    if(i>0)
                    {
                        top = lines[i - 1].topY;
                        high = lines[i - 1].height;
                    }
                    break;
                }
            }
            index *= 4;
            if (info.startDock == 1)
                index -= 2;
            point.x = vertex[index].position.x;
            float y = vertex[index].position.y;
            float down = top - high;
            point.z = high;
            point.y = top - high * 0.5f;
            if (down > y)
            {
                point.y -= high;
                return false;
            }
            return true;
        }
        static void ChangePoint(TextInfo info)
        {
            int index = info.startSelect;
            if (index < 0)
                index = 0;
            var text = info.text;
            if (text == null)
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
                float left = Point.x - 0.5f;
                float right = Point.x + 0.5f;
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
            float fs = info.fontSize;
            IList<UILineInfo> lines = info.lines;
            if (lines == null)
                return 0;
     
            IList<UIVertex> verts = info.vertex;
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
                return info.visibleCount;
            lable:;
                float ax = verts[index].position.x;
                float bx = verts[index + 2].position.x;
                float cx = ax + (bx - ax) * 0.5f;
                index /= 4;
                if (mx > cx)
                {
                    index++;
                    dock = 1;
                }
                return index;
            }
            else return lines[r].startCharIdx;
        }

        static Vector2 GetLineRect(IList<UIVertex> vertex, int start, int end,bool warp)
        {
            if (vertex.Count == 0)
                return Vector2.zero;
            int s = start * 4;
            int e = end * 4;
            if (warp)
                e += 2;
            if (e > vertex.Count)
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
        static void GetChoiceArea(TextInfo info)
        {
            if (info == null)
                return;
            IList<UILineInfo> lines = info.lines;
            if (lines == null)
                return;
            IList<UIVertex> vertex = info.vertex;
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
            int s = info.startSelect;
            int e = info.endSelect;
            if(e<s)
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
                    var w = GetLineRect(vertex, start, end,warp);
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
            info.text = info.buffer.ToString();
            info.endSelect = -1;
            info.startSelect = s;
            info.CaretStyle = 1;
            return true;
        }
    }
    public class TextInfo
    {
        public StringBuilder buffer = new StringBuilder();
        public string text;
        public float fontSize;
        public IList<UILineInfo> lines;
        public IList<UIVertex> vertex;
        public int characterCount;
        public int visibleCount;
        public int startSelect;
        public int startDock;
        public int endSelect;
        public int endDock;
        public List<UIVertex> selectVertex=new List<UIVertex>();
        public List<int> selectTri=new List<int>();
        public Color color;
        public int CaretStyle;
        public Color caretColor = new Color(1, 1, 1, 0.8f);
        public Color areaColor = new Color(0.65882f, 0.8078f, 1, 0.4f);
    }
}
