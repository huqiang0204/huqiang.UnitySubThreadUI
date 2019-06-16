using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang
{
    public class Coordinates
    {
        public Vector3 Postion;
        public Quaternion quaternion;
        public Vector3 Scale;
    }
    public class Tool
    {
        public static Vector3 Limit(Vector2 parentSize, Vector3 sonPos, Vector2 sonSize)
        {
            if (sonSize.x >= parentSize.x)
            {
                sonPos.x = 0;
                if (sonSize.y >= parentSize.y)
                {
                    sonPos.y = 0;
                    return sonPos;
                }
                else
                {
                    float y = sonPos.y * -0.5f;
                    float py = parentSize.y * -0.5f;
                    if (y + sonPos.y < py)
                    {
                        sonPos.y = py - y;
                    }
                    else
                    {
                        y = -y;
                        py = -py;
                        if (y + sonPos.y > py)
                        {
                            sonPos.y = py - y;
                        }
                    }
                }
            }
            else
            {
                float x = sonSize.x * -0.5f;
                float px = parentSize.x * -0.5f;
                if (x + sonPos.x < px)
                {
                    sonPos.x = px - x;
                }
                else
                {
                    x = -x;
                    px = -px;
                    if (x + sonPos.x > px)
                    {
                        sonPos.x = px - x;
                    }
                }
                if (sonSize.y >= parentSize.y)
                {
                    sonPos.y = 0;
                    return sonPos;
                }
                else
                {
                    float y = sonSize.y * -0.5f;
                    float py = parentSize.y * -0.5f;
                    if (y + sonPos.y < py)
                    {
                        sonPos.y = py - y;
                    }
                    else
                    {
                        y = -y;
                        py = -py;
                        if (y + sonPos.y > py)
                        {
                            sonPos.y = py - y;
                        }
                    }
                }
            }
            return sonPos;
        }
        /// <summary>
        /// 校正
        /// </summary>
        /// <param name="parentSize">父轴尺寸</param>
        /// <param name="sonPos">子位置</param>
        /// <param name="sonSize">子尺寸</param>
        /// <returns></returns>
        public static Vector3 Correction(Vector2 parentSize, Vector3 sonPos, Vector2 sonSize)
        {
            if (sonSize.x <= parentSize.x)
            {
                sonPos.x = 0;
                if (sonSize.y <= parentSize.y)
                {
                    sonPos.y = 0;
                    return sonPos;
                }
            }
            else
            {
                if (sonSize.y <= parentSize.y)
                {
                    sonPos.y = 0;
                }
            }

            Vector2 dotA = Vector2.zero;
            if (sonPos.x != 0)
            {
                float right = parentSize.x * 0.5f;
                float left = -right;
                float w = sonSize.x * 0.5f;
                float a = sonPos.x - w;
                if (a > left)
                {
                    sonPos.x = left + w;
                }
                else
                {
                    a = sonPos.x + w;
                    if (a < right)
                        sonPos.x = right - w;
                }
            }
            if (sonPos.y != 0)
            {
                float top = parentSize.y * 0.5f;
                float down = -top;
                float h = sonSize.y * 0.5f;
                float a = sonPos.y - h;
                if (a > down)
                {
                    sonPos.y = down + h;
                }
                else
                {
                    a = sonPos.y + h;
                    if (a < top)
                        sonPos.y = top - h;
                }
            }
            return sonPos;
        }
        public static float GetTimeLineValue(Vector2[] cl, float time)
        {
            int len = cl.Length;
            int last = len - 1;
            float max = cl[last].x;
            time %= max;
            for (int i = last; i >= 0; i--)
                if (time > cl[i].x)
                {
                    int s = i + 1;
                    if (s < len)
                    {
                        float tl = cl[s].x - cl[i].x;
                        float r = (time - cl[i].x) / tl;
                        float vl = cl[s].y - cl[i].y;
                        return cl[i].y + vl * r;
                    }
                    else return cl[i].y;
                }
            return cl[last].y;
        }
        public static byte[] EncodeToWAV(int channels, int frequency, int samples, byte[] data)
        {
            byte[] bytes = null;

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(new byte[44], 0, 44);//预留44字节头部信息  

                byte[] bytesData = data;

                memoryStream.Write(bytesData, 0, bytesData.Length);

                memoryStream.Seek(0, SeekOrigin.Begin);

                byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
                memoryStream.Write(riff, 0, 4);

                byte[] chunkSize = BitConverter.GetBytes(memoryStream.Length - 8);
                memoryStream.Write(chunkSize, 0, 4);

                byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
                memoryStream.Write(wave, 0, 4);

                byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
                memoryStream.Write(fmt, 0, 4);

                byte[] subChunk1 = BitConverter.GetBytes(16);
                memoryStream.Write(subChunk1, 0, 4);

                UInt16 two = 2;
                UInt16 one = 1;

                byte[] audioFormat = BitConverter.GetBytes(one);
                memoryStream.Write(audioFormat, 0, 2);

                byte[] numChannels = BitConverter.GetBytes(channels);
                memoryStream.Write(numChannels, 0, 2);

                byte[] sampleRate = BitConverter.GetBytes(frequency);
                memoryStream.Write(sampleRate, 0, 4);

                byte[] byteRate = BitConverter.GetBytes(frequency * channels * 2); // sampleRate * bytesPerSample*number of channels  
                memoryStream.Write(byteRate, 0, 4);

                UInt16 blockAlign = (ushort)(channels * 2);
                memoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                UInt16 bps = 16;
                byte[] bitsPerSample = BitConverter.GetBytes(bps);
                memoryStream.Write(bitsPerSample, 0, 2);

                byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
                memoryStream.Write(datastring, 0, 4);

                byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
                memoryStream.Write(subChunk2, 0, 4);

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        public static Texture2D LoadImage(byte[] data)
        {
            try
            {
                Texture2D t2d = new Texture2D(1, 1);
                t2d.LoadImage(data);
                t2d.Resize(t2d.width, t2d.height, t2d.format, false);
                t2d.LoadImage(data);
                return t2d;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.StackTrace.ToString());
                return null;
            }
        }
        /// <summary>
        /// 复制一个纹理，让纹理可读可写
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
        public static byte[] FloatToByte(float[] data)
        {
            unsafe
            {
                int len = data.Length;
                byte[] temp = new byte[len * 4];
                fixed (byte* pb = &temp[0])
                fixed (float* pf = &data[0])
                {
                    float* pa = (float*)pb;
                    float* pc = pf;
                    for (int i = 0; i < len; i++)
                    {
                        *pa = *pc;
                        pa++;
                        pc++;
                    }
                    return temp;
                }
            }
        }
        public static float[] ByteToFloat(byte[] data)
        {
            unsafe
            {
                int len = data.Length / 4;
                float[] temp = new float[len];
                fixed (byte* pb = &data[0])
                fixed (float* pf = &temp[0])
                {
                    float* pa = (float*)pb;
                    float* pc = pf;
                    for (int i = 0; i < len; i++)
                    {
                        *pc = *pa;
                        pa++;
                        pc++;
                    }
                    return temp;
                }
            }
        }
        public static void FloatToByte(float[] data, byte[] buff, int start)
        {
            unsafe
            {
                int len = data.Length;
                fixed (byte* pb = &buff[start])
                fixed (float* pf = &data[0])
                {
                    float* pa = (float*)pb;
                    float* pc = pf;
                    for (int i = 0; i < len; i++)
                    {
                        *pa = *pc;
                        pa++;
                        pc++;
                    }
                }
            }
        }
        public static void ByteToFloat(byte[] data, float[] buff, int start)
        {
            unsafe
            {
                int len = buff.Length;
                fixed (byte* pb = &data[start])
                fixed (float* pf = &buff[0])
                {
                    float* pa = (float*)pb;
                    float* pc = pf;
                    for (int i = 0; i < len; i++)
                    {
                        *pc = *pa;
                        pa++;
                        pc++;
                    }
                }
            }
        }
        /// <summary>
        /// 获取tranfrom的全局信息
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="Includeroot"></param>
        /// <returns></returns>
        public static Coordinates GetGlobaInfo(Transform rect, bool Includeroot = true)
        {
            Transform[] buff = new Transform[32];
            buff[0] = rect;
            var parent = rect.parent;
            int max = 1;
            if (parent != null)
                for (; max < 32; max++)
                {
                    buff[max] = parent;
                    parent = parent.parent;
                    if (parent == null)
                        break;
                }
            Vector3 pos, scale;
            Quaternion quate;
            if (Includeroot)
            {
                var p = buff[max];
                pos = p.localPosition;
                scale = p.localScale;
                quate = p.localRotation;
                max--;
            }
            else
            {
                pos = Vector3.zero;
                scale = Vector3.one;
                quate = Quaternion.identity;
                max--;
            }
            for (; max >= 0; max--)
            {
                var rt = buff[max];
                Vector3 p = rt.localPosition;
                Vector3 o = Vector3.zero;
                o.x = p.x * scale.x;
                o.y = p.y * scale.y;
                o.z = p.z * scale.z;
                pos += o;
                quate *= rt.localRotation;
                Vector3 s = rt.localScale;
                scale.x *= s.x;
                scale.y *= s.y;
            }
            Coordinates coord = new Coordinates();
            coord.Postion = pos;
            coord.quaternion = quate;
            coord.Scale = scale;
            return coord;
        }
        public unsafe static byte[] GetByteArray(byte* p,int len)
        {
            byte[] buff = new byte[len];
            for (int i = 0; i < len; i++)
            { buff[i] = *p; p++; }
            return buff;
        }
        public unsafe static void WitreToStruct(void* tar, void* src, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)src;
            for (int i = 0; i < size; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        public unsafe static void WitreToStructArray(void* tar, void* src, int array, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)src;
            for (int i = 0; i < array * size; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        public static void WritePolygonToFile(Vector3[] list, string filename, string vName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Vector3[] ");
            sb.Append(vName);
            sb.Append("=new Vector3[]{");
            bool multi = false;
            for (int i = 0; i < list.Length; i++)
            {
                if (multi)
                    sb.Append(",");
                var v = list[i];
                sb.Append("new Vector3(");
                sb.Append(v.x);
                sb.Append("f,");
                sb.Append(v.y);
                sb.Append("f,");
                sb.Append(v.z);
                sb.Append("f)");
                multi = true;
            }
            sb.Append("};");
            byte[] buf = Encoding.UTF8.GetBytes(sb.ToString());
            string path = Application.dataPath + "\\" + filename + ".txt";
            if (File.Exists(path))
                File.Delete(path);
            var fs = File.Create(path);
            fs.Write(buf, 0, buf.Length);
            fs.Dispose();
        }
    }
}
