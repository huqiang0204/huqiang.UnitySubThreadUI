using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class UICompositeMenu
{
    [MenuItem("GameObject/UIComposite/UISlider", false, 1)]
    static public void AddSlider(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        Debug.Log("create");
    }
    [MenuItem("GameObject/UIComposite/ScrollX", false, 2)]
    static public void AddScrollX(MenuCommand menuCommand)
    {
        Debug.Log("create");
    }
    [MenuItem("GameObject/UIComposite/ScrollY", false, 3)]
    static public void AddScrollY(MenuCommand menuCommand)
    {
        Debug.Log("create");
    }
    [MenuItem("GameObject/UIComposite/UIRocker", false, 4)]
    static public void AddRocker(MenuCommand menuCommand)
    {
        Debug.Log("create");
    }
    [MenuItem("GameObject/UIComposite/UIPalette", false, 5)]
    static public void AddPalette(MenuCommand menuCommand)
    {
        Debug.Log("create");
    }
}