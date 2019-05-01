using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    public class Physics2D
    {
        #region collision check
        /// <summary>
        /// 检查点在弧形里面
        /// </summary>
        /// <param name="ori">弧形圆心</param>
        /// <param name="r">弧形半径</param>
        /// <param name="direct">弧形方向,0-360角度</param>
        /// <param name="scope">弧形范围0-180角度</param>
        /// <param name="dot">点</param>
        public static bool DotToArc(Vector2 ori, float r, float direct, float scope, Vector2 dot)
        {
            r *= r;
            float dx = dot.x - ori.x;
            float d = dx * dx;
            float dy = dot.y - ori.y;
            d += dy * dy;
            if (d < r)//在半径内
            {
                float ta = MathH.atan(dx, dy);
                float a = direct - scope;
                if (ta == a)
                    return true;
                float b = direct + scope;
                if (ta == b)
                    return true;
                if (a < 0)
                {
                    if (ta < b)
                        return true;
                    a += 360;
                    if (ta > a)
                        return true;
                }
                else if (b > 360)
                {
                    if (ta > a)
                        return true;
                    b -= 360;
                    if (ta < b)
                        return true;
                }
                else if (ta > a & ta < b)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查点在椭圆里面
        /// </summary>
        /// <param name="ell_location"></param>
        /// <param name="dot"></param>
        /// <param name="xlen"></param>
        /// <param name="ylen"></param>
        /// <returns></returns>
        public static bool DotToEllipse(Vector2 ell_location, Vector2 dot, float xlen, float ylen)
        {
            float x = ell_location.x - dot.x;
            x *= x;
            float y = ell_location.y - dot.y;
            y *= y;
            xlen *= xlen;
            x /= xlen;
            ylen *= ylen;
            y /= ylen;
            return x + y < 1;
        }
        /// <summary>
        ///  检测一个点是否在多边形里面
        /// </summary>
        /// <param name="A">多边形,按顺序连接</param>
        /// <param name="B">点</param>
        /// <returns>在里面返回true，反之返回false</returns>
        public static bool DotToPolygon(Vector2[] A, Vector2 B)
        {
            int count = 0;
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 p1 = A[i];
                Vector2 p2 = i == A.Length - 1 ? A[0] : A[i + 1];
                if (B.y >= p1.y & B.y <= p2.y | B.y >= p2.y & B.y <= p1.y)
                {
                    float t = (B.y - p1.y) / (p2.y - p1.y);
                    float xt = p1.x + t * (p2.x - p1.x);
                    if (B.x == xt)
                        return true;
                    if (B.x < xt)
                        count++;
                }
            }
            return count % 2 > 0 ? true : false;
        }
        /// <summary>
        ///  检测一个点是否在多边形里面
        /// </summary>
        /// <param name="A">多边形,按顺序连接</param>
        /// <param name="B">点</param>
        /// <returns>在里面返回true，反之返回false</returns>
        public static bool DotToPolygon(Vector3[] A, Vector2 B)
        {
            int count = 0;
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 p1 = A[i];
                Vector2 p2 = i == A.Length - 1 ? A[0] : A[i + 1];
                if (B.y >= p1.y & B.y <= p2.y | B.y >= p2.y & B.y <= p1.y)
                {
                    float t = (B.y - p1.y) / (p2.y - p1.y);
                    float xt = p1.x + t * (p2.x - p1.x);
                    if (B.x == xt)
                        return true;
                    if (B.x < xt)
                        count++;
                }
            }
            return count % 2 > 0 ? true : false;
        }
        public static Vector2[] GetPointsOffset(Vector3 location, Vector2[] offsest)
        {
            Vector2[] temp = new Vector2[offsest.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].x = location.x + offsest[i].x;
                temp[i].y = location.y + offsest[i].y;
            }
            return temp;
        }
        public static bool DotToPolygon(Vector2 origin, Vector2[] A, Vector2 B)//offset
        {
            int count = 0;
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 p1 = A[i];
                p1.x += origin.x;
                p1.y += origin.y;
                Vector2 p2 = i == A.Length - 1 ? A[0] : A[i + 1];
                p2.x += origin.x;
                p2.y += origin.y;
                if (B.y >= p1.y & B.y <= p2.y | B.y >= p2.y & B.y <= p1.y)
                {
                    float t = (B.y - p1.y) / (p2.y - p1.y);
                    float xt = p1.x + t * (p2.x - p1.x);
                    if (B.x == xt)
                        return true;
                    if (B.x < xt)
                        count++;
                }
            }
            return count % 2 > 0 ? true : false;
        }
        public static bool CircleToCircle(Vector2 A, Vector2 B, float radiusA, float radiusB)
        {
            return radiusA + radiusB > Mathf.Sqrt((A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y));
        }
        public static Vector2 RotatePoint2(ref Vector2 p, ref Vector2 location, float angle)//a=绝对角度 d=直径
        {
            float a = p.x + angle;
            if (a < 0)
                a += 360;
            if (a > 360)
                a -= 360;
            a *= 0.0174533f;//change angle to radin
            float d = p.y;
            Vector2 temp = new Vector2();
            temp.x = location.x - Mathf.Sin(a) * d;
            temp.y = location.y + Mathf.Cos(a) * d;
            return temp;
        }
        public static Vector3 RotateVector3(Vector2 p, ref Vector3 location, float angle)//a=绝对角度 d=直径
        {
            int a = (int)(p.x + angle);
            if (a < 0)
                a += 360;
            if (a > 360)
                a -= 360;
            float d = p.y;
            Vector3 temp = location;
            temp.x = location.x - MathH.Sin(a) * d;
            temp.y = location.y + MathH.Cos(a) * d;
            return temp;
        }
        public static void RotatePoint2(ref Vector2 p, ref Vector2 location, float angle, ref Vector3 o)//a=绝对角度 d=直径
        {
            int a = (int)(p.x + angle);
            if (a < 0)
                a += 360;
            if (a > 360)
                a -= 360;
            float d = p.y;
            o.x = location.x - MathH.Sin(a) * d;
            o.y = location.y + MathH.Cos(a) * d;
        }
        public static Vector2[] RotatePoint2(ref Vector2[] P, Vector2 location, float angle)//p[].x=绝对角度 p[].y=直径
        {
            Vector2[] temp = new Vector2[P.Length];
            for (int i = 0; i < P.Length; i++)
            {
                int a = (int)(P[i].x + angle);//change angle to radin
                if (a < 0)
                    a += 360;
                if (a >= 360)
                    a -= 360;
                temp[i].x = location.x - MathH.Sin(a) * P[i].y;
                temp[i].y = location.y + MathH.Cos(a) * P[i].y;
            }
            return temp;
        }
        public static Vector4 RotatePoint(Vector4 P, Vector4 A, float rad, float r, bool isClockwise)//弧度只能表示180°所以用正反转表示
        {
            //点Temp1
            Vector4 Temp1 = new Vector4();
            Temp1.x = P.x - A.x;
            Temp1.y = P.x - A.x;
            //∠T1OX弧度
            float angT1OX = radPOX(Temp1.x, Temp1.y);
            //∠T2OX弧度（T2为T1以O为圆心旋转弧度rad）
            float angT2OX = angT1OX - (isClockwise ? 1 : -1) * rad;
            //点Temp2
            Vector4 Temp2 = new Vector4();
            Temp2.x = r * Mathf.Cos(angT2OX) + A.x;
            Temp2.y = r * Mathf.Sin(angT2OX) + A.y;
            //点Q
            return Temp2;
        }
        public static Vector4[] RotatePoints(ref Vector4[] P, Vector4 origion, float angle)//弧度只能表示180°所以用正反转表示
        {
            if (angle > 180)
                angle += -360;
            angle /= 57.29577951f;
            for (int i = 0; i < P.Length; i++)
            {
                //点Temp1
                Vector4 Temp1 = new Vector4();
                Temp1.x = P[i].x - origion.x;
                Temp1.y = P[i].x - origion.x;
                //∠T1OX弧度
                float angT1OX = radPOX(Temp1.x, Temp1.y);
                //∠T2OX弧度（T2为T1以O为圆心旋转弧度rad）
                float angT2OX = angT1OX - angle;
                //点Temp2

                P[i].x = P[i].z * Mathf.Cos(angT2OX) + origion.x;
                P[i].y = P[i].z * Mathf.Sin(angT2OX) + origion.y;
                //点Q
            }
            return P;
        }
        public static float radPOX(float x, float y)
        {
            //P在(0,0)的情况
            if (x == 0 && y == 0) return 0;

            //P在四个坐标轴上的情况：x正、x负、y正、y负
            if (y == 0 && x > 0) return 0;
            if (y == 0 && x < 0) return Mathf.PI;
            if (x == 0 && y > 0) return Mathf.PI / 2;
            if (x == 0 && y < 0) return Mathf.PI / 2 * 3;

            //点在第一、二、三、四象限时的情况
            if (x > 0 && y > 0) return Mathf.Atan(y / x);
            if (x < 0 && y > 0) return Mathf.PI - Mathf.Atan(y / -x);
            if (x < 0 && y < 0) return Mathf.PI + Mathf.Atan(-y / -x);
            if (x > 0 && y < 0) return Mathf.PI * 2 - Mathf.Atan(-y / x);

            return 0;
        }
        /// <summary>
        /// 多边形与多边形相交
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool PToP2(Vector2[] A, Vector2[] B)
        {
            //Cos A=(b²+c²-a²)/2bc
            float min1 = 0, max1 = 0, min2 = 0, max2 = 0;
            int second = 0;
            Vector2 a, b;
        label1:
            for (int i = 0; i < A.Length; i++)
            {
                int id;
                a = A[i];
                if (i == A.Length - 1)
                {
                    b = A[0];
                    id = 1;
                }
                else
                {
                    b = A[i + 1];
                    id = i + 2;
                }
                float x = a.x - b.x;
                float y = a.y - b.y;//向量
                a.x = y;
                a.y = -x;//法线点a
                b.x = -y;
                b.y = x;//法线点b
                        // float ab = (x + x) * (x + x) + (y + y) * (y + y);//b 平方
                        //x = c.x - a.x;
                        //y = c.y - a.y;
                float ac;// = x * x + y * y;//c 平方
                //x = b.x - c.x;
                //y = b.y - c.y;
                float bc;// = x * x + y * y;//a 平方
                //float d = Mathf.Sqrt(bc) + Mathf.Sqrt(ac) - Mathf.Sqrt(ab);
                float d;// = ac - bc;
                //min1 = d;
                //max1 = d;
                for (int o = 0; o < A.Length; o++)
                {
                    float x1 = A[o].x - a.x;
                    x1 *= x1;
                    float y1 = A[o].y - a.y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.x - A[o].x;
                    x2 *= x2;
                    float y2 = b.y - A[o].y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                    {
                        min1 = max1 = d;
                    }
                    else
                    {
                        if (d < min1)
                            min1 = d;
                        else if (d > max1)
                            max1 = d;
                    }
                }
                for (int o = 0; o < B.Length; o++)
                {
                    float x1 = B[o].x - a.x;
                    x1 *= x1;
                    float y1 = B[o].y - a.y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.x - B[o].x;
                    x2 *= x2;
                    float y2 = b.y - B[o].y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                        max2 = min2 = d;
                    else
                    {
                        if (d < min2)
                            min2 = d;
                        else if (d > max2)
                            max2 = d;
                    }
                }
                if (min2 > max1 | min1 > max2)
                    return false;
            }
            second++;
            if (second < 2)
            {
                Vector2[] temp = A;
                A = B;
                B = temp;
                goto label1;
            }
            return true;
        }
        /// <summary>
        /// 多边形与多边形相交
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool PToP2A(Vector2[] A, Vector2[] B, ref Vector3 location)
        {
            //formule
            //A.x+x1*V1.x=B.x+x2*V2.x
            //x2*V2.x=A.x+x1*V1.x-B.x
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //A.y+x1*V1.y=B.y+x2*V2.y
            //A.y+x1*V1.y=B.y+(A.x+x1*V1.x-B.x)/V2.x*V2.y
            //x1*V1.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y+x1*V1.x/V2.x*V2.y
            //x1*V1.y-x1*V1.x/V2.x*V2.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1*(V1.y-V1.x/V2.x*V2.y)=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1=(B.y-A.y+(A.x-B.x)/V2.x*V2.y)/(V1.y-V1.x/V2.x*V2.y)
            //改除为乘防止除0溢出
            //if((V1.y*V2.x-V1.x*V2.y)==0) 平行
            //x1=((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)/(V1.y*V2.x-V1.x*V2.y)
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //x2=(A.y+x1*V1.y-B.y)/V2.y
            //if(x1>=0&x1<=1 &x2>=0 &x2<=1) cross =true
            //location.x=A.x+x1*V1.x
            //location.y=A.x+x1*V1.y
            Vector2[] VB = new Vector2[B.Length];
            for (int i = 0; i < B.Length; i++)
            {
                if (i == B.Length - 1)
                {
                    VB[i].x = B[0].x - B[i].x;
                    VB[i].y = B[0].y - B[i].y;
                }
                else
                {
                    VB[i].x = B[i + 1].x - B[i].x;
                    VB[i].y = B[i + 1].y - B[i].y;
                }
            }
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 VA = new Vector2();
                if (i == A.Length - 1)
                {
                    VA.x = A[0].x - A[i].x;
                    VA.y = A[0].y - A[i].y;
                }
                else
                {
                    VA.x = A[i + 1].x - A[i].x;
                    VA.y = A[i + 1].y - A[i].y;
                }
                for (int c = 0; c < B.Length; c++)
                {
                    //(V1.y*V2.x-V1.x*V2.y)
                    float y = VA.y * VB[c].x - VA.x * VB[c].y;
                    if (y == 0)
                        break;
                    //((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)
                    float x = (B[c].y - A[i].y) * VB[c].x + (A[i].x - B[c].x) * VB[c].y;
                    float d = x / y;
                    if (d >= 0 & d <= 1)
                    {
                        if (VB[c].x == 0)
                        {
                            //x2=(A.y+x1*V1.y-B.y)/V2.y
                            y = (A[i].y - B[c].y + d * VA.y) / VB[c].y;
                        }
                        else
                        {
                            //x2=(A.x+x1*V1.x-B.x)/V2.x
                            y = (A[i].x - B[c].x + d * VA.x) / VB[c].x;
                        }
                        //location.x=A.x+x1*V1.x
                        //location.y=A.x+x1*V1.y
                        if (y >= 0 & y <= 1)
                        {
                            location.x = A[i].x + d * VA.x;
                            location.y = A[i].y + d * VA.y;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 多边形与多边形相交
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="la"></param>
        /// <param name="lb"></param>
        /// <returns></returns>
        public static bool PToP2A(Vector2[] A, Vector2[] B, ref Vector3 la, ref Vector3 lb)
        {
            //formule
            //A.x+x1*V1.x=B.x+x2*V2.x
            //x2*V2.x=A.x+x1*V1.x-B.x
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //A.y+x1*V1.y=B.y+x2*V2.y
            //A.y+x1*V1.y=B.y+(A.x+x1*V1.x-B.x)/V2.x*V2.y
            //x1*V1.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y+x1*V1.x/V2.x*V2.y
            //x1*V1.y-x1*V1.x/V2.x*V2.y=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1*(V1.y-V1.x/V2.x*V2.y)=B.y+(A.x-B.x)/V2.x*V2.y-A.y
            //x1=(B.y-A.y+(A.x-B.x)/V2.x*V2.y)/(V1.y-V1.x/V2.x*V2.y)
            //改除为乘防止除0溢出
            //if((V1.y*V2.x-V1.x*V2.y)==0) 平行
            //x1=((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)/(V1.y*V2.x-V1.x*V2.y)
            //x2=(A.x+x1*V1.x-B.x)/V2.x
            //x2=(A.y+x1*V1.y-B.y)/V2.y
            //if(x1>=0&x1<=1 &x2>=0 &x2<=1) cross =true
            //location.x=A.x+x1*V1.x
            //location.y=A.x+x1*V1.y
            bool re = false;
            Vector2[] VB = new Vector2[B.Length];
            for (int i = 0; i < B.Length; i++)
            {
                if (i == B.Length - 1)
                {
                    VB[i].x = B[0].x - B[i].x;
                    VB[i].y = B[0].y - B[i].y;
                }
                else
                {
                    VB[i].x = B[i + 1].x - B[i].x;
                    VB[i].y = B[i + 1].y - B[i].y;
                }
            }
            for (int i = 0; i < A.Length; i++)
            {
                Vector2 VA = new Vector2();
                if (i == A.Length - 1)
                {
                    VA.x = A[0].x - A[i].x;
                    VA.y = A[0].y - A[i].y;
                }
                else
                {
                    VA.x = A[i + 1].x - A[i].x;
                    VA.y = A[i + 1].y - A[i].y;
                }
                for (int c = 0; c < B.Length; c++)
                {
                    //(V1.y*V2.x-V1.x*V2.y)
                    float y = VA.y * VB[c].x - VA.x * VB[c].y;
                    if (y == 0)
                        break;
                    //((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)
                    float x = (B[c].y - A[i].y) * VB[c].x + (A[i].x - B[c].x) * VB[c].y;
                    float d = x / y;
                    if (d >= 0 & d <= 1)
                    {
                        if (VB[c].x == 0)
                        {
                            //x2=(A.y+x1*V1.y-B.y)/V2.y
                            y = (A[i].y - B[c].y + d * VA.y) / VB[c].y;
                        }
                        else
                        {
                            //x2=(A.x+x1*V1.x-B.x)/V2.x
                            y = (A[i].x - B[c].x + d * VA.x) / VB[c].x;
                        }
                        //location.x=A.x+x1*V1.x
                        //location.y=A.x+x1*V1.y
                        if (y >= 0 & y <= 1)
                        {
                            if (re)
                            {
                                lb.x = A[i].x + d * VA.x;
                                lb.y = A[i].y + d * VA.y;
                                return true;
                            }
                            else
                            {
                                la.x = A[i].x + d * VA.x;
                                la.y = A[i].y + d * VA.y;
                                re = true;
                            }
                        }
                    }
                }
            }
            return re;
        }
        /// <summary>
        /// 多边形与多边形相交
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool PToP3(Vector3[] A, Vector3[] B)
        {
            //Cos A=(b²+c²-a²)/2bc
            float min1 = 0, max1 = 0, min2 = 0, max2 = 0;
            int second = 0;
            Vector3 a, b;
        label1:
            for (int i = 0; i < A.Length; i++)
            {
                int id;
                a = A[i];
                if (i == A.Length - 1)
                {
                    b = A[0];
                    id = 1;
                }
                else
                {
                    b = A[i + 1];
                    id = i + 2;
                }
                float x = a.x - b.x;
                float y = a.y - b.y;//向量
                a.x = y;
                a.y = -x;//法线点a
                b.x = -y;
                b.y = x;//法线点b
                        // float ab = (x + x) * (x + x) + (y + y) * (y + y);//b 平方
                        //x = c.x - a.x;
                        //y = c.y - a.y;
                float ac;// = x * x + y * y;//c 平方
                //x = b.x - c.x;
                //y = b.y - c.y;
                float bc;// = x * x + y * y;//a 平方
                //float d = Mathf.Sqrt(bc) + Mathf.Sqrt(ac) - Mathf.Sqrt(ab);
                float d;// = ac - bc;
                //min1 = d;
                //max1 = d;
                for (int o = 0; o < A.Length; o++)
                {
                    float x1 = A[o].x - a.x;
                    x1 *= x1;
                    float y1 = A[o].y - a.y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.x - A[o].x;
                    x2 *= x2;
                    float y2 = b.y - A[o].y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                    {
                        min1 = max1 = d;
                    }
                    else
                    {
                        if (d < min1)
                            min1 = d;
                        else if (d > max1)
                            max1 = d;
                    }
                }
                for (int o = 0; o < B.Length; o++)
                {
                    float x1 = B[o].x - a.x;
                    x1 *= x1;
                    float y1 = B[o].y - a.y;
                    ac = x1 + y1 * y1;//ac
                    float x2 = b.x - B[o].x;
                    x2 *= x2;
                    float y2 = b.y - B[o].y;
                    bc = x2 + y2 * y2;//bc
                    d = ac - bc;//ab+ac-bc
                    if (o == 0)
                        max2 = min2 = d;
                    else
                    {
                        if (d < min2)
                            min2 = d;
                        else if (d > max2)
                            max2 = d;
                    }
                }
                if (min2 > max1 | min1 > max2)
                    return false;
            }
            second++;
            if (second < 2)
            {
                Vector3[] temp = A;
                A = B;
                B = temp;
                goto label1;
            }
            return true;
        }
        /// <summary>
        /// 三角形和多边形相交
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool TriangleToPolygon(Vector2[] A, Vector2[] B)
        {
            Vector2[] a = new Vector2[3]
            {
            new Vector2(A[1].x - A[0].x, A[1].y - A[0].y),
            new Vector2(A[2].x - A[1].x, A[2].y - A[1].y),
            new Vector2(A[0].x - A[2].x, A[0].y - A[2].y)
            };
            int again = 0;
        label1:
            for (int i = 0; i < a.Length; i++)
            {
                float min1 = 1000, min2 = 1000, max1 = 0, max2 = 0;
                float sxy = a[i].x * a[i].x + a[i].y * a[i].y;
                for (int l = 0; l < 3; l++)
                {
                    float dxy = A[l].x * a[i].x + A[l].y * a[i].y;
                    float x = dxy / sxy * a[i].x;
                    if (x < 0)
                        x = 0 - x;
                    float y = dxy / sxy * a[i].y;
                    if (y < 0)
                        y = 0 - y;
                    x = x + y;
                    if (x > max1)
                        max1 = x;
                    if (x < min1)
                        min1 = x;
                }
                for (int l = 0; l < B.Length; l++)
                {
                    float dxy = B[l].x * a[i].x + B[l].y * a[i].y;
                    float x = dxy / sxy * a[i].x;
                    if (x < 0)
                        x = 0 - x;
                    float y = dxy / sxy * a[i].y;
                    if (y < 0)
                        y = 0 - y;
                    x = x + y;
                    if (x > max2)
                        max2 = x;
                    if (x < min2)
                        min2 = x;
                }
                if (min1 > max2 | min2 > max1)
                {
                    return false;
                }
            }
            if (again > 0)
                return true;
            a = new Vector2[B.Length];
            for (int i = 0; i < B.Length - 1; i++)
            {
                a[i].x = B[i + 1].x - B[i].x;
                a[i].y = B[i + 1].y - B[i].y;
            }
            a[a.Length - 1].x = B[0].x - B[a.Length - 1].x;
            a[a.Length - 1].y = B[0].y - B[a.Length - 1].y;
            again++;
            goto label1;
        }
        /// <summary>
        /// 圆与多边形相交
        /// </summary>
        /// <param name="C"></param>
        /// <param name="r"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        public static bool CircleToPolygon(Vector2 C, float r, Vector2[] P)
        {
            Vector2 A = new Vector2();
            Vector2 B = new Vector2();
            float z = 10, r2 = r * r, x = 0, y = 0;
            float[] d = new float[P.Length];
            int id = 0;
            for (int i = 0; i < P.Length; i++)
            {
                x = C.x - P[i].x;
                y = C.y - P[i].y;
                x = x * x + y * y;
                if (x <= r2)
                    return true;
                d[i] = x;
                if (x < z)
                {
                    z = x;
                    id = i;
                }
            }
            int p1 = id - 1;
            if (p1 < 0)
                p1 = P.Length - 1;
            float a, b, c;
            c = d[p1];
            a = d[id];
            B = P[id];
            A = P[p1];
            x = B.x - A.x;
            x *= x;
            y = B.y - A.y;
            y *= y;
            b = x + y;
            x = c - a;
            if (x < 0)
                x = -x;
            if (x <= b)
            {
                y = b + c - a;
                y = y * y / 4 / b;
                if (c - y <= r2)
                    return true;
            }
            else
            {
                p1 = id + 1;
                if (p1 == P.Length)
                    p1 = 0;
                c = d[p1];
                A = P[p1];
                x = B.x - A.x;
                x *= x;
                y = B.y - A.y;
                y *= y;
                b = x + y;
                x = c - a;
                if (x < 0)
                    x = -x;
                if (x <= b)
                {
                    y = b + c - a;
                    y = y * y / 4 / b;
                    if (c - y <= r2)
                        return true;
                }
            }
            return DotToPolygon(P, new Vector2(C.x, C.y));//circle inside polygon
        }
        /// <summary>
        /// 圆与线相交
        /// </summary>
        /// <param name="C"></param>
        /// <param name="r"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool CircleToLine(Vector2 C, float r, Vector2 A, Vector2 B)
        {
            float vx1 = C.x - A.x;
            float vy1 = C.y - A.y;
            float vx2 = B.x - A.x;
            float vy2 = B.y - A.y;
            float len = Mathf.Sqrt(vx2 * vx2 + vy2 * vy2);
            vx2 /= len;
            vy2 /= len;
            float u = vx1 * vx2 + vy1 * vy2;
            float x0 = 0f;
            float y0 = 0f;
            if (u <= 0)
            {
                x0 = A.x;
                y0 = A.y;
            }
            else if (u >= len)
            {
                x0 = B.x;
                y0 = B.y;
            }
            else
            {
                x0 = A.x + vx2 * u;
                y0 = A.y + vy2 * u;
            }
            return (C.x - x0) * (C.x - x0) + (C.y - y0) * (C.y - y0) <= r * r;
        }
        /// <summary>
        /// 圆与线相交
        /// </summary>
        /// <param name="C"></param>
        /// <param name="r"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool CircleToLineA(Vector2 C, float r, Vector2 A, Vector2 B)
        {
            r *= r;
            float x = C.x - B.x;
            x *= x;
            float y = C.y - B.y;
            y *= y;
            float a = x + y;
            if (a <= r)
                return true;
            x = A.x - C.x;
            x *= x;
            y = A.y - C.y;
            y *= y;
            float c = x + y;
            if (c <= r)
                return true;
            x = B.x - A.x;
            x *= x;
            y = B.y - A.y;
            y *= y;
            float b = x + y;
            x = c - a;
            if (x < 0)
                x = -x;
            if (x > b)
                return false;
            y = b + c - a;
            y *= y / 4 / b;
            if (c - y <= r)
                return true;
            return false;
        }
        /// <summary>
        /// 检测一个点是否在线段上
        /// </summary>
        /// <param name="dot">点</param>
        /// <param name="a">线段起点</param>
        /// <param name="b">线段终点</param>
        /// <returns></returns>
        public static bool DotToLine(ref Vector2 dot, ref Vector2 a,ref Vector2 b)
        {
            float dx = dot.x - a.x;
            float dy = dot.y - a.y;
            float bx = b.x - a.x;
            float by = b.y - a.y;
            if (dx / dy != bx / by)
                return false;
            if (dx == bx)
                return true;
            float r = dx / bx;
            if (r > 0 & r < 1)
                return true;
            return false;
        }
        /// <summary>
        /// 线与垂直线相交
        /// </summary>
        ///  <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="va">垂直线点a</param>
        /// <param name="vb">垂直线点b</param>
        /// <returns></returns>
        public static bool LineToVerticalLine(ref Vector2 c, ref Vector2 d, ref Vector2 va, ref Vector2 vb, ref Vector2 o)//注意垂直线自底向上
        {
            Vector2 VB = new Vector2();
            VB.x = d.x - c.x;
            if (VB.x == 0)//垂直所以平行
                return false;
            VB.y = d.y - c.y;
            float x = (va.x - c.x) / VB.x;
            if (x >= 0 & x <= 1)
            {
                float y = VB.y * x + c.y;
                if (y >= va.y & y <= vb.y)
                {
                    o.x = va.x;
                    o.y = y;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 求两条线段是否相交点,并得出相交点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool LineToLine(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 d, ref Vector2 o)//相交线相交点
        {
            float ax = b.x - a.x;
            float ay = b.y - a.y;
            float cx = d.x - c.x;
            float cy = d.y - c.y;
            //(V1.y*V2.x-V1.x*V2.y)
            float y = ay * cx - ax * cy;
            if (y == 0)
                return false;
            //((B.y-A.y)*V2.x+(A.x-B.x)*V2.y)
            float x = (c.y - a.y) * cx + (a.x - c.x) * cy;
            float r = x / y;
            if (r >= 0 & r <= 1)
            {
                if (cx == 0)
                {
                    //x2=(A.y+x1*V1.y-B.y)/V2.y
                    y = (a.y - c.y + r * ay) / cy;
                }
                else
                {
                    //x2=(A.x+x1*V1.x-B.x)/V2.x
                    y = (a.x - c.x + r * ax) / cx;
                }
                //location.x=A.x+x1*V1.x
                //location.y=A.x+x1*V1.y
                if (y >= 0 & y <= 1)
                {
                    o.x = a.x + r * ax;
                    o.y = a.y + r * ay;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 求两条线段的交点,比上面快一丁点,但无法确定是否相交
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="o"></param>
        public static void LineToLineA(ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, ref Vector2 p4, ref Vector2 o)
        {
            float r0 = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
            if (r0 < 0)
                r0 = -r0;
            float r1 = (p2.x - p1.x) * (p4.y - p2.y) - (p2.y - p1.y) * (p4.x - p2.x);
            if (r1 < 0)
                r1 = -r1;
            float k = r0 / r1;
            o.x = (p3.x + k * p4.x) / (1 + k);
            o.y = (p3.y + k * p4.y) / (1 + k);
        }
        #endregion
    }
}
