using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct RelativeJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public float maxForce;
        public float maxTorque;
        public float correctionScale;
        public bool autoConfigureOffset;
        public Vector2 linearOffset;
        public float angularOffset;
        public static int Size = sizeof(RelativeJointData);
        public static int ElementSize = Size / 4;
    }
    public class RelativeJointModel:DataConversion
    {
        RelativeJointData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(RelativeJointData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref RelativeJointData data)
        {
            var obj = game.GetComponent<RelativeJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data.enableCollision;
            obj.breakForce = data.breakForce;
            obj.breakTorque = data.breakTorque;
            obj.maxForce = data.maxForce;
            obj.maxTorque = data.maxTorque;
            obj.correctionScale = data.correctionScale;
            obj.autoConfigureOffset = data.autoConfigureOffset;
            obj.linearOffset = data.linearOffset;
            obj.angularOffset = data.angularOffset;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as RelativeJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, RelativeJointData.ElementSize);
            RelativeJointData* dj = (RelativeJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->maxForce = obj.maxForce;
            dj->maxTorque =obj.maxTorque;
            dj->correctionScale = obj.correctionScale;
            dj->autoConfigureOffset = obj.autoConfigureOffset;
            dj->linearOffset = obj.linearOffset;
            dj->angularOffset = obj.angularOffset;
            return fake;
        }
    }
}
