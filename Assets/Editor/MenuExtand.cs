
using huqiang.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MenuExtand
{
    [MenuItem("GameObject/CreateMeshData", priority = 0)]
    static void CreateMeshData()
    {
        var selected = Selection.activeObject;
        if (selected is GameObject)
        {
            string o_path = EditorUtility.SaveFilePanel("Save Resource", "Assets", selected.name, "bin");
            var mesh = MeshData.LoadFromGameObject((selected as GameObject).transform);
            if (File.Exists(o_path))
                File.Delete(o_path);
            var fs = File.Create(o_path);
            mesh.WriteToStream(fs);
            fs.Dispose();
        }
    }
}


