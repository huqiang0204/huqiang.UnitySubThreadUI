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
    public class IME
    {
        private  static bool haveMainWindow = false;
        private  static IntPtr mainWindowHandle = IntPtr.Zero;
        private int processId = 0;

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

        private const int GCS_COMPSTR = 8;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_CHAR = 0x0102;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_RESULTSTR = 0x0800;

        static IntPtr hIMC= IntPtr.Zero;


        public static string CurrentCompStr()
        {
            int readType = GCS_RESULTSTR;
            try
            {
                if (Keyboard.InputString != "")
                {
                    int strLen = ImmGetCompositionStringW(hIMC, readType, null, 0);
                    if (strLen > 0)
                    {
                        byte[] buffer = new byte[strLen];
                        ImmGetCompositionStringW(hIMC, readType, buffer, strLen);
                        return Encoding.Unicode.GetString(buffer, 0, strLen);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.StackTrace);
                return "";
            }
        }
        public static void Initial()
        {
             var pro = Process.GetCurrentProcess();
             new IME().GetMainWindowHandle(pro.Id);
             hIMC = ImmGetContext(mainWindowHandle);
        }
        public static void Dispose()
        {
            ImmReleaseContext(mainWindowHandle, hIMC);
        }
    }
}
