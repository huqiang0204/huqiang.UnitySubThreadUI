using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 基本动画
    /// </summary>
    public class AnimatBase
    {
        public object DataContext { get; set; }
        /// <summary>
        /// 总计时长
        /// </summary>
        protected float m_time;
        public float Delay { get; set; }
        public float Time { get { return m_time; } set { m_time = value; } }
        public bool Loop;
        /// <summary>
        /// 播放累计时长
        /// </summary>
        protected float c_time;
        protected bool playing = false;
        public bool AutoHide = false;
        public virtual void Play()
        {
            playing = true;
            c_time = 0;
        }
        public virtual void Pause()
        {
            playing = false;
        }
        /// <summary>
        /// 动画运动线
        /// </summary>
        public LinearTransformation Linear;
        /// <summary>
        /// 用于缓存数据
        /// </summary>
        public float[] DataCache;
        public Vector3[] points;
        /// <summary>
        /// 设置一个抛物线的中间点，x的值在0-1范围内,返回一个抛物线一般表达式的a，b，c的值，
        /// </summary>
        /// <param name="ani"></param>
        /// <param name="point">参数不能为(1,1)</param>
        public static AnimatBase SetParabola(AnimatBase ani, Vector2 point)
        {
            var v = MathH.Parabola(Vector2.zero, point, new Vector2(1, 1));
            ani.DataCache = new float[3];
            ani.DataCache[0] = v.x;
            ani.DataCache[1] = v.y;
            ani.DataCache[2] = v.z;
            ani.Linear = Parabola;
            return ani;
        }
        static float Parabola(AnimatBase ani, float a)
        {
            var temp = ani.DataCache;
            return temp[0] * a * a + a * temp[1] + temp[2];
        }
        /// <summary>
        /// 设置一个单弧线
        /// </summary>
        /// <param name="ani"></param>
        /// <param name="r">弧度0-180</param>
        public static AnimatBase SetArc(AnimatBase ani, float r)
        {
            ani.DataCache = new float[3];
            ani.DataCache[0] = r;//弧度
            ani.DataCache[1] = 270 - r * 0.5f;//起始弧度
            ani.DataCache[2] = MathH.Cos(270 + r * 0.5f);//弧度总长
            ani.Linear = Arc;
            return ani;
        }
        static float Arc(AnimatBase ani, float a)
        {
            float s = ani.DataCache[1] + a * ani.DataCache[0];
            return 0.5f + 0.5f * MathH.Cos(s) / ani.DataCache[2];//180-360 -1到1之间
        }
        /// <summary>
        /// 设置一个S行曲线
        /// </summary>
        /// <param name="ani"></param>
        /// <param name="x">0-0.5f</param>
        /// <param name="y">0-0.5f</param>
        public static AnimatBase SetSArc(AnimatBase ani, float x = 0.4f, float y = 0.1f)
        {
            var tmp = new Vector3[5];
            tmp[1].x = x;
            tmp[1].y = y;
            tmp[2] = new Vector3(0.5f, 0.5f, 0);
            tmp[3].x = 0.5f + y;
            tmp[3].y = 0.5f + x;
            tmp[4] = new Vector3(1, 1, 0);
            ani.points = tmp;
            ani.Linear = SArc;
            return ani;
        }
        static float SArc(AnimatBase ani, float a)
        {
            var tmp = ani.points;
            if (tmp == null)
                return a;
            if (a < 0.5f)
            {
                a *= 2;
                return MathH.BezierPoint(a, ref tmp[0], ref tmp[1], ref tmp[2]).y;
            }
            else
            {
                a = (a - 0.5f) * 2;
                return MathH.BezierPoint(a, ref tmp[2], ref tmp[3], ref tmp[4]).y;
            }
        }
    }
}
