using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.ModelManager2D
{
    public unsafe struct BoxColliderData
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
        public Vector2 size;
        public float edgeRadius;
        public bool autoTiling;
        public static int Size = sizeof(BoxColliderData);
        public static int ElementSize = Size / 4;
    }
    public class BoxColliderModel:DataConversion
    {
        BoxColliderData data;
        string sharedMaterial;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(BoxColliderData*)fake.ip;
            sharedMaterial = fake.buffer.GetData(data.sharedMaterial) as string;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data,sharedMaterial);
        }
        public static void LoadToObject(Component game, ref BoxColliderData data,string mat)
        {
            var obj = game.GetComponent<BoxCollider2D>();
            if (obj == null)
                return;
            obj.offset = data.offset;
            obj.usedByComposite = data.usedByComposite;
            obj.usedByEffector = data.usedByEffector;
            obj.isTrigger = data.isTrigger;
            obj.density = data.density;
            obj.size = data.size;
            obj.edgeRadius = data.edgeRadius;
            obj.autoTiling = data.autoTiling;
            if (mat != null)
                obj.sharedMaterial = new PhysicsMaterial2D(mat);
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as BoxCollider2D;
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, BoxColliderData.ElementSize);
            BoxColliderData* data = (BoxColliderData*)fake.ip;
            if (ae.sharedMaterial != null)
                data->sharedMaterial = buffer.AddData(ae.sharedMaterial.name);
            data->offset = ae.offset;
            data->usedByComposite = ae.usedByComposite;
            data->usedByEffector = ae.usedByEffector;
            data->isTrigger = ae.isTrigger;
            data->density= ae.density;
            data->size = ae.size;
            data->edgeRadius = ae.edgeRadius;
            data->autoTiling= ae.autoTiling;
            return fake;
        }
    }
}