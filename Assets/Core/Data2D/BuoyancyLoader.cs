using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct BuoyancyData
    {
        public bool useColliderMask;
        public int colliderMask;
        public float surfaceLevel;
        public float density;
        public float linearDrag;
        public float angularDrag;
        public float flowAngle;
        public float flowMagnitude;
        public float flowVariation;
        public static int Size = sizeof(BuoyancyData);
        public static int ElementSize = Size / 4;
    }
    public class BuoyancyLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake,Component game)
        {
            BuoyancyData* data = (BuoyancyData*)fake.ip;
            var obj = game.GetComponent<BuoyancyEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data->useColliderMask;
            obj.colliderMask = data->colliderMask;
            obj.surfaceLevel = data->surfaceLevel;
            obj.density = data->density;
            obj.linearDrag = data->linearDrag;
            obj.angularDrag = data->angularDrag;
            obj.flowAngle = data->flowAngle;
            obj.flowMagnitude = data->flowMagnitude;
            obj.flowVariation = data->flowVariation;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as BuoyancyEffector2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, BuoyancyData.ElementSize);
            BuoyancyData* bd = (BuoyancyData*)fake.ip;
            bd->useColliderMask = obj.useColliderMask;
            bd->colliderMask = obj.colliderMask;
            bd->surfaceLevel = obj.surfaceLevel;
            bd->density = obj.density;
            bd->linearDrag = obj.linearDrag;
            bd->angularDrag = obj.angularDrag;
            bd->flowAngle = obj.flowAngle;
            bd->flowMagnitude = obj.flowMagnitude;
            bd->flowVariation = obj.flowVariation;
            return fake;
        }
    }
}
