using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace huqiang
{
#if UNITY_STANDALONE_WIN ||UNITY_EDITOR
    public class IME
    {
        public  static bool haveMainWindow = false;
        public  static IntPtr mainWindowHandle = IntPtr.Zero;
        public int processId = 0;

        public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        public  IntPtr GetMainWindowHandle(int processId)
        {
            if (!haveMainWindow)
            {
                mainWindowHandle = IntPtr.Zero;
                this.processId = processId;
                EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(this.EnumWindowsCallback);
                EnumWindows(callback, IntPtr.Zero);
                GC.KeepAlive(callback);

                haveMainWindow = true;
            }
            return mainWindowHandle;
        }

        private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
        {
            int num;
            GetWindowThreadProcessId(new HandleRef(this, handle), out num);
            if ((num == this.processId) && this.IsMainWindow(handle))
            {
                mainWindowHandle = handle;
                return false;
            }
            return true;
        }

        private bool IsMainWindow(IntPtr handle)
        {
            return (!(GetWindow(new HandleRef(this, handle), 4) != IntPtr.Zero) && IsWindowVisible(new HandleRef(this, handle)));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HandleRef hWnd);
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        public static extern int ImmSetCompositionStringW(IntPtr himc, int dwIndex, IntPtr lpComp, int dw, int lpRead, int dw2);

        public const int GCS_COMPSTR = 8;
        public const int WM_IME_SETCONTEXT = 0x0281;
        public const int WM_IME_CHAR = 0x0286;
        public const int WM_CHAR = 0x0102;
        public const int WM_IME_COMPOSITION = 0x010F;
        public const int GCS_RESULTSTR = 0x0800;
        public const int SCS_SETRECONVERTSTRING = 0x00010000;
        public const int SCS_QUERYRECONVERTSTRING = 0x00020000;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECONVERTSTRING
        {
            public uint dwSize;
            public uint dwVersion;
            public uint dwStrLen;
            public uint dwStrOffset;
            public uint dwCompStrLen;
            public uint dwCompStrOffset;
            public uint dwTargetStrLen;
            public uint dwTargetStrOffset;
        }

        static IntPtr hIMC= IntPtr.Zero;
        public static string CurrentCompStr()
        {
            int readType = GCS_RESULTSTR;
            try
            {
                int strLen = ImmGetCompositionStringW(hIMC, readType, null, 0);
                if (strLen > 0)
                {
                    byte[] buffer = new byte[strLen];
                    ImmGetCompositionStringW(hIMC, readType, buffer, strLen);
                    string str = Encoding.Unicode.GetString(buffer, 0, strLen);
                    return str;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.StackTrace);
                return "";
            }
        }
        static void SetIMEString(string str)
        {
            unsafe
            {
                uint len = 0;
                RECONVERTSTRING* reconv = (RECONVERTSTRING*)Marshal.AllocHGlobal(
                  sizeof(RECONVERTSTRING) + Encoding.Unicode.GetByteCount(str) + 1);

                char* paragraph = (char*)((byte*)reconv + sizeof(RECONVERTSTRING));

                reconv->dwSize
                  = (uint)sizeof(RECONVERTSTRING) + (uint)Encoding.Unicode.GetByteCount(str) + 1;
                reconv->dwVersion = 0;
                reconv->dwStrLen = (uint)str.Length;
                reconv->dwStrOffset = (uint)sizeof(RECONVERTSTRING);

                reconv->dwCompStrLen = 0;
                reconv->dwCompStrOffset = len * sizeof(char);

                reconv->dwTargetStrLen = 0;
                reconv->dwTargetStrOffset = len * sizeof(char);

                for (int i = 0; i < str.Length; i++)
                {
                    paragraph[i] = str[i];
                }
                ImmSetCompositionStringW(hIMC, SCS_SETRECONVERTSTRING, (IntPtr)reconv,
      sizeof(RECONVERTSTRING) + Encoding.Unicode.GetByteCount(str) + 1, 0, 0);
            }
           
        }
        public static void Initial()
        {
             var pro = Process.GetCurrentProcess();
             mainWindowHandle = new IME().GetMainWindowHandle(pro.Id);
             hIMC = ImmGetContext(mainWindowHandle);
        }
        public static void Dispose()
        {
            ImmReleaseContext(mainWindowHandle, hIMC);
        }
    }
#endif
}
