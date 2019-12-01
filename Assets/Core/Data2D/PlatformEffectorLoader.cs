using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct PlatformEffectorData
    {
        public bool useColliderMask;
        public int colliderMask;
        public bool useOneWay;
        public bool useOneWayGrouping;
        public bool useSideFriction;
        public bool useSideBounce;
        public float surfaceArc;
        public float sideArc;
        public float rotationalOffset;
        public static int Size = sizeof(PlatformEffectorData);
        public static int ElementSize = Size / 4;
    }
    public class PlatformEffectorLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            PlatformEffectorData* data = (PlatformEffectorData*)fake.ip;
            var obj = game.GetComponent<PlatformEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data->useColliderMask;
            obj.colliderMask = data->colliderMask;
            obj.useOneWay = data->useOneWay;
            obj.useOneWayGrouping = data->useOneWayGrouping;
            obj.useSideFriction = data->useSideFriction;
            obj.useSideBounce = data->useSideBounce;
            obj.surfaceArc = data->surfaceArc;
            obj.sideArc = data->sideArc;
            obj.rotationalOffset = data->rotationalOffset;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as PlatformEffector2D;
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, PlatformEffectorData.ElementSize);
            PlatformEffectorData* data = (PlatformEffectorData*)fake.ip;
            data->useColliderMask = ae.useColliderMask;
            data->colliderMask = ae.colliderMask;
            data->useOneWayGrouping = ae.useOneWayGrouping;
            data->useSideFriction = ae.useSideFriction;
            data->useSideBounce = ae.useSideBounce;
            data->surfaceArc = ae.surfaceArc;
            data->sideArc = ae.sideArc;
            data->rotationalOffset = ae.rotationalOffset;
            return fake;
        }
    }
}
