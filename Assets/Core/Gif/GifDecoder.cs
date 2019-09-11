﻿using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using huqiang;

public class GifDecoder
{

    static GifData parseGifData(byte[] bytes)
    {
        GifData gifData = new GifData();

        gifData.header = Encoding.UTF8.GetString(bytes, 0, 6);
        gifData.canvasWidth = bytes.ReadInt16(6); //BitHelper.getInt16FromBytes(bytes, 6);
        gifData.canvasHeight = bytes.ReadInt16(8); //BitHelper.getInt16FromBytes(bytes, 8);
        gifData.globalColorTableFlag = BitHelper.getIntFromPackedByte(bytes[10], 0, 1) == 1;
        gifData.bitsPerPixel = BitHelper.getIntFromPackedByte(bytes[10], 1, 4) + 1;
        gifData.sortFlag = BitHelper.getIntFromPackedByte(bytes[10], 4, 5) == 1;
        gifData.globalColorTableSize = BitHelper.getIntFromPackedByte(bytes[10], 5, 8);
        gifData.backgroundColorIndex = bytes[11];
        gifData.pixelAspectRatio = bytes[12];

        if (gifData.globalColorTableFlag)
        {
            gifData.globalColorTable = readColorTable(bytes, 13, 1 << (gifData.globalColorTableSize + 1));
        }
        readBlocks(gifData, bytes);
        return gifData;
    }

    static Color[] readColorTable(byte[] bytes, int offset, int size)
    {
        Color[] colorTable = new Color[size];

        for (int i = 0; i < size; i++)
        {
            int startIndex = offset + i * 3;
            colorTable[i].r = ((float)bytes[startIndex])/255;
            colorTable[i].g = ((float)bytes[startIndex+1])/255;
            colorTable[i].b = ((float)bytes[startIndex+2])/255;
            colorTable[i].a = 1;
        }

        return colorTable;
    }

    static void readBlocks(GifData gifData, byte[] bytes)
    {
        int currentOffset = 13;

        while (true)
        {
            // Stop at last byte
            if (currentOffset == bytes.Length - 1)
            {
                break;
            }

            // Search for graphics control extensions
            if (bytes[currentOffset] == 0x21 && bytes[currentOffset + 1] == 0xF9 && bytes[currentOffset + 7] == 0x00)
            {
                GifGraphicsControlExtension graphicsControlExt;
                GifImageDescriptor imageDescriptor;
                GifImageData imageData;

                // Graphics control extension
                graphicsControlExt = readGraphicsControlExtension(gifData, bytes, currentOffset);
                gifData.graphicsControlExtensions.Add(graphicsControlExt);
                currentOffset += 8;

                // Image descriptor
                imageDescriptor = readImageDescriptor(gifData, bytes, currentOffset);
                gifData.imageDescriptors.Add(imageDescriptor);
                currentOffset += 10;

                // Local color table
                if (imageDescriptor.localColorTableFlag)
                {
                    int colorTableSize = 1 << (imageDescriptor.localColorTableSize + 1);

                    imageDescriptor.localColorTable = readColorTable(bytes, currentOffset, colorTableSize);
                    currentOffset += 3 * colorTableSize;
                }

                // Image data

                imageData = readImageData(gifData, bytes, currentOffset);

                gifData.imageDatas.Add(imageData);

                // Connect graphics control extension, image descriptor, and image data
                graphicsControlExt.imageData = imageData;
                graphicsControlExt.imageDescriptor = imageDescriptor;
                imageData.graphicsControlExt = graphicsControlExt;
                imageData.imageDescriptor = imageDescriptor;
                imageDescriptor.graphicsControlExt = graphicsControlExt;
                imageDescriptor.imageData = imageData;

                // Decode image data
                //imageData.decode();
                // Advance
                currentOffset = imageData.endingOffset;
            }
            else
            {
                currentOffset++;
            }
        }
    }

    static GifGraphicsControlExtension readGraphicsControlExtension(GifData gifData, byte[] bytes, int offset)
    {
        GifGraphicsControlExtension gce = new GifGraphicsControlExtension(gifData);

        gce.disposalMethod = BitHelper.getIntFromPackedByte(bytes[offset + 3], 3, 6);
        gce.transparentColorFlag = BitHelper.getIntFromPackedByte(bytes[offset + 3], 7, 8) == 1;
        gce.delayTime = bytes.ReadInt16(offset+4); //BitHelper.getInt16FromBytes(bytes, offset + 4);
        gce.transparentColorIndex = bytes[offset + 6];

        return gce;
    }

    static GifImageDescriptor readImageDescriptor(GifData gifData, byte[] bytes, int offset)
    {
        GifImageDescriptor id = new GifImageDescriptor(gifData);

        id.imageLeft = bytes.ReadInt16(offset + 1); //BitHelper.getInt16FromBytes(bytes, offset + 1);
        id.imageTop = bytes.ReadInt16(offset + 3);  //BitHelper.getInt16FromBytes(bytes, offset + 3);
        id.imageWidth = bytes.ReadInt16(offset + 5);  //BitHelper.getInt16FromBytes(bytes, offset + 5);
        id.imageHeight = bytes.ReadInt16(offset + 7); //BitHelper.getInt16FromBytes(bytes, offset + 7);
        id.localColorTableFlag = BitHelper.getIntFromPackedByte(bytes[offset + 9], 0, 1) == 1;
        id.interlaceFlag = BitHelper.getIntFromPackedByte(bytes[offset + 9], 1, 2) == 1;
        id.sortFlag = BitHelper.getIntFromPackedByte(bytes[offset + 9], 2, 3) == 1;
        id.localColorTableSize = BitHelper.getIntFromPackedByte(bytes[offset + 9], 5, 8);

        // Interlace flag
        if (id.interlaceFlag)
        {
            throw new NotImplementedException("Use of interlace flag not implemented.");
        }

        return id;
    }

