using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class AuxiliaryLine
    {
        public static Vector3[] CreateLine(Vector3 start, Vector3 end, float lineWidth = 1)
        {
            Vector3[] tmp = new Vector3[4];
            CreateLine(ref start, ref end, lineWidth, tmp, 0);
            return tmp;
        }
        static void CreateLine(ref Vector3 start, ref Vector3 end, float lineWidth, Vector3[] vert, int index = 0)
        {

            float vx = end.x - start.x;
            float vy = end.y - start.y;
            float r = Mathf.Sqrt(lineWidth * lineWidth / (vx * vx + vy * vy));
            float nx = vx * r;
            float ny = vy * r;
          
            vert[index].x = start.x + ny;
            vert[index].y = start.y - nx;
            index++;
            vert[index].x = start.x - ny;
            vert[index].y = start.y + nx;
            index++;
            vert[index].x = end.x - ny;
            vert[index].y = end.y + nx;
            index++;
            vert[index].x = end.x + ny;
            vert[index].y = end.y - nx;
        }
        public static Vector3[] CreateLine(Vector3[] vert, int[] lines, float lineWidth = 1)
        {
            int a = lines.Length / 2;
            Vector3[] tmp = new Vector3[a * 4];
            for (int i = 0; i < a; i++)
            {
                int index = i * 2;
                CreateLine(ref vert[lines[index]], ref vert[lines[index + 1]], lineWidth, tmp, i * 4);
            }
            return tmp;
        }
        public static void GetLineVert(Vector3[] vectors, List<UIVertex> vert, List<int> tris, Color color)
        {
            int Start = vert.Count;
            for (int i = 0; i < vectors.Length; i++)
            {
                UIVertex vertex = new UIVertex();
                vertex.position = vectors[i];
                vertex.color = color;
                vert.Add(vertex);
            }
            int len = vectors.Length / 4;
            for (int i = 0; i < len; i++)
            {
                int index = i * 4 + Start;
                tris.Add(index);
                tris.Add(index + 1);
                tris.Add(index + 3);
                tris.Add(index + 3);
                tris.Add(index + 1);
                tris.Add(index + 2);
            }
        }
        public static int[] BoxLine = new int[] { 0,1,1,2,2,3,3,0,4,5,5,6,6,7,7,4,0,4,1,5,2,6,3,7};
        /// <summary>
        /// 将世界坐标的顶点投射到屏幕坐标
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="camera"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector3[] BoxToScreenPoint(Transform transform,Camera camera,Vector3 size)
        {
            var c = transform.position;
            float rx = size.x * 0.5f;
            float ry = size.y * 0.5f;
            float rz = size.z * 0.5f;
            var r = transform.rotation;

            var tmp = new Vector3[8];
            tmp[0] = c + r * new Vector3(-rx, -ry, rz);
            tmp[1] = c + r * new Vector3(rx, -ry, rz);
            tmp[2] = c + r * new Vector3(rx, -ry, -rz);
            tmp[3] = c + r * new Vector3(-rx, -ry, -rz);

            tmp[4] = c + r * new Vector3(-rx, ry, rz);
            tmp[5] = c + r * new Vector3(rx, ry, rz);
            tmp[6] = c + r * new Vector3(rx, ry, -rz);
            tmp[7] = c + r * new Vector3(-rx, ry, -rz);

            for (int i = 0; i < 8; i++)
                tmp[i] = camera.WorldToScreenPoint(tmp[i]);

            return tmp;
        }
    }
}
