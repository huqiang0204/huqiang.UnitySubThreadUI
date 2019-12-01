using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct CircleColliderData
    {
        /// <summary>
        /// name
        /// </summary>
        public Int32 sharedMaterial;
        public Vector2 offset;
        public bool usedByComposite;
        public bool usedByEffector;
        public bool isTrigger;
        public float density;
        public float radius;
        public static int Size = sizeof(CircleColliderData);
        public static int ElementSize = Size / 4;
    }
    public class CircleColliderLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            CircleColliderData* data = (CircleColliderData*)fake.ip;
            var obj = game.GetComponent<CircleCollider2D>();
            if (obj == null)
                return;
            obj.offset = data->offset;
            obj.usedByComposite = data->usedByComposite;
            obj.usedByEffector = data->usedByEffector;
            obj.isTrigger = data->isTrigger;
            obj.density = data->density;
            obj.radius = data->radius;
            string mat = fake.buffer.GetData(data->sharedMaterial) as string;
            if (mat != null)
                obj.sharedMaterial = new PhysicsMaterial2D(mat);
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as CircleCollider2D;
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, CircleColliderData.ElementSize);
            CircleColliderData* data = (CircleColliderData*)fake.ip;
            if (ae.sharedMaterial != null)
                data->sharedMaterial = buffer.AddData(ae.sharedMaterial.name);
            data->offset = ae.offset;
            data->usedByComposite = ae.usedByComposite;
            data->usedByEffector = ae.usedByEffector;
            data->isTrigger = ae.isTrigger;
            data->density = ae.density;
            data->radius = ae.radius;
            return fake;
        }
    }
}
