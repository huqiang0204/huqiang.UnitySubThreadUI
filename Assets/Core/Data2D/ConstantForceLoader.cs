using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct ConstantForceData
    {
        public Vector2 force;
        public Vector2 relativeForce;
        public float torque;
        public static int Size = sizeof(ConstantForceData);
        public static int ElementSize = Size / 4;
    }
    public class ConstantForceLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            ConstantForceData* data = (ConstantForceData*)fake.ip;
            var obj = game.GetComponent<ConstantForce2D>();
            if (obj == null)
                return;
            obj.force = data->force;
            obj.relativeForce = data->relativeForce;
            obj.torque = data->torque;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as ConstantForce2D;
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, ConstantForceData.ElementSize);
            ConstantForceData* data = (ConstantForceData*)fake.ip;
            data->force = ae.force;
            data->relativeForce = ae.relativeForce;
            data->torque = ae.torque;
            return fake;
        }
    }
}