    static GifImageData readImageData(GifData gifData, byte[] bytes, int offset)
    {
        GifImageData imgData = new GifImageData(gifData);
        int subblockOffset = offset + 1;
        int subblockCount = 0;

        imgData.lzwMinimumCodeSize = bytes[offset];

        // Read subblock data
        while (true)
        {
            int subblockSize = bytes[subblockOffset];

            if (subblockSize == 0)
            {
                break;
            }
            else
            {
                for (int i = 0; i < subblockSize; i++)
                {
                    imgData.blockBytes.Add(bytes[subblockOffset + 1 + i]);
                }

                subblockOffset += subblockSize + 1;
                subblockCount++;
            }
        }
        imgData.endingOffset = subblockOffset;
        return imgData;
    }

    static void CalculColors(GifData gifData)
    {
        Color[] previousFrame = new Color[gifData.canvasWidth * gifData.canvasHeight];
        Color[] currentFrame = new Color[gifData.canvasWidth * gifData.canvasHeight];
        Color[] transparentFrame = new Color[gifData.canvasWidth * gifData.canvasHeight];

        // Create sprites
        for (int i = 0; i < gifData.graphicsControlExtensions.Count; i++)
        {
            GifGraphicsControlExtension graphicsControlExt = gifData.graphicsControlExtensions[i];
            GifImageDescriptor imageDescriptor = graphicsControlExt.imageDescriptor;
            GifImageData imageData = imageDescriptor.imageData;
            int top = imageDescriptor.imageTop;
            int left = imageDescriptor.imageLeft;
            int disposalMethod = graphicsControlExt.disposalMethod;

            int transparencyIndex = graphicsControlExt.transparentColorFlag ? graphicsControlExt.transparentColorIndex : -1;
            Color[] colorTabel = imageData.imageDescriptor.localColorTableFlag ? imageData.imageDescriptor.localColorTable : gifData.globalColorTable;

            for (int j = 0; j < imageDescriptor.imageWidth; j++)
            {
                for (int k = 0; k < imageDescriptor.imageHeight; k++)
                {
                    int x = left + j;
                    int y = (gifData.canvasHeight - 1) - (top + k);
                    int colorIndex = imageData.colorIndices[j + k * imageDescriptor.imageWidth];
                    int pixelOffset = x + y * gifData.canvasWidth;

                    if (colorIndex != transparencyIndex)
                    {
                        currentFrame[pixelOffset] = colorTabel[colorIndex];//imageData.getColor(colorIndex);
                    }
                }
            }
            // Set texture pixels and create sprite
            // Store current frame as previous before continuing, and reset current frame
            currentFrame.CopyTo(previousFrame, 0);
            if (disposalMethod == 0 || disposalMethod == 2)
            {
                currentFrame = new Color[currentFrame.Length];
                imageData.colors = currentFrame;
            }
            else
            {
                imageData.colors = new Color[currentFrame.Length];
                currentFrame.CopyTo(imageData.colors,0);
            }
        }
    }
    static List<Texture2D> CreateAnimator(GifData gifData)
    {
        List<Texture2D> sprites = new List<Texture2D>();
        // Create sprites
        for (int i = 0; i < gifData.graphicsControlExtensions.Count; i++)
        {
            GifGraphicsControlExtension graphicsControlExt = gifData.graphicsControlExtensions[i];
            GifImageDescriptor imageDescriptor = graphicsControlExt.imageDescriptor;
            GifImageData imageData = imageDescriptor.imageData;

            Texture2D texture = new Texture2D(gifData.canvasWidth, gifData.canvasHeight);
    
            // Set texture pixels and create sprite
            texture.SetPixels(imageData.colors);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            sprites.Add(texture);
        }
        return sprites;
    }
    public class Mission
    {
        public string tag;
        public byte[] dat;
        public int count;
        public Action<Mission> CallBack;
        public GifData gifdata;
        public List<Texture2D> texture2Ds;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dat"></param>
    /// <param name="tag"></param>
    /// <param name="callback">返回Mission</param>
    public static void AsyncDecode(byte[] dat,string tag,Action<Mission> callback)
    {
        Mission m = new Mission();
        m.dat = dat;
        m.tag = tag;
        m.CallBack = callback;
        ThreadMission.AddMission(Decode, m, "gif");
    }

    static  void Decode(object mis)
    {
        Mission m = mis as Mission;
        m.gifdata = parseGifData(m.dat);
        m.count = m.gifdata.graphicsControlExtensions.Count;
        while(m.gifdata.DecodeNext())
        {
            //ThreadMission.AddMission();
            ThreadMission.InvokeToMain(DataToTexture,m);
        }
        //CalculColors(m.gifdata);
        //ThreadMission.InvokeToMain(DataToTexture,m);
    }
    static void DataToTexture(object mis)
    {
        Mission m = mis as Mission;
        m.gifdata.CreateNextTexture();

        ThreadMission.AddMission((o) =>
        {
            if (m.CallBack != null)
                m.CallBack(m);
        }, mis, "UI");
    }
}
