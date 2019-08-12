using huqiang.Data;
using huqiang.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompositeHelp : MonoBehaviour
{
    [HideInInspector]
    public bool ForbidChild = false;
    // Start is called before the first frame update
    public virtual object ToBufferData(DataBuffer data)
    {
        return null;
    }
}
