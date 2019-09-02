using huqiang.UI;
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
        public static float Accelerationtime = 60;
        public static int TimeSlice;
        public static int LastTime;
        public static long Ticks;
        public static void Update()
        {
            DateTime now = DateTime.Now;
            Ticks = now.Ticks;
            int s = now.Millisecond;
            int t = s - LastTime;
            if (t < 0)
                t += 1000;
            TimeSlice = t;
            LastTime = s;
        }
        public int Id { get; private set; }
        public Vector2 LastPosition;
        public Vector2 CanPosition;
        public Vector2 Position;
        public Vector2 Motion;
        Vector2 m_Velocities;
        public Vector2 Velocities { get { return m_Velocities; } }
        public bool IsMoved { get; set; }
        public bool FingerStationary { get; set; }
        public bool IsLeftButtonDown { get; set; }
        public bool isPressed { get; set; }
        public bool IsLeftButtonUp { get; set; }
        public Vector2 rawPosition { get; set; }
        public int tapCount { get; set; }
        public float altitudeAngle { get; set; }
        public float azimuthAngle { get; set; }
        public float radius { get; set; }
        public float radiusVariance { get; set; }
        public float PressTime { get; set; }
        public long EventTicks { get; set; }
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
            IsRightButtonDown = false;
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
        public float MouseWheelDelta { get; set; }
        public bool IsMouseWheel { get; set; }
        public bool IsMiddleButtonDown { get; set; }
        public bool IsRightButtonDown { get; set; }
        public bool IsMiddleButtonUp { get; set; }
        public bool IsRightButtonUp { get; set; }
        public bool IsRightPressed { get; set; }
        public bool IsMiddlePressed { get; set; }
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
            IsRightButtonDown = Input.GetMouseButtonDown(1);
            IsMiddleButtonDown = Input.GetMouseButtonDown(2);
            IsLeftButtonUp = Input.GetMouseButtonUp(0);
            IsRightButtonUp = Input.GetMouseButtonUp(1);
            IsMiddleButtonUp = Input.GetMouseButtonUp(2);
            isPressed = Input.GetMouseButton(0);
            IsRightPressed = Input.GetMouseButton(1);
            IsMiddlePressed = Input.GetMouseButton(2);
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
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
        public void CopyAction(UserAction action, Vector2 FormSize)
        {
            LastPosition = CanPosition;
            IsActive = true;
            MouseWheelDelta = action.MouseWheelDelta;
            if (MouseWheelDelta != 0)
                IsMouseWheel = true;
            else IsMouseWheel = false;
            IsLeftButtonDown = action.IsLeftButtonDown;
            IsRightButtonDown = action.IsRightButtonDown;
            IsMiddleButtonDown = action.IsMiddleButtonDown;
            IsLeftButtonUp = action.IsLeftButtonUp;
            IsRightButtonUp = action.IsRightButtonUp;
            IsMiddleButtonUp = action.IsMiddleButtonUp;
            isPressed = action.isPressed;
            IsRightPressed = action.IsRightPressed;
            IsMiddlePressed = action.IsMiddlePressed;
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
            {
                EventTicks = Ticks;
                PressTime = 0;
                rawPosition = action.rawPosition;
            }
            else { PressTime += TimeSlice; }
            IsMoved = false;
            float x = action.Position.x;
            Motion.x = x - Position.x;
            if (Motion.x != 0)
            {
                IsMoved = true;
            }
            Position.x = x;
            float y = action.Position.y;
            Motion.y = y - Position.y;
            if (Motion.y != 0)
            {
                IsMoved = true;
            }
            Position.y = y;
            x = FormSize.x;
            x *= 0.5f;
            y = FormSize.y;
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
        /// <summary>
        /// 子线程
        /// </summary>
        public void Dispatch(ModelElement root)
        {
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
            {
                List<EventCallBack> tmp = MultiFocus;
                LastFocus.Clear();
                MultiFocus = LastFocus;
                LastFocus = tmp;
            }
            EventCallBack.DispatchEvent(this, root);
            if (IsLeftButtonDown | IsRightButtonDown | IsMiddleButtonDown)
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
                    f.OnMouseWheel(this);
                }
            }
            CheckMouseLeave();
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
