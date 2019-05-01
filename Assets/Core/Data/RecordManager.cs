using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public class RecordManager
    {
        static string persistentDataPath = Application.persistentDataPath;
        static List<RecordFile> records;
        static FileStream CreateRecord(string type, string name)
        {
            string path = persistentDataPath + "/records";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/"+type;
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + name;
            var file = File.Open(path, FileMode.OpenOrCreate);
            file.Seek(file.Length, SeekOrigin.Begin);
            return file;
        }
        static FileStream FindRecord(string type,string name)
        {
            if(records==null)
            {
                records = new List<RecordFile>();
            }
            else
            {
                for(int i=0;i<records.Count;i++)
                {
                    if (records[i].type == type)
                        if (records[i].name == name)
                            return records[i].stream;
                }
            }
            var file = CreateRecord(type, name);
            RecordFile record = new RecordFile();
            record.type = type;
            record.name = name;
            record.stream = file;
            records.Add(record);
            return file;
        }
        static byte[] tmp = new byte[8];
        static void WriteRecord(FileStream fs, int type, string json)
        {
            var buff = Encoding.UTF8.GetBytes(json);
            unsafe
            {
                fixed (byte* bp = &tmp[0])
                {
                    int* ip = (int*)bp;
                    *ip = buff.Length;
                    ip++;
                    *ip = type;
                }
            }
            fs.Write(tmp, 0, 8);
            fs.Write(buff, 0, buff.Length);
        }
        public static void WriteRecord(string type, string name, string data, int tag)
        {
            if (type == null | type == ""| name == null | name == "")
                return;
            var fs= FindRecord(type,name);
            WriteRecord(fs,tag,data);
        }
        public static void ReleaseAll()
        {
            if (records != null)
            {
                for (int i = 0; i < records.Count; i++)
                    records[i].stream.Dispose();
                records.Clear();
            }
        }
        public static void ReleaseRecord(string type,string name)
        {
            if (records != null)
            {
                for(int i=0;i<records.Count;i++)
                {
                    if(records[i].type==type)
                        if(records[i].name==name)
                        {
                            records[i].stream.Dispose();
                            records.RemoveAt(i);
                            break;
                        }
                }
            }
        }
        static List<Record> ReadRecord(byte[] buff)
        {
            List<Record> tmp = new List<Record>();
            int point = 0;
            for (int i = 0; i < buff.Length; i++)
            {
                unsafe
                {
                    fixed (byte* bp = &buff[point])
                    {
                        int* ip = (int*)bp;
                        int len = *ip;
                        ip++;
                        int t = *ip;
                        string str = Encoding.UTF8.GetString(buff, point + 8, len);
                        tmp.Add(new Record() { tag = t, data = str });
                        point += len + 8;
                        if (point >= buff.Length)
                            break;
                    }
                }
            }
            return tmp;
        }
        public static List<Record> ReadRecord(string type, string name)
        {
            var fs = FindRecord(type,name);
            if (fs.Length > 0)
            {
                var buff = new byte[fs.Length];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(buff, 0, buff.Length);
                return ReadRecord(buff);
            }
            else return null;
        }
        public static void DeleteRecord(string type,string name)
        {
            if (records != null)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    if (records[i].type == type)
                        if (records[i].name == name)
                        {
                            records[i].stream.Dispose();
                            records.RemoveAt(i);
                            break;
                        }
                }
            }
            string path = persistentDataPath + "/records/"+type+"/"+name;
            if (File.Exists(path))
                File.Delete(path);
        }
        public static void DeleteRecords(string type)
        {
            if (records != null)
            {
                int top = records.Count - 1;
                for (; top>=0; top--)
                {
                    if (records[top].type == type)
                    {
                        records[top].stream.Dispose();
                        records.RemoveAt(top);
                    }
                }
            }
            string path = persistentDataPath + "/records/" + type;
            if (Directory.Exists(path))
                Directory.Delete(path);
        }
    }
    [Serializable]
    public class Record
    {
        public int tag;
        public string data;
    }
    class RecordFile
    {
        public FileStream stream;
        public string type;
        public string name;
    }
}