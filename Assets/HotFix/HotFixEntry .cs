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
using huqiang;

namespace HotFix
{
    public class HotFixEntry:UIPage
    {
        IlRuntime runtime;
        public override  void Initial(ModelElement model, object dat = null)
        {
            runtime = new IlRuntime(dat as byte[]);
        }
        public override void Show(object dat = null)
        {
            runtime.Show(dat);
        }
        public override void Cmd(DataBuffer dat)
        {
            runtime.Cmd(dat);
        }
        public override void Cmd(string cmd, object dat)
        {
            runtime.UpdateData(cmd,dat);
        }
        public override void ReSize()
        {
            runtime.Resize();
        }
        public override void ChangeLanguage()
        {
            runtime.ChnageLanguage();
        }
        public override  void Dispose()
        {
            runtime.Dispose();
        }
    }
    public class IlRuntime
    {
        ILRuntime.Runtime.Enviorment.AppDomain _app;
        ILType mainScript;
        MemoryStream m;
        IMethod mStart, mUpdate, mShow, mCmd, mUpdateData, mResize, mChangeLanguage, mDispose;
        public IlRuntime(byte[] dat)
        {
            _app = new ILRuntime.Runtime.Enviorment.AppDomain();
            RegDelegate();
            MemoryStream m = new MemoryStream(dat);
                _app.LoadAssembly(m);
            RegAdaptor();
            mainScript = _app.GetType("Main") as ILType;
            mStart = mainScript.GetMethod("Start");
            mUpdate = mainScript.GetMethod("Update");
            mShow = mainScript.GetMethod("Show");
            mCmd = mainScript.GetMethod("Cmd");
            mUpdateData = mainScript.GetMethod("UpdateData");
            mResize = mainScript.GetMethod("Resize");
            mChangeLanguage = mainScript.GetMethod("ChangeLanguage");
            mDispose = mainScript.GetMethod("Dispose");
            if (mStart != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mStart.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        void RegDelegate()
        {
            _app.DelegateManager.RegisterMethodDelegate<object, object, int>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollItem, GameObject>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack, UserAction>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<EventCallBack, UserAction, Vector2>();

            _app.DelegateManager.RegisterMethodDelegate<GestureEvent>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollX>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollX, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<DragContent, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<GridScroll>();
            _app.DelegateManager.RegisterMethodDelegate<GridScroll, Vector2>();

            _app.DelegateManager.RegisterMethodDelegate<DropdownEx, object>();
            _app.DelegateManager.RegisterMethodDelegate<UIPalette>();
            _app.DelegateManager.RegisterMethodDelegate<UISlider>();

            _app.DelegateManager.RegisterFunctionDelegate<TextInput>();
            _app.DelegateManager.RegisterFunctionDelegate<TextInput, UserAction>();
            _app.DelegateManager.RegisterFunctionDelegate<TextInput, int, char, char>();
        }
        void RegAdaptor()
        {
            //_app.RegisterCrossBindingAdaptor(new UIPageInheritanceAdaptor());
            _app.RegisterCrossBindingAdaptor(new SceneInheritanceAdaptor());
        }
        public void Show(object dat)
        {
            if (mShow != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mShow.Name, mainScript,dat);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void Update(float time)
        {
            if (mUpdate != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mUpdate.Name, mainScript,time);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void Cmd(DataBuffer db)
        {
            if (mCmd != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mCmd.Name, mainScript, db);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void UpdateData(string cmd, object dat)
        {
            if (mUpdateData != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mUpdateData.Name, mainScript, cmd,dat);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void Resize()
        {
            if (mResize != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mResize.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void ChnageLanguage()
        {
            if (mChangeLanguage != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mChangeLanguage.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void Dispose()
        {
            if (mDispose != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, mDispose.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
            if (m!=null)
            {
                m.Dispose();
                m = null;
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
            base.Dispose();
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
            mSHow = instance.Type.GetMethod("Show", 1);
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
