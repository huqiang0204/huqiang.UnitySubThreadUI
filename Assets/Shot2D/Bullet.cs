using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Shot2D
{
    public class Bullet
    {
        public Vector3 Postion;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Vector2[] uv;
        public float Speed;
        public float Attack;
        public Action<Bullet> Move;
        public Action<Bullet> Collision;
        BulletCarrier carrier;
        int index;
        public Bullet(BulletCarrier car,int inx)
        {
            carrier = car;
            index = inx;
        }
        public void Update()
        {
            if (Move != null)
                Move(this);
            if (Collision != null)
                Collision(this);
        }
        public virtual void Destroy()
        {
            carrier.RemoveBullet(index);
        }

    }
}

