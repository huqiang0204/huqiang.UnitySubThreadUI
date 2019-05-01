using huqiang.Data;
using System;
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
        RawImage Context;
        public RawImageData data;
        public string assetName;
        public string textureName;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(RawImageData*)fake.ip;
            shader = fake.buffer.GetData(data.shader) as string;
            assetName = fake.buffer.GetData(data.assetName) as string;
            textureName = fake.buffer.GetData(data.textureName) as string;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data, this);
        }
        public static void LoadToObject(Component game, ref RawImageData dat, RawImageElement image)
        {
            var a = game.GetComponent<RawImage>();
            if (a == null)
                return;
            a.uvRect = dat.uvRect;
            a.color = dat.color;
            a.raycastTarget = false;
            if (image.shader != "Default UI Material")
                a.material = new Material(Shader.Find(image.shader));
            if (image.textureName != null)
                a.texture = ElementAsset.FindTexture(image.assetName, image.textureName);
            image.Context = a;
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
                var an = ElementAsset.TxtureFormAsset(img.texture.name);
                data->assetName = buffer.AddData(an);
                data->textureName = buffer.AddData(tn);
            }
            if (img.material != null)
                data->shader = buffer.AddData(img.material.shader.name);
            return fake;
        }
    }
}
