using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Shot2D
{
    public class CircleBullet : Bullet
    {
        public CircleBullet(BulletCarrier car, int inx) : base(car, inx)
        {
        }
        public float Radius;

    }
}
