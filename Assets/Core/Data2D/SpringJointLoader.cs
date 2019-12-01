﻿using huqiang.Data;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct SpringJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public bool autoConfigure;
        public bool autoConfigureDistance;
        public float distance;
        public float dampingRatio;
        public float frequency;
        public static int Size = sizeof(SpringJointData);
        public static int ElementSize = Size / 4;
    }
    public class SpringJointLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            var data = (SpringJointData*)fake.ip;
            var obj = game.GetComponent<SpringJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data->enableCollision;
            obj.breakForce = data->breakForce;
            obj.breakTorque = data->breakTorque;
            obj.anchor = data->anchor;
            obj.connectedAnchor = data->connectedAnchor;
            obj.autoConfigureConnectedAnchor = data->autoConfigure;
            obj.autoConfigureDistance = data->autoConfigureDistance;
            obj.distance = data->distance;
            obj.dampingRatio = data->dampingRatio;
            obj.frequency = data->frequency;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as SpringJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, SpringJointData.ElementSize);
            SpringJointData* dj = (SpringJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->autoConfigure = obj.autoConfigureConnectedAnchor;
            dj->autoConfigureDistance = obj.autoConfigureDistance;
            dj->distance = obj.distance;
            dj->dampingRatio = obj.dampingRatio;
            dj->frequency = obj.frequency;
            return fake;
        }
    }
}
