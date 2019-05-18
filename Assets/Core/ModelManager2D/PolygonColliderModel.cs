using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct PolygonColliderData
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
        public bool autoTiling;
        public int pathCount;
        public static int Size = sizeof(PolygonColliderData);
        public static int ElementSize = Size / 4;
    }
    public class PolygonColliderModel:DataConversion
    {
        PolygonColliderData data;
        string sharedMaterial;
        Vector2[] points;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(PolygonColliderData*)fake.ip;
            sharedMaterial = fake.buffer.GetData(data.sharedMaterial) as string;
            points = fake.buffer.GetArray<Vector2>(data.points);
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data, this);
        }
        public static void LoadToObject(Component game, ref PolygonColliderData data, PolygonColliderModel edge)
        {
            var obj = game.GetComponent<PolygonCollider2D>();
            if (obj == null)
                return;
            if (edge.sharedMaterial != null)
                obj.sharedMaterial = new PhysicsMaterial2D(edge.sharedMaterial);
            obj.points = edge.points;
            obj.offset = data.offset;
            obj.usedByEffector = data.usedByEffector;
            obj.isTrigger = data.isTrigger;
            obj.density = data.density;
            obj.autoTiling = data.autoTiling;
            obj.pathCount = data.pathCount;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as PolygonCollider2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, PolygonColliderData.ElementSize);
            PolygonColliderData* ec = (PolygonColliderData*)fake.ip;
            if (obj.sharedMaterial != null)
                ec->sharedMaterial = buffer.AddData(obj.sharedMaterial.name);
            ec->offset = obj.offset;
            ec->usedByEffector = obj.usedByEffector;
            ec->isTrigger = obj.isTrigger;
            ec->density = obj.density;
            ec->autoTiling = obj.autoTiling;
            ec->pathCount = obj.pathCount;
            if (obj.points != null)
                ec->points = buffer.AddArray<Vector2>(obj.points);
            return fake;
        }
    }
}
