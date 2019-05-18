using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct SurfaceEffectorData
    {
        public bool useColliderMask;
        public int colliderMask;
        public float speed;
        public float speedVariation;
        public float forceScale;
        public bool useContactForce;
        public bool useFriction;
        public bool useBounce;
        public static int Size = sizeof(SurfaceEffectorData);
        public static int ElementSize = Size / 4;
    }
    public class SurfaceEffectorModel:DataConversion
    {
        SurfaceEffectorData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(SurfaceEffectorData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref SurfaceEffectorData data)
        {
            var obj = game.GetComponent<SurfaceEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data.useColliderMask;
            obj.colliderMask = data.colliderMask;
            obj.speed = data.speed;
            obj.speedVariation = data.speedVariation;
            obj.forceScale = data.forceScale;
            obj.useContactForce = data.useContactForce;
            obj.useFriction = data.useFriction;
            obj.useBounce = data.useBounce;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as SurfaceEffector2D;
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, SurfaceEffectorData.ElementSize);
            SurfaceEffectorData* data = (SurfaceEffectorData*)fake.ip;
            data->useColliderMask = ae.useColliderMask;
            data->colliderMask = ae.colliderMask;
            data->speed = ae.speed;
            data->speedVariation = ae.speedVariation;
            data->forceScale = ae.forceScale;
            data->useContactForce = ae.useContactForce;
            data->useFriction = ae.useFriction;
            data->useBounce = ae.useBounce;
            return fake;
        }
    }
}
