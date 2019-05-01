using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
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
    public class TargetJointModel:DataConversion
    {
        TargetJointData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(TargetJointData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref TargetJointData data)
        {
            var obj = game.GetComponent<TargetJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data.enableCollision;
            obj.breakForce = data.breakForce;
            obj.breakTorque = data.breakTorque;
            obj.maxForce = data.maxForce;
            obj.anchor = data.anchor;
            obj.target = data.target;
            obj.autoConfigureTarget = data.autoConfigureTarget;
            obj.dampingRatio = data.dampingRatio;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
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
