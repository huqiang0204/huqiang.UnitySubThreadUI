using UnityEngine;
using System.Collections;

public class GifImageDescriptor 
{
    private GifData _gifData;

    public int imageLeft;
    public int imageTop;
    public int imageWidth;
    public int imageHeight;
    public bool localColorTableFlag;
    public bool interlaceFlag;
    public bool sortFlag;
    public int localColorTableSize;
    public GifGraphicsControlExtension graphicsControlExt;
    public GifImageData imageData;
    public Color[] localColorTable;

    public GifImageDescriptor(GifData gifData)
    {
        _gifData = gifData;
    }
}
