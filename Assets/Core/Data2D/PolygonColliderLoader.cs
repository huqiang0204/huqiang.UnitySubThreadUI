using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data2D
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
    public class PolygonColliderLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            PolygonColliderData* data = (PolygonColliderData*)fake.ip;
            var obj = game.GetComponent<PolygonCollider2D>();
            if (obj == null)
                return;
            obj.points = fake.buffer.GetArray<Vector2>(data->points);
            obj.offset = data->offset;
            obj.usedByEffector = data->usedByEffector;
            obj.isTrigger = data->isTrigger;
            obj.density = data->density;
            obj.autoTiling = data->autoTiling;
            obj.pathCount = data->pathCount;
            string mat = fake.buffer.GetData(data->sharedMaterial) as string;
            if (mat != null)
                obj.sharedMaterial = new PhysicsMaterial2D(mat);
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
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
