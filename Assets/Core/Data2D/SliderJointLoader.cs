﻿using huqiang.Data;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct SliderJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public bool autoConfigure;
        public bool autoConfigureAngle;
        public float angle;
        public bool useMotor;
        public bool useLimits;
        public JointMotor2D motor;
        public JointTranslationLimits2D limits;
        public static int Size = sizeof(SliderJointData);
        public static int ElementSize = Size / 4;
    }
    public class SliderJointLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            var data = (SliderJointData*)fake.ip;
            var obj = game.GetComponent<SliderJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data->enableCollision;
            obj.breakForce = data->breakForce;
            obj.breakTorque = data->breakTorque;
            obj.anchor = data->anchor;
            obj.connectedAnchor = data->connectedAnchor;
            obj.autoConfigureConnectedAnchor = data->autoConfigure;
            obj.autoConfigureAngle = data->autoConfigureAngle;
            obj.angle = data->angle;
            obj.useMotor = data->useMotor;
            obj.useLimits = data->useLimits;
            obj.motor = data->motor;
            obj.limits = data->limits;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as SliderJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, SliderJointData.ElementSize);
            SliderJointData* dj = (SliderJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->autoConfigure = obj.autoConfigureConnectedAnchor;
            dj->autoConfigureAngle = obj.autoConfigureAngle;
            dj->angle = obj.angle;
            dj->useMotor = obj.useMotor;
            dj->useLimits = obj.useLimits;
            dj->motor = obj.motor;
            dj->limits = obj.limits;
            return fake;
        }
    }
}
