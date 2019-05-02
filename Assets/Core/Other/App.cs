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
            ThreadPool.Initial();
            EmojiText.Emoji = UnityEngine.Resources.Load<Texture2D>("emoji");
            if(Application.platform == RuntimePlatform.Android |Application.platform==RuntimePlatform.IPhonePlayer)
            {
                UserAction.inputType = UserAction.InputType.OnlyTouch;
            }
            else
            {
                UserAction.inputType = UserAction.InputType.Blend;
            }
            if (Application.platform == RuntimePlatform.WindowsEditor | Application.platform == RuntimePlatform.WindowsPlayer)
            {
                IME.Initial();
            }
        }
        static void InitialUI()
        {
            ModelManagerUI.RegComponent(new ComponentType<RectTransform, ModelElement>(ModelElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Image, ImageElement>(ImageElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<EmojiText, TextElement>(TextElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Text, TextElement>(TextElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<CustomRawImage, RawImageElement>(RawImageElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<RawImage, RawImageElement>(RawImageElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Mask, MaskElement>(MaskElement.LoadFromObject));
            ModelManagerUI.RegComponent(new ComponentType<Outline, OutLineElement>(OutLineElement.LoadFromObject));
        }
        static RectTransform UIRoot;
        static ThreadMission mission;
        public static void Initial(Transform uiRoot)
        {
            Initial();
            InitialUI();
            if (uiRoot == null)
            {
                uiRoot = new GameObject("UI", typeof(Canvas)).transform;
                uiRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                UIRoot = new GameObject("uiRoot", typeof(RectTransform)).transform as RectTransform;
              
            }
            var mod = Page.Root = new UI.ModelElement();
            mod.name = "uiRoot";
            mod.data.localScale = Vector3.one;
            mod.data.anchorMax=mod.data.anchorMin=
            mod.data.pivot = new Vector2(0.5f,0.5f);
            Page.Root.activeSelf = true;
            Page.Root.Context = new GameObject("uiRoot",typeof(RectTransform)).transform as RectTransform;
            Page.Root.Context.SetParent(uiRoot);
            Page.Root.Context.localPosition = Vector3.zero;
            Page.Root.Context.localScale = Vector3.one;
            EventCallBack.InsertRoot(Page.Root);
            var buff = new GameObject("buffer", typeof(Canvas));
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
            mission.AddSubMission(SubThread,null);
            ThreadPool.ExtcuteMain();
            ModelManagerUI.RecycleGameObject();
            time += UserAction.TimeSlice;
            if(time>=FrameTime)
            {
                time -= FrameTime;
                Page.Root.Apply();//更新UI
            }
            AllTime += Time.deltaTime;
            //DownloadManager.UpdateMission();
        }
        static void SubThread(object obj)
        {
            UserAction.SubDispatch();
            Resize();
            Page.Refresh(UserAction.TimeSlice);
        }
        static void Resize()
        {
            float w = Scale.ScreenCurrentWidth;
            float h = Scale.ScreenCurrentHeight;
            //float s = Scale.ScreenScale;
            //Page.Root.data.localScale = new Vector3(s, s, s);
            //w /= s;
            //h /= s;
            if (Scale.ScreenWidth != w | Scale.ScreenHeight != h)
            {
                Scale.ScreenWidth = w;
                Scale.ScreenHeight = h;
                Page.Root.data.sizeDelta = new Vector2(w, h);
                Page.Root.IsChanged = true;
                if (Page.CurrentPage != null)
                    Page.CurrentPage.ReSize();
            }
        }
        public static void Dispose()
        {
            EventCallBack.ClearEvent();
            ThreadPool.Dispose();
            RecordManager.ReleaseAll();
            mission.Dispose();
        }
    }
}