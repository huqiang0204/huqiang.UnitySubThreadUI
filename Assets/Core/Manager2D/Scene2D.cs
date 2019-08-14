using System;
using UnityEngine;

namespace huqiang.Manager2D
{
    public class Scene2D
    {
        public Transform Root;
        public Camera camera;
        public GameObject Instance;
        public ScenePage CurrentPage;
        public Scene2D()
        {
            Instance = new GameObject("root");
            Root = Instance.transform;
            GameObject cam = new GameObject("cam");
            camera = cam.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Nothing;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localScale = Vector3.one;
        }
        public RenderTexture texture;
        bool _textureMode;
        public bool TextureMode { set {
                _textureMode = value;
                if (value)
                    camera.targetTexture = texture;
                else camera.targetTexture = null;
            } }
        public void ReSize(int w,int h)
        {
            texture = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
            if (_textureMode)
                camera.targetTexture = texture;
        }
        public void Dispose()
        {
            if (CurrentPage != null)
                CurrentPage.Dispose();
            CurrentPage = null;
            if (Instance != null)
                GameObject.Destroy(Instance);
            Instance = null;
        }
        public void ChangedScene<T>(object dat)where T:ScenePage,new()
        {
            if (CurrentPage is T)
            {
                CurrentPage.Show(dat);
                return;
            }
            UIAnimation.Manage.ReleaseAll();
            if (CurrentPage != null)
            {
                CurrentPage.Dispose();
            }
            var t = new T();
            t.Scene = this;
            t.Initial(Root, dat);
            CurrentPage = t;
        }
        public void Update()
        {
            if (CurrentPage != null)
                CurrentPage.Update();
        }
    }
}
