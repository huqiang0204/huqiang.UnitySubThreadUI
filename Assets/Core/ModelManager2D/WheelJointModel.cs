﻿using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct WheelJointData
    {
        public bool enableCollision;
        public float breakForce;
        public float breakTorque;
        public Vector2 anchor;
        public Vector2 connectedAnchor;
        public bool autoConfigure;
        public JointSuspension2D suspension;
        public bool useMotor;
        public JointMotor2D motor;
        public float frequency { get; set; }
        public static int Size = sizeof(WheelJointData);
        public static int ElementSize = Size / 4;
    }
    public class WheelJointModel:DataConversion
    {
        WheelJointData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(WheelJointData*)fake.ip;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data);
        }
        public static void LoadToObject(Component game, ref WheelJointData data)
        {
            var obj = game.GetComponent<WheelJoint2D>();
            if (obj == null)
                return;
            obj.enableCollision = data.enableCollision;
            obj.breakForce = data.breakForce;
            obj.breakTorque = data.breakTorque;
            obj.anchor=data.anchor;
            obj.connectedAnchor = data.connectedAnchor;
            obj.autoConfigureConnectedAnchor = data.autoConfigure;
            obj.suspension = data.suspension;
            obj.useMotor = data.useMotor;
            obj.motor = data.motor;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as WheelJoint2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, WheelJointData.ElementSize);
            WheelJointData* dj = (WheelJointData*)fake.ip;
            dj->enableCollision = obj.enableCollision;
            dj->breakForce = obj.breakForce;
            dj->breakTorque = obj.breakTorque;
            dj->anchor = obj.anchor;
            dj->connectedAnchor = obj.connectedAnchor;
            dj->autoConfigure = obj.autoConfigureConnectedAnchor;
            dj->suspension = obj.suspension;
            dj->useMotor = obj.useMotor;
            dj->motor = obj.motor;
            return fake;
        }
    }
}
