using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MessagePack.LZ4;

namespace huqiang.LZ4
{
    public class LZ4Streams
    {
        static void EncodeFiles(string folder, string dic,List<string> files,Stream stream)
        {
            var fs = Directory.GetFiles(dic);
            for(int i=0;i<fs.Length;i++)
            {
                int len = 0;
                int ori = 0;
                var dat = Encode(fs[i],ref len,ref ori);
                stream.Write(len.ToBytes(),0,4);
                stream.Write(ori.ToBytes(), 0, 4);
                if (len > 0)
                    stream.Write(dat, 0, len);
                string name = fs[i].Replace(folder, "");
                files.Add(name);
            }
            var fds = Directory.GetDirectories(dic);
            for(int i=0;i<fds.Length;i++)
            {
                EncodeFiles(folder,fds[i],files,stream);
            }
        }
        static byte[] Encode(string file,ref int len,ref int ori)
        {
            var fs = File.Open(file,FileMode.Open);
            if(fs.Length>0)
            {
                byte[] buff = new byte[fs.Length];
                ori = buff.Length;
                fs.Read(buff, 0, buff.Length);
                fs.Dispose();
                byte[] tmp = new byte[buff.Length];
                len = LZ4Codec.Encode64Unsafe(buff, 0, buff.Length, tmp, 0, tmp.Length);
                return tmp;
            }
            return null;
        }
        public static void CompressFolder(string folder,string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            var fs = File.Create(filePath);
            List<string> files = new List<string>();
            EncodeFiles(folder,folder,files,fs);
            int len = (int)fs.Length;
            for (int i = 0; i < files.Count; i++)
                fs.WriteString(files[i]);
            fs.Write(len.ToBytes(),0,4);
            fs.Write(files.Count.ToBytes(),0,4);
            fs.Dispose();
        }
        public static void DecompressFolder(string filepath,string folder)
        {
            var fs = File.Open(filepath,FileMode.Open);
            byte[] tmp = new byte[4];
            int len = (int)fs.Length;
            fs.Seek(len-8,SeekOrigin.Begin);
            fs.Read(tmp,0,4);
            int start = tmp.ReadInt32(0);
            fs.Read(tmp, 0, 4);
            int count=tmp.ReadInt32(0);
            List<string> files = new List<string>();
            byte[] buff = new byte[512];
            fs.Seek(start,SeekOrigin.Begin);
            for(int i=0;i<count;i++)
            {
                fs.Read(tmp,0,4);
                int l= tmp.ReadInt32(0);
                fs.Read(buff,0,l);
                files.Add(Encoding.UTF8.GetString(buff,0,l));
            }
            fs.Seek(0,SeekOrigin.Begin);
            for (int i = 0; i < count; i++)
            {
                var f = files[i];
                fs.Read(tmp, 0, 4);
                int l = tmp.ReadInt32(0);
                fs.Read(tmp, 0, 4);
                int o = tmp.ReadInt32(0);
                if(l>0)
                {
                    byte[] src = new byte[l];
                    fs.Read(src, 0, l);
                    byte[] tar = new byte[o];
                    int k = LZ4Codec.Decode64Unsafe(src, 0, l, tar, 0, o);
                    WriteFile(folder, f, tar, k);
                }
                else WriteFile(folder, f, null, 0);
            }
            fs.Dispose();
        }
        static void WriteFile(string folder,string name,byte[] dat,int len)
        {
            string path = folder;
            var ss = name.Split('\\');
            for(int i=0;i<ss.Length-1;i++)
            {
                path += "\\" + ss[i];
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            path = folder + name;
            if (File.Exists(path))
                File.Delete(path);
            var fs = File.Create(path);
            if (len > 0)
                fs.Write(dat, 0, len);
            fs.Dispose();
        }
    }
}
