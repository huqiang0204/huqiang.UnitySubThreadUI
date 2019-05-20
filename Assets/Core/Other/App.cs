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
        static void Initial()
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
            ModelManagerUI.RegComponent(new ComponentType<Mask, MaskElement>(MaskElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Outline, OutLineElement>(OutLineElement.LoadFromObject));
        }
        static RectTransform UIRoot;
        static ThreadMission mission;
        static ModelElement root;
        public static void Initial(Transform uiRoot)
        {
            ThreadMission.SetMianId();
            Initial();
            InitialUI();
            UIRoot = uiRoot as RectTransform;
            if (UIRoot == null)
            {
                UIRoot = new GameObject("UI", typeof(Canvas)).transform as RectTransform;
                UIRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
       
            root = ModelElement.CreateNew("Root");
            root.Context = new GameObject("Root",typeof(RectTransform)).transform as RectTransform;
            root.Context.SetParent(UIRoot);
            root.Context.localPosition = Vector3.zero;
            root.Context.sizeDelta = new Vector2(Screen.width,Screen.height);
            EventCallBack.InsertRoot(root);

            var page = ModelElement.CreateNew("page");
            page.Context = new GameObject("page", typeof(RectTransform)).transform as RectTransform;
            page.Context.SetParent(root.Context);
            page.SetParent(root);
            UIPage.Root = page;
            var notify = ModelElement.CreateNew("notify");
            notify.Context = new GameObject("notify", typeof(RectTransform)).transform as RectTransform;
            notify.SetParent(root);
            notify.Context.SetParent(root.Context);
            UINotify.Root = notify;
            
          
            var buff = new GameObject("buffer", typeof(RectTransform));
            buff.transform.SetParent(UIRoot);
            buff.SetActive(false);
            ModelManagerUI.CycleBuffer = buff.transform;
            mission = new ThreadMission();
        }
        public static float AllTime;
        public static float FrameTime = 33;
        static float time;
        public static void Update()
        {
            Scale.ScreenCurrentWidth = Screen.width;
            Scale.ScreenCurrentHeight= Screen.height;
            UserAction.DispatchEvent();
            Keyboard.DispatchEvent();
            mission.AddSubMission(SubThread,null);
            root.Apply();//更新UI
            ThreadMission.ExtcuteMain();
            ModelManagerUI.RecycleGameObject();
            //time -= FrameTime;
       
            AnimationManage.Manage.Update();
            AllTime += Time.deltaTime;
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
            float w = Scale.ScreenCurrentWidth;
            float h = Scale.ScreenCurrentHeight;
            if (Scale.ScreenWidth != w | Scale.ScreenHeight != h)
            {
                Scale.ScreenWidth = w;
                Scale.ScreenHeight = h;
                Vector2 v = new Vector2(w, h);
                UIPage.Root.data.sizeDelta = v;
                UIPage.Root.IsChanged = true;
                if (UIPage.CurrentPage != null)
                    UIPage.CurrentPage.ReSize();
                UINotify.Root.data.sizeDelta = v;
                UINotify.Root.IsChanged = true;
                if (UINotify.CurrentPage != null)
                    UINotify.CurrentPage.ReSize();
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