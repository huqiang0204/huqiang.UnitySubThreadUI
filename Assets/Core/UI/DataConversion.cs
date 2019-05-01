using huqiang.Data;
using UnityEngine;

namespace huqiang.UI
{
    public class DataConversion
    {
        public string type;
        public GameObject Main;
        public virtual void Load(FakeStruct fake) { }
        public virtual void LoadToObject(Component game) { }
        public bool IsChanged;
        public virtual void Apply() { IsChanged = false; }
    }
}
