using huqiang.Data;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace huqiang.ModelManager2D
{
    public unsafe struct SpriteRenderData
    {
        public uint renderingLayerMask;
        public int rendererPriority;
        public Int32 sortingLayerName;
        public int sortingLayerID;
        public int sortingOrder;
        public bool allowOcclusionWhenDynamic;
        public int lightmapIndex;
        public int realtimeLightmapIndex;
        public Vector4 lightmapScaleOffset;
        public Vector4 realtimeLightmapScaleOffset;
        public Int32 shaderName;
        public ReflectionProbeUsage reflectionProbeUsage;
        public LightProbeUsage lightProbeUsage;
        public bool receiveShadows;
        public MotionVectorGenerationMode motionVectorGenerationMode;
        public bool enabled;
        public ShadowCastingMode shadowCastingMode;
        public Int32 assetName;
        public Int32 textureName;
        public Int32 spriteName;
        public SpriteDrawMode drawMode;
        public Vector2 size;
        public float adaptiveModeThreshold;
        public SpriteTileMode tileMode;
        public Color color;
        public SpriteMaskInteraction maskInteraction;
        public bool flipX;
        public bool flipY;
        public SpriteSortPoint spriteSortPoint;
        public static int Size = sizeof(SpriteRenderData);
        public static int ElementSize = Size / 4;
    }
    public class SpriteRenderModel:DataConversion
    {
        SpriteRenderData data;
        public string assetsName;
        public string textureName;
        public string spriteName;
        public string sortingLayerName;
        public string shader;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(SpriteRenderData*)fake.ip;
            assetsName = fake.buffer.GetData(data.assetName) as string;
            textureName = fake.buffer.GetData(data.textureName) as string;
            spriteName = fake.buffer.GetData(data.spriteName) as string;
            sortingLayerName = fake.buffer.GetData(data.sortingLayerName) as string;
            shader = fake.buffer.GetData(data.shaderName) as string;
        }
        public override void LoadToObject(Component game)
        {
            LoadToObject(game, ref data,this);
        }
        public static void LoadToObject(Component game, ref SpriteRenderData data,SpriteRenderModel mod)
        {
            var obj = game.GetComponent<SpriteRenderer>();
            if (obj == null)
                return;
            obj.renderingLayerMask = data.renderingLayerMask;
            obj.rendererPriority = data.rendererPriority;
            obj.sortingLayerID = data.sortingLayerID;
            obj.sortingOrder = data.sortingOrder;
            obj.allowOcclusionWhenDynamic = data.allowOcclusionWhenDynamic;
            obj.lightmapIndex = data.lightmapIndex;
            obj.realtimeLightmapIndex = data.realtimeLightmapIndex;
            obj.lightmapScaleOffset = data.lightmapScaleOffset;
            obj.realtimeLightmapScaleOffset = data.realtimeLightmapScaleOffset;
            obj.reflectionProbeUsage=data.reflectionProbeUsage;
            obj.lightProbeUsage = data.lightProbeUsage;
            obj.receiveShadows = data.receiveShadows;
            obj.motionVectorGenerationMode = data.motionVectorGenerationMode;
            obj.enabled = data.enabled;
            obj.shadowCastingMode = data.shadowCastingMode;
            obj.drawMode = data.drawMode;
            obj.size = data.size;
            obj.adaptiveModeThreshold = data.adaptiveModeThreshold;
            obj.tileMode = data.tileMode;
            obj.color = data.color;
            obj.maskInteraction = data.maskInteraction;
            obj.flipX = data.flipX;
            obj.flipY = data.flipY;
            obj.spriteSortPoint = data.spriteSortPoint;
            if (mod.textureName != null)
                obj.sprite = ElementAsset.FindSprite(mod.assetsName, mod.textureName, mod.spriteName);
            else obj.sprite = null;
            obj.sortingLayerName = mod.sortingLayerName;
            if (mod.shader != null)
                obj.material = new Material(Shader.Find(mod.shader));
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var sr= com as SpriteRenderer;
            if (sr == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, SpriteRenderData.ElementSize);
            SpriteRenderData* data = (SpriteRenderData*)fake.ip;
            data->renderingLayerMask = sr.renderingLayerMask;
            data->rendererPriority = sr.rendererPriority;
            data->sortingLayerID = sr.sortingLayerID;
            data->sortingOrder = sr.sortingOrder;
            data->allowOcclusionWhenDynamic = sr.allowOcclusionWhenDynamic;
            data->lightmapIndex = sr.lightmapIndex;
            data->realtimeLightmapIndex = sr.realtimeLightmapIndex;
            data->lightmapScaleOffset = sr.lightmapScaleOffset;
            data->realtimeLightmapScaleOffset = sr.realtimeLightmapScaleOffset;
            data->reflectionProbeUsage = sr.reflectionProbeUsage;
            data->lightProbeUsage = sr.lightProbeUsage;
            data->receiveShadows = sr.receiveShadows;
            data->motionVectorGenerationMode = sr.motionVectorGenerationMode;
            data->enabled = sr.enabled;
            data->shadowCastingMode = sr.shadowCastingMode;
            data->drawMode = sr.drawMode;
            data->size = sr.size;
            data->adaptiveModeThreshold = sr.adaptiveModeThreshold;
            data->tileMode = sr.tileMode;
            data->color = sr.color;
            data->maskInteraction = sr.maskInteraction;
            data->flipX = sr.flipX;
            data->flipY = sr.flipY;
            data->spriteSortPoint = sr.spriteSortPoint;
            data->sortingLayerName = buffer.AddData(sr.sortingLayerName);
            if(sr.sprite!=null)
            {
                var tn = sr.sprite.texture.name;
                var sn = sr.sprite.name;
                var an = ElementAsset.TxtureFormAsset(sr.sprite.texture.name);
                data->assetName = buffer.AddData(an);
                data->textureName = buffer.AddData(tn);
                data->spriteName = buffer.AddData(sn);
            }
            if(sr.material!=null)
            {
                data->shaderName= buffer.AddData(sr.material.shader.name);
            }
            return fake;
        }
    }
}
