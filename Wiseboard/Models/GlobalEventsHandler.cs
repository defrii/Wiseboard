using Wiseboard.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;

namespace Wiseboard.Models
{
    class GlobalEventsHandler
    {
        const int WM_HOTKEY = 0x0312;
        const int WM_KEYUP = 0x0101;
        const int WM_CLIPBOARDUPDATE = 0x031D;
        const int CONTROL = 0x0002;

        const int VK_CONTROL = 0x11;
        const int VK_C = 0x43;
        const int VK_V = 0x56;

        bool _isRunning = false;
        bool _keyNextHandled = false;

        enum HotKeyId { Paste };

        readonly IntPtr _wndHandler;

        readonly InputSimulator _inputSimulator = new InputSimulator();

        readonly Stopwatch _timer = new Stopwatch();

        readonly ClipboardView _clipboardDisplayer;

        readonly LinkedList<IClipboardData> _extendedClipboard = new LinkedList<IClipboardData>();
        int _clipboardIndex = -1;

        readonly KeySinglePressInterceptor _singleInterceptor;

        public static SettingsModel Settings { get; set; } = new SettingsModel();

        public GlobalEventsHandler(PresentationSource source, IntPtr wndHandler)
        {
            _clipboardDisplayer = new ClipboardView(Settings, _extendedClipboard);

            _wndHandler = wndHandler;
            _singleInterceptor = new KeySinglePressInterceptor(source);

            _singleInterceptor.HookedKeys.Add(Key.V);
            _singleInterceptor.HookedKeys.Add(Key.LeftCtrl);
            _singleInterceptor.HookedKeys.Add(Key.RightCtrl);

            _singleInterceptor.KeyUp += KeyUpKeyHandle;
            _singleInterceptor.KeyDown += KeyDownKeyHandle;

            Copy();
        }

        public bool SwitchMode()
        {
            if (!_isRunning)
            {
                AddClipboardFormatListener(_wndHandler);
                RegisterHotKey(_wndHandler, (int)HotKeyId.Paste, CONTROL, VK_V);
                _isRunning = true;
                return true;
            }

            RemoveClipboardFormatListener(_wndHandler);
            UnregisterHotKey(_wndHandler, (int)HotKeyId.Paste);
            _isRunning = false;
            return false;
        }

        public IntPtr CaptureKeyCombinations(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY:
                    PasteStart();
                    break;

                case WM_CLIPBOARDUPDATE:
                    Copy();
                    break;
            }

            return IntPtr.Zero;
        }

        public void CloseClipboardView()
        {
            _clipboardDisplayer.Close();
        }

        public void UnregisterAll()
        {
            RemoveClipboardFormatListener(_wndHandler);
            UnregisterHotKey(_wndHandler, (int)HotKeyId.Paste);
            _singleInterceptor.UnHook();
        }

        void Copy()
        {
            if (Clipboard.ContainsText())
            {
                _extendedClipboard.AddFirst(new ClipboardData(Clipboard.GetText(), false));
            }

            else if (Clipboard.ContainsFileDropList())
            {
                string fileNames = string.Empty;
                foreach (var fileName in Clipboard.GetFileDropList())
                    fileNames += fileName + '\n';

                _extendedClipboard.AddFirst(new ClipboardData(Clipboard.GetDataObject(), true, fileNames));
            }

            while (_extendedClipboard.Count > Settings.MaxSize)
                _extendedClipboard.RemoveLast();
        }

        void PasteStart()
        {
            if (!_timer.IsRunning)
            {
                _timer.Start();
                _keyNextHandled = false;
                WaitForDisplayExtendedClipboard();
            }
        }

        async void WaitForDisplayExtendedClipboard()
        {
            await Task.Run(() =>
            {
                lock (_timer)
                {
                    while (_timer.ElapsedMilliseconds < Settings.TimeToElapse) ;
                }
            });

            if (_timer.IsRunning)
                _clipboardDisplayer.DisplayClipboard();
        }

        void SetNextElement()
        {
            if (_extendedClipboard.Count > 0 && !_keyNextHandled
                && _timer.ElapsedMilliseconds >= Settings.TimeToElapse)
            {
                if (++_clipboardIndex >= _extendedClipboard.Count)
                    _clipboardIndex = 0;

                _clipboardDisplayer.SetNextElement(_clipboardIndex);

                _keyNextHandled = true;
            }
        }

        void PasteStop()
        {
            if (_timer.IsRunning)
            {
                _timer.Reset();

                UnregisterHotKey(_wndHandler, (int)HotKeyId.Paste);
                _singleInterceptor.UnHook();
                RemoveClipboardFormatListener(_wndHandler);

                IClipboardData currentElement = null;

                if (_clipboardIndex >= 0)
                {
                    currentElement = _extendedClipboard.ElementAt(_clipboardIndex);
                    if (!_extendedClipboard.ElementAt(_clipboardIndex).IsLinkOrLinks())
                        Clipboard.SetText((string)currentElement.GetData());
                    else
                        Clipboard.SetDataObject(currentElement.GetData());
                }
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

                AddClipboardFormatListener(_wndHandler);
                _singleInterceptor.Hook();
                RegisterHotKey(_wndHandler, (int)HotKeyId.Paste, CONTROL, VK_V);

                _clipboardDisplayer.CloseClipboard();
                _clipboardDisplayer.ClearBackground();

                if (currentElement != null)
                {
                    _extendedClipboard.Remove(currentElement);
                    _extendedClipboard.AddFirst(currentElement);
                }
                _clipboardIndex = -1;
            }
        }

        void KeyUpKeyHandle(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            switch (key)
            {
                case Key.V:
                    if (_timer.ElapsedMilliseconds < Settings.TimeToElapse)
                        PasteStop();
                    _keyNextHandled = false;
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    PasteStop();
                    break;
            }
        }

        void KeyDownKeyHandle(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            switch (key)
            {
                case Key.V:
                    SetNextElement();
                    break;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
