using huqiang.Data;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct TargetJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public float maxForce;
        public Vector2 anchor;
        public Vector2 target;
        public bool autoConfigureTarget;
        public float dampingRatio;
        public float frequency { get; set; }
        public static int Size = sizeof(TargetJointData);
        public static int ElementSize = Size / 4;
    }
    public class TargetJointLoader : DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            var data = (TargetJointData*)fake.ip;
            var obj = game.GetComponent<TargetJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data->enableCollision;
            obj.breakForce = data->breakForce;
            obj.breakTorque = data->breakTorque;
            obj.maxForce = data->maxForce;
            obj.anchor = data->anchor;
            obj.target = data->target;
            obj.autoConfigureTarget = data->autoConfigureTarget;
            obj.dampingRatio = data->dampingRatio;
        }

        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as TargetJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, TargetJointData.ElementSize);
            TargetJointData* dj = (TargetJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->maxForce = obj.maxForce;
            dj->anchor = obj.anchor;
            dj->target = obj.target;
            dj->autoConfigureTarget = obj.autoConfigureTarget;
            dj->dampingRatio = obj.dampingRatio;
            return fake;
        }
    }
}
