using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace huqiang.Data
{
    public class DownloadManager
    {
        public static List<DownLoadMission> Mission;
        public static void UpdateMission()
        {
            if (Mission == null)
                return;
            int c = Mission.Count - 1;
            for (; c >= 0; c--)
            {
                var mis = Mission[c];
                if (mis.webRequest != null)
                {
                    mis.err = (int)mis.webRequest.responseCode;
                    if (mis.webRequest.isDone)
                    {
                        mis.result = mis.webRequest.downloadHandler.data;
                        Mission.RemoveAt(c);
                        if (mis.OnDone != null)
                            mis.OnDone(mis);
                        mis.webRequest.Dispose();
                    }
                    else if (mis.webRequest.isHttpError)
                    {
                        Mission.RemoveAt(c);
                        if (mis.OnDone != null)
                            mis.OnDone(mis);
                        mis.webRequest.Dispose();
                    }
                }
            }
        }
        public static DownLoadMission FindMission(string cmd)
        {
            for (int i = 0; i < Mission.Count; i++)
            {
                if (cmd == Mission[i].cmd)
                    return Mission[i];
            }
            return null;
        }
        public static DownLoadMission DownloadAsset(string cmd, string name, string url,  object context,Action<DownLoadMission> done, int version=0)
        {
            if (Mission == null)
                Mission = new List<DownLoadMission>();
            else
            {
                for (int i = 0; i < Mission.Count; i++)
                {
                    var mis = Mission[i];
                    if (mis.type == name)
                        return mis;//任务不能重复添加
                }
            }
            DownLoadMission r = new DownLoadMission();
            r.filename = name;
            r.cmd = cmd;
            r.DataContext = context;
            r.url = url;
            r.OnDone = done;
            r.version = version;
            var web = UnityWebRequest.Get(url);// (uint)version,0
            web.SendWebRequest();
            r.webRequest = web;
            Mission.Add(r);
            return r;
        }
    }
    public class DownLoadMission
    {
        public string cmd;
        public object DataContext;
        public byte[] result;
        public string type;
        public string filename;
        public string url;
        public int err;
        public int version;

        public UnityWebRequest webRequest;
        public Action<DownLoadMission> OnDone;
    }
}