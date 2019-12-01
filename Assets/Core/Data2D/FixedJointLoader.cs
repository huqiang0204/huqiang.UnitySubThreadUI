using huqiang.Data;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct FixedJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public bool autoConfigure;
        public float dampingRatio;
        public float frequency;
        public static int Size = sizeof(FixedJointData);
        public static int ElementSize = Size / 4;
    }
    public class FixedJointLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            FixedJointData* data = (FixedJointData*)fake.ip;
            var obj = game.GetComponent<FixedJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data->enableCollision;
            obj.breakForce = data->breakForce;
            obj.breakTorque = data->breakTorque;
            obj.anchor = data->anchor;
            obj.connectedAnchor = data->connectedAnchor;
            obj.autoConfigureConnectedAnchor = data->autoConfigure;
            obj.dampingRatio = data->dampingRatio;
            obj.frequency = data->frequency;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as FixedJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, FixedJointData.ElementSize);
            FixedJointData* dj = (FixedJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->autoConfigure = obj.autoConfigureConnectedAnchor;
            dj->dampingRatio = obj.dampingRatio;
            dj->frequency = obj.frequency;
            return fake;
        }
    }
}
