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
    }
}
