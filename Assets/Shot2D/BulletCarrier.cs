using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Shot2D
{
    public class BulletCarrier
    {
        public Vector3[] buffer;
        public Vector2[] uv;
        public int[] tri;
        public Bullet[] bullets;
        int max;
        public void Initial(int count)
        {
            max = count;
            bullets = new Bullet[count];
        }
        public void Update()
        {
            if (bullets == null)
                return;
            for (int i = 0; i < max; i++)
                if (bullets[i] != null)
                    bullets[i].Update();
        }
        public void Applay()
        {

        }
        public void RemoveBullet(int index)
        {

        }
    }
}
