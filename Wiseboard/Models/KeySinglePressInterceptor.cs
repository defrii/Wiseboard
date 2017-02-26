using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Wiseboard.Models
{
    class KeySinglePressInterceptor
    {
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);
        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        public List<Key> HookedKeys { get; set; } = new List<Key>();
        List<Key> CurrentHandle = new List<Key>();

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        IntPtr hHook = IntPtr.Zero;
        IntPtr hInstance = IntPtr.Zero;

        private static KeyboardHookProc callbackDelegate;

        PresentationSource Source;

        public KeySinglePressInterceptor(PresentationSource source)
        {
            Source = source;
            Hook();
        }

        public void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            callbackDelegate = new KeyboardHookProc(CaptureKeySinglePress);
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, callbackDelegate, hInstance, 0);
            if (hHook == IntPtr.Zero) throw new Win32Exception();
        }

        public void UnHook()
        {
            if (callbackDelegate == null) return;
            bool ok = UnhookWindowsHookEx(hHook);
            if (!ok) throw new Win32Exception();
            callbackDelegate = null;
        }

        public int CaptureKeySinglePress(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Key key = KeyInterop.KeyFromVirtualKey(lParam.vkCode);
                if (HookedKeys.Contains(key) && !CurrentHandle.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(Keyboard.PrimaryDevice,
                                           Source, 0, key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        CurrentHandle.Add(key);
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        CurrentHandle.Add(key);
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;

                    CurrentHandle.Remove(key);
                }
            }
            return CallNextHookEx(hHook, code, wParam, ref lParam);
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
    }
}
