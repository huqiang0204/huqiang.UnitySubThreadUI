using huqiang.UI;
using System;
using UnityEngine;

namespace huqiang
{
    public static class UIAnimationExtand
    {
        public static UIMove FindMoveAni(this ModelElement trans)
        {
            if (trans == null)
                return null;
            return UIAnimation.Manage.FindAni<UIMove>((o) => { return o.Target == trans ? true : false; });
        }
        public static UIMove MoveTo(this UIMove ani, Vector3 pos)
        {
            ani.StartPosition = ani.Target.data.localPosition;
            ani.EndPosition = pos;
            return ani;
        }
        public static UIMove ScaleTo(this UIMove ani, Vector3 pos)
        {
            ani.StartScale = ani.Target.data.localScale;
            ani.EndScale= pos;
            return ani;
        }
        public static UIMove AngleTo(this UIMove ani, Vector3 pos)
        {
            ani.StartAngle = ani.Target.data.localRotation.eulerAngles;
            ani.EndAngle = pos;
            return ani;
        }
        public static UIMove SizeTo(this UIMove ani, Vector2 pos)
        {
            ani.StartSize = ani.Target.data.sizeDelta;
            ani.EndSize = pos;
            return ani;
        }
        public static UIMove MoveTo(this ModelElement trans, Vector3 pos, float time, bool hide = false, float delay = 0, Action<UIMove> over = null, bool cover = true)
        {
            if (trans == null)
                return null;
            trans.activeSelf = true;
            var ani = UIAnimation.Manage.FindAni<UIMove>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new UIMove(trans);
            else if (!cover)
                return null;
            ani.StartPosition = trans.data.localPosition;
            ani.EndPosition = pos;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
            return ani;
        }
        public static UISprites FindSpritesAni(this ImageElement img)
        {
            if (img == null)
                return null;
            img.model.activeSelf = true;
            return UIAnimation.Manage.FindAni<UISprites>((o) => { return o.image == img ? true : false; });
        }
        public static void Play(this ImageElement img, Sprite[] sprites, float inter = 16, bool loop = false, Action<UISprites> over = null, bool hide = true, bool cover = true)
        {
            if (img == null)
                return;
            img.model.activeSelf = true;
            var ani = AnimationManage.Manage.FindAni<UISprites>((o) => { return o.image == img ? true : false; });
            if (ani == null)
                ani = new UISprites(img);
            else if (!cover)
                return;
            ani.autoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Loop = loop;
            ani.Interval = inter;
            ani.Play(sprites);
        }

    }
}
