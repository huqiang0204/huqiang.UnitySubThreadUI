using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data
{
    public abstract class DataLoader
    {
        public GameobjectBuffer gameobjectBuffer;
        public virtual void LoadToObject(FakeStruct fake,Component com) { }
        public virtual FakeStruct LoadFromObject(Component com,DataBuffer buffer) { return null; }
    }
}
