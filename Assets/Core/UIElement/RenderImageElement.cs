using huqiang.Manager2D;
using System;
using UnityEngine;

namespace huqiang.UI
{
    public class RenderImageElement:RawImageElement
    {
        static float offset = 1000;
        public Scene2D Scene;
        int w;
        int h;
        public void LoadAsync<T>(object dat)where T:ScenePage,new ()
        {
            ThreadMission.InvokeToMain((o) => {
                base.Apply();
                ApplyScene();
                Scene.ChangedScene<T>(dat);
            }, null);
        }
        public void Invoke(Action<RenderImageElement> action)
        {
            ThreadMission.InvokeToMain((o) => { action(this); }, null);
        }
        public void InvokePage<T>(Action<T> action) where T : ScenePage, new()
        {
            ThreadMission.InvokeToMain((o)=> { action(Scene.CurrentPage as T); }, null);
        }
        void ApplyScene()
        {
            int lw = (int)model.data.sizeDelta.x;
            int lh = (int)model.data.sizeDelta.y;
            if (Scene == null)
            {
                Scene = new Scene2D();
                Scene.TextureMode = true;
                Scene.ReSize(w, h);
                offset += 100;
                Scene.Root.position = new Vector3(offset,0,0);
            }
            else
            {
                if (w != lw | h != lh)
                    Scene.ReSize(w, h);
            }
            w = lw;
            h = lh;
        }
        public override void Apply()
        {
            base.Apply();
            ApplyScene();
            Scene.Update();
        }
    }
}
