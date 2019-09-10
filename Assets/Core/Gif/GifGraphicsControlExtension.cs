using UnityEngine;
using System.Collections;

public class GifGraphicsControlExtension 
{
    private GifData _gifData;

    public int disposalMethod;
    public bool transparentColorFlag;
    public int delayTime;
    public int transparentColorIndex;
    public GifImageDescriptor imageDescriptor;
    public GifImageData imageData;

    public GifGraphicsControlExtension(GifData gifData)
    {
        _gifData = gifData;
    }
}
