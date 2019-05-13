using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class GestureEvent : EventCallBack
    {
        internal static void Dispatch(List<UserAction> actions)
        {
            if (events == null)
                events = new List<GestureEvent>();
            else events.Clear();
            for(int i=0;i<actions.Count;i++)
            {
                if(actions[i].IsActive)
                {
                    var cur = actions[i].MultiFocus;
                    for (int j = 0; j < cur.Count; j++)
                        if (cur[j] is GestureEvent)
                        {
                            var act = cur[j] as GestureEvent;
                            if (events.Contains(act))
                            {
                                act.actions.Add(actions[i]);
                            }
                            else
                            {
                                var tmp = act.lastActions;
                                act.lastActions = act.actions;
                                act.actions = tmp;
                                act.actions.Clear();
                                events.Add(act);
                                act.actions.Add(actions[i]);
                            }
                        }
                }
            }
            for (int i = 0; i < events.Count; i++)
            {
                var eve = events[i];
                if(!eve.forbid)
                eve.Analysis();
            }
                
        }
        static List<GestureEvent> events;
        List<UserAction> lastActions;
        public List<UserAction> actions;
        int Fingers;
        int pressCount;
        int downCount;
        int upCount;
        public float CurScale { get; private set; }
        public float DirPix { get; private set; }
        public float DeltaPix { get; private set; }
        public float DirAngle { get; private set; }
        public float DeltaAngle { get; private set; }

        void Analysis()
        {
            Fingers = actions.Count;
            pressCount = 0;
            downCount = 0;
            upCount = 0;
            for(int i=0;i<actions.Count;i++)
            {
                var act = actions[i];
                if (act.isPressed)
                    pressCount++;
                if (act.IsLeftButtonDown)
                    downCount++;
                if (act.IsLeftButtonUp)
                    upCount++;
            }
            if (downCount > 0)
            {
                OnPressd();
                return;
            }
            if(upCount>0)
            {
                OnUp();
                return;
            }
            OnMove();
        }
        public GestureEvent()
        {
            CurScale = 1;
            ForceEvent = true;
            lastActions = new List<UserAction>();
            actions = new List<UserAction>();
        }
        public override void OnMouseDown(UserAction action)
        {
            if (!action.MultiFocus.Contains(this))
                action.MultiFocus.Add(this);
        }
        protected override void OnMouseMove(UserAction action)
        {
            if (actions.Count == 1)
                base.OnMouseMove(action);
        }
        protected override void OnMouseUp(UserAction action)
        {
        }
        internal override void OnFocusMove(UserAction action)
        {
            if (actions.Count == 1)
                base.OnFocusMove(action);
        }
        internal override void OnLostFocus(UserAction action)
        {
            actions.Remove(action);
            if (actions.Count == 0)
                base.OnLostFocus(action);
        }
        protected override void OnDrag(UserAction action)
        {
        }
        internal override void OnDragEnd(UserAction action)
        {
            if (actions.Count == 1)
                base.OnDragEnd(action);
        }

        long PressTime2;
        long PressTime3;
        long PressTime4;
        long PressTime5;
        Vector2 RawPos0;
        Vector2 RawPos1;
        Vector2 RawPos2;
        Vector2 RawPos3;
        Vector2 RawPos4;
        Vector2 LastPos0;
        Vector2 LastPos1;
        Vector2 LastPos2;
        Vector2 LastPos3;
        Vector2 LastPos4;
        float StartScale;
        float CurrentScale;
        void OnPressd()
        {
            Pressed = true;
            switch (Fingers)
            {
                case 1:
                    base.OnMouseDown(actions[0]);
                    break;
                case 2:
                    PressTime2 = DateTime.Now.Ticks;
                    LastPos0 = RawPos0 = actions[0].CanPosition;
                    LastPos1 = RawPos1 = actions[1].CanPosition;
                    var d = RawPos0 - RawPos1;
                    float l = d.x * d.x + d.y * d.y;
                    l = Mathf.Sqrt(l);
                    StartScale = l;
                    CurrentScale = l;
                    if (TowFingerPressd != null)
                        TowFingerPressd(this);
                    break;
                case 3:
                    PressTime3 = DateTime.Now.Ticks;
                    LastPos0 = RawPos0 = actions[0].CanPosition;
                    LastPos1 = RawPos1 = actions[1].CanPosition;
                    LastPos2 = RawPos3 = actions[2].CanPosition;
                    if (ThreeFingerPressd != null)
                        ThreeFingerPressd(this);
                    break;
                case 4:
                    PressTime4 = DateTime.Now.Ticks;
                    LastPos0 = RawPos0 = actions[0].CanPosition;
                    LastPos1 = RawPos1 = actions[1].CanPosition;
                    LastPos2 = RawPos2 = actions[2].CanPosition;
                    LastPos3 = RawPos3 = actions[3].CanPosition;
                    if (FourFingerPressd != null)
                        FourFingerPressd(this);
                    break;
                case 5:
                    PressTime5 = DateTime.Now.Ticks;
                    LastPos0 = RawPos0 = actions[0].CanPosition;
                    LastPos1 = RawPos1 = actions[1].CanPosition;
                    LastPos2 = RawPos2 = actions[2].CanPosition;
                    LastPos3 = RawPos3 = actions[3].CanPosition;
                    LastPos4 = RawPos4 = actions[4].CanPosition;
                    if (FiveFingerPressd != null)
                        FiveFingerPressd(this);
                    break;
            }
        }
        void OnMove()
        {
            switch (pressCount)
            {
                case 1:
                    if (Pressed)
                        base.OnDrag(actions[0]);
                    else base.OnMouseMove(actions[0]);
                    break;
                case 2:
                    var d0 = actions[0].CanPosition;
                    var d1 = actions[1].CanPosition;
                    Vector2 d = d0 - d1 ;
                    float s = d.x * d.x + d.y * d.y;
                    s = Mathf.Sqrt(s);
                    DeltaPix = s - CurrentScale;
                    CurrentScale = s;
                    DirPix = s - StartScale;
                    s /= StartScale;
                    CurScale = s;
                    var v0 = RawPos0 - RawPos1;
                    float a0 = MathH.atan(v0.x, v0.y);//原始角度
                    var a1 = MathH.atan(d.x, d.y);//当前角度
                    DirAngle = a0 - a1;
                    v0 = LastPos0 - LastPos1;
                    a0 = MathH.atan(v0.x, v0.y);//上一帧角度
                    DeltaAngle = a0 - a1;
                    LastPos0 = d0;
                    LastPos1 = d1;
                    if (TowFingerMove != null)
                        TowFingerMove(this);
                    break;
                case 3:
                    LastPos0 = actions[0].CanPosition;
                    LastPos1 = actions[1].CanPosition;
                    LastPos2 = actions[2].CanPosition;
                    if (ThreeFingerMove != null)
                        ThreeFingerMove(this);
                    break;
                case 4:
                    LastPos0 = actions[0].CanPosition;
                    LastPos1 = actions[1].CanPosition;
                    LastPos2 = actions[2].CanPosition;
                    LastPos3 = actions[3].CanPosition;
                    if (FourFingerMove != null)
                        FourFingerMove(this);
                    break;
                case 5:
                    LastPos0 = actions[0].CanPosition;
                    LastPos1 = actions[1].CanPosition;
                    LastPos2 = actions[2].CanPosition;
                    LastPos3 = actions[3].CanPosition;
                    LastPos4 = actions[4].CanPosition;
                    if (FiveFingerMove != null)
                        FiveFingerMove(this);
                    break;
            }
        }
        void OnUp()
        {
            if(pressCount==0)
            {
                var t = DateTime.Now.Ticks;
                if (t - PressTime5 < ClickTime)
                {
                    if (FiveFingerClick != null)
                        FiveFingerClick(this);
                }
                else if (t - PressTime4 < ClickTime)
                {
                    if (FourFingerClick != null)
                        FourFingerClick(this);
                }
                else if (t - PressTime3 < ClickTime)
                {
                    if (ThreeFingerClick != null)
                        ThreeFingerClick(this);
                }
                else if (t - PressTime2 < ClickTime)
                {
                    if (TowFingerClick != null)
                        TowFingerClick(this);
                }
                else
                {
                    base.OnMouseUp(actions[0]);
                }
                Pressed = false;
            }
            else
            switch (upCount)
            {
                case 1:
                    base.OnMouseUp(actions[0]);
                    break;
                case 2:
                    if (TowFingerUp != null)
                        TowFingerUp(this);
                    break;
                case 3:
                    if (ThreeFingerUp != null)
                        ThreeFingerUp(this);
                    break;
                case 4:
                    if (FourFingerUp != null)
                        FourFingerUp(this);
                    break;
                case 5:
                    if (FiveFingerUp != null)
                        FiveFingerUp(this);
                    break;
            }
        }
        public Action<GestureEvent> TowFingerPressd;
        public Action<GestureEvent> ThreeFingerPressd;
        public Action<GestureEvent> FourFingerPressd;
        public Action<GestureEvent> FiveFingerPressd;
        public Action<GestureEvent> TowFingerMove;
        public Action<GestureEvent> ThreeFingerMove;
        public Action<GestureEvent> FourFingerMove;
        public Action<GestureEvent> FiveFingerMove;
        public Action<GestureEvent> TowFingerUp;
        public Action<GestureEvent> ThreeFingerUp;
        public Action<GestureEvent> FourFingerUp;
        public Action<GestureEvent> FiveFingerUp;
        public Action<GestureEvent> TowFingerClick;
        public Action<GestureEvent> ThreeFingerClick;
        public Action<GestureEvent> FourFingerClick;
        public Action<GestureEvent> FiveFingerClick;
    }
}
