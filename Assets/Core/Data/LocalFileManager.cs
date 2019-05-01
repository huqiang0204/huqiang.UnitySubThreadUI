using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class LocalFileManager
    {
        public static string persistentDataPath = Application.persistentDataPath;
        /// <summary>
        /// 将数据写入具有允许权限的磁盘
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">文件名</param>
        /// <param name="data">数据</param>
        public static void WriteData(string type,string name,byte[] data)
        {
            if (type == null | type == "" | name == null | name == "" | data == null)
                return;
            string path = persistentDataPath + "/data";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + type;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + name;
            if (File.Exists(path))
                File.Delete(path);
            var fs = File.Create(path);
            fs.Write(data,0,data.Length);
            fs.Dispose();
        }
        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] ReadData(string type,string name)
        {
            if (type == null | type == "" | name == null | name == "")
                return null;
            string path = persistentDataPath + "/data";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + type;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + name;
            if(File.Exists(path))
            {
                var fs = File.Open(path, FileMode.Open);
                byte[] buf = null;
                if (fs.Length > 0)
                {
                    buf = new byte[fs.Length];
                    fs.Read(buf, 0, buf.Length);
                }
                fs.Dispose();
                return buf;
            }
            return null;
        }
        /// <summary>
        /// 查询assetbundle的完整路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FindAssetBundle(string name)
        {
            string path = persistentDataPath + "\\bundle";
            if(Directory.Exists(path))
            {
                var files= Directory.GetFiles(path);
                if (files != null)
                    for (int i = 0; i < files.Length; i++)
                    {
                        var ss= files[i].Split('\\');
                        var str = ss[ss.Length - 1];
                        if (str.Split('_')[0] == name)
                            return str;
                    }
            }
            return null;
        }
        /// <summary>
        /// 读取assetbundle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] LoadAssetBundle(string name)
        {
            string fullname= FindAssetBundle(name);
            if(fullname!=null)
            {
               string path =  persistentDataPath + "\\bundle\\" + fullname;
                var fs = File.Open(path,FileMode.Open);
                byte[] buf = new byte[fs.Length];
                fs.Read(buf,0,buf.Length);
                fs.Dispose();
                return buf;
            }
            return null;
        }
        /// <summary>
        /// 删除当前的AssetBundle
        /// </summary>
        /// <param name="name"></param>
        static void DeleteAssetBundle(string name)
        {
            string path = persistentDataPath + "\\bundle";
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                if (files != null)
                    for (int i = 0; i < files.Length; i++)
                    {
                        var ss = files[i].Split('\\');
                        var str = ss[ss.Length - 1];
                        if (str.Split('_')[0] == name)
                        {
                            File.Delete(files[i]);
                        }
                    }
            }
        }
        /// <summary>
        /// 查询当前AssetBundle的版本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetBundleVersion(string name)
        {
            if (name == null | name == "")
                return 0;
            var str= FindAssetBundle(name);
            if (str == null)
                return 0;
            int v = 0;
            var ss = str.Split('_');
            if (ss.Length > 1)
                int.TryParse(ss[1],out v);
            return v;
        }
        /// <summary>
        /// 保存一个AssetBundle
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="version">版本号</param>
        /// <param name="data">数据</param>
        public static void SaveAssetBundle(string name, int version, byte[] data)
        {
            string path = persistentDataPath + "\\bundle";
            if (Directory.Exists(path))
                DeleteAssetBundle(name);
            else Directory.CreateDirectory(path);
            path += "\\" + name + "_" + version.ToString();
            var fs = File.Create(path);
            fs.Write(data, 0, data.Length);
            fs.Dispose();
        }
        /// <summary>
        /// 删除所有AssetBundle
        /// </summary>
        public static void ClearAssetBundle()
        {
            string path = persistentDataPath + "\\bundle";
            if (Directory.Exists(path))
                Directory.Delete(path);
        }
    }
}
