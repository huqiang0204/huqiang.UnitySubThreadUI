using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class EventCallBack
    {
        static void Reset(EventCallBack eventCall)
        {
            eventCall.PointerDown = null;
            eventCall.PointerUp = null;
            eventCall.Click = null;
            eventCall.PointerEntry = null;
            eventCall.PointerMove = null;
            eventCall.PointerLeave = null;
            eventCall.Drag = null;
            eventCall.DragEnd = null;
            eventCall.Scrolling = null;
            eventCall.AutoColor = true;
            eventCall.forbid = false;
            eventCall.mVelocity = Vector2.zero;
            eventCall.maxVelocity = Vector2.zero;
            eventCall.CutRect = false;
            eventCall.ForceEvent = false;
            eventCall.Penetrate = false;
        }
        public static long ClickTime = 1800000;
        public static float ClickArea = 400;
        public static void ClearEvent()
        {
            events.Clear();
        }
        static List<EventCallBack> events=new List<EventCallBack>();
        public static T RegEvent<T>(ModelElement element)where T:EventCallBack,new()
        {
            for(int i=0;i<events.Count;i++)
            {
                if (events[i].Context == element)
                {
                    T t = events[i] as T;
                    if (t != null)
                        return t;
                    events.RemoveAt(i);
                    break;
                }
            }
            T u = new T();
            u.Context = element;
            u.g_color = u.Context.color;
            element.baseEvent = u;
            events.Add(u);
            u.Initial();
            return u;
        }
        public static object RegEvent(ModelElement element,Type type)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].Context == element)
                {
                    events.RemoveAt(i);
                    break;
                }
            }
            EventCallBack u = Activator.CreateInstance(type) as EventCallBack;
            u.Context = element;
            u.g_color = u.Context.color;
            element.baseEvent = u;
            events.Add(u);
            u.Initial();
            return u;
        }
        public static void RegEvent<T>(T t)where T: EventCallBack
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i]== t)
                    return;
            }
            events.Add(t);
        }
        internal static void DispatchEvent(UserAction action,ModelElement root)
        {
            if (events.Count == 0)
                return;
            if (root != null)
            {
                var child = root.child;
                for (int i = child.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        if (DispatchEvent(child[i], Vector3.zero, Vector3.one, Quaternion.identity, action))
                            return;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }
                }
                //DispatchEvent(root, Vector3.zero, Vector3.one, Quaternion.identity, action);
            }
        }
        public static bool DispatchEvent(ModelElement ui, Vector3 pos, Vector3 scale, Quaternion quate, UserAction action)
        {
            if (ui == null)
            {
                return false;
            }
            if (!ui.activeSelf)
                return false;
            Vector3 p = quate* ui.data.localPosition;
            Vector3 o = Vector3.zero;
            o.x = p.x * scale.x;
            o.y = p.y * scale.y;
            o.z = p.z * scale.z;
            o += pos;
            Vector3 s = ui.data.localScale;
            Quaternion q =  quate* ui.data.localRotation;
            s.x *= scale.x;
            s.y *= scale.y;
            var callBack = ui.baseEvent;
            if (callBack == null)
            {
                var child = ui.child;
                for (int i = child.Count - 1; i >= 0; i--)
                {
                    if (DispatchEvent(child[i], o, s, q, action))
                    {
                        return true;
                    }
                }
            }
            else if (callBack.forbid)
            {
                var child = ui.child;
                for (int i = child.Count - 1; i >= 0; i--)
                {
                    if (DispatchEvent(child[i], o, s, q, action))
                        return true;
                }
            }
            else
            {
                callBack.pgs = scale;
                callBack.GlobalScale = s;
                callBack.GlobalPosition = o;
                callBack.GlobalRotation = q;
                bool inside = false;
                float w,h;
                if (callBack.UseAssignSize)
                {
                    w = callBack.boxSize.x*s.x;
                    h = callBack.boxSize.y*s.y;
                }
                else
                {
                    w = ui.data.sizeDelta.x * s.x;
                    h = ui.data.sizeDelta.y * s.y;
                }
                if (callBack.IsCircular)
                {
                    float x = action.CanPosition.x - o.x;
                    float y = action.CanPosition.y - o.y;
                    w *= 0.5f;
                    if (x * x + y * y < w * w)
                        inside = true;
                }
                else
                {
                    float x1 = 0.5f * w;
                    float x0 = -x1;
                    float y1 = 0.5f * h;
                    float y0 = -y1;

                    var v = action.CanPosition;
                    var Rectangular = callBack.Rectangular;
                    Rectangular[0] = q * new Vector3(x0, y0) + o;
                    Rectangular[1] = q * new Vector3(x0, y1) + o;
                    Rectangular[2] = q * new Vector3(x1, y1) + o;
                    Rectangular[3] = q * new Vector3(x1, y0) + o;
                    inside = Physics2D.DotToPolygon(Rectangular, v);
                }
                if (inside)
                {
                    action.CurrentEntry.Add(callBack);
                    var child = ui.child;
                    for (int i = child.Count- 1; i >= 0; i--)
                    {
                        if (DispatchEvent(child[i], o, s, q, action))
                        {
                            if (callBack.ForceEvent)
                            {
                                if (!callBack.forbid)
                                    break;
                            }
                            return true;
                        }
                    }
                    if (action.IsLeftButtonDown | action.IsRightButtonDown | action.IsMiddleButtonDown)
                    {
                        callBack.OnMouseDown(action);
                    }
                    else if (action.IsLeftButtonUp | action.IsRightButtonUp | action.IsMiddleButtonUp)
                    {
                         callBack.OnMouseUp(action);
                    }
                    else
                    {
                        callBack.OnMouseMove(action);
                    }
                    if (callBack.Penetrate)
                        return false;
                    return true;
                }
                else if (!callBack.CutRect)
                {
                    var child = ui.child;
                    for (int i = child.Count- 1; i >= 0; i--)
                    {
                        if (DispatchEvent(child[i], o, s, q, action))
                            return true;
                    }
                }
            }
            return false;
        }
        public static void Rolling()
        {
            for (int i = 0; i < events.Count; i++)
                if (events[i] != null)
                    if (!events[i].forbid)
                        if (!events[i].Pressed)
                            DuringSlide(events[i]);
        }
        static void DuringSlide(EventCallBack back)
        {
            if (back.mVelocity.x == 0 & back.mVelocity.y == 0)
                return;
            back.xTime += UserAction.TimeSlice;
            back.yTime += UserAction.TimeSlice;
            float x = 0, y = 0;
            bool endx = false, endy = false;
            if (back.mVelocity.x != 0)
            {
                float t = (float)MathH.PowDistance(back.DecayRateX, back.maxVelocity.x, back.xTime);
                x = t - back.lastX;
                back.lastX = t;
                float vx = Mathf.Pow(back.DecayRateX, back.xTime) * back.maxVelocity.x;
                if (vx < 0.001f & vx > -0.001f)
                {
                    back.mVelocity.x = 0;
                    endx = true;
                }
                else back.mVelocity.x = vx;
            }
            if (back.mVelocity.y != 0)
            {
                float t = (float)MathH.PowDistance(back.DecayRateY, back.maxVelocity.y, back.yTime);
                y = t - back.lastY;
                back.lastY = t;
                float vy = Mathf.Pow(back.DecayRateY, back.yTime) * back.maxVelocity.y;
                if (vy < 0.001f & vy > -0.001f)
                {
                    back.mVelocity.y = 0;
                    endy = true;
                }
                else back.mVelocity.y = vy;
            }
            if (back.Scrolling != null)
                back.Scrolling(back, new Vector2(x, y));
            if (endx)
                if (back.ScrollEndX != null)
                    back.ScrollEndX(back);
            if (endy)
                if (back.ScrollEndY != null)
                    back.ScrollEndY(back);
        }

        public ModelElement Context;
        Vector2 mVelocity;
        public float VelocityX { get { return mVelocity.x; } set { maxVelocity.x = mVelocity.x = value; RefreshRateX(); } }
        public float VelocityY { get { return mVelocity.y; } set { maxVelocity.y = mVelocity.y = value; RefreshRateY(); } }
        public void StopScroll()
        {
            mVelocity.x = 0;
            mVelocity.y = 0;
        }
        int xTime;
        int yTime;
        float lastX;
        float lastY;
        public Vector2 boxSize;
        Vector2 maxVelocity;
        Vector2 sDistance;
        public float ScrollDistanceX
        {
            get { return sDistance.x; }
            set
            {
                if (value == 0)
                    maxVelocity.x = 0;
                else
                    maxVelocity.x = (float)MathH.DistanceToVelocity(DecayRateX, value);
                mVelocity.x = maxVelocity.x;
                sDistance.x = value;
                xTime = 0;
                lastX = 0;
            }
        }
        public float ScrollDistanceY
        {
            get { return sDistance.y; }
            set
            {
                if (value == 0)
                    maxVelocity.y = 0;
                else
                    maxVelocity.y = (float)MathH.DistanceToVelocity(DecayRateY, value);
                mVelocity.y = maxVelocity.y;
                sDistance.y = value;
                yTime = 0;
                lastY = 0;
            }
        }
        public float DecayRateX = 0.998f;
        public float DecayRateY = 0.998f;
        public float speed = 1f;
        public Vector2 RawPosition { get; protected set; }
        Vector2 LastPosition;
        public int HoverTime { get; protected set; }
        public long pressTime { get; internal set; }
        public long entryTime { get; protected set; }
        public long stayTime { get; protected set; }
        public bool Pressed { get; internal set; }
        public bool forbid;
        /// <summary>
        /// 开启此项,范围外不会把事件传给子组件
        /// </summary>
        public bool CutRect = false;
        /// <summary>
        /// 强制事件不被子组件拦截
        /// </summary>
        public bool ForceEvent = false;
        /// <summary>
        /// 允许事件穿透
        /// </summary>
        public bool Penetrate = false;
        /// <summary>
        ///  使用指定尺寸
        /// </summary>
        public bool UseAssignSize = false;

        public bool IsCircular = false;
        public bool entry { get; protected set; }
        private int index;
        public bool AutoColor = true;
        Color g_color;
        public object DataContext;
        /// <summary>
        /// Parent GlobalScale
        /// </summary>
        Vector3 pgs = Vector3.one;
        public Vector3 GlobalScale = Vector3.one;
        public Vector3 GlobalPosition;
        public Quaternion GlobalRotation;
        public Vector3[] Rectangular { get; private set; }
        public EventCallBack()
        {
            Rectangular = new Vector3[4];
        }
        void RefreshRateX()
        {
            xTime = 0;
            lastX = 0;
            if (maxVelocity.x == 0)
                sDistance.x = 0;
            else
                sDistance.x = (float)MathH.PowDistance(DecayRateX, maxVelocity.x, 1000000);
        }
        void RefreshRateY()
        {
            yTime = 0;
            lastY = 0;
            if (maxVelocity.y == 0)
                sDistance.y = 0;
            else
                sDistance.y = (float)MathH.PowDistance(DecayRateY, maxVelocity.y, 1000000);
        }
        public Vector3 ScreenToLocal(Vector3 v)
        {
            v -= GlobalPosition;
            if (GlobalScale.x != 0)
                v.x /= GlobalScale.x;
            else v.x = 0;
            if (GlobalScale.y != 0)
                v.y /= GlobalScale.y;
            else v.y = 0;
            if (GlobalScale.z != 0)
                v.z /= GlobalScale.z;
            else v.z = 0;
            var q = Quaternion.Inverse(GlobalRotation);
            v = q * v;
            return v;
        }
        #region event
        public Action<EventCallBack, UserAction> PointerDown;
        public Action<EventCallBack, UserAction> PointerUp;
        public Action<EventCallBack, UserAction> Click;
        public Action<EventCallBack, UserAction> PointerEntry;
        public Action<EventCallBack, UserAction> PointerMove;
        public Action<EventCallBack, UserAction> PointerLeave;
        public Action<EventCallBack, UserAction> PointerHover;
        public Action<EventCallBack, UserAction> MouseWheel;
        public Action<EventCallBack, UserAction, Vector2> Drag;
        public Action<EventCallBack, UserAction, Vector2> DragEnd;
        public Action<EventCallBack, Vector2> Scrolling;
        public Action<EventCallBack> ScrollEndX;
        public Action<EventCallBack> ScrollEndY;
        public Action<EventCallBack, UserAction> LostFocus;

        UserAction FocusAction;
        public virtual void OnMouseDown(UserAction action)
        {
            if (!action.MultiFocus.Contains(this))
                action.MultiFocus.Add(this);
            Pressed = true;
            pressTime = action.EventTicks;
            RawPosition = action.CanPosition;
            if (AutoColor)
            {
                Color a = Color.white;
                a.r = g_color.r * 0.8f;
                a.g = g_color.g * 0.8f;
                a.b = g_color.b * 0.8f;
                a.a = g_color.a;
                Context.color = a;
            }
            if (PointerDown != null)
                PointerDown(this, action);
            entry = true;
            FocusAction = action;
            mVelocity = Vector2.zero;
        }
        protected virtual void OnMouseUp(UserAction action)
        {
            entry = false;
            if (AutoColor)
            {
                if (!forbid)
                    Context.color = g_color;
            }
            if (PointerUp != null)
                PointerUp(this, action);
            if(Pressed)
            {
                Pressed = false;
                long r = DateTime.Now.Ticks - pressTime;
                if (r <= ClickTime)
                {
                    float x = RawPosition.x - action.CanPosition.x;
                    float y = RawPosition.y - action.CanPosition.y;
                    x *= x;
                    y *= y;
                    x += y;
                    if (x < ClickArea)
                        if (Click != null)
                            Click(this, action);
                }
            }
        }
         
        protected virtual void OnMouseMove(UserAction action)
        {
            if (!entry)
            {
                entry = true;
                entryTime = UserAction.Ticks;
                if (PointerEntry != null)
                    PointerEntry(this, action);
                LastPosition = action.CanPosition;
            }
            else
            {
                stayTime = action.EventTicks - entryTime;
                if (action.CanPosition == LastPosition)
                {
                    HoverTime += UserAction.TimeSlice * 10000;
                    if (HoverTime > ClickTime)
                        if (PointerHover != null)
                            PointerHover(this, action);
                }
                else
                {
                    HoverTime = 0;
                    LastPosition = action.CanPosition;
                    if (PointerMove != null)
                        PointerMove(this, action);
                }
            }
        }
        internal virtual void OnMouseLeave(UserAction action)
        {
            entry = false;
            if (AutoColor)
            {
                if (!forbid)
                    Context.color = g_color;
            }
            if (PointerLeave != null)
                PointerLeave(this, action);
        }
        internal virtual void OnFocusMove(UserAction action)
        {
            if (Pressed)
                OnDrag(action);
        }
        protected virtual void OnDrag(UserAction action)
        {
            if(action.CanPosition!=action.LastPosition)
            if (Drag != null)
            {
                var v = action.Motion;
                v.x /= pgs.x;
                v.y /= pgs.y;
                Drag(this, action, v);
            }
        }
        internal virtual void OnDragEnd(UserAction action)
        {
            if (Scrolling != null)
            {
                var v = action.Velocities;
                v.x /= GlobalScale.x;
                v.y /= GlobalScale.y;
                maxVelocity = mVelocity = v;
                RefreshRateX();
                RefreshRateY();
            }
            if (DragEnd != null)
            {
                var v = action.Motion;
                v.x /= pgs.x;
                v.y /= pgs.y;
                DragEnd(this, action, v);
            }
        }
        internal virtual void OnLostFocus(UserAction action)
        {
            FocusAction = null;
            if (LostFocus != null)
                LostFocus(this, action);
        }
        public virtual void Reset()
        {
            Reset(this);
        }
        protected virtual void Initial()
        {
        }
        public void RemoveFocus()
        {
            if(FocusAction!=null)
            {
                Pressed = false;
                FocusAction.RemoveFocus(this);
                FocusAction = null;
            }
        }
        public void Dispose()
        {
            RemoveFocus();
            events.Remove(this);
        }
        #endregion
    }
}
