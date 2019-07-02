using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang
{
    public class App
    {
        static void InitialInput()
        {
            if(Application.platform == RuntimePlatform.Android |Application.platform==RuntimePlatform.IPhonePlayer)
            {
                UserAction.inputType = UserAction.InputType.OnlyTouch;
            }
            else
            {
                UserAction.inputType = UserAction.InputType.Blend;
            }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            IME.Initial();
#endif     
        }
        static void InitialUI()
        {
            ModelManagerUI.RegComponent(new ComponentType<RectTransform, ModelElement>(ModelElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Image, ImageElement>(ImageElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<EmojiText, EmojiElement>(TextElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Text, TextElement>(TextElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<CustomRawImage, RawImageElement>(RawImageElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<RawImage, RawImageElement>(RawImageElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<RectMask2D, RectMaskElement>(RectMaskElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Mask, MaskElement>(MaskElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Outline, OutLineElement>(OutLineElement.LoadFromObject));
        }
        static UIRoot uiroot;
        static void CreateUI()
        {
            uiroot = new UIRoot("Root");
            uiroot.SetCanvas(UIRoot);
            root = uiroot.model;
            UIPage.Root = uiroot.AddNode("page");
            UIMenu.Root = uiroot.AddNode("menu");
            UINotify.Root = uiroot.AddNode("notify");

            var buff = new GameObject("buffer", typeof(RectTransform));
            buff.transform.SetParent(UIRoot);
            buff.SetActive(false);
            ModelManagerUI.CycleBuffer = buff.transform;
        }
        static RectTransform UIRoot;
        static ThreadMission mission;
        static ModelElement root;
        public static void Initial(Transform uiRoot)
        {
            ThreadMission.SetMianId();
            Scale.Initial();
            InitialInput();
            InitialUI();
            UIRoot = uiRoot as RectTransform;
            if (UIRoot == null)
            {
                UIRoot = new GameObject("UI", typeof(Canvas)).transform as RectTransform;
                UIRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
            CreateUI();
            mission = new ThreadMission("UI");
        }
        public static float AllTime;
        public static float FrameTime = 33;
        static float time;
        public static void Update()
        {
            Scale.MainUpdate();
           
            UserAction.DispatchEvent();
            Keyboard.DispatchEvent();
            root.Apply();//更新UI
            ThreadMission.ExtcuteMain();
            ModelManagerUI.RecycleGameObject();
            AnimationManage.Manage.Update();
            AllTime += Time.deltaTime;
            mission.AddSubMission(SubThread, null);
            //DownloadManager.UpdateMission();
        }
        static void SubThread(object obj)
        {
            UserAction.SubDispatch();
            Resize();
            UIPage.Refresh(UserAction.TimeSlice);
            UINotify.Refresh(UserAction.TimeSlice);
            UIAnimation.Manage.Update();
        }
        static void Resize()
        {
            if(Scale.ScreenChanged())
            {
                Vector2 v = new Vector2(Scale.LayoutWidth, Scale.LayoutHeight);
                UIPage.Root.data.sizeDelta = v;
                if (Scale.DpiScale)
                {
                    var dr = Scale.DpiRatio;
                    UIPage.Root.data.localScale = new Vector3(dr, dr, dr);
                }
                else UIPage.Root.data.localScale = Vector3.one;
                UIPage.Root.IsChanged = true;
                if (UIPage.CurrentPage != null)
                    UIPage.CurrentPage.ReSize();
                if (UIMenu.Instance != null)
                    UIMenu.Instance.ReSize();
                if (UINotify.Instance != null)
                    UINotify.Instance.ReSize();
            }
        }
        public static void Dispose()
        {
            EventCallBack.ClearEvent();
            ThreadMission.DisposeAll();
            RecordManager.ReleaseAll();
        }
    }
}