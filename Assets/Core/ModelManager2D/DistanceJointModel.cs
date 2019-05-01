using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
     public unsafe struct DistanceJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public bool autoConfigure;
        public bool autoConfigureDistance;
        public float distance;
        public bool maxDistanceOnly;
        public static int Size = sizeof(DistanceJointData);
        public static int ElementSize = Size / 4;
    }
    public class DistanceJointModel:DataConversion
    {
        DistanceJointData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(DistanceJointData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref DistanceJointData data)
        {
            var obj = game.GetComponent<DistanceJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data.enableCollision;
            obj.breakForce = data.breakForce;
            obj.breakTorque = data.breakTorque;
            obj.anchor = data.anchor;
            obj.connectedAnchor = data.connectedAnchor;
            obj.autoConfigureConnectedAnchor = data.autoConfigure;
            obj.autoConfigureDistance = data.autoConfigureDistance;
            obj.distance = data.distance;
            obj.maxDistanceOnly = data.maxDistanceOnly;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj= com as DistanceJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, DistanceJointData.ElementSize);
            DistanceJointData* dj = (DistanceJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->autoConfigure= obj.autoConfigureConnectedAnchor;
            dj->autoConfigureDistance = obj.autoConfigureDistance;
            dj->distance = obj.distance;
            dj->maxDistanceOnly = obj.maxDistanceOnly;
            return fake;
        }
    }
}
