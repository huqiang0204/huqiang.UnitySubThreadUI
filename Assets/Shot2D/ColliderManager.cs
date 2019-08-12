using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Shot2D
{
    public class ColliderManager
    {
        static List<Collider2D>[] buffer = new List<Collider2D>[32];
        public static void Initial()
        {
            for(int i=0;i<32;i++)
            {
                buffer[i] = new List<Collider2D>();
            }
        }
        public static void RegCollider(Collider2D collider)
        {
            buffer[collider.layer].Add(collider);
        }
        public static void RemoveCollider(Collider2D collider)
        {
            buffer[collider.layer].Remove(collider);
        }
    }
}
