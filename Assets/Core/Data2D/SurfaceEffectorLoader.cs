using huqiang.Data;
using UnityEngine;

namespace huqiang.Data2D
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
    public class SurfaceEffectorLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            var data = (SurfaceEffectorData*)fake.ip;
            var obj = game.GetComponent<SurfaceEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data->useColliderMask;
            obj.colliderMask = data->colliderMask;
            obj.speed = data->speed;
            obj.speedVariation = data->speedVariation;
            obj.forceScale = data->forceScale;
            obj.useContactForce = data->useContactForce;
            obj.useFriction = data->useFriction;
            obj.useBounce = data->useBounce;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
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
