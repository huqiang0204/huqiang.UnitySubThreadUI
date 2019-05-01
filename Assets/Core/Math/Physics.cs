using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang
{
    public class Physics
    {
        // Determine whether a ray intersect with a triangle
        // Parameters
        // orig: origin of the ray
        // dir: direction of the ray
        // v0, v1, v2: vertices of triangle
        // t(out): weight of the intersection for the ray
        // u(out), v(out): barycentric coordinate of intersection
        public static bool IntersectTriangle(ref Vector3 orig,ref Vector3 dir, ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 point)
        {
            // E1
            Vector3 E1 = v1 - v0;
            // E2
            Vector3 E2 = v2 - v0;
            // P
            Vector3 P = Vector3.Cross(dir, E2);
            // determinant
            float det = Vector3.Dot(E1, P);
            // keep det > 0, modify T accordingly
            Vector3 T;
            if (det > 0)
            {
                T = orig - v0;
            }
            else
            {
                T = v0 - orig;
                det = -det;
            }
            // If determinant is near zero, ray lies in plane of triangle
            if (det < 0.0001f)
                return false;
            // Calculate u and make sure u <= 1
            float u = Vector3.Dot(T, P);
            if (u < 0.0f || u > det)
                return false;
            // Q
            Vector3 Q = Vector3.Cross(T, E1);
            // Calculate v and make sure u + v <= 1
            float v = Vector3.Dot(dir, Q);
            if (v < 0.0f || u + v > det)
                return false;
            // Calculate t, scale parameters, ray intersects triangle
            float t = Vector3.Dot(E2, Q);
            float fInvDet = 1.0f / det;
            t *= fInvDet;
            u *= fInvDet;
            v *= fInvDet;
            point = (1 - u - v) * v0 + u * v1 + v * v2;
            return true;
        }
    }
}
