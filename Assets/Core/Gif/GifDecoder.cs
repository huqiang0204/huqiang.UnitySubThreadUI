/*  Copyright © 2016 Graeme Collins. All Rights Reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. The name of the author may not be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY GRAEME COLLINS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. */

using UnityEngine;
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
        gifData.canvasWidth = BitHelper.getInt16FromBytes(bytes, 6);
        gifData.canvasHeight = BitHelper.getInt16FromBytes(bytes, 8);
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

    static GifColor[] readColorTable(byte[] bytes, int offset, int size)
    {
        GifColor[] colorTable = new GifColor[size];

        for (int i = 0; i < size; i++)
        {
            int startIndex = offset + i * 3;
            GifColor color = new GifColor(bytes[startIndex], bytes[startIndex + 1], bytes[startIndex + 2], 1);

            colorTable[i] = color;
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
                imageData.decode();

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
        gce.delayTime = BitHelper.getInt16FromBytes(bytes, offset + 4);
        gce.transparentColorIndex = bytes[offset + 6];

        return gce;
    }

    static GifImageDescriptor readImageDescriptor(GifData gifData, byte[] bytes, int offset)
    {
        GifImageDescriptor id = new GifImageDescriptor(gifData);

        id.imageLeft = BitHelper.getInt16FromBytes(bytes, offset + 1);
        id.imageTop = BitHelper.getInt16FromBytes(bytes, offset + 3);
        id.imageWidth = BitHelper.getInt16FromBytes(bytes, offset + 5);
        id.imageHeight = BitHelper.getInt16FromBytes(bytes, offset + 7);
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

        //Debug.Log("Number of subblocks read: " + subblockCount);

        return imgData;
    }

    static List<Texture2D> createAnimator(GifData gifData)
    {
        List<Texture2D> sprites = new List<Texture2D>();
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
            Texture2D texture = new Texture2D(gifData.canvasWidth, gifData.canvasHeight);
            int transparencyIndex = graphicsControlExt.transparentColorFlag ? graphicsControlExt.transparentColorIndex : -1;

            // Determine base pixels
            if (i == 0)
            {
                texture.SetPixels(transparentFrame);
            }
            else
            {
                if (disposalMethod == 1)
                {
                    texture.SetPixels(previousFrame);
                }
                else if (disposalMethod == 2)
                {
                    texture.SetPixels(transparentFrame);
                }
                else if (disposalMethod == 3)
                {
                    throw new NotImplementedException("Disposal method 3 is not implemented.");
                }
            }

            // Set pixels from image data
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
                        GifColor gifColor = imageData.getColor(colorIndex);

                        currentFrame[pixelOffset] = new Color(gifColor.r / 255f, gifColor.g / 255f, gifColor.b / 255f);
                    }
                }
            }

            // Set texture pixels and create sprite
            texture.SetPixels(currentFrame);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            sprites.Add(texture);

            // Store current frame as previous before continuing, and reset current frame
            currentFrame.CopyTo(previousFrame, 0);
            if (disposalMethod == 0 || disposalMethod == 2)
            {
                currentFrame = new Color[currentFrame.Length];
            }
        }
        return sprites;
    }

    public class Mission
    {
        public string tag;
        public byte[] dat;
        public Action<object> CallBack;
        public GifData gifdata;
        public List<Texture2D> texture2Ds;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dat"></param>
    /// <param name="tag"></param>
    /// <param name="callback">返回Mission</param>
    public static void AsyncDecode(byte[] dat,string tag,Action<object> callback)
    {
        Mission m = new Mission();
        m.dat = dat;
        m.tag = tag;
        m.CallBack = callback;
        ThreadMission.AddMission(Decode, m, 1);
    }

    static  void Decode(object mis)
    {
        Mission m = mis as Mission;
        m.gifdata = parseGifData(m.dat);
        ThreadMission.InvokeToMain(DataToTexture,m);
    }
    static void DataToTexture(object mis)
    {
        Mission m = mis as Mission;
        m.texture2Ds= createAnimator(m.gifdata);
        if (m.CallBack != null)
            m.CallBack(m);
    }
}
