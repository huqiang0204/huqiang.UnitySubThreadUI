using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UI
{
    public class RenderForm
    {
        static List<RenderForm> forms=new List<RenderForm>();
        public static void LoadAction()
        {
            for (int i = 0; i < forms.Count; i++)
                forms[i].LoadUserAction();
        }
        public static void DispatchAction()
        {
            for (int i = 0; i < forms.Count; i++)
                forms[i].DispatchUserAction();
        }
        public static void VertexCalculationAll()
        {
            for (int i = 0; i < forms.Count; i++)
                forms[i].model.Update();
        }
        public static void ApplyAll()
        {
            for (int i = 0; i < forms.Count; i++)
                forms[i].model.Apply();
        }
        public ModelElement model;
        public ModelElement AddNode(string name)
        {
            var node = ModelElement.CreateNew(name);
            node.SetParent(model);
            return node;
        }
        public RenderForm(string name)
        {
            model = ModelElement.CreateNew(name);
            forms.Add(this);
        }
        public void SetCanvas(RectTransform rect)
        {
            if (model.Context == null)
                model.Apply();
            model.Context.SetParent(rect);
            model.Context.localPosition = Vector3.zero;
            model.Context.localScale = Vector3.one;
        }
        ~RenderForm()
        {
            Dispose();
        }
        public virtual void Dispose()
        {
            PauseEvent = true;
            forms.Remove(this);
        }
        public UserAction[] inputs;
        public bool PauseEvent;
        public UserAction.InputType inputType = UserAction.InputType.OnlyMouse;
        void DispatchTouch()
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
                }
                else inputs[i].IsActive = false;
                label:;
            }
        }
        void DispatchMouse()
        {
            if (inputs == null)
            {
                inputs = new UserAction[1];
                inputs[0] = new UserAction(0);
            }
            var action = inputs[0];
            action.LoadMouse();
        }
        void DispatchWin()
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
        public virtual void LoadUserAction()
        {
            if (inputType == UserAction.InputType.OnlyMouse)
            {
                DispatchMouse();
            }
            else if (inputType == UserAction.InputType.OnlyTouch)
            {
                DispatchTouch();
            }
            else
            {
                DispatchWin();
            }
        }
        public void DispatchUserAction()
        {
            if (PauseEvent)
                return;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] != null)
                {
#if DEBUG
                    try
                    {
#endif
                        if (inputs[i].IsActive)
                            inputs[i].Dispatch(model);
#if DEBUG
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.StackTrace);
                    }
#endif
                }
            }
            if(inputs.Length>1)
            GestureEvent.Dispatch(new List<UserAction>(inputs));
        }
        public  void ClearAllAction()
        {
            if (inputs != null)
                for (int i = 0; i < inputs.Length; i++)
                    inputs[i].Clear();
        }
    }
}
