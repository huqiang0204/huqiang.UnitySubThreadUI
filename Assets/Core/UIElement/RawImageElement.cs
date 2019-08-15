using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UI
{
    public unsafe struct RawImageData
    {
        public Rect uvRect;
        public Color color;
        public Int32 material;
        public Int32 shader;
        public Int32 assetName;
        public Int32 textureName;
        public static int Size = sizeof(RawImageData);
        public static int ElementSize = Size / 4;
    }
    public class RawImageElement: GraphicE
    {
        public RawImage Context;
        public RawImageData data;
        public string assetName;
        public string textureName;
        bool textureChanged;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(RawImageData*)fake.ip;
            color = data.color;
            shader = fake.buffer.GetData(data.shader) as string;
            assetName = fake.buffer.GetData(data.assetName) as string;
            textureName = fake.buffer.GetData(data.textureName) as string;
            textureChanged = true;
        }
        public override void LoadToObject(Component game)
        {
           Context = game.GetComponent<RawImage>();
            if (Context == null)
                return;
            LoadToObject(Context, ref data, this);
        }
        public static void LoadToObject(RawImage raw, ref RawImageData dat, RawImageElement image)
        {
            raw.uvRect = dat.uvRect;
            raw.color = image._color;
            raw.raycastTarget = false;
            if (image.shader != "Default UI Material")
            {
                Shader sha = Shader.Find(image.shader);
                if (sha != null)
                    raw.material = new Material(sha);
            }
            image.Context = raw;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var img = com as RawImage;
            if (img == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, RawImageData.ElementSize);
            RawImageData* data = (RawImageData*)fake.ip;
            data->uvRect = img.uvRect;
            data->color = img.color;
            if (img.texture != null)
            {
                var tn = img.texture.name;
                if(tn!="")
                {
                    var an = ElementAsset.TxtureFormAsset(img.texture.name);
                    data->assetName = buffer.AddData(an);
                    data->textureName = buffer.AddData(tn);
                }
            }
            if (img.material != null)
                data->shader = buffer.AddData(img.material.shader.name);
            return fake;
        }
        public void ChangeTexture(string tName)
        {
            textureName = tName;
            textureChanged = true;
        }
        public void ChangeTexture(string tName, string aName)
        {
            assetName = aName;
            textureName = tName;
            textureChanged = true;
        }
        public override void Apply()
        {
            if(IsChanged)
            {
                UpdateMaterial();
                if(Context!=null)
                {
                    Context.uvRect = data.uvRect;
                    Context.color = _color;
                }
                IsChanged = false;
            }
            if(textureChanged)
            {
                textureChanged = false;
                if(Context!=null)
                (Context as RawImage).texture = ElementAsset.FindTexture(assetName, textureName);
            }
        }
    }
}
