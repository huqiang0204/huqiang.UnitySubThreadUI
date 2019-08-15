using huqiang.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UI
{
    public unsafe struct ImageData
    {
        public float alphaHit;
        public float fillAmount;
        public bool fillCenter;
        public bool fillClockwise;
        public Image.FillMethod fillMethod;
        public Int32 fillOrigin;
        public bool preserveAspect;
        public Image.Type type;
        public Int32 shader;
        public Color color;
        public Int32 assetName;
        public Int32 textureName;
        public Int32 spriteName;
        public static int Size = sizeof(ImageData);
        public static int ElementSize = Size / 4;
    }
    public class ImageElement : GraphicE
    {
        public Image Context;
        public ImageData data;
        public string assetName;
        public string textureName;
        string mSprite;
        bool msChanged;
        Sprite _sprite;
        public Sprite sprite { set { _sprite = value;IsChanged = true; } }
        public string spriteName {
            get { return mSprite; }
            set { mSprite = value;
                msChanged = true; }
        }
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(ImageData*)fake.ip;
            color = data.color;
            shader= fake.buffer.GetData(data.shader) as string;
            assetName = fake.buffer.GetData(data.assetName) as string;
            textureName = fake.buffer.GetData(data.textureName) as string;
            spriteName = fake.buffer.GetData(data.spriteName) as string;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data,this);
        }
        public static void LoadToObject(Component game, ref ImageData dat,ImageElement image)
        {
            var a = game.GetComponent<Image>();
            if (a== null)
                return;
            LoadToObject(a,ref dat,image);
        }
        public static void LoadToObject(Image a, ref ImageData dat, ImageElement image)
        {
            a.alphaHitTestMinimumThreshold = dat.alphaHit;
            a.fillAmount = dat.fillAmount;
            a.fillCenter = dat.fillCenter;
            a.fillClockwise = dat.fillClockwise;
            a.fillMethod = dat.fillMethod;
            a.fillOrigin = dat.fillOrigin;
            a.preserveAspect = dat.preserveAspect;
            a.type = dat.type;
            a.raycastTarget = false;
            a.color = image.color;
            image.Context = a;
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var img = com as Image;
            if (img == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, ImageData.ElementSize);
            ImageData* data = (ImageData*)fake.ip;
            data->alphaHit = img.alphaHitTestMinimumThreshold;
            data->fillAmount = img.fillAmount;
            data->fillCenter = img.fillCenter;
            data->fillClockwise = img.fillClockwise;
            data->fillMethod = img.fillMethod;
            data->fillOrigin = img.fillOrigin;
            data->preserveAspect = img.preserveAspect;
            data->type = img.type;
            data->color = img.color;
            if (img.sprite != null)
            {
                var tn = img.sprite.texture.name;
                var sn = img.sprite.name;
                var an = ElementAsset.TxtureFormAsset(img.sprite.texture.name);
                data->assetName = buffer.AddData(an);
                data->textureName = buffer.AddData(tn);
                data->spriteName = buffer.AddData(sn);
            }
            if (img.material != null)
                data->shader= buffer.AddData(img.material.shader.name);
            return fake;
        }
        public void SetSprite(string sName)
        {
            mSprite = sName;
            msChanged = true;
        }
        public void SetSprite(string sName, string tName)
        {
            textureName = tName;
            mSprite = sName;
            msChanged = true;
        }
        public void SetSprite(string sName, string tName, string aName)
        {
            assetName = aName;
            textureName = tName;
            mSprite = sName;
            msChanged = true;
        }
        public override void Apply()
        {
            if(IsChanged)
            {
                UpdateMaterial();
                LoadToObject(Context, ref data, this);
                IsChanged = false;
                Context.enabled = true;
            }
            if(msChanged)
            {
                msChanged = false;
                var a = Context as Image;
                a.material = material;
                if (mSprite == null)
                    a.sprite = null;
                else a.sprite = ElementAsset.FindSprite(assetName, textureName, spriteName);
            }
        }
    }
}
