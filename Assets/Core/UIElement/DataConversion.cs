using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UI
{
    public interface DataStorage
    {
        FakeStruct ToBufferData(DataBuffer data);
    }
    public class DataConversion
    {
        public virtual void Reset() { }
        public string type;
        public GameObject Main;
        public virtual ModelElement model { get; set; }
        public virtual void Load(FakeStruct fake) { }
        public virtual void RestoringRelationships(List<AssociatedInstance> table)
        {
        }
        public virtual void LoadToObject(Component game) { }
        public bool IsChanged = true;
        public virtual void Apply() { IsChanged = false; }
        public override string ToString()
        {
            return type;
        }
        /// <summary>
        /// 当此物体为非实体时,不予创建GameObject
        /// </summary>
        public bool Entity = true;
    }
}
