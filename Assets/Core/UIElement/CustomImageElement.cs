using System;
using System.Collections.Generic;
using huqiang.Data;
using UGUI;
using UnityEngine;

namespace huqiang.UI
{
    public class CustomImageElement:RawImageElement
    {
        public CustomRawImage custom;
        protected bool vertChanged;
        protected List<UIVertex> vertex;
        protected List<int> tris;
        public override void LoadToObject(Component game)
        {
            base.LoadToObject(game);
            custom = Context as CustomRawImage;
        }
        public void UpdateVert(List<UIVertex> vertices,List<int> tri)
        {
            vertex = vertices;
            tris = tri;
            vertChanged = true;
        }
        public override void Apply()
        {
            base.Apply();
            if(vertChanged)
            {
                custom.uIVertices = vertex;
                custom.triangle = tris;
                custom.SetVerticesDirty();
                vertChanged = false;
            }
        }
    }

}
