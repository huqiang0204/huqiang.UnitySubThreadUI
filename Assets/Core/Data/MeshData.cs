using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public struct Coordinate
    {
        public Vector3 pos;
        public Vector3 scale;
        public Quaternion quat;
        public Color color;
        public unsafe static Int32 Size { get { return sizeof(Coordinate); } }
    }
    public class MeshDataType
    {
        public const Int32 Coordinate = -1;
        public const Int32 Name = 0;
        public const Int32 Vertices = 1;
        public const Int32 Normals = 2;
        public const Int32 UV = 3;
        public const Int32 Triangles = 4;
        public const Int32 Child = 5;
        public const Int32 Type = 6;
        public const Int32 Materials = 7;
        public const Int32 SubTriangles = 8;
    }
    public class MeshData
    {
        public static Color DefColor = new Color(0.5f, 0.5f, 0.5f, 1);
        public static byte[] Zreo = new byte[4];
        public MeshData[] child;
        public int subMeshCount;
        public string name;
        public Vector3[] vertex;
        public Vector2[] uv;
        public Vector3[] normals;
        public Int32[] tris;
        public Int32[][] subtris;
        public Coordinate coordinate;
        public string materials;
        public MeshData()
        {
            coordinate.scale = Vector3.one;
            coordinate.quat.w = 1;
            coordinate.color = DefColor;
        }

        public unsafe byte* LoadFromBytes(byte* bp)
        {
            int seg = *(Int32*)bp;
            bp += 4;
            for (int i = 0; i < seg; i++)
            {
                int tag = *(Int32*)bp;
                bp += 4;
                switch (tag)
                {
                    case MeshDataType.Type:
                        subMeshCount = *(Int32*)bp;
                        bp += 4;
                        break;
                    case MeshDataType.Coordinate:
                        int c = *(Int32*)bp;
                        bp += 4;
                        ReadCoordinate(bp, c);
                        bp += c;
                        break;
                    case MeshDataType.Name:
                        c = *(Int32*)bp;
                        bp += 4;
                        name = Encoding.UTF8.GetString(Tool.GetByteArray(bp, c));
                        bp += c;
                        break;
                    case MeshDataType.Vertices:
                        vertex = ReadVectors(bp);
                        if (vertex != null)
                            bp += vertex.Length * 12;
                        bp += 4;
                        break;
                    case MeshDataType.Normals:
                        normals = ReadVectors(bp);
                        if (normals != null)
                            bp += normals.Length * 12;
                        bp += 4;
                        break;
                    case MeshDataType.UV:
                        uv = ReadUV(bp);
                        if (uv != null)
                            bp += uv.Length * 8;
                        bp += 4;
                        break;
                    case MeshDataType.Triangles:
                        tris = ReadTri(bp);
                        if (tris != null)
                            bp += tris.Length * 4;
                        bp += 4;
                        break;
                    case MeshDataType.Child:
                        bp = ReadChild(bp, this);
                        break;
                    case MeshDataType.Materials:
                        c = *(Int32*)bp;
                        bp += 4;
                        materials = Encoding.UTF8.GetString(Tool.GetByteArray(bp, c));
                        bp += c;
                        break;
                    case MeshDataType.SubTriangles:
                        c = *(Int32*)bp;
                        bp += 4;
                        subtris = ReadTris(bp);
                        bp += c;
                        break;
                }
            }
            return bp;
        }
        unsafe static Vector3[] ReadVectors(byte* bp)
        {
            int len = *(Int32*)bp;
            bp += 4;
            if (len > 0)
            {
                len /= 12;
                Vector3* p = (Vector3*)bp;
                var buf = new Vector3[len];
                for (int i = 0; i < len; i++)
                {
                    buf[i] = *p;
                    p++;
                }
                return buf;
            }
            return null;
        }
        unsafe static Vector2[] ReadUV(byte* bp)
        {
            int len = *(Int32*)bp;
            bp += 4;
            if (len > 0)
            {
                len /= 8;
                Vector2* p = (Vector2*)bp;
                var buf = new Vector2[len];
                for (int i = 0; i < len; i++)
                {
                    buf[i] = *p;
                    p++;
                }
                return buf;
            }
            return null;
        }
        unsafe void ReadCoordinate(byte* bp, int c)
        {
            float* tp = (float*)bp;
            fixed (Coordinate* cp = &coordinate)
            {
                int len = Coordinate.Size;
                if (len < c)
                    len = c;
                len /= 4;
                float* fp = (float*)cp;
                for (int i = 0; i < len; i++)
                {
                    *fp = *tp;
                    fp++;
                    tp++;
                }
            };
        }
        unsafe static int[] ReadTri(byte* bp)
        {
            var len = *(Int32*)bp;
            bp += 4;
            if (len > 0)
            {
                len /= 4;
                Int32* p = (Int32*)bp;
                var buf = new int[len];
                for (int i = 0; i < len; i++)
                {
                    buf[i] = *p;
                    p++;
                }
                return buf;
            }
            return null;
        }
        unsafe static int[][] ReadTris(byte* bp)
        {
            var len = *(Int32*)bp;
            int[][] tris = new int[len][];
            bp += 4;
            for(int i=0;i<len;i++)
            {
                int l = *(Int32*)bp;
                bp += 4;
                if (l > 0)
                {
                    Int32* p = (Int32*)bp;
                    bp += l;
                    l /= 4;
                    var buf = new int[l];
                    for (int j = 0; j < l; j++)
                    {
                        buf[j] = *p;
                        p++;
                    }
                    tris[i] = buf;
                }
            }
            return tris;
        }
        unsafe static byte* ReadChild(byte* bp, MeshData mesh)
        {
            var len = *(Int32*)bp;
            bp += 4;
            if (len > 0)
            {
                mesh.child = new MeshData[len];
                for (int i = 0; i < len; i++)
                {
                    MeshData sub = new MeshData();
                    mesh.child[i] = sub;
                    bp = sub.LoadFromBytes(bp);
                }
            }
            return bp;
        }

        int GetSegment()
        {
            int seg = 2;
            if (name != null)
                seg++;
            if (vertex != null)
                seg++;
            if (uv != null)
                seg++;
            if (normals != null)
                seg++;
            if (tris != null)
                seg++;
            if (child != null)
                seg++;
            if (materials != null)
                seg++;
            if (subtris != null)
                seg++;
            return seg;
        }
        public void WriteToStream(Stream stream)
        {
            Int32 seg = GetSegment();
            var tmp = seg.ToBytes();
            stream.Write(tmp, 0, 4);
            stream.Write((MeshDataType.Type).ToBytes(), 0, 4);
            stream.Write(subMeshCount.ToBytes(), 0, 4);
            unsafe
            {
                fixed (Coordinate* coor = &coordinate)
                {
                    WriteCoordinate(stream, coor);
                }
            }
            if (name != null)
                WriteString(stream, MeshDataType.Name, name);
            if (materials != null)
                WriteString(stream,MeshDataType.Materials, materials);
            if (vertex != null)
            {
                tmp = (MeshDataType.Vertices).ToBytes();
                stream.Write(tmp, 0, 4);
                WriteVectors(stream, vertex);
            }
            if (normals != null)
            {
                tmp = (MeshDataType.Normals).ToBytes();
                stream.Write(tmp, 0, 4);
                WriteVectors(stream, normals);
            }
            if (uv != null)
                WriteUV(stream, uv);
            if (tris != null)
                WriteTri(stream, tris);
            if (child != null)
                WriteChild(stream, child);
            if (subtris != null)
                WriteSubTri(stream,subtris);
        }
        public void WriteToFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            var fs = File.Create(path);
            WriteToStream(fs);
            fs.Dispose();
        }
        unsafe static void WriteCoordinate(Stream stream, Coordinate* coordinate)
        {
            var tmp = (MeshDataType.Coordinate).ToBytes();
            stream.Write(tmp, 0, 4);
            int size = Coordinate.Size;
            stream.Write(size.ToBytes(), 0, 4);
            byte[] buf = new byte[size];
            byte* fp = (byte*)coordinate;
            for (int i = 0; i < size; i++)
            {
                buf[i] = *fp;
                fp++;
            }
            stream.Write(buf, 0, size);
        }
        static void WriteString(Stream stream, int type, string name)
        {
            var tmp = type.ToBytes();
            stream.Write(tmp, 0, 4);
            var buf = Encoding.UTF8.GetBytes(name);
            Int32 len = buf.Length;
            stream.Write(len.ToBytes(), 0, 4);
            stream.Write(buf, 0, len);
        }
        static void WriteVectors(Stream stream, Vector3[] vectors)
        {
            int len = vectors.Length * 12;
            IntPtr v = Marshal.UnsafeAddrOfPinnedArrayElement(vectors, 0);
            var buf = new byte[len];
            Marshal.Copy(v, buf, 0, len);
            stream.Write(len.ToBytes(), 0, 4);
            stream.Write(buf, 0, buf.Length);
        }
        static void WriteUV(Stream stream, Vector2[] uvs)
        {
            var tmp = (MeshDataType.UV).ToBytes();
            stream.Write(tmp, 0, 4);
            int len = uvs.Length * 8;
            IntPtr v = Marshal.UnsafeAddrOfPinnedArrayElement(uvs, 0);
            var buf = new byte[len];
            Marshal.Copy(v, buf, 0, len);
            stream.Write(len.ToBytes(), 0, 4);
            stream.Write(buf, 0, buf.Length);
        }
        static void WriteTri(Stream stream, int[] tri)
        {
            var tmp = (MeshDataType.Triangles).ToBytes();
            stream.Write(tmp, 0, 4);
            int len = tri.Length * 4;
            IntPtr v = Marshal.UnsafeAddrOfPinnedArrayElement(tri, 0);
            var buf = new byte[len];
            Marshal.Copy(v, buf, 0, len);
            stream.Write(len.ToBytes(), 0, 4);
            stream.Write(buf, 0, buf.Length);
        }
        /// <summary>
        /// 4字节标记,4字节总长度,4字节数组长度,数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tris"></param>
        static void WriteSubTri(Stream stream, int[][] tris)
        {
            var tmp = (MeshDataType.SubTriangles).ToBytes();
            stream.Write(tmp, 0, 4);
            int len = tris.Length;
            int all = 0;
            for (int i = 0; i < len; i++)
            {
                all += tris[i].Length * 4 + 4;
            }
            all += 4;
            stream.Write(all.ToBytes(), 0, 4);
            stream.Write(len.ToBytes(), 0, 4);
            for (int i = 0; i < len; i++)
            {
                var tri = tris[i];
                var l = tri.Length * 4;
                IntPtr v = Marshal.UnsafeAddrOfPinnedArrayElement(tri, 0);
                var buf = new byte[l];
                Marshal.Copy(v, buf, 0, l);
                stream.Write(l.ToBytes(), 0, 4);
                stream.Write(buf, 0, buf.Length);
            }
        }
        static void WriteChild(Stream stream, MeshData[] meshes)
        {
            var tmp = (MeshDataType.Child).ToBytes();
            stream.Write(tmp, 0, 4);
            int len = meshes.Length;
            tmp = len.ToBytes();
            stream.Write(tmp, 0, 4);
            for (int i = 0; i < len; i++)
            {
                meshes[i].WriteToStream(stream);
            }
        }

        public GameObject AssociatedObject;
        public GameObject CreateGameObject(Action<MeshData,GameObject> action = null)
        {
            GameObject game = new GameObject(name);
            AssociatedObject = game;
            if (vertex != null)
            {
                game.AddComponent<MeshRenderer>();
                var mf = game.AddComponent<MeshFilter>();
                var ms = mf.mesh;
                ms.vertices = vertex;
                if (normals != null)
                    ms.normals = normals;
                if (tris != null)
                    ms.triangles = tris;
                else if(subtris!=null)
                {
                    ms.subMeshCount = subMeshCount;
                    for (int i = 0; i < subMeshCount; i++)
                        ms.SetTriangles(subtris[i],i);
                }
                else
                {
                    tris = new int[vertex.Length];
                    for (int i = 0; i < tris.Length; i++)
                        tris[i] = i;
                    ms.triangles = tris;
                }
            }
            if (child != null)
                for (int i = 0; i < child.Length; i++)
                {
                    var ch = child[i];
                    var son = ch.CreateGameObject(action);
                    var tran = son.transform;
                    tran.SetParent(game.transform);
                    tran.localPosition = ch.coordinate.pos;
                    tran.localScale = ch.coordinate.scale;
                    tran.localRotation = ch.coordinate.quat;
                    if (action != null)
                        action(ch,son);
                }
            return game;
        }
        public void CreateNormal()
        {
            if (vertex != null)
                if (tris != null)
                {
                    normals = new Vector3[vertex.Length];
                    int c = tris.Length / 3;
                    for (int i = 0; i < c; i++)
                    {
                        int s = i * 3;
                        var nor = MathH.GetTriangleNormal(vertex[tris[s]], vertex[tris[s + 1]], vertex[tris[s + 2]]);
                        normals[s] = nor;
                        normals[s + 1] = nor;
                        normals[s + 2] = nor;
                    }
                }
        }
        public static MeshData LoadFromGameObject(Transform game, Action<MeshData, GameObject> action=null)
        {
            var mesh = new MeshData();
            mesh.AssociatedObject = game.gameObject;
            mesh.name = game.name;
            var mf = game.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Mesh me;
                if (Application.isPlaying)
                {
                    me = mf.mesh;
                }
                else
                {
                    me = mf.sharedMesh;
                }
                mesh.vertex = me.vertices;
                mesh.normals = me.normals;
                mesh.subMeshCount = me.subMeshCount;
                if (mesh.subMeshCount > 1)
                {
                    int[][] tris = new int[mesh.subMeshCount][];
                    for (int i = 0; i < mesh.subMeshCount; i++)
                        tris[i] = me.GetTriangles(i);
                    mesh.subtris = tris;
                }
                else mesh.tris = me.triangles;
            }
            var t = game.transform;
            mesh.coordinate.pos = t.localPosition;
            mesh.coordinate.scale = t.localScale;
            mesh.coordinate.quat = t.localRotation;
            int c = t.childCount;
            if (c > 0)
            {
                mesh.child = new MeshData[c];
                for (int i = 0; i < c; i++)
                    mesh.child[i] = LoadFromGameObject(t.GetChild(i),action);
            }
            if (action != null)
                action(mesh,mesh.AssociatedObject);
            return mesh;
        }
        public MeshData FindAssociatedMesh(GameObject game)
        {
            if (game == AssociatedObject)
                return this;
            if (child != null)
            {
                for (int i = 0; i < child.Length; i++)
                {
                    var mesh = child[i].FindAssociatedMesh(game);
                    if (mesh != null)
                        return mesh;
                }
            }
            return null;
        }
    }
}
