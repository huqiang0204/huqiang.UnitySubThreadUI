using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang
{
    public class LTBezier
    {
        public float length;
        private Vector3 a;
        private Vector3 aa;
        private Vector3 bb;
        private Vector3 cc;
        private float len;
        private float[] arcLengths;

        public LTBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision)
        {
            this.a = a;
            aa = (-a + 3 * (b - c) + d);
            bb = 3 * (a + c) - 6 * b;
            cc = 3 * (b - a);

            this.len = 1.0f / precision;
            arcLengths = new float[(int)this.len + (int)1];
            arcLengths[0] = 0;

            Vector3 ov = a;
            Vector3 v;
            float clen = 0.0f;
            for (int i = 1; i <= this.len; i++)
            {
                v = bezierPoint(i * precision);
                clen += (ov - v).magnitude;
                this.arcLengths[i] = clen;
                ov = v;
            }
            this.length = clen;
        }

        private float map(float u)
        {
            float targetLength = u * this.arcLengths[(int)this.len];
            int low = 0;
            int high = (int)this.len;
            int index = 0;
            while (low < high)
            {
                index = low + ((int)((high - low) / 2.0f) | 0);
                if (this.arcLengths[index] < targetLength)
                {
                    low = index + 1;
                }
                else
                {
                    high = index;
                }
            }
            if (this.arcLengths[index] > targetLength)
                index--;
            if (index < 0)
                index = 0;

            return (index + (targetLength - arcLengths[index]) / (arcLengths[index + 1] - arcLengths[index])) / this.len;
        }

        private Vector3 bezierPoint(float t)
        {
            return ((aa * t + (bb)) * t + cc) * t + a;
        }

        public Vector3 point(float t)
        {
            return bezierPoint(map(t));
        }
    }
    public class BezierPathC
    {
        private LTBezier[] beziers;
        public float length;
        private float[] lengthRatio;
        public void SetPoints(Vector3[] points)
        {
            length = 0;
            int len = points.Length / 3 - 1;
            beziers = new LTBezier[len];
            lengthRatio = new float[len];
            for (int i = 0; i < len; i++)
            {
                int s = i * 3;
                beziers[i] = new LTBezier(points[s], points[s + 2], points[s + 4], points[s + 3], 0.05f);
                length += beziers[i].length;
            }
            for (int i = 0; i < len; i++)
            {
                lengthRatio[i] = beziers[i].length / length;
            }
        }
        public Vector3 Point(float ratio)
        {
            float added = 0.0f;
            for (int i = 0; i < lengthRatio.Length; i++)
            {
                added += lengthRatio[i];
                if (added >= ratio)
                    return beziers[i].point((ratio - (added - lengthRatio[i])) / lengthRatio[i]);
            }
            return beziers[lengthRatio.Length - 1].point(1.0f);
        }
        public Vector3 Place2d(float ratio, ref Vector3 angle)
        {
            var pos = Point(ratio);
            ratio += 0.001f;
            if (ratio <= 1.0f)
            {
                Vector3 v3Dir = Point(ratio) - pos;
                angle.x = 0;
                angle.y = 0;
                angle.z = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
            }
            return pos;
        }
    }
    public class LinePath
    {
        private float[] lengthRatio;
        public float length;
        Vector3[] buff;
        public void SetPoints(Vector3[] points)
        {
            length = 0;
            buff = points;
            int len = buff.Length - 1;
            lengthRatio = new float[len];
            for (int i = 0; i < len; i++)
            {
                var dir = buff[i + 1] - buff[i];
                lengthRatio[i] = dir.magnitude;
                length += lengthRatio[i];
            }
            for (int i = 0; i < len; i++)
            {
                lengthRatio[i] = lengthRatio[i] / length;
            }
        }
        public void SetBezierPoints(Vector3[] points)
        {
            int c = points.Length / 3;
            Vector3[] tmp = new Vector3[c];
            for (int i = 0; i < c; i++)
                tmp[i] = points[i * 3];
            SetPoints(tmp);
        }
        public Vector3 Point(float ratio)
        {
            float added = 0.0f;
            for (int i = 0; i < lengthRatio.Length; i++)
            {
                added += lengthRatio[i];
                if (added >= ratio)
                {
                    var r = (ratio - (added - lengthRatio[i])) / lengthRatio[i];
                    return buff[i] + (buff[i + 1] - buff[i]) * r;
                }
            }
            return buff[lengthRatio.Length];
        }
        public Vector3 Place2d(float ratio, ref Vector3 angle)
        {
            var pos = Point(ratio);
            ratio += 0.001f;
            if (ratio <= 1.0f)
            {
                Vector3 v3Dir = Point(ratio) - pos;
                angle.x = 0;
                angle.y = 0;
                angle.z = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
            }
            return pos;
        }
    }
}
