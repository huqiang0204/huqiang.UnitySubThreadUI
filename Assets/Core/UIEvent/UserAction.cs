using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class UserAction
    {
        public enum InputType
        {
            OnlyMouse, OnlyTouch, Blend
        }
        public static InputType inputType = InputType.OnlyMouse;
        public int Id { get; private set; }
        public Vector2 LastPosition;
        public Vector2 CanPosition;
        public Vector2 Position;
        public Vector2 Motion;
        Vector2 m_Velocities;
        public Vector2 Velocities { get { return m_Velocities; } }
        public bool IsMoved { get; private set; }
        public bool FingerStationary { get; private set; }
        public bool IsLeftButtonDown { get; private set; }
        public bool isPressed { get; private set; }
        public bool IsLeftButtonUp { get; private set; }
        public Vector2 rawPosition { get; set; }
        public int tapCount { get; private set; }
        public float altitudeAngle { get; private set; }
        public float azimuthAngle { get; private set; }
        public float radius { get; set; }
        public float radiusVariance { get; set; }
        public float PressTime { get; private set; }
        public static float Accelerationtime = 60;
        public long EventTicks { get; private set; }
        Vector3[] FramePos = new Vector3[16];
        int Frame;
        List<EventCallBack> LastEntry;
        public List<EventCallBack> CurrentEntry;
        List<EventCallBack> LastFocus;
        public List<EventCallBack> MultiFocus;
        public UserAction(int id)
        {
            Id = id;
            LastEntry = new List<EventCallBack>();
            CurrentEntry = new List<EventCallBack>();
            LastFocus = new List<EventCallBack>();
            MultiFocus = new List<EventCallBack>();
        }
        public void Clear()
        {
            LastEntry.Clear();
            CurrentEntry.Clear();
            LastFocus.Clear();
            MultiFocus.Clear();
            IsLeftButtonDown = false;
            IsRightButtonPressed = false;
            isPressed = false;
            IsLeftButtonUp = false;
            IsRightButtonUp = false;
            IsRightButtonUp = false;
            IsMoved = false;
            FingerStationary = false;
        }
        public void ReleaseFocus()
        {
            LastEntry.Clear();
            CurrentEntry.Clear();
            LastFocus.Clear();
            MultiFocus.Clear();
        }
        public float MouseWheelDelta { get; private set; }
        public bool IsMouseWheel { get; private set; }
        public bool IsMiddleButtonPressed { get; private set; }
        public bool IsRightButtonPressed { get; private set; }
        public bool IsMiddleButtonUp { get; private set; }
        public bool IsRightButtonUp { get; private set; }
        public bool IsRightPressed { get; private set; }
        public bool IsMiddlePressed { get; private set; }
        public bool IsActive { get; set; }
        void CalculVelocities()
        {
            if (PressTime > Accelerationtime)
            {
                int s = Frame;
                float time = 0;
                for (int i = 0; i < 6; i++)
                {
                    time += FramePos[s].z;
                    if (time >= Accelerationtime)
                        break;
                    s--;
                    if (s < 0)
                        s = 15;
                }
                float x = FramePos[Frame].x - FramePos[s].x;
                float y = FramePos[Frame].y - FramePos[s].y;
                m_Velocities.x = x / time;
                m_Velocities.y = y / time;
            }
            else m_Velocities = Vector2.zero;
        }
        public void LoadFinger(ref Touch touch)
        {
            LastPosition = CanPosition;
            Id = touch.fingerId;
            switch (touch.phase)
            {
                case TouchPhase.Began://pointer down
                    IsLeftButtonDown = true;
                    FingerStationary = false;
                    isPressed = true;
                    IsLeftButtonUp = false;
                    IsMoved = false;
                    break;
                case TouchPhase.Moved:
                    IsLeftButtonDown = false;
                    isPressed = true;
                    IsLeftButtonUp = false;
                    IsMoved = true;
                    FingerStationary = false;
                    break;
                case TouchPhase.Ended://pointer up
                    IsLeftButtonDown = false;
                    isPressed = false;
                    IsLeftButtonUp = true;
                    IsMoved = false;
                    FingerStationary = false;
                    break;
                case TouchPhase.Stationary://悬停
                    IsLeftButtonDown = false;
                    isPressed = true;
                    IsLeftButtonUp = false;
                    IsMoved = false;
                    FingerStationary = true;
                    break;
                default:
                    IsLeftButtonDown = false;
                    //isPressed = false;
                    //IsLeftButtonUp = false;
                    IsMoved = false;
                    FingerStationary = false;
                    break;
            }
            if (IsLeftButtonDown)
            {
                EventTicks = Ticks;
                PressTime = 0;
            }
            else PressTime += TimeSlice;
            Motion = touch.deltaPosition;
            tapCount = touch.tapCount;
            rawPosition = touch.rawPosition;
            Position = touch.position;
            float x = Screen.width;
            x *= 0.5f;
            float y = Screen.height;
            y *= 0.5f;
            CanPosition.x = Position.x - x;
            CanPosition.y = Position.y - y;

            FramePos[Frame].x = Position.x;
            FramePos[Frame].y = Position.y;
            FramePos[Frame].z = TimeSlice;
            CalculVelocities();
            Frame++;
            if (Frame >= 16)
                Frame = 0;
            altitudeAngle = touch.altitudeAngle;
            azimuthAngle = touch.azimuthAngle;
            radius = touch.radius;
            radiusVariance = touch.radiusVariance;
        }
        public void LoadMouse()
        {
            LastPosition = CanPosition;
            IsActive = true;
            MouseWheelDelta = Input.mouseScrollDelta.y;
            if (MouseWheelDelta != 0)
                IsMouseWheel = true;
            else IsMouseWheel = false;
            IsLeftButtonDown = Input.GetMouseButtonDown(0);
            IsRightButtonPressed = Input.GetMouseButtonDown(1);
            IsMiddleButtonPressed = Input.GetMouseButtonDown(2);
            IsLeftButtonUp = Input.GetMouseButtonUp(0);
            IsRightButtonUp = Input.GetMouseButtonUp(1);
            IsMiddleButtonUp = Input.GetMouseButtonUp(2);
            isPressed = Input.GetMouseButton(0);
            IsRightPressed = Input.GetMouseButton(1);
            IsMiddlePressed = Input.GetMouseButton(2);
            if (IsLeftButtonDown | IsRightButtonPressed | IsMiddleButtonPressed)
            {
                EventTicks = Ticks;
                PressTime = 0;
                rawPosition = Input.mousePosition;
            }
            else { PressTime += TimeSlice; }
            IsMoved = false;
            float x = Input.mousePosition.x;
            Motion.x = x - Position.x;
            if (Motion.x != 0)
            {
                IsMoved = true;
            }
            Position.x = x;
            float y = Input.mousePosition.y;
            Motion.y = y - Position.y;
            if (Motion.y != 0)
            {
                IsMoved = true;
            }
            Position.y = y;
            x = Screen.width;
            x *= 0.5f;
            y = Screen.height;
            y *= 0.5f;
            CanPosition.x = Position.x - x;
            CanPosition.y = Position.y - y;
            FramePos[Frame].x = Position.x;
            FramePos[Frame].y = Position.y;
            FramePos[Frame].z = TimeSlice;
            CalculVelocities();
            Frame++;
            if (Frame >= 16)
                Frame = 0;
        }
        public static void SubDispatch()
        {
#if DEBUG
            try
            {
#endif
                EventCallBack.Rolling();
#if DEBUG
            }catch(Exception ex)
            {
                Debug.Log(ex.StackTrace);
            }
#endif
            for (int i=0;i<inputs.Length;i++)
            {
                if (inputs[i] != null)
                {
#if DEBUG
                    try
                    {
#endif
                        if (inputs[i].IsActive)
                            inputs[i].Dispatch();
#if DEBUG
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.StackTrace);
                    }
#endif
                }
            }
            TextInput.SubDispatch();
            GestureEvent.Dispatch(new List<UserAction>(inputs));
        }
        /// <summary>
        /// 子线程
        /// </summary>
        void Dispatch()
        {
            if (IsLeftButtonDown | IsRightButtonPressed | IsMiddleButtonPressed)
            {
                List<EventCallBack> tmp = MultiFocus;
                LastFocus.Clear();
                MultiFocus = LastFocus;
                LastFocus = tmp;
            }
            EventCallBack.DispatchEvent(this);
            if (IsLeftButtonDown | IsRightButtonPressed | IsMiddleButtonPressed)
            {
                for (int i = 0; i < LastFocus.Count; i++)
                {
                    var f = LastFocus[i];
                    for (int j = 0; j < MultiFocus.Count; j++)
                    {
                        if (f == MultiFocus[j])
                            goto label2;
                    }
                    if (!f.Pressed)
                        f.OnLostFocus(this);
                    label2:;
                }
            }
            else if (IsLeftButtonUp | IsRightButtonUp | IsMiddleButtonUp)
            {
                for (int i = 0; i < MultiFocus.Count; i++)
                {
                    var f = MultiFocus[i];
                    f.Pressed = false;
                    f.OnDragEnd(this);
                }
            }
            else
            {
                for (int i = 0; i < MultiFocus.Count; i++)
                    MultiFocus[i].OnFocusMove(this);
            }
            if (IsMouseWheel)
            {
                for (int i = 0; i < MultiFocus.Count; i++)
                {
                    var f = MultiFocus[i];
                    if (f.MouseWheel != null)
                        f.MouseWheel(f, this);
                }
            }
            CheckMouseLeave();
        }
        public static int TimeSlice;
        public static int LastTime;
        public static long Ticks;
        static UserAction[] inputs;
        public static UserAction GetInput(int id)
        {
            if (inputs == null)
                return null;
            if (id > inputs.Length)
                return inputs[inputs.Length - 1];
            if (id < 0)
                return inputs[0];
            return inputs[id];
        }
        static void DispatchTouch()
        {
            if (inputs == null)
            {
                inputs = new UserAction[10];
                for (int i = 0; i < 10; i++)
                    inputs[i] = new UserAction(i);
            }
            var touches = Input.touches;
            for (int i = 0; i < 10; i++)
            {
                if (touches != null)
                {
                    for (int j = 0; j < touches.Length; j++)
                    {
                        if (touches[j].fingerId == i)
                        {
                            inputs[i].LoadFinger(ref touches[j]);
                            inputs[i].IsActive = true;
                            goto label;
                        }
                    }
                }
                if (inputs[i].isPressed)
                {
                    inputs[i].isPressed = false;
                    inputs[i].IsLeftButtonUp = true;
                }else inputs[i].IsActive = false;
            label:;
            }
        }
        static void DispatchMouse()
        {
            if (inputs == null)
            {
                inputs = new UserAction[1];
                inputs[0] = new UserAction(0);
            }
            var action = inputs[0];
            action.LoadMouse();
        }
        static void DispatchWin()
        {
            if (inputs == null)
            {
                inputs = new UserAction[10];
                for (int i = 0; i < 10; i++)
                    inputs[i] = new UserAction(i);
            }
            var touches = Input.touches;
            for (int i = 0; i < 10; i++)
            {
                int id = i;
                if (touches != null)
                {

                    for (int j = 0; j < touches.Length; j++)
                    {
                        if (touches[j].fingerId == id)
                        {
                            inputs[id].LoadFinger(ref touches[j]);
                            inputs[id].IsActive = true;
                            goto label;
                        }
                    }
                }
                if (touches.Length > 0 & inputs[id].isPressed)
                {
                    inputs[id].isPressed = false;
                    inputs[id].IsLeftButtonUp = true;
                }
                else inputs[id].IsActive = false;
                label:;
            }
            if (touches.Length == 0)
            {
                var action = inputs[0];
                action.LoadMouse();
            }
        }
        public static void DispatchEvent()
        {
            DateTime now = DateTime.Now;
            Ticks = now.Ticks;
            int s = now.Millisecond;
            int t = s - LastTime;
            if (t < 0)
                t += 1000;
            TimeSlice = t;
            LastTime = s;
            if (inputType == InputType.OnlyMouse)
            {
                DispatchMouse();
            }
            else if (inputType == InputType.OnlyTouch)
            {
                DispatchTouch();
            }
            else
            {
                DispatchWin();
            }
            InputCaret.UpdateCaret();
        }
        public static void ClearAll()
        {
            if (inputs != null)
                for (int i = 0; i < inputs.Length; i++)
                    inputs[i].Clear();
        }
        void CheckMouseLeave()
        {
            for (int i = 0; i < LastEntry.Count; i++)
            {
                var eve = LastEntry[i];
                if (eve != null)
                {
                    for (int j = 0; j < CurrentEntry.Count; j++)
                    {
                        if (CurrentEntry[j] == eve)
                            goto label;
                    }
                    eve.OnMouseLeave(this);
                label:;
                }
            }
            List<EventCallBack> tmp = LastEntry;
            tmp.Clear();
            LastEntry = CurrentEntry;
            CurrentEntry = tmp;
        }
        public bool ExistFocus(EventCallBack eve)
        {
            return MultiFocus.Contains(eve);
        }
        public void AddFocus(EventCallBack eve)
        {
            if (MultiFocus.Contains(eve))
                return;
            MultiFocus.Add(eve);
            eve.Pressed = isPressed;
            if (eve.Pressed)
                eve.pressTime = EventTicks;
        }
        public void RemoveFocus(EventCallBack callBack)
        {
            LastEntry.Remove(callBack);
            CurrentEntry.Remove(callBack);
            LastFocus.Remove(callBack);
            MultiFocus.Remove(callBack);
            callBack.Pressed = false;
        }
    }
}
