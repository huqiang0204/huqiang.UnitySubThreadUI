using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct SpriteMaskData
    {
        public Int32 assetName;
        public Int32 textureName;
        public Int32 spriteName;
        public float alphaCutoff;
        public bool isCustomRangeActive;
        public int frontSortingLayerID;
        public int frontSortingOrder;
        public int backSortingLayerID;
        public int backSortingOrder;
        public SpriteSortPoint spriteSortPoint;
        public static int Size = sizeof(SpriteMaskData);
        public static int ElementSize = Size / 4;
    }
    public class SpriteMaskLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component game)
        {
            var data = (SpriteMaskData*)fake.ip;
            var obj = game.GetComponent<SpriteMask>();
            if (obj == null)
                return;
            obj.alphaCutoff = data->alphaCutoff;
            obj.isCustomRangeActive = data->isCustomRangeActive;
            obj.frontSortingLayerID = data->frontSortingLayerID;
            obj.frontSortingOrder = data->frontSortingOrder;
            obj.backSortingLayerID = data->backSortingLayerID;
            obj.backSortingOrder = data->backSortingOrder;
            obj.spriteSortPoint = data->spriteSortPoint;
            string assetsName = fake.buffer.GetData(data->assetName) as string;
            string textureName = fake.buffer.GetData(data->textureName) as string;
            string spriteName = fake.buffer.GetData(data->spriteName) as string;
            if (textureName != null)
                obj.sprite = ElementAsset.FindSprite(assetsName, textureName, spriteName);
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var sr = com as SpriteMask;
            if (sr == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, SpriteMaskData.ElementSize);
            SpriteMaskData* data = (SpriteMaskData*)fake.ip;
            data->alphaCutoff = sr.alphaCutoff;
            data->isCustomRangeActive = sr.isCustomRangeActive;
            data->frontSortingLayerID = sr.frontSortingLayerID;
            data->frontSortingOrder = sr.frontSortingOrder;
            data->backSortingLayerID = sr.backSortingLayerID;
            data->backSortingOrder = sr.backSortingOrder;
            data->spriteSortPoint = sr.spriteSortPoint;
            if (sr.sprite != null)
            {
                var tn = sr.sprite.texture.name;
                var sn = sr.sprite.name;
                var an = ElementAsset.TxtureFormAsset(sr.sprite.texture.name);
                data->assetName = buffer.AddData(an);
                data->textureName = buffer.AddData(tn);
                data->spriteName = buffer.AddData(sn);
            }
            return fake;
        }
    }
}
