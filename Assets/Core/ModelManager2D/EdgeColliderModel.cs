using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct EdgeColliderData
    {
        /// <summary>
        /// name
        /// </summary>
        public Int32 sharedMaterial;
        public Vector2 offset;
        public bool usedByEffector;
        public bool isTrigger;
        public float density;
        public float edgeRadius;
        public Int32 points;
        public static int Size = sizeof(EdgeColliderData);
        public static int ElementSize = Size / 4;
    }
    public class EdgeColliderModel:DataConversion
    {
        EdgeColliderData data;
        string sharedMaterial;
        Vector2[] points;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(EdgeColliderData*)fake.ip;
            sharedMaterial = fake.buffer.GetData(data.sharedMaterial) as string;
            points = fake.buffer.GetArray<Vector2>(data.points);
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data,this);
        }
        public static void LoadToObject(Component game, ref EdgeColliderData data,EdgeColliderModel edge)
        {
            var obj = game.GetComponent<EdgeCollider2D>();
            if (obj == null)
                return;
            if (edge.sharedMaterial != null)
                obj.sharedMaterial = new PhysicsMaterial2D(edge.sharedMaterial);
            obj.points = edge.points;
            obj.offset = data.offset;
            obj.usedByEffector = data.usedByEffector;
            obj.isTrigger = data.isTrigger;
            obj.density = data.density;
            obj.edgeRadius = data.edgeRadius;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as EdgeCollider2D;
            if (obj== null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, EdgeColliderData.ElementSize);
            EdgeColliderData* ec = (EdgeColliderData*)fake.ip;
            if (obj.sharedMaterial != null)
                ec->sharedMaterial = buffer.AddData(obj.sharedMaterial.name);
            ec->offset = obj.offset;
            ec->usedByEffector = obj.usedByEffector;
            ec->isTrigger = obj.isTrigger;
            ec->density = obj.density;
            ec->edgeRadius = obj.edgeRadius;
            if (obj.points != null)
                ec->points = buffer.AddArray<Vector2>(obj.points);
            return fake;
        }
    }
}