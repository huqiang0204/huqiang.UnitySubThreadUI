using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Data
{
    public class SpriteData
    {
        class SpriteCategory
        {
            public string txtName;
            public List<Sprite> sprites = new List<Sprite>();
        }
        public unsafe struct SpriteDataS
        {
            public Int32 name;
            public Vector2 txtSize;
            public Rect rect;
            public Vector2 pivot;
            public static int Size = sizeof(SpriteDataS);
            public static int ElementSize = Size / 4;
        }
        List<SpriteCategory> lsc = new List<SpriteCategory>();
        public void AddSprite(Sprite sprite)
        {
            if (sprite == null)
                return;
            string tname = sprite.texture.name;
            for (int i = 0; i < lsc.Count; i++)
            {
                if (tname == lsc[i].txtName)
                {
                    lsc[i].sprites.Add(sprite);
                    return;
                }
            }
            SpriteCategory category = new SpriteCategory();
            category.txtName = tname;
            category.sprites.Add(sprite);
            lsc.Add(category);
        }
        FakeStructArray SaveCategory(DataBuffer buffer)
        {
            FakeStructArray array = new FakeStructArray(buffer, 2, lsc.Count);
            for (int i=0;i<lsc.Count;i++)
            {
                array.SetData(i,0,lsc[i].txtName);
                array.SetData(i,1,SaveSprites(buffer,lsc[i].sprites));
            }
            return array;
        }
        unsafe FakeStructArray SaveSprites(DataBuffer buffer,List<Sprite> sprites)
        {
            FakeStructArray array = new FakeStructArray(buffer, SpriteDataS.ElementSize, sprites.Count);
            float tx = sprites[0].texture.width;
            float ty = sprites[0].texture.height;
            for (int i = 0; i < sprites.Count; i++)
            {
                var sprite = sprites[i];
                string name = sprite.name;
                SpriteDataS* sp = (SpriteDataS*)array[i];
                sp->name = buffer.AddData(name);
                sp->txtSize.x = tx;
                sp->txtSize.y = ty;
                sp->rect = sprite.rect;
                sp->pivot = sprite.pivot;
            }
            return array;
        }
        public void Save(string name,string path)
        {
            DataBuffer buffer = new DataBuffer(4096);
            var fs = buffer.fakeStruct = new FakeStruct(buffer, 2);
            fs.SetData(0, name);
            fs.SetData(1, SaveCategory(buffer));
            byte[] dat = buffer.ToBytes();
            File.WriteAllBytes(path,dat);
        }
        public void Clear()
        {
            lsc.Clear();
        }
        FakeStruct fakeStruct;
        public void LoadSpriteData(byte[] data)
        {
            fakeStruct = new DataBuffer(data).fakeStruct;
        }
    }
}
