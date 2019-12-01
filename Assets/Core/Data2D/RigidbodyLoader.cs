using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct RigibodyData
    {
        public float rotation;
        public Vector2 velocity;
        public float angularVelocity;
        public bool useAutoMass;
        public float mass;
        /// <summary>
        /// name
        /// </summary>
        public Int32 sharedMaterial;
        public Vector2 centerOfMass;
        public float inertia;
        public float drag;
        public float angularDrag;
        public float gravityScale;
        public RigidbodyType2D bodyType;
        public bool useFullKinematicContacts;
        public bool isKinematic;
        public bool freezeRotation;
        public RigidbodyConstraints2D constraints;
        public bool simulated;
        public RigidbodyInterpolation2D interpolation;
        public RigidbodySleepMode2D sleepMode;
        public Vector2 position;
        public CollisionDetectionMode2D collisionDetectionMode;
        public static int Size = sizeof(RigibodyData);
        public static int ElementSize = Size / 4;
    }
    public class RigidbodyLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            var data = (RigibodyData*)fake.ip;
            var obj = game.GetComponent<Rigidbody2D>();
            if (obj == null)
                return;
            obj.rotation = data->rotation;
            obj.velocity = data->velocity;
            obj.angularVelocity = data->angularVelocity;
            obj.useAutoMass = data->useAutoMass;
            obj.mass = data->mass;
            obj.centerOfMass = data->centerOfMass;
            obj.inertia = data->inertia;
            obj.drag = data->drag;
            obj.angularDrag = data->angularDrag;
            obj.gravityScale = data->gravityScale;
            obj.bodyType = data->bodyType;
            obj.useFullKinematicContacts = data->useFullKinematicContacts;
            obj.isKinematic = data->isKinematic;
            obj.freezeRotation = data->freezeRotation;
            obj.constraints = data->constraints;
            obj.simulated = data->simulated;
            obj.interpolation = data->interpolation;
            obj.sleepMode = data->sleepMode;
            obj.position = data->position;
            obj.collisionDetectionMode = data->collisionDetectionMode;
            string mat = fake.buffer.GetData(data->sharedMaterial) as string;
            if (mat != null)
                obj.sharedMaterial = new PhysicsMaterial2D(mat);
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var obj = com as Rigidbody2D;
            if (obj == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, RigibodyData.ElementSize);
            RigibodyData* data = (RigibodyData*)fake.ip;
            if (obj.sharedMaterial != null)
                data->sharedMaterial = buffer.AddData(obj.sharedMaterial.name);
            data->rotation = obj.rotation;
            data->velocity = obj.velocity;
            data->angularVelocity = obj.angularVelocity;
            data->useAutoMass = obj.useAutoMass;
            data->mass = obj.mass;
            data->centerOfMass = obj.centerOfMass;
            data->inertia = obj.inertia;
            data->drag = obj.drag;
            data->angularDrag = obj.angularDrag;
            data->gravityScale = obj.gravityScale;
            data->bodyType =obj.bodyType;
            data->useFullKinematicContacts = obj.useFullKinematicContacts;
            data->isKinematic = obj.isKinematic;
            data->freezeRotation = obj.freezeRotation;
            data->constraints = obj.constraints;
            data->simulated = obj.simulated;
            data->interpolation = obj.interpolation;
            data->sleepMode = obj.sleepMode;
            data->position = obj.position;
            data->collisionDetectionMode = obj.collisionDetectionMode;
            return fake;
        }
    }
}
