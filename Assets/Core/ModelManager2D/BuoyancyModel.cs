using huqiang.Data;
using huqiang.UI;
using System;
using UnityEngine;

namespace huqiang.ModelManager2D
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
    public class BuoyancyModel:DataConversion
    {
        BuoyancyData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(BuoyancyData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref BuoyancyData data)
        {
            var obj = game.GetComponent<BuoyancyEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data.useColliderMask;
            obj.colliderMask = data.colliderMask;
            obj.surfaceLevel = data.surfaceLevel;
            obj.density = data.density;
            obj.linearDrag = data.linearDrag;
            obj.angularDrag = data.angularDrag;
            obj.flowAngle = data.flowAngle;
            obj.flowMagnitude = data.flowMagnitude;
            obj.flowVariation = data.flowVariation;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
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
