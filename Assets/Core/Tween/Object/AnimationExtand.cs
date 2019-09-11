using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang
{
    public static class AnimationExtand
    {
        public static MoveAnimat FindMoveAni(this Transform trans)
        {
            if (trans == null)
                return null;
           return AnimationManage.Manage.FindAni<MoveAnimat>((o) => { return o.Target == trans ? true : false; });
        }
        public static void MoveTo(this Transform trans, Vector3 pos, float time,bool hide=false, float delay = 0, Action<MoveAnimat> over = null, bool cover = true)
        {
            if (trans == null)
                return;
            trans.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<MoveAnimat>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new MoveAnimat(trans);
            else if (!cover)
                return;
            ani.StartPosition = trans.localPosition;
            ani.EndPosition = pos;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        public static RotateAnimat FindRotateAni(this Transform trans)
        {
            if (trans == null)
                return null;
            return AnimationManage.Manage.FindAni<RotateAnimat>((o) => { return o.Target == trans ? true : false; });
        }
        public static void RotateTo(this Transform trans, Vector3 angle, float time, bool hide = false, float delay = 0, Action<RotateAnimat> over = null, bool cover = true)
        {
            if (trans == null)
                return;
            trans.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<RotateAnimat>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new RotateAnimat(trans);
            else if (!cover)
                return;
            ani.StartAngle = trans.localEulerAngles;
            ani.EndAngle = angle;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        public static ScaleAnimat FindScaleAni(this Transform trans)
        {
            if (trans == null)
                return null;
            return AnimationManage.Manage.FindAni<ScaleAnimat>((o) => { return o.Target == trans ? true : false; });
        }
        public static void ScaleTo(this Transform trans, Vector3 scale, float time, bool hide = false, float delay=0, Action<ScaleAnimat> over = null, bool cover = true)
        {
            if (trans == null)
                return;
            trans.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<ScaleAnimat>((o) => { return o.Target == trans ? true : false; });
            if (ani == null)
                ani = new ScaleAnimat(trans);
            else if (!cover)
                return;
            ani.StartScale = trans.localScale;
            ani.EndScale = scale;
            ani.Time = time;
            ani.Delay = delay;
            ani.AutoHide = hide;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        public static ColorAnimat FindColorAni(this Graphic grap)
        {
            if (grap == null)
                return null;
            grap.gameObject.SetActive(true);
            return AnimationManage.Manage.FindAni<ColorAnimat>((o) => { return o.Target == grap ? true : false; });
        }
        public static void ColorTo(this Graphic grap, Color col, float time, float delay=0, Action<ColorAnimat> over = null, bool cover = true)
        {
            if (grap == null)
                return;
            grap.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<ColorAnimat>((o) => { return o.Target == grap ? true : false; });
            if (ani == null)
                ani = new ColorAnimat(grap);
            else if (!cover)
                return;
            ani.StartColor = grap.color;
            ani.EndColor = col;
            ani.Time = time;
            ani.Delay = delay;
            if (over == null)
                ani.PlayOver = (o) => { o.Dispose(); };
            else ani.PlayOver = over;
            ani.Play();
        }
        public static ImageAnimat FindSpritesAni(this Image img)
        {
            if (img == null)
                return null;
            img.gameObject.SetActive(true);
            return AnimationManage.Manage.FindAni<ImageAnimat>((o) => { return o.image == img ? true : false; });
        }
        public static ImageAnimat FindOrCreateSpritesAni(this Image img)
        {
            if (img == null)
                return null;
            img.gameObject.SetActive(true);
            var ani= AnimationManage.Manage.FindAni<ImageAnimat>((o) => { return o.image == img ? true : false; });
            if(ani==null)
                ani = new ImageAnimat(img);
            return ani;
        }
        public static void Play(this Image img, Sprite[] sprites, float inter = 16, bool loop = false, Action<ImageAnimat> over = null, bool hide = true, bool cover = true)
        {
            if (img == null)
                return;
            img.gameObject.SetActive(true);
            var ani = AnimationManage.Manage.FindAni<ImageAnimat>((o) => { return o.image == img ? true : false; });
            if (ani == null)
                ani = new ImageAnimat(img);
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
        public static void Play(this Material mat,string name, float sv,float ev,float time,float delay=0, Action<ShaderAnimat> over = null, bool cover =true)
        {
            if (mat == null)
                return;
            var ani = AnimationManage.Manage.FindAni<ShaderAnimat>((o) => { return o.Target == mat ? true : false; });
            if (ani == null)
            {
                ani = new ShaderAnimat(mat);
                ShaderFloat sf = new ShaderFloat();
                sf.ParameterName = name;
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            else
            {
                var sf = ani.FindFloatShader(name);
                if (sf != null)
                {
                    if (!cover)
                        return;
                }
                else sf = new ShaderFloat();
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            ani.Play();
        }
        public static void Play(this Material mat, string name, Vector4 sv, Vector4 ev, float time, float delay = 0, Action<ShaderAnimat> over = null, bool cover = true)
        {
            if (mat == null)
                return;
            var ani = AnimationManage.Manage.FindAni<ShaderAnimat>((o) => { return o.Target == mat ? true : false; });
            if (ani == null)
            {
                ani = new ShaderAnimat(mat);
                ShaderVector4 sf = new ShaderVector4();
                sf.ParameterName = name;
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            else
            {
                var sf = ani.FindVectorShader(name);
                if (sf != null)
                {
                    if (!cover)
                        return;
                }
                else sf = new ShaderVector4();
                sf.StartValue = sv;
                sf.EndValue = ev;
                sf.Time = time;
                sf.Delay = delay;
                ani.Time = time;
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            ani.Play();
        }
        public static GifAnimat Findt2dsAni(this RawImageElement raw)
        {
            if (raw == null)
                return null;
            raw.model.activeSelf = true;
           return AnimationManage.Manage.FindAni<GifAnimat>((o) => { return o.image == raw ? true : false; });
        }
        public static void Play(this RawImageElement raw, List<Texture2D> t2ds, bool loop = true, int count=0, int inter = 66, Action<GifAnimat> over = null,bool hide=true,bool cover=true)
        {
            if (raw == null)
                return;
            raw.model.activeSelf = true;
            var ani = AnimationManage.Manage.FindAni<GifAnimat>((o) => { return o.image == raw ? true : false; });
            if (ani == null)
            {
                ani = new GifAnimat(raw);
                if (over == null)
                    ani.PlayOver = (o) => { o.Dispose(); };
                else ani.PlayOver = over;
            }
            else if (!cover)
                return;
            ani.Play(t2ds);
            ani.Loop = loop;
            ani.Interval = inter;
            if (count > 0)
                ani.gifCount = count;
        }
    }
}