using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Shot2D
{
    public class Collider2D
    {
        protected enum ColliderType
        {
            Dot, Circle, Line, Polygon, Ellipse,Arc
        }
        int _layer = 0;
        public int layer { get { return _layer; }
            set {
                ColliderManager.RemoveCollider(this);
                _layer = value;
                ColliderManager.RemoveCollider(this);
            } }
        public Collider2D()
        {
            ColliderManager.RegCollider(this);
        }
        public Vector2 position;
        public Vector2[] vertex;
        protected ColliderType type;
        protected static bool CheckDotCollider(Collider2D collision, Collider2D collision2)
        {
            switch (collision2.type)
            {
                case ColliderType.Dot:
                    if (collision.position == collision2.position)
                        return true;
                    break;
                case ColliderType.Circle:
                    float r = (collision2 as CircleCollider2D).Radius;
                    float dx = collision.position.x - collision2.position.x;
                    float dy = collision.position.y - collision2.position.y;
                    if (dx * dx + dy * dy <= r * r)
                        return true;
                    break;
                case ColliderType.Line:
                    return Physics2D.DotToLine(ref collision.position,ref collision2.vertex[0],ref collision2.vertex[1]);
                case ColliderType.Polygon:
                    return Physics2D.DotToPolygon(collision2.vertex,collision.position);
                case ColliderType.Ellipse:
                    EllipseCollider2D ellipse = collision2 as EllipseCollider2D;
                    return Physics2D.DotToEllipse(collision2.position,collision.position,ellipse.Width,ellipse.Height);
            }
            return false;
        }
        protected static void CheckCircleCollider(CircleCollider2D circle,Collider2D collider)
        {
            switch(collider.type)
            {
                case ColliderType.Dot:
                    
                    break;
                case ColliderType.Circle:
                    break;
                case ColliderType.Line:
                    break;
                case ColliderType.Polygon:
                    break;
                case ColliderType.Ellipse:
                    break;
            }
        }
        protected static void CheckLineCollider()
        {

        }
        protected static void CheckPolygonCollider()
        {

        }
        protected static void CheckEllipseCollider()
        {

        }
    }
}
