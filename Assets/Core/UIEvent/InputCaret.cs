using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGUI;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class InputCaret
    {
        static CustomRawImage m_Caret;
        static CustomRawImage Caret
        {
            get
            {
                if (m_Caret == null)
                {
                    var g = new GameObject("m_caret",typeof(CustomRawImage));
                    m_Caret = g.GetComponent<CustomRawImage>();
                    m_Caret.rectTransform.sizeDelta = Vector2.zero;
                    m_Caret.material = new Material(Shader.Find("UI/Default"));
                }
                else if (m_Caret.name == "buff")
                {
                    var g = new GameObject("m_caret", typeof(CustomRawImage));
                    m_Caret = g.GetComponent<CustomRawImage>();
                    m_Caret.rectTransform.sizeDelta = Vector2.zero;
                    m_Caret.material = new Material(Shader.Find("UI/Default"));
                }
                return m_Caret;
            }
        }
        static float time;
        public static void ChangeCaret(TextInfo info)
        {
            if (m_Caret != null)
            {
                m_Caret.uIVertices = info.selectVertex;
                m_Caret.triangle = info.selectTri;
                m_Caret.Refresh();
                time = 0;
                CaretStyle = info.CaretStyle;
            }
        }
        public static int CaretStyle;
        public static void UpdateCaret()
        {
            switch (CaretStyle)
            {
                case 1:
                    time += Time.deltaTime;
                    if (time > 1.6f)
                    {
                        time = 0;
                    }
                    else if (time > 0.8f)
                    {
                        Caret.gameObject.SetActive(false);
                    }
                    else
                    {
                        Caret.gameObject.SetActive(true);
                    }
                    break;
                case 2:

                    break;
                default:
                    Caret.gameObject.SetActive(false);
                    break;
            }
        }
        public static void SetParent(RectTransform rect)
        {
            var t = Caret.transform;
            t.SetParent(rect);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
        }
        public static void Active()
        {
            if (m_Caret != null)
            {
                m_Caret.gameObject.SetActive(true);
            }
        }
        public static void Hide()
        {
            if (m_Caret != null)
            {
                m_Caret.gameObject.SetActive(false);
            }
        }
    }
}
