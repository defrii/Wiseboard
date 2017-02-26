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
            public int VkCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        public List<Key> HookedKeys { get; set; } = new List<Key>();
        readonly List<Key> _currentHandle = new List<Key>();

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        IntPtr _hHook = IntPtr.Zero;

        private static KeyboardHookProc _callbackDelegate;

        readonly PresentationSource _source;

        public KeySinglePressInterceptor(PresentationSource source)
        {
            _source = source;
            Hook();
        }

        public void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            _callbackDelegate = CaptureKeySinglePress;
            _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, _callbackDelegate, hInstance, 0);
            if (_hHook == IntPtr.Zero) throw new Win32Exception();
        }

        public void UnHook()
        {
            if (_callbackDelegate == null) return;
            bool ok = UnhookWindowsHookEx(_hHook);
            if (!ok) throw new Win32Exception();
            _callbackDelegate = null;
        }

        public int CaptureKeySinglePress(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Key key = KeyInterop.KeyFromVirtualKey(lParam.VkCode);
                if (HookedKeys.Contains(key) && !_currentHandle.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(Keyboard.PrimaryDevice,
                                           _source, 0, key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        _currentHandle.Add(key);
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        _currentHandle.Add(key);
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;

                    _currentHandle.Remove(key);
                }
            }
            return CallNextHookEx(_hHook, code, wParam, ref lParam);
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
