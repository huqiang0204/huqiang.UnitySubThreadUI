using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HotFix
{
    public class HotFixEntry
    {
        static IlRuntime runtime;
        public static void Initial(Transform parent, object dat = null)
        {
            runtime = new IlRuntime(dat as byte[], parent as RectTransform);
        }
        public static void Cmd(DataBuffer dat)
        {
            runtime.RuntimeCmd(dat);
        }
        public void ReSize()
        {
            runtime.RuntimeReSize();
        }
        public  void Dispose()
        {
        }
        public  void Update(float time)
        {
            runtime.RuntimeUpdate(time);
        }
    }
    public class IlRuntime
    {
        ILRuntime.Runtime.Enviorment.AppDomain _app;
        ILType mainScript;
        IMethod Update;
        IMethod Cmd;
        IMethod Resize;
        public IlRuntime(byte[] dat, RectTransform uiRoot)
        {
            _app = new ILRuntime.Runtime.Enviorment.AppDomain();
            RegDelegate();
            using (MemoryStream m = new MemoryStream(dat))
            {
                _app.LoadAssembly(m);
            }
            mainScript = _app.GetType("Main") as ILType;
            var start = mainScript.GetMethod("Start");
            if (start != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, start.Name, mainScript, uiRoot);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
            Update = mainScript.GetMethod("Update");
            Resize = mainScript.GetMethod("Resize");
            Cmd = mainScript.GetMethod("Cmd");
        }
        void RegDelegate()
        {
            _app.DelegateManager.RegisterMethodDelegate<object, object, int>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollItem, GameObject>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack, UserAction>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack, UserAction, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<TextInput>();
            _app.DelegateManager.RegisterMethodDelegate<TextInput, UserAction>();
            _app.DelegateManager.RegisterMethodDelegate<GestureEvent>();
            //_app.DelegateManager.RegisterMethodDelegate<ScrollX>();
            //_app.DelegateManager.RegisterMethodDelegate<ScrollX, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY, Vector2>();
            //_app.DelegateManager.RegisterMethodDelegate<ScrollExC, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<DragContent, Vector2>();
            //_app.DelegateManager.RegisterMethodDelegate<ScrollExY, Vector2>();
            //_app.DelegateManager.RegisterFunctionDelegate<GridScroll>();
           // _app.DelegateManager.RegisterFunctionDelegate<GridScroll, Vector2>();
           // _app.DelegateManager.RegisterFunctionDelegate<DropdownEx, object>();
            _app.DelegateManager.RegisterFunctionDelegate<TextInput, int, char, char>();
        }
        public void RuntimeUpdate(float time)
        {
            if (Update != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Update.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void RuntimeReSize()
        {
            if (Resize != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Resize.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void RuntimeCmd(DataBuffer buffer)
        {
            if (Cmd != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Cmd.Name, mainScript, buffer);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
    }
    public class RuntimeData
    {
        public byte[] dll;
        public AssetBundle asset;
    }
}
