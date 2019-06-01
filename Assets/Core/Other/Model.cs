using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace huqiang
{
    public class Model
    {
        public struct ConeData
        {
            public Vector3[] Vertex;
            public int[] Tri;
        }
        /// <summary>
        /// 创建一个圆锥，返回顶点和三角形
        /// </summary>
        /// <param name="r">半径</param>
        /// <param name="h">高度</param>
        /// <param name="arc">三角形弧度，越小精度越高，范围0-360取整</param>
        /// <returns>顶点，三角形</returns>
        public static ConeData CreateCone(float r, float h, float arc)
        {
            int a = (int)arc;
            int c = 360 / a;
            int vc = c + 2;
            Vector3[] vertex = new Vector3[vc];
            int[] tri = new int[c * 6];
            int o = c;
            int s = 0;
            int i = 0;
            for (; i < c; i++)
            {
                vertex[i].x = -MathH.Sin(s) * r; 
                vertex[i].z = MathH.Cos(s) * r;
                tri[i * 3] = i + 1;
                tri[i * 3 + 1] = c;
                tri[i * 3 + 2] = i;
                tri[o * 3] = i;
                tri[o * 3 + 1] = c + 1;
                tri[o * 3 + 2] = i + 1;
                o++;
                s += a;
            }
            i--;
            o--;
            vertex[c + 1].y = h;
            tri[i * 3] = 0;
            tri[o * 3 + 2] = 0;
            ConeData cd = new ConeData();
            cd.Vertex = vertex;
            cd.Tri = tri;
            return cd;
        }
        public static Vector3 GetCenter(Mesh mesh)
        {
            var vert = mesh.vertices;
            if (vert == null || vert.Length <= 0)
                return Vector3.zero;
            float xi = vert[0].x;
            float xx = vert[0].x;
            float yi = vert[0].y;
            float yx = vert[0].y;
            float zi = vert[0].z;
            float zx = vert[0].z;
            for (int i = 1; i < vert.Length; i++)
            {
                if (vert[i].x < xi)
                    xi = vert[i].x;
                else if (vert[i].x > xx)
                    xx = vert[i].x;
                if (vert[i].y < yi)
                    yi = vert[i].y;
                else if (vert[i].y > yx)
                    yx = vert[i].y;
                if (vert[i].z < zi)
                    zi = vert[i].z;
                else if (vert[i].z > zx)
                    zx = vert[i].z;
            }
            return new Vector3((xi + xx) * 0.5f, (yi + yx) * 0.5f, (zi + zx) * 0.5f);
        }
        static int[] Cubetri = new int[] {
                0,1,2,2,3,0,
                1,5,2,2,5,6,
                5,4,6,6,4,7,
                0,3,4,3,7,4,
                1,0,4,4,5,1,
                3,2,7,7,2,6};
        public static ConeData CreateCube(Vector3 size)
        {
            float rx = size.x * 0.5f;
            float ry = size.y * 0.5f;
            float rz = size.z * 0.5f;

            var vert = new Vector3[8];
            vert[0] = new Vector3(-rx, -ry, -rz);
            vert[1] = new Vector3(-rx, ry, -rz);
            vert[2] = new Vector3(rx, ry, -rz);
            vert[3] = new Vector3(rx, -ry, -rz);

            vert[4] = new Vector3(-rx, -ry, rz);
            vert[5] = new Vector3(-rx, ry, rz);
            vert[6] = new Vector3(rx, ry, rz);
            vert[7] = new Vector3(rx, -ry, rz);

            ConeData cd = new ConeData();
            cd.Vertex = vert;
            cd.Tri = Cubetri;
            return cd;
        }
    }
}
