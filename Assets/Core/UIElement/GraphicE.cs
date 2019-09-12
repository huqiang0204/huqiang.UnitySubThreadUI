using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UI
{
    public class GraphicE : DataConversion,Coloring,UpdateInterface
    {
        struct FloatValue
        {
            public string name;
            public float value;
        }
        struct VectorValue
        {
            public string name;
            public Vector4 value;
        }
        struct TextureValue
        {
            public string name;
            public string assetName;
            public string t2dName;
        }
        protected Material material;
        protected Color _color=Color.white;
        public override ModelElement model { get => base.model; set { base.model = value; value.ColorController = this;  value.updating = this; } }
        public Color color { get { return _color; } set { _color = value;IsChanged = true; } }
        string mShader;
        bool shderChanged;
        protected string shader { get { return mShader;  } set { shderChanged = mShader == value ? false : true; mShader = value; } }
        FloatValue[] floats = new FloatValue[16];
        int fMax=0;
        VectorValue[] vectors = new VectorValue[16];
        int vMax=0;
        TextureValue[] textures = new TextureValue[16];
        int tMax=0;
        public void SetFloat(string name, float value)
        {
            for(int i=0;i<fMax;i++)
                if(name==floats[i].name)
                {
                    floats[i].value = value;
                    return;
                }
            if(fMax<16)
            {
                floats[fMax].value = value;
                fMax++;
            }
            IsChanged = true;
        }
        public void SetVector(string name,Vector4 value)
        {
            for (int i = 0; i < vMax; i++)
                if (name ==vectors[i].name)
                {
                    vectors[i].value = value;
                    return;
                }
            if (vMax < 16)
            {
                vectors[vMax].value = value;
                vMax++;
            }
            IsChanged = true;
        }
        public void SetTexture(string name,string assetName)
        {
            for (int i = 0; i < tMax; i++)
                if (name == textures[i].name)
                {
                    textures[i].assetName = assetName;
                    textures[i].t2dName = name;
                    return;
                }
            if (tMax < 16)
            {
                textures[tMax].assetName = assetName;
                textures[tMax].t2dName = name;
                tMax++;
            }
            IsChanged = true;
        }
        protected void UpdateMaterial()
        {
            if(material==null)
            {
                if (shader != "Default UI Material")
                {
                    var sha = Shader.Find(shader);
                    if (sha != null)
                        material = new Material(sha);
                }
            }
            if(material!=null)
            {
                for (int i = 0; i < fMax; i++)
                    material.SetFloat(floats[i].name, floats[i].value);
                for (int i = 0; i < vMax; i++)
                    material.SetVector(vectors[i].name, vectors[i].value);
                for (int i = 0; i < tMax; i++)
                {
                    var t2d = ElementAsset.FindTexture(textures[i].assetName, textures[i].t2dName);
                    material.SetTexture(textures[i].name, t2d);
                }
            }
            fMax = 0;
            vMax = 0;
            tMax = 0;
        }
        public virtual void Update()
        {
        }
    }
}
