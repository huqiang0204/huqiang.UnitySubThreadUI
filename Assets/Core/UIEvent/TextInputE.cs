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
        static float KeySpeed = 0.22f;
        static float MaxSpeed = 0.03f;
        static float KeyPressTime;
        static EditState KeyPressed()
        {
            KeyPressTime -= Time.deltaTime;
            if (Keyboard.GetKey(KeyCode.Backspace))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        //if(InputEvent.DeleteLeft())
                        //{
                        //    InputEvent.RefreshText();
                        //    InputEvent.ChangePoint(InputEvent.startSelect);
                        //}
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
                        //if (InputEvent.DeleteRight())
                        //{
                        //    InputEvent.RefreshText();
                        //    InputEvent.ChangePoint(InputEvent.startSelect);
                        //}
                    }
                    KeySpeed *= 0.5f;
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
                        //if(InputEvent.MoveLeft())
                        //{
                        //    InputEvent.ChangePoint(InputEvent.startSelect);
                        //}
                    }
                    KeySpeed *= 0.5f;
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
                        //if(InputEvent.MoveRight())
                        //{
                        //    InputEvent.ChangePoint(InputEvent.startSelect);
                        //}
                    }
                    KeySpeed *= 0.5f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            KeySpeed = 0.3f;
            if (Keyboard.GetKeyDown(KeyCode.Home))
            {
                InputEvent.ChangePoint(0);
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.End))
            {
                InputEvent.ChangePoint(InputEvent.emojiString.Length);
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.A))
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        //InputEvent.startSelect = 0;
                        //InputEvent.endSelect = InputEvent.emojiString.Length;
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
            if (Keyboard.GetKeyDown(KeyCode.Space))
            {
                if (InputEvent != null)
                {
                    //InputEvent.InsertString(" ");
                    //InputEvent.RefreshText();
                    //InputEvent.ChangePoint(InputEvent.startSelect);
                }
                return EditState.Done;
            }
            return EditState.Continue;
        }
        internal static void Dispatch()
        {
            if (InputEvent != null)
            {
                if (!InputEvent.ReadOnly)
                    if (!InputEvent.Pressed)
                    {
                        if (IsTouchKeyboard)
                        {
                            //InputCaret.CaretStyle = 0;
                            var str = m_touch.text;
                            InputEvent.ApplyText(str);
                            if (m_touch.status == TouchScreenKeyboard.Status.Done)
                            {
                                if (InputEvent.OnSubmit != null)
                                    InputEvent.OnSubmit(InputEvent);
                                InputEvent = null;
                                //InputCaret.UpdateCaret();
                                return;
                            }
                        }
                        else
                        {
                            var state = KeyPressed();
                            if (state == EditState.Continue)
                            {
                                string str = Input.inputString;
                                if (str != null & str != "")
                                {
#if UNITY_EDITOR||UNITY_STANDALONE_WIN
                                    var tmp = IME.CurrentCompStr();
                                    InputEvent.InputNewString(tmp);
#else
                                    InputEvent.InputNewString(str);
#endif
                                }
                            }
                            else if (state == EditState.Finish)
                            {
                                if (InputEvent.OnSubmit != null)
                                    InputEvent.OnSubmit(InputEvent);
                                InputEvent = null;
                                //InputCaret.CaretStyle = 0;
                                return;
                            }
                            else if (state == EditState.NewLine)
                            {
                                InputEvent.InputNewString("\n");
                                if (InputEvent.OnValueChanged != null)
                                    InputEvent.OnValueChanged(InputEvent);
                                return;
                            }
                        }
                    }
            }
            //InputCaret.UpdateCaret();
        }
        public static bool GetIndexPoint(TextInfo info, int index, ref Vector3 point)
        {
            ///仅可见区域的顶点 0=左上1=右上2=右下3=左下
            IList<UIVertex> vertex = info.vertex;
            ///仅可见区域的行数
            IList<UILineInfo> lines = info.lines;
            float top = lines[lines.Count - 1].topY;
            float high = lines[lines.Count - 1].height;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].startCharIdx > index)
                {
                    top = lines[i - 1].topY;
                    high = lines[i - 1].height;
                    break;
                }
            }
            int max = vertex.Count;
            index *= 4;
            if (index >= max)
                index = max - 3;
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

        public static int GetPressIndex(Text text, EventCallBack callBack, ref Vector3 point, UserAction action)
        {
            if (text == null)
                return -1;
            if (text.text == "" | text.text == null)
                return -1;
            float fs = text.fontSize;
            IList<UILineInfo> lines = text.cachedTextGenerator.lines;
            IList<UIVertex> vertex = text.cachedTextGenerator.verts;
            var pos = callBack.GlobalPosition;
            var scale = callBack.GlobalScale;
            for (int i = 0; i < lines.Count; i++)
            {
                float top = lines[i].topY;
                float high = lines[i].height;
                float down = top - high;
                down *= scale.y;
                down += pos.y;
                if (down < action.CanPosition.y)
                {
                    int index = lines[i].startCharIdx;
                    int end = text.text.Length - index;
                    if (i < lines.Count - 1)
                        end = lines[i + 1].startCharIdx - index;
                    int p = index * 4;
                    for (int j = 0; j < end; j++)
                    {
                        float x = vertex[p + 2].position.x;
                        x *= scale.x;
                        x += pos.x;
                        if (x > action.CanPosition.x)
                        {
                            point.x = vertex[p].position.x;
                            point.y = top - high * 0.5f;
                            point.z = high;
                            return index;
                        }
                        index++;
                        p += 4;
                        if (p + 2 >= vertex.Count)
                            break;
                    }
                    if (index == text.text.Length)
                    {
                        float it = lines[lines.Count - 1].topY;
                        float ih = lines[lines.Count - 1].height;
                        point.x = vertex[vertex.Count - 2].position.x;
                        point.y = it - ih * 0.5f;
                        point.z = ih;
                    }
                    else
                    {
                        point.x = vertex[p - 4].position.x;
                        point.y = top - high * 0.5f;
                        point.z = high;
                    }
                    return index;
                }
            }
            float t = lines[lines.Count - 1].topY;
            float h = lines[lines.Count - 1].height;
            point.x = vertex[vertex.Count - 1].position.x;
            point.y = t - h * 0.5f;
            point.z = h;
            return text.cachedTextGenerator.characterCountVisible;
        }
        static Vector2 GetLineRect(IList<UIVertex> vertex, int start, int end)
        {
            if (vertex.Count == 0)
                return Vector2.zero;
            int s = start * 4;
            int e = end * 4 + 2;
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
            IList<UIVertex> vertex = info.vertex;
            float top = 0;
            float down = 0;
            int end = info.characterCount;
            int max = lines.Count;
            int se = max - 1;
            var vert = info.vert;
            var color = info.color;
            var tri = info.tri;
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
                int state = CommonArea(info.startIndex, info.endIndex, ref start, ref end);
                if (state == 2)
                {
                    break;
                }
                else
                if (state == 1)
                {
                    top = lines[i].topY;
                    down = top - lines[i].height;
                    var w = GetLineRect(vertex, start, end);
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

    }
    public class TextInfo
    {
        public IList<UILineInfo> lines;
        public IList<UIVertex> vertex;
        public int characterCount;
        public int visibleCount;
        public int startIndex;
        public int endIndex;
        public List<UIVertex> vert;
        public List<int> tri;
        public Color color;
    }
}
