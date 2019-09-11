using UnityEngine;
using System.Collections.Generic;

public class GifData
{
    public string header;
    public int canvasWidth;
    public int canvasHeight;
    public bool globalColorTableFlag;
    public int bitsPerPixel;
    public bool sortFlag;
    public int globalColorTableSize;
    public int backgroundColorIndex;
    public int pixelAspectRatio;
    public Color[] globalColorTable;

    public List<GifGraphicsControlExtension> graphicsControlExtensions;
    public List<GifImageDescriptor> imageDescriptors;
    public List<GifImageData> imageDatas;

    public GifData()
    {
        graphicsControlExtensions = new List<GifGraphicsControlExtension>();
        imageDescriptors = new List<GifImageDescriptor>();
        imageDatas = new List<GifImageData>();
    }
    public Color[] previousFrame;
    public Color[] currentFrame;
    public Color[] transparentFrame;
    static void CalculColors(GifData gifData,int index)
    {
        Color[] previousFrame = gifData.previousFrame;
        if (previousFrame == null)
            previousFrame = new Color[gifData.canvasWidth * gifData.canvasHeight];
        Color[] currentFrame = gifData.currentFrame;
        if (currentFrame == null)
            currentFrame = new Color[gifData.canvasWidth * gifData.canvasHeight];
        Color[] transparentFrame = gifData.transparentFrame;
        if (transparentFrame == null)
            transparentFrame = new Color[gifData.canvasWidth * gifData.canvasHeight];

        GifGraphicsControlExtension graphicsControlExt = gifData.graphicsControlExtensions[index];
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
                    currentFrame[pixelOffset] = colorTabel[colorIndex];
                }
            }
        }

        currentFrame.CopyTo(previousFrame, 0);
        if (disposalMethod == 0 || disposalMethod == 2)
        {
            currentFrame = new Color[currentFrame.Length];
            imageData.colors = currentFrame;
        }
        else
        {
            imageData.colors = new Color[currentFrame.Length];
            currentFrame.CopyTo(imageData.colors, 0);
        }
        gifData.previousFrame = previousFrame;
        gifData.currentFrame = currentFrame;
        gifData.transparentFrame = transparentFrame;
    }
    void Decode(int index)
    {
        var dat = graphicsControlExtensions[index].imageData;
        dat.decode();
        CalculColors(this, index);
    }
    int decodeCount = 0;
    public bool DecodeNext()
    {
        if(decodeCount<graphicsControlExtensions.Count)
        {
            Decode(decodeCount);
            decodeCount++;
            return true;
        }
        return false;
    }
    public List<Texture2D> textures;
    int createCount = 0;
    public void CreateNextTexture()
    {
        if (textures == null)
            textures = new List<Texture2D>();
        int c = decodeCount;
        for (int i = createCount; i < c; i++)
        {
            GifGraphicsControlExtension graphicsControlExt = graphicsControlExtensions[i];
            GifImageDescriptor imageDescriptor = graphicsControlExt.imageDescriptor;
            GifImageData imageData = imageDescriptor.imageData;

            Texture2D texture = new Texture2D(canvasWidth, canvasHeight);
            texture.SetPixels(imageData.colors);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            textures.Add(texture);
        }
        createCount = c;
    }
}
