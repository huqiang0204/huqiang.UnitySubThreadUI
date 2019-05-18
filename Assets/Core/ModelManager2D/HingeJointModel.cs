using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct HingeJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public bool autoConfigure;
        public bool useMotor;
        public bool useLimits;
        public JointMotor2D motor;
        public JointAngleLimits2D limits;
        public static int Size = sizeof(HingeJointData);
        public static int ElementSize = Size / 4;
    }
    public class HingeJointModel:DataConversion
    {
        HingeJointData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(HingeJointData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref HingeJointData data)
        {
            var obj = game.GetComponent<HingeJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data.enableCollision;
            obj.breakForce = data.breakForce;
            obj.breakTorque = data.breakTorque;
            obj.anchor = data.anchor;
            obj.connectedAnchor = data.connectedAnchor;
            obj.autoConfigureConnectedAnchor = data.autoConfigure;
            obj.useMotor = data.useMotor;
            obj.useLimits = data.useLimits;
            obj.motor = data.motor;
            obj.limits = data.limits;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as HingeJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HingeJointData.ElementSize);
            HingeJointData* dj = (HingeJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->autoConfigure = obj.autoConfigureConnectedAnchor;
            dj->useMotor = obj.useMotor;
            dj->useLimits = obj.useLimits;
            dj->motor = obj.motor;
            dj->limits = obj.limits;
            return fake;
        }
    }
}
