using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct CompositeColliderData
    {
        /// <summary>
        /// name
        /// </summary>
        public Int32 sharedMaterial;
        public Vector2 offset;
        public bool usedByComposite;
        public bool usedByEffector;
        public bool isTrigger;
        public float density;
        public CompositeCollider2D.GeometryType geometryType;
        public CompositeCollider2D.GenerationType generationType;
        public float vertexDistance;
        public static int Size = sizeof(CompositeColliderData);
        public static int ElementSize = Size / 4;
    }
    public class CompositeColliderModel:DataConversion
    {
        CompositeColliderData data;
        string sharedMaterial;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(CompositeColliderData*)fake.ip;
            sharedMaterial = fake.buffer.GetData(data.sharedMaterial) as string;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data, sharedMaterial);
        }
        public static void LoadToObject(Component game, ref CompositeColliderData data, string mat)
        {
            var obj = game as CompositeCollider2D;
            if (obj == null)
                return;
            obj.offset = data.offset;
            obj.usedByComposite = data.usedByComposite;
            obj.usedByEffector = data.usedByEffector;
            obj.isTrigger = data.isTrigger;
            obj.density = data.density;
            obj.geometryType = data.geometryType;
            obj.generationType = data.generationType;
            obj.vertexDistance = data.vertexDistance;
            if (mat != null)
                obj.sharedMaterial = new PhysicsMaterial2D(mat);
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com.GetComponent<CompositeCollider2D>();
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, CompositeColliderData.ElementSize);
            CompositeColliderData* data = (CompositeColliderData*)fake.ip;
            if (ae.sharedMaterial != null)
                data->sharedMaterial = buffer.AddData(ae.sharedMaterial.name);
            data->offset = ae.offset;
            data->usedByComposite = ae.usedByComposite;
            data->usedByEffector = ae.usedByEffector;
            data->isTrigger = ae.isTrigger;
            data->density = ae.density;
            data->geometryType = ae.geometryType;
            data->generationType = ae.generationType;
            data->vertexDistance = ae.vertexDistance;
            return fake;
        }
    }
}
