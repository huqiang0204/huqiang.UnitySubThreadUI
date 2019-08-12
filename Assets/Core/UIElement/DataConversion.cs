﻿using huqiang.Data;
using UnityEngine;

namespace huqiang.UI
{
    public class DataConversion
    {
        public virtual void Reset() { }
        public string type;
        public GameObject Main;
        public virtual ModelElement model { get; set; }
        public virtual void Load(FakeStruct fake) { }
        public virtual void LoadToObject(Component game) { }
        public bool IsChanged = true;
        public virtual void Apply() { IsChanged = false; }
        public override string ToString()
        {
            return type;
        }
    }
}
