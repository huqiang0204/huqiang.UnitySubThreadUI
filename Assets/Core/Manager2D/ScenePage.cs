using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Manager2D
{
    public class ScenePage
    {
        public Scene2D Scene;
        public GameObject Instance;
        public virtual void Initial(Transform trans,object dat)
        {
        }
        public virtual void Show(object dat)
        {
        }
        public virtual void Update()
        {
        }
        public virtual void Dispose()
        {
            if(Instance!=null)
            {
                GameObject.Destroy(Instance);
                Instance = null;
            }
        }
    }
}
