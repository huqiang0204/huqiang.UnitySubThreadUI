using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Data
{
    public struct Box
    {
        public Vector3 center;
        public Vector3 size;
        public static bool Contains(ref Box abox, ref Box bbox)
        {
            float x = bbox.size.x * 0.5f;
            float right = bbox.center.x + x;
            float mx = abox.size.x * 0.5f;
            if (right > abox.center.x + mx)
                return false;
            float left = bbox.center.x - x;
            if (left < abox.center.x - mx)
                return false;

            float y = bbox.size.y * 0.5f;
            float up = bbox.center.y + y;
            float my = abox.size.y * 0.5f;
            if (up > abox.center.x + my)
                return false;
            float down = bbox.center.y - y;
            if (down < abox.center.y - my)
                return false;

            float z = bbox.size.z * 0.5f;
            float behind = bbox.center.z + z;
            float mz = abox.size.z * 0.5f;
            if (behind > abox.center.z + mz)
                return false;
            float front = bbox.center.z - z;
            if (front < abox.center.z - mz)
                return false;

            return true;
        }
        public static Box GetCenter(Vector3[] vert)
        {
            if (vert == null)
                return new Box();
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
            Box box = new Box();
            box.size.x = xx - xi;
            box.size.y = yx - yi;
            box.size.z = zx - zi;
            box.center.x = (xi + xx) * 0.5f;
            box.center.y = (yi + yx) * 0.5f;
            box.center.z = (zi + zx) * 0.5f;
            return box;
        }
        static List<Vector3> vectors = new List<Vector3>();
        /// <summary>
        /// 推荐使用GetMeshCenter
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Box MeshGetCenter(MeshData mesh)
        {
            vectors.Clear();
            return GetMeshGetCenter(mesh);
        }

        static Box GetMeshGetCenter(MeshData mesh)
        {
            if (mesh.vertex != null)
                vectors.AddRange(mesh.vertex);

            if (mesh.child == null)
                return GetCenter(vectors.ToArray());
            for (int i = 0; i < mesh.child.Length; i++)
            {
                GetMeshGetCenter(mesh.child[i]);
            }
            return GetCenter(vectors.ToArray());
        }

        public static void ReLocation(Vector3[] vert, ref Vector3 location)
        {
            var box = GetCenter(vert);
            var v = location - box.center;
            for (int i = 0; i < vert.Length; i++)
                vert[i] += v;
            location = box.center;
        }

        [ThreadStatic]
        static float txi;
        [ThreadStatic]
        static float tyi;
        [ThreadStatic]
        static float tzi;
        [ThreadStatic]
        static float txx;
        [ThreadStatic]
        static float tyx;
        [ThreadStatic]
        static float tzx;
        /// <summary>
        /// 推荐使用
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Box GetMeshCenter(MeshData mesh)
        {
            txi = 0;
            txx = 0;
            tyi = 0;
            tyx = 0;
            tzi = 0;
            tzx = 0;
            GetMeshCenterS(mesh);
            Box box = new Box();
            box.size.x = txx - txi;
            box.size.y = tyx - tyi;
            box.size.z = tzx - tzi;
            box.center.x = (txi + txx) * 0.5f;
            box.center.y = (tyi + tyx) * 0.5f;
            box.center.z = (tzi + tzx) * 0.5f;
            return box;
        }
        static void GetMeshCenterS(MeshData mesh)
        {
            var vert = mesh.vertex;
            if (vert != null)
                for (int i = 0; i < vert.Length; i++)
                {
                    if (vert[i].x < txi)
                        txi = vert[i].x;
                    else if (vert[i].x > txx)
                        txx = vert[i].x;
                    if (vert[i].y < tyi)
                        tyi = vert[i].y;
                    else if (vert[i].y > tyx)
                        tyx = vert[i].y;
                    if (vert[i].z < tzi)
                        tzi = vert[i].z;
                    else if (vert[i].z > tzx)
                        tzx = vert[i].z;
                }
            var child = mesh.child;
            if (child != null)
                for (int i = 0; i < child.Length; i++)
                {
                    GetMeshCenterS(child[i]);
                }
        }
    }
}
