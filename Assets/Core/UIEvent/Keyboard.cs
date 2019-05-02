using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class Keyboard
    {
        static KeyCode[] keys;
        public static List<KeyCode> KeyPress;
        public static List<KeyCode> KeyDowns;
        public static List<KeyCode> KeyUps;
        public static string InputString;
        public static string CorrectionInput;
        public static string TouchString;
        public static string CorrectionTouch;
        static TouchScreenKeyboard m_touch;
        static bool _touch = false;
        public static void DispatchEvent()
        {
            if (keys == null)
            {
                if (Application.platform == RuntimePlatform.Android |
                Application.platform == RuntimePlatform.IPhonePlayer |
                Application.platform == RuntimePlatform.WSAPlayerARM |
                Application.platform == RuntimePlatform.WSAPlayerX64 |
                Application.platform == RuntimePlatform.WSAPlayerX86)
                    _touch = true;
                keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
                KeyPress = new List<KeyCode>();
                KeyUps = new List<KeyCode>();
                KeyDowns = new List<KeyCode>();
                Input.imeCompositionMode = IMECompositionMode.On;
            }
            KeyPress.Clear();
            KeyUps.Clear();
            KeyDowns.Clear();
            for(int i=0;i<keys.Length;i++)
            {
                var key = keys[i];
                if (Input.GetKey(key))
                    KeyDowns.Add(key);
                if (Input.GetKeyDown(key))
                    KeyPress.Add(key);
                if (Input.GetKeyUp(key))
                    KeyUps.Add(key);
            }
            InputString = Input.inputString;
            if(_touch)
            {
                if(m_touch!=null)
                {
                    if(m_touch.active)
                    {
                        targetDisplay = m_touch.targetDisplay;
                        type = m_touch.type;
                        selection = m_touch.selection;
                        text = m_touch.text;
                        canGetSelection = m_touch.canGetSelection;
                        status = m_touch.status;
                    }
                    active = m_touch.active;
                }
            }
        }
        public static void OnInput(string str, TouchScreenKeyboardType type,bool multiLine,bool passward,int limit)
        {
            if(_touch)
            {
                m_touch = TouchScreenKeyboard.Open("", type, true, multiLine, passward);
                m_touch.characterLimit = limit;
            }
        }
        public static void EndInput()
        {
            if(_touch)
            {
                if (m_touch != null)
                    m_touch.active = false;
            }
        }

        public static int targetDisplay { get; set; }
        public  static TouchScreenKeyboardType type { get; private set; }
        public static RangeInt selection { get; set; }
        public static string text { get; set; }
        public static bool canSetSelection { get; private set; }
        public static bool active { get; set; }
        public static bool canGetSelection { get; private set; }
        public static TouchScreenKeyboard.Status status { get; private set; }
        public static bool GetKey(KeyCode key)
        {
            if (KeyDowns.Contains(key))
                return true;
            return false;
        }
        public static bool GetKeyDown(KeyCode key)
        {
            if (KeyPress.Contains(key))
                return true;
            return false;
        }
        public static bool GetKeyUp(KeyCode key)
        {
            if (KeyUps.Contains(key))
                return true;
            return false;
        }
    }
}
