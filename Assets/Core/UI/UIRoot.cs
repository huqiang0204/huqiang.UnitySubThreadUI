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
    public class UIRoot
    {
        public ModelElement model;
        public ModelElement AddNode(string name)
        {
            var node = ModelElement.CreateNew(name);
            node.SetParent(model);
            return node;
        }
        public UIRoot(string name)
        {
            model = ModelElement.CreateNew(name);
            EventCallBack.InsertRoot(model);
        }
        public void SetCanvas(RectTransform rect)
        {
            if (model.Context == null)
                model.Apply();
            model.Context.SetParent(rect);
            model.Context.localPosition = Vector3.zero;
            model.Context.localScale = Vector3.one;
        }
        ~UIRoot()
        {
            EventCallBack.RemoveRoot(model);
        }
        public void Dispose()
        {
            EventCallBack.RemoveRoot(model);
        }
        public UserAction[] inputs;
        public bool PauseEvent;
        public void SubDispatch()
        {
#if DEBUG
            try
            {
#endif
                //EventCallBack.Rolling();
#if DEBUG
            }
            catch (Exception ex)
            {
                Debug.Log(ex.StackTrace);
            }
#endif
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
            //TextInput.SubDispatch();
            GestureEvent.Dispatch(new List<UserAction>(inputs));
        }
    }
}
