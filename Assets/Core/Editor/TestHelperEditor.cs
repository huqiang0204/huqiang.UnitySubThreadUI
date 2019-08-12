using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

[CustomEditor(typeof(CreateTestHelper), true)]
[CanEditMultipleObjects]
public class TestHelperEditor : Editor
{
    protected virtual void OnEnable() {
        (target as CreateTestHelper).Build();
    }
}
