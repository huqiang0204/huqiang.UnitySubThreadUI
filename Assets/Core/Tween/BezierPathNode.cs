using huqiang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BezierPathNode:MonoBehaviour
{
    public RectTransform LastDir;
    public RectTransform NextDir;
    public void ReFresh()
    {
        if (LastDir == null)
            CreateLastDir();
        if (NextDir == null)
            CreateNextDir();
    }
    void CreateLastDir()
    {
        LastDir = CreateObject("Last").transform as RectTransform;
        LastDir.SetParent(transform);
        LastDir.sizeDelta = new Vector2(10, 10);
        var pos = transform.localPosition;
        pos.y -= 60;
        LastDir.localPosition = pos;
    }
    void CreateNextDir()
    {
        NextDir = CreateObject("Next").transform as RectTransform;
        NextDir.SetParent(transform);
        NextDir.sizeDelta = new Vector2(10, 10);
        var pos = transform.localPosition;
        pos.x += 60;
        NextDir.localPosition = pos;
    }
    GameObject CreateObject(string name)
    {
        GameObject game = new GameObject(name,typeof(RectTransform));
        var img = game.AddComponent<Image>();
        img.color = Color.green;
        return game;
    }
    private void OnDrawGizmos()
    {
        if (LastDir != null)
            Gizmos.DrawLine(transform.position, LastDir.position);
        if(NextDir!=null)
            Gizmos.DrawLine(transform.position, NextDir.position);
    }
}
