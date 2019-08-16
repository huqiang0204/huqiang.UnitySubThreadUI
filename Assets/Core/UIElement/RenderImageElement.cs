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
        public void LoadAsync(Func<ScenePage> FPage,object dat) 
        {
            ThreadMission.InvokeToMain((o) => {
                base.Apply();
                ApplyScene();
                Scene.ChangeScene(FPage(),dat);
            }, null);
        }
        public void LoadAsync<T>(object dat)where T:ScenePage,new ()
        {
            ThreadMission.InvokeToMain((o) => {
                base.Apply();
                ApplyScene();
                Scene.ChangeScene<T>(dat);
            }, null);
        }
        public void Invoke(Action<RenderImageElement,object> action,object data)
        {
            ThreadMission.InvokeToMain((o) => { action(this,data); }, null);
        }
        public void InvokePage<T,U>(Action<T,U> action,U data) where T : ScenePage, new()
        {
            ThreadMission.InvokeToMain((o)=> { action(Scene.CurrentPage as T,data); }, null);
        }
        void ApplyScene()
        {
            int lw = (int)model.data.sizeDelta.x;
            int lh = (int)model.data.sizeDelta.y;
            if (Scene == null)
            {
                Scene = new Scene2D();
                Scene.TextureMode = true;
                Scene.ReSize(lw, lh);
                offset += 100;
                Scene.Root.position = new Vector3(offset,0,0);
                Context.texture = Scene.texture;
            }
            else
            {
                if (w != lw | h != lh)
                {
                    Scene.ReSize(w, h);
                    Context.texture = Scene.texture;
                }
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
