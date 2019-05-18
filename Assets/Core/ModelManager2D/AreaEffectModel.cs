using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using huqiang.Data;
using huqiang.UI;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct AreaEffectData
    {
        public bool useColliderMask;
        public int colliderMask;
        public float forceAngle;
        public bool useGlobalAngle;
        public float forceMagnitude;
        public float forceVariation;
        public float drag;
        public float angularDrag;
        public EffectorSelection2D forceTarget;
        public static int Size = sizeof(AreaEffectData);
        public static int ElementSize = Size / 4;
    }
    public class AreaEffectModel:DataConversion
    {
        AreaEffectData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(AreaEffectData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game,ref data);
        }
        public static void LoadToObject(Component game,ref AreaEffectData data)
        {
            var obj = game.GetComponent<AreaEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data.useColliderMask;
            obj.colliderMask = data.colliderMask;
            obj.forceAngle = data.forceAngle;
            obj.useGlobalAngle = data.useGlobalAngle;
            obj.forceMagnitude = data.forceMagnitude;
            obj.forceVariation = data.forceVariation;
            obj.drag = data.drag;
            obj.angularDrag = data.angularDrag;
            obj.forceTarget = data.forceTarget;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as AreaEffector2D;
            if (ae== null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, AreaEffectData.ElementSize);
            AreaEffectData* data = (AreaEffectData *)fake.ip;
            data->useColliderMask = ae.useColliderMask;
            data->colliderMask = ae.colliderMask;
            data->forceAngle = ae.forceAngle;
            data->useGlobalAngle = ae.useGlobalAngle;
            data->forceMagnitude = ae.forceMagnitude;
            data->forceVariation = ae.forceVariation;
            data->drag = ae.drag;
            data->angularDrag = ae.angularDrag;
            data->forceTarget = ae.forceTarget;
            return fake;
        }
    }
}
