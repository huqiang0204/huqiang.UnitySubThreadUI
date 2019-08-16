using huqiang.Data;
using huqiang.Manager2D;
using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
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
            RegAdaptor();
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
            _app.DelegateManager.RegisterFunctionDelegate<TextInput>();
            _app.DelegateManager.RegisterFunctionDelegate<TextInput, UserAction>();
            _app.DelegateManager.RegisterFunctionDelegate<TextInput, int, char, char>();
            _app.DelegateManager.RegisterMethodDelegate<GestureEvent>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollX>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollX, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<DragContent, Vector2>();
            _app.DelegateManager.RegisterFunctionDelegate<GridScroll>();
            _app.DelegateManager.RegisterFunctionDelegate<GridScroll, Vector2>();

             _app.DelegateManager.RegisterFunctionDelegate<DropdownEx, object>();
    
        }
        void RegAdaptor()
        {
            _app.RegisterCrossBindingAdaptor(new UIPageInheritanceAdaptor());
            _app.RegisterCrossBindingAdaptor(new SceneInheritanceAdaptor());
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
    public class UIPageInheritanceAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType => typeof(UIPage);

        public override Type AdaptorType => typeof(UIPageAdaptor);

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new UIPageAdaptor(appdomain,instance);
        }
    }
    public class UIPageAdaptor : UIPage, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        IMethod mInitial,mUpdate,mSHow, mChangeLanguage,mCmd,mDispose;
        bool isTestVirtualInvoking = false;
        public ILTypeInstance ILInstance => instance;
        public UIPageAdaptor()
        {
        }
        public UIPageAdaptor(ILRuntime.Runtime.Enviorment.AppDomain app, ILTypeInstance ins)
        {
            appdomain = app;
            instance = ins;
            mInitial = instance.Type.GetMethod("Initial", 2);
            mUpdate = instance.Type.GetMethod("Update",1);
            mSHow= instance.Type.GetMethod("Show", 1);
            mChangeLanguage = instance.Type.GetMethod("ChangeLanguage");
            mCmd = instance.Type.GetMethod("Cmd",1);
            mDispose = instance.Type.GetMethod("Dispose");
        }
        public override void Initial(ModelElement parent, object dat = null)
        {
            if (mInitial != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mInitial, instance, parent, dat);
                isTestVirtualInvoking = false;
            }
            else
                base.Initial(parent,dat);
        }
        public override void Show(object dat = null)
        {
            if (mSHow != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mSHow, instance, dat);
                isTestVirtualInvoking = false;
            }
        }
        public override void Update(float time)
        {
            if (mUpdate != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mUpdate, instance, time);
                isTestVirtualInvoking = false;
            }
        }
        public override void ChangeLanguage()
        {
            if (mChangeLanguage != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mChangeLanguage, instance);
                isTestVirtualInvoking = false;
            }
        }
        public override void Cmd(DataBuffer dat)
        {
            if (mCmd != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mCmd, instance, dat);
                isTestVirtualInvoking = false;
            }
        }
        public override void Dispose()
        {
            if (mDispose != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mDispose, instance);
                isTestVirtualInvoking = false;
            }
        }
        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
                return instance.ToString();
            else
                return instance.Type.FullName;
        }
    }
    public class SceneInheritanceAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType => typeof(UIPage);

        public override Type AdaptorType => typeof(UIPageAdaptor);

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new ScenePageAdaptor(appdomain, instance);
        }
    }
    public class ScenePageAdaptor : ScenePage, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        public ILTypeInstance ILInstance => instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        IMethod mInitial, mUpdate,mSHow, mCmd, mDispose;
        bool isTestVirtualInvoking = false;
        public ScenePageAdaptor(ILRuntime.Runtime.Enviorment.AppDomain app, ILTypeInstance ins)
        {
            appdomain = app;
            instance = ins;
            mInitial = instance.Type.GetMethod("Initial", 2);
            mUpdate = instance.Type.GetMethod("Update");
            mCmd = instance.Type.GetMethod("Cmd", 1);
            mDispose = instance.Type.GetMethod("Dispose");
        }
        public override void Initial(Transform trans, object dat)
        {
            if (mInitial != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mInitial, instance, trans, dat);
                isTestVirtualInvoking = false;
            }
            else
                base.Initial(trans, dat);
        }
        public override void Show(object dat)
        {
            if (mSHow != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mSHow, instance, dat);
                isTestVirtualInvoking = false;
            }
        }
        public override void Update()
        {
            if (mUpdate != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mUpdate, instance);
                isTestVirtualInvoking = false;
            }
        }
        public override void Dispose()
        {
            if (mDispose != null && !isTestVirtualInvoking)
            {
                isTestVirtualInvoking = true;
                appdomain.Invoke(mDispose, instance);
                isTestVirtualInvoking = false;
            }
        }
    }
}
