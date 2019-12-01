using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct FrictionJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public float maxForce;
        public float maxTorque;
        public static int Size = sizeof(FrictionJointData);
        public static int ElementSize = Size / 4;
    }
    public class FrictionJointLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            FrictionJointData* data = (FrictionJointData*)fake.ip;
            var obj = game.GetComponent<FrictionJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data->enableCollision;
            obj.breakForce = data->breakForce;
            obj.breakTorque = data->breakTorque;
            obj.anchor = data->anchor;
            obj.connectedAnchor = data->connectedAnchor;
            obj.maxForce = data->maxForce;
            obj.maxTorque = data->maxTorque;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as FrictionJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, FrictionJointData.ElementSize);
            FrictionJointData* dj = (FrictionJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->maxForce= obj.maxForce;
            dj->maxTorque = obj.maxTorque;
            return fake;
        }
    }
}
