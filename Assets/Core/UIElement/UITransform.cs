using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UI
{
    public interface UITransform
    {
        Quaternion LocalRotate { get; set; }
        Vector2 SizeDelta { get; set; }
        Vector3 LocalPosition { get; set; }
        Vector2 LocalScale { get; set; }
        Vector3 GlobalPosition { get; }
        Vector3 GlobalScale { get; }
        Quaternion GlobalRotate { get; }
    }
    public interface Coloring
    {
        Color color { get; set; }
    }
    
}
