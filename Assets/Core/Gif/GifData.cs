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
}
