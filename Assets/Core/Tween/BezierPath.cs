using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BezierPath : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a">起点</param>
    /// <param name="b">起点方向b-a</param>
    /// <param name="c">终点方向d-c</param>
    /// <param name="d">终点</param>
    /// <param name="arrowSize">箭头尺寸</param>
    /// <param name="arrowTransform"></param>
    public static void DrawBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float arrowSize = 0.0f, Transform arrowTransform = null)
    {
        Gizmos.color = Color.green;
        Vector3 last = a;
        Vector3 p;
        Vector3 aa = (-a + 3 * (b - c) + d);
        Vector3 bb = 3 * (a + c) - 6 * b;
        Vector3 cc = 3 * (b - a);

        float t;

        if (arrowSize > 0.0f)
        {
            Vector3 beforePos = arrowTransform.position;
            Quaternion beforeQ = arrowTransform.rotation;
            float distanceTravelled = 0f;

            for (float k = 1.0f; k <= 120.0f; k+=9.9f)
            {
                t = k / 120.0f;
                p = ((aa * t + (bb)) * t + cc) * t + a;
                Gizmos.DrawLine(last, p);
                distanceTravelled += (p - last).magnitude;
                if (distanceTravelled > 1f)
                {
                    distanceTravelled = distanceTravelled - 1f;
                    arrowTransform.position = p;
                    arrowTransform.LookAt(last, Vector3.forward);
                    Vector3 to = arrowTransform.TransformDirection(Vector3.right);
                    Vector3 back = (last - p);
                    back = back.normalized;
                    Gizmos.DrawLine(p, p + (to + back) * arrowSize);
                    to = arrowTransform.TransformDirection(-Vector3.right);
                    Gizmos.DrawLine(p, p + (to + back) * arrowSize);
                }
                last = p;
            }

            arrowTransform.position = beforePos;
            arrowTransform.rotation = beforeQ;
        }
        else
        {
            for (float k = 1.0f; k <= 30.0f; k++)
            {
                t = k / 30.0f;
                p = ((aa * t + (bb)) * t + cc) * t + a;
                Gizmos.DrawLine(last, p);
                last = p;
            }
        }
    }
    [HideInInspector]
    public List<BezierPathNode> nodes = new List<BezierPathNode>();
    public void Initial()
    {
        nodes.Clear();
        for(int i=0;i<transform.childCount;i++)
        {
            var node = transform.GetChild(i).GetComponent<BezierPathNode>();
            if (node != null)
                nodes.Add(node);
        }
        if (nodes.Count < 2)
        {
            if (nodes.Count < 1)
            {
                var first = CreateNode("node");
                nodes.Add(first);
                first.transform.localPosition = Vector3.zero;
            }
            var end = CreateNode("node");
            nodes.Add(end);
            end.transform.localPosition = new Vector3(100, 100);
        }
    }
    public void Refresh()
    {
        Initial();

    }
    public void OnDrawGizmos()
    {
        Initial();
        ShowLine();
    }
    BezierPathNode CreateNode(string name)
    {
        var game = new GameObject(name, typeof(RectTransform));
        var img = game.AddComponent<Image>();
        img.color = Color.blue;
        var node = game.AddComponent<BezierPathNode>();
        node.ReFresh();
        game.transform.SetParent(transform);
        (game.transform as RectTransform).sizeDelta = new Vector2(20,20);
        return node;
    }
    public void InsertNode(int index)
    {
        Initial();
        if (index < 0)
            index = 0;
        else if (index >= nodes.Count)
            index = nodes.Count -1;
        Vector3 pos = Vector3.zero;
        if (index == 0)
        {
            pos = nodes[0].transform.localPosition;
            pos.x -= 100;
            pos.y -= 100;
        }
        else if (index == nodes.Count - 1)
        {
            pos = nodes[nodes.Count - 1].transform.localPosition;
            pos.x += 100;
            pos.y += 100;
        }
        else
        {
            var start = nodes[index].transform.localPosition;
            var end = nodes[index+1].transform.localPosition;
            pos = (end - start) * 0.5f + start;
        }
        var node = CreateNode("node");
        node.transform.SetSiblingIndex(index+1);
        node.transform.localPosition = pos;
        nodes.Insert(index, node);
    }
    public void ShowLine()
    {
        if (nodes.Count > 1)
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                DrawLine(nodes[i], nodes[i + 1]);
            }
    }
    void DrawLine(BezierPathNode start, BezierPathNode end)
    {
        var a = start.transform.position;
        var b = start.NextDir.position;
        var c = end.LastDir.position;
        var d =  end.transform.position;
        DrawBezierPath(a, b, c, d, 4, start.transform);
    }
}
