using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct ConstantForceData
    {
        public Vector2 force;
        public Vector2 relativeForce;
        public float torque;
        public static int Size = sizeof(ConstantForceData);
        public static int ElementSize = Size / 4;
    }
    public class ConstantForceModel:DataConversion
    {
        ConstantForceData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(ConstantForceData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref ConstantForceData data)
        {
            var obj = game.GetComponent<ConstantForce2D>();
            if (obj == null)
                return;
            obj.force = data.force;
            obj.relativeForce = data.relativeForce;
            obj.torque = data.torque;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
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
