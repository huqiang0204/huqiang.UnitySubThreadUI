using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang
{
    public  class MathH
    {
        #region angle table  用于二维旋转，-sin(angle) cos(angle)
        /// <summary>
        /// 初始化数据
        /// </summary>
        static MathH()
        {
            anglebuff = huqiang.Resources.Properties.Resources.sin9000;
            unsafe
            {
                fixed (byte* bp = &anglebuff[0])
                    ap = (float*)bp;
            }
        }
        static byte[] anglebuff;
        unsafe static float* ap;
        /// <summary>
        /// 范围为0-360， 精度为0.01
        /// </summary>
        /// <param name="ax"></param>
        /// <returns></returns>
        public unsafe static float Sin(float ax)
        {
            if (ax >= 360)
                ax -= 360;
            if (ax < 0)
                ax += 360;
            if (ax > 270)
            {
                ax = 360 - ax;
                goto label1;
            }
            else if (ax > 180)
            {
                ax -= 180;
                goto label1;
            }
            else if (ax > 90)
            {
                ax = 180 - ax;
            }
            int id = (int)(ax * 100);
            return *(ap + id);
            label1:;
            id = (int)(ax * 100);
            return -*(ap + id);
        }
        /// <summary>
        /// 范围为0-360， 精度为0.01
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float Cos(float angle)
        {
            return Sin(angle + 90);
        }
        /// <summary>
        /// 范围为0-360， 精度为0.01
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float Tan(float angle)
        {
            var a = Sin(angle);
            var b = Cos(angle);
            if (b == 0)
                return 0;
            return a / b;
        }
        public static Vector2 Tan2(float angle)
        {
            var a = Sin(angle);
            var b = Cos(angle);
            return new Vector2(a, b);
        }
        #endregion

        #region function
        /// <summary>
        /// 反正切快速算法， 返回范围为0-360， 精度为0.01
        /// </summary>
        /// <param name="dx">x</param>
        /// <param name="dy">y</param>
        /// <returns>角度</returns>
        public static float atan(float dx, float dy)
        {
            if (dx == 0)
            {
                if (dy < 0)
                    return 180;
                return 0;
            }
            else if (dy == 0)
            {
                if (dx > 0)
                    return 90;
                if (dx == 0)
                    return 0;
                return 270;
            }
            //ax<ay
            float ax = dx < 0 ? -dx : dx, ay = dy < 0 ? -dy : dy;
            float a;
            if (ax < ay) a = ax / ay; else a = ay / ax;
            float s = a * a;
            float r = ((-0.0464964749f * s + 0.15931422f) * s - 0.327622764f) * s * a + a;
            if (ay > ax) r = 1.57079637f - r;
            r *= 5729.5795f;
            if (dx > 0)
            {
                if (dy < 0)
                    r += 9000;
                else r = 9000 - r;
            }
            else
            {
                if (dy < 0)
                    r = 27000 - r;
                else r += 27000;
            }
            r = (int)r;
            r *= 0.01f;
            return r;
        }
        #endregion

        /// <summary>
        /// 求三个点夹角的中间点
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Vector3 AngleCenter(Vector3 A, Vector3 B, Vector3 C)
        {
            float ab = Vector3.Distance(A, B);
            float cb = Vector3.Distance(C, B);
            float r = ab / cb;
            Vector3 s = B + (C - B) * r;
            Vector3 o = A + (s - A) * 0.5f;
            return o;
        }
        /// <summary>
        /// 抛物线解析式，返回一般表达式 y=a*x*x+b*x+c的 a,b,c的值
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Vector3 Parabola(Vector2 A, Vector2 B, Vector2 C)
        {
            float a = 0, b = 0, c = 0;
            float x1 = A.x, y1 = A.y, x2 = B.x, x3 = C.x, y2 = B.y, y3 = C.y;
            float m;
            m = x1 * x1 * x2 + x2 * x2 * x3 + x1 * x3 * x3 - x3 * x3 * x2 - x2 * x2 * x1 - x1 * x1 * x3;

            if ((m + 1) == 1)
            {
            }
            else
            {
                a = (y1 * x2 + y2 * x3 + y3 * x1 - y3 * x2 - y2 * x1 - y1 * x3) / m;
                b = (x1 * x1 * y2 + x2 * x2 * y3 + x3 * x3 * y1 - x3 * x3 * y2 - x2 * x2 * y1 - x1 * x1 * y3) / m;
                c = (x1 * x1 * x2 * y3 + x2 * x2 * x3 * y1 + x3 * x3 * x1 * y2 - x3 * x3 * x2 * y1 - x2 * x2 * x1 * y3 - x1 * x1 * x3 * y2) / m;
            }
            return new Vector3(a, b, c);
        }
        /// <summary>
        /// 一阶贝塞尔曲线
        /// </summary>
        /// <param name="t">比率</param>
        /// <param name="p0">起点</param>
        /// <param name="p1">中间点</param>
        /// <param name="p2">结束点</param>
        /// <returns></returns>
        public static Vector3 BezierPoint(float t, ref Vector3 p0, ref Vector3 p1, ref Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;
            return p;
        }
        /// <summary>
        /// 二阶贝塞尔曲线
        /// </summary>
        /// <param name="t">比率</param>
        /// <param name="p0">起点</param>
        /// <param name="p1">中间点1</param>
        /// <param name="p2">中间点2</param>
        /// <param name="p3">结束点</param>
        /// <returns></returns>
        public static Vector3 BezierPoint(float t, ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }
        /// <summary>
        /// 欧拉角转四元数
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public static Vector4 EulerToQuaternion(Vector3 euler)
        {
            float x = euler.x * 0.5f;
            float y = euler.y * 0.5f;
            float z = euler.z * 0.5f;
            float sx = Mathf.Sin(Mathf.PI * x / 180.0f);
            float sy = Mathf.Sin(Mathf.PI * x / 180.0f);
            float sz = Mathf.Sin(Mathf.PI * x / 180.0f);
            float cx = Mathf.Cos(Mathf.PI * x / 180.0f);
            float cy = Mathf.Cos(Mathf.PI * x / 180.0f);
            float cz = Mathf.Cos(Mathf.PI * x / 180.0f);
            Vector4 q = Vector4.zero;
            q.w = sx * sy * sz + cx * cy * cz;
            q.x = sx * cy * cz + cx * sy * sz;
            q.y = cx * sy * cz - sx * cy * sz;
            q.z = cx * cy * sz - sx * sy * cz;
            return q;
        }
        /// <summary>
        /// 四元数相乘，代码来源xenko
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector4 MultiplyQuaternion(Vector4 left, Vector4 right)
        {
            float lx = left.x;
            float ly = left.y;
            float lz = left.z;
            float lw = left.w;
            float rx = right.x;
            float ry = right.y;
            float rz = right.z;
            float rw = right.w;
            var result = Vector4.zero; ;
            result.x = (rx * lw + lx * rw + ry * lz) - (rz * ly);
            result.y = (ry * lw + ly * rw + rz * lx) - (rx * lz);
            result.z = (rz * lw + lz * rw + rx * ly) - (ry * lx);
            result.w = (rw * lw) - (rx * lx + ry * ly + rz * lz);
            return result;
        }
        /// <summary>
        /// 旋转顶点，代码来源xenko
        /// </summary>
        /// <param name="q"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 QuaternionMultiplyVector(Vector4 q,Vector3 v)
        {
            var t = Vector4.zero;
            t.x = v.x;
            t.y = v.y;
            t.z = v.z;
            var cc = Vector4.zero;
            cc.x = -q.x;
            cc.y = -q.y;
            cc.z = -q.z;
            cc.w = q.w;
            return MultiplyQuaternion(MultiplyQuaternion(cc, t),q);
        }
        /// <summary>
        /// 旋转网格所有顶点
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="quat"></param>
        public static void RotationVertex(Vector3[] vertex, Vector4 quat)
        {
            var cc = Vector4.zero;
            cc.x = -quat.x;
            cc.y = -quat.y;
            cc.z = -quat.z;
            cc.w = quat.w;
            for (int i=0;i<vertex.Length;i++)
                vertex[i] = MultiplyQuaternion(MultiplyQuaternion(cc,vertex[i]),quat);
        }
        /// <summary>
        /// 旋转网格所有顶点
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="angle"></param>
        public static void RotationVertex(Vector3[] vertex,Vector3 angle)
        {
            RotationVertex(vertex,EulerToQuaternion(angle));
        }
        /// <summary>
        /// 计算三角形法线
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector3 GetTriangleNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var ab = Vector3.zero;
            ab.x = a.x - b.x;
            ab.y = a.y - b.y;
            ab.z = a.z - b.z;
            var bc = Vector3.zero;
            bc.x = b.x - c.x;
            bc.y = b.y - c.y;
            bc.z = b.z - c.z;
            //然后计算法线，即另一个向量。求该对象的法向量（norm）。下面的代码用于计算向量ab和bc的外积：叉乘
            var nor = Vector3.zero;
            nor.x = (ab.y * bc.z) - (ab.z * bc.y);
            nor.y = -((ab.x * bc.z) - (ab.z * bc.x));
            nor.z = (ab.x * bc.y) - (ab.y * bc.x);
            return nor;
        }
        /// <summary>
        /// 解一元二次方程
        /// </summary>
        /// <param name="a">ax²</param>
        /// <param name="b">bx</param>
        /// <param name="c"></param>
        /// <returns>数组0为实数1为虚数</returns>
        public static double[] SolutionTowEquation(double a, double b, double c)
        {
            double delt = b * b - 4 * a * c;
            double[] v =new double[2];
            if (delt >= 0)
            {
                if (a > 1e-10)
                {
                    v[0] = (float)((-b + Math.Sqrt(delt)) / (2 * a));
                    v[1] = (float)((-b - Math.Sqrt(delt)) / (2 * a));

                }
                else
                {
                    v[0] = (float)((2 * c) / (-b + Math.Sqrt(delt)));
                    v[1] = (float)((2 * c) / (-b - Math.Sqrt(delt)));
                }
            }
            return v;
        }
        /// <summary>
        /// 解一元三次方式，盛金公式法
        /// </summary>
        /// <param name="_a">ax³</param>
        /// <param name="_b">bx²</param>
        /// <param name="_c">cx</param>
        /// <param name="_d"></param>
        /// <returns></returns>
        public static Complex[] ThreeEquationShengjin(double _a, double _b = 0, double _c = 0, double _d = 0)
        {
            Shengjin _Shengjin = new Shengjin(_a, _b, _c, _d);
            return _Shengjin.calc();
        }
        class Shengjin
        {
            double a;
            double b;
            double c;
            double d;

            public Shengjin(double _a, double _b = 0, double _c = 0, double _d = 0)
            {
                Debug.Assert(_a != 0, "三次系数，不能为0");
                a = _a;
                b = _b;
                c = _c;
                d = _d;
            }

            public Complex[] calc()
            {
                Complex[] x = new Complex[3];

                double A = b * b - 3 * a * c;
                double B = b * c - 9 * a * d;
                double C = c * c - 3 * b * d;

                double sj = B * B - 4 * A * C;

                if (A == 0 && B == 0)
                {
                    //盛金公式1
                    x[0] = x[1] = x[2] = new Complex(-b / (3 * a));
                }
                else if (sj > 0)
                {
                    //盛金公式2
                    double Y1 = A * b + 1.5 * a * (-B + Math.Pow(sj, 0.5));
                    double Y2 = A * b + 1.5 * a * (-B - Math.Pow(sj, 0.5));
                    //Y1立方根+Y2立方根
                    double t1 = Y1 > 0 ? Math.Pow(Y1, 1.0 / 3) : -Math.Pow(-Y1, 1.0 / 3);
                    double t2 = Y2 > 0 ? Math.Pow(Y2, 1.0 / 3) : -Math.Pow(-Y2, 1.0 / 3);
                    double Y12 = t1 + t2;
                    //Y1立方根-Y2立方根
                    double _Y12 = t1 - t2;

                    x[0] = new Complex((-b - Y12) / (3 * a));
                    x[1] = new Complex((-b + 0.5 * Y12) / (3 * a), Math.Pow(3, 0.5) * _Y12 / (6 * a));
                    x[2] = new Complex((-b + 0.5 * Y12) / (3 * a), -Math.Pow(3, 0.5) * _Y12 / (6 * a));
                }
                else if (sj == 0)
                {
                    //盛金公式3
                    double K = B / A;
                    x[0] = new Complex(-b / K);
                    x[1] = x[2] = new Complex(-K / 2);
                }
                else
                {
                    //盛金公式4,sj<0
                    double T = (2 * A * b - 3 * a * B) / (2 * Math.Pow(A, 1.5));
                    double o = Math.Acos(T);
                    x[0] = new Complex((-b - 2 * Math.Pow(A, 0.5) * Math.Cos(o / 3.0)) / (3 * a));
                    x[1] = new Complex((-b + Math.Pow(A, 0.5) * (Math.Cos(o / 3.0) + Math.Sqrt(3) * Math.Sin(o / 3.0))) / (3 * a));
                    x[2] = new Complex((-b + Math.Pow(A, 0.5) * (Math.Cos(o / 3.0) - Math.Sqrt(3) * Math.Sin(o / 3.0))) / (3 * a));
                }

                return x;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r">AttenuationRate</param>
        /// <param name="v">Velocity</param>
        /// <param name="t">time</param>
        /// <returns>最大速率行驶到当前时间产生的距离</returns>
        public static double PowDistance(double r, double v, int t)
        {
            //S= [a^(n+1) -a]/（a-1)
            return v * (Math.Pow(r, t + 1) - r) / (r - 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r">AttenuationRate</param>
        /// <param name="d">Distance</param>
        /// <returns>行驶到指定位置所需的最大速率</returns>
        public static double DistanceToVelocity(double r, double d)
        {
            return d * (r - 1) / (Math.Pow(r, 1000000) - r);
        }
        public static void Cross(ref Vector3 left, ref Vector3 right, ref Vector3 result)
        {
            result.x = (left.y * right.z) - (left.z * right.y);
            result.y = (left.z * right.x) - (left.x * right.z);
            result.z = (left.x * right.y) - (left.y * right.x);
        }
        [ThreadStatic]
        static Vector3 E1;
        [ThreadStatic]
        static Vector3 E2;
        [ThreadStatic]
        static Vector3 P;
        [ThreadStatic]
        static Vector3 T;
        [ThreadStatic]
        static Vector3 Q;
    }
    /// <summary>
    /// 表示一个复数
    /// </summary>
    public class Complex
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Complex() : this(0, 0) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="real">实部</param>
        public Complex(double real) : this(real, 0) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="real">实部</param>
        /// <param name="image">虚部</param>
        public Complex(double real, double image)
        {
            this.real = real;
            this.image = image;
        }

        private double real;

        /// <summary>
        /// 实部
        /// </summary>
        public double Real
        {
            get { return real; }
            set { real = value; }
        }

        private double image;

        /// <summary>
        /// 虚部
        /// </summary>
        public double Image
        {
            get { return image; }
            set { image = value; }
        }

        /// <summary>
        /// 复数的加法运算
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.real + c2.real, c1.image + c2.image);
        }

        /// <summary>
        /// 复数的减法运算
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.real - c2.real, c1.image - c2.image);
        }

        /// <summary>
        /// 复数的乘法运算
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.real * c2.real - c1.image * c2.image, c1.image * c2.real + c1.real * c2.image);
        }

        /// <summary>
        /// 复数的求模运算
        /// </summary>
        /// <returns></returns>
        public double ToModul()
        {
            return Math.Sqrt(real * real + image * image);
        }

        public override string ToString()
        {
            if (Real == 0 && Image == 0)
            {
                return string.Format("{0}", 0);
            }

            if (Real == 0 && (Image != 1 && Image != -1))
            {
                return string.Format("{0} i", Image);
            }

            if (Image == 0)
            {
                return string.Format("{0}", Real);
            }

            if (Image == 1)
            {
                return string.Format("i");
            }

            if (Image == -1)
            {
                return string.Format("- i");
            }

            if (Image < 0)
            {
                return string.Format("{0} - {1} i", Real, -Image);
            }
            return string.Format("{0} + {1} i", Real, Image);
        }

    }
}