
using huqiang.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MenuExtand
{
    [MenuItem("GameObject/Mesh/CreateMeshData", priority = 0)]
    static void CreateMeshData()
    {
        var selected = Selection.activeObject;
        if (selected is GameObject)
        {
            string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets", selected.name, "bin");
            if (o_path == null | o_path == "")
                return;
            var mesh = MeshData.LoadFromGameObject((selected as GameObject).transform);
            if (File.Exists(o_path))
                File.Delete(o_path);
            var fs = File.Create(o_path);
            mesh.WriteToStream(fs);
            fs.Dispose();
        }
    }
    [MenuItem("GameObject/Mesh/ReverseMeshData", priority = 1)]
    static void ReverseMeshData()
    {
        var selected = Selection.activeObject;
        var go = selected as GameObject;
        if (go != null)
        {
            var mf = go.GetComponent<MeshFilter>();
            if (mf != null)
            {
                int[] tri = mf.sharedMesh.triangles;
                int len = tri.Length / 3;
                for (int i = 0; i < len; i++)
                {
                    int index = i * 3;
                    int a = tri[index];
                    int c = tri[index + 1];
                    int b = tri[index + 2];
                    tri[index + 1] = b;
                    tri[index + 2] = c;
                }
                mf.sharedMesh.triangles = tri;
                var nor = mf.sharedMesh.normals;
                for (int i = 0; i < nor.Length; i++)
                    nor[i] = -nor[i];
                mf.sharedMesh.normals = nor;
            }
        }
    }
    [MenuItem("GameObject/Mesh/PositionCorrection", priority = 2)]
    static void PositionCorrection()
    {
        var go = Selection.activeObject as GameObject;
        if (go != null)
        {
            float lx = 0;
            float rx = 0;
            float ty = 0;
            float dy = 0;
            float fz = 0;
            float bz = 0;
            var t = go.transform;
            var c = t.childCount;
            if (c > 1)
            {
                var s = t.GetChild(0);
                lx = rx = s.localPosition.x;
                ty = dy = s.localPosition.y;
                fz = bz = s.localPosition.z;
                for (int i = 1; i < c; i++)
                {
                    var o = t.GetChild(i);
                    var p = o.localPosition;
                    if (p.x < lx)
                        lx = p.x;
                    else if (p.x > rx)
                        rx = p.x;
                    if (p.y < dy)
                        dy = p.y;
                    else if (p.y > ty)
                        ty = p.y;
                    if (p.z > fz)
                        fz = p.z;
                    else if (p.z < bz)
                        bz = p.z;
                }
            }
            Vector3 os = Vector3.zero;
            os.x = (rx - lx) * 0.5f + lx;
            os.y = (ty - dy) * 0.5f + dy;
            os.z = (fz - bz) * 0.5f + bz;
            for (int i = 0; i < c; i++)
            {
                var o = t.GetChild(i);
                var p = o.localPosition;
                p -= os;
                o.localPosition = p;
            }
        }
    }
    [MenuItem("GameObject/Mesh/AngleZero", priority = 3)]
    static void AngleZero()
    {
        var go = Selection.activeObject as GameObject;
        if (go != null)
        {
            var t = go.transform;
            var q = t.localRotation;
            int c = t.childCount;
            for (int i = 0; i < c; i++)
            {
                var o = t.GetChild(i);
                o.localRotation = q * o.localRotation;
                o.localPosition = q * o.localPosition;
            }
            t.localEulerAngles = Vector3.zero;
        }
    }
    [MenuItem("GameObject/Mesh/HorizontalCorrection", priority = 4)]
    static void HorizontalCorrection()
    {
        var gs = Selection.gameObjects;
        if (gs != null)
        {
            if (gs.Length == 3)
            {
                Vector3[] pos = new Vector3[3];
                for (int i = 0; i < 3; i++)
                {
                    var t = gs[i].transform;
                    var p = t.position;
                    var q = t.rotation;
                    var s = t.localScale;
                    var mf = t.GetComponent<MeshFilter>();
                    if (mf != null)
                    {
                        var m = mf.sharedMesh;
                        if (m != null)
                        {
                            var vert = m.vertices;
                            for (int j = 0; j < vert.Length; j++)
                            {
                                var o = p + q * vert[j];
                                if (o.y < pos[i].y)
                                    pos[i] = o;
                            }
                        }
                    }
                }
            }
        }
    }
    [MenuItem("GameObject/Mesh/SaveTranformTree", priority = 5)]
    static void SaveTranformTree()
    {
        var selected = Selection.activeObject;
        if (selected is GameObject)
        {
            string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets", selected.name, "bin");
            if (o_path == null | o_path == "")
                return;
            var mesh = TransformTree.Save((selected as GameObject).transform);
            if (File.Exists(o_path))
                File.Delete(o_path);
            File.WriteAllBytes(o_path, mesh);
        }
    }
    [MenuItem("GameObject/Mesh/LoadTranformTree", priority = 6)]
    static void LoadTranformTree()
    {
        var selected = Selection.activeObject;
        if (selected is GameObject)
        {
            string o_path = EditorUtility.OpenFilePanel("Assets", Application.dataPath, "bin");
            if (o_path == null | o_path == "")
                return;
            if (File.Exists(o_path))
            {
                TransformTree.Load((selected as GameObject).transform,File.ReadAllBytes(o_path));
            }
        }
    }
}


