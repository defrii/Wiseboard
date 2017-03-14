using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;
using Wiseboard.Handlers.Helpers;
using Wiseboard.Models;
using Wiseboard.Observers;
using Wiseboard.Views;

namespace Wiseboard.Handlers
{
    class PastingHandler
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

        enum HotKeyId { Paste };

        Key _shortcutKey;
        int _shortcutModifier;

        const int WH_KEYBOARD_LL = 13;
        const int WM_HOTKEY = 0x0312;
        const int WM_CLIPBOARDUPDATE = 0x031D;
        const int CONTROL = 0x0002;
        const int VK_V = 0x56;

        public ClipboardView ClipboardDisplayer { get; set; }
        public LinkedList<IClipboardData> ExtendedClipboard { get; set; } = new LinkedList<IClipboardData>();
        private readonly List<IChangedStatusObserver> _changedStatusObservers = new List<IChangedStatusObserver>();

        public static SettingsModel Settings { get; set; } = new SettingsModel();
        readonly ClipboardEventsHandler _clipboardEventsHandler = new ClipboardEventsHandler();

        public bool Running { get; set; } = true;
        int _clipboardIndex;

        readonly InputSimulator _inputSimulator = new InputSimulator();
        readonly List<Key> _hookedKeys = new List<Key>();
        readonly Stopwatch _timer = new Stopwatch();

        IntPtr _hHook = IntPtr.Zero;

        private static KeyboardHookProc _callbackDelegate;
        readonly PresentationSource _source;

        readonly IntPtr _wndHandler;

        public PastingHandler(PresentationSource source, IntPtr wndHandler)
        {
            _source = source;
            _wndHandler = wndHandler;
            ClipboardDisplayer = new ClipboardView(Settings.AppearanceSettingsModel, ExtendedClipboard);

            if (Running)
            {
                AddClipboardFormatListener(_wndHandler);
                RegisterHotKey(_wndHandler, (int)HotKeyId.Paste, CONTROL, VK_V);
            }

            _hookedKeys.Add(Key.V);
            _hookedKeys.Add(Key.LeftCtrl);
            _hookedKeys.Add(Key.RightCtrl);

            Hook();
            Copy();
        }

        public int CaptureKeySinglePress(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Key key = KeyInterop.KeyFromVirtualKey(lParam.VkCode);

                if (_clipboardEventsHandler.IsPastedBeforeDisplayClipboard())
                {
                    if (HandleEarlyPastedKeyPressed(key, wParam))
                        return 1;
                }
                KeyEventArgs kea = new KeyEventArgs(Keyboard.PrimaryDevice, _source, 0, key);
                if (TypeOfPressChecker.IsKeyDownPressed(wParam))
                    KeyDownKeyHandle(kea);
                else if (TypeOfPressChecker.IsKeyUpPressed(wParam))
                    KeyUpKeyHandle(kea);
            }
            return CallNextHookEx(_hHook, code, wParam, ref lParam);
        }

        bool HandleEarlyPastedKeyPressed(Key key, int typeOfPress)
        {
            if (key == Key.V && TypeOfPressChecker.IsKeyDownPressed(typeOfPress) && !IsPastingStarted())
            {
                PasteStart();
            }
            else if (_hookedKeys.Contains(key) && TypeOfPressChecker.IsKeyUpPressed(typeOfPress)
                && IsPastingStarted() && !_clipboardEventsHandler.IsItemSelectingFromClipboard())
            {
                PasteStop();
            }
            else if (key == Key.V && TypeOfPressChecker.IsKeyDownPressed(typeOfPress)
                && _clipboardEventsHandler.IsItemSelectingFromClipboard()
                && !_clipboardEventsHandler.CanChangePositionOfSelectedItem())
            {
                SetNextElement();
                _clipboardEventsHandler.SetChangePositionOfSelectedItem(true);
            }
            else if (key == Key.V && TypeOfPressChecker.IsKeyUpPressed(typeOfPress))
            {
                _clipboardEventsHandler.SetChangePositionOfSelectedItem(false);
            }
            if (key == Key.V)
                return true;

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

        public void SetNextElement()
        {
            if (ExtendedClipboard.Count > 0 && !_clipboardEventsHandler.CanChangePositionOfSelectedItem()
                && _timer.ElapsedMilliseconds >= Settings.GeneralSettingsModel.TimeToElapse)
            {
                if (++_clipboardIndex >= ExtendedClipboard.Count)
                    _clipboardIndex = 0;

                ClipboardDisplayer.SetNextElement(_clipboardIndex);

                _clipboardEventsHandler.SetChangePositionOfSelectedItem(true);
            }
        }

        public void PasteStart()
        {
            if (!IsPastingStarted())
            {
                _timer.Start();
                _clipboardEventsHandler.SetChangePositionOfSelectedItem(false);
                WaitForDisplayExtendedClipboard();
            }
        }

        public void PasteStop()
        {
            if (!IsPastingStarted() || ExtendedClipboard.Count == 0) return;
            _timer.Reset();

            UnregisterAll();

            IClipboardData currentElement = null;

            if (_clipboardIndex >= 0)
            {
                currentElement = ExtendedClipboard.ElementAt(_clipboardIndex);
                if (!ExtendedClipboard.ElementAt(_clipboardIndex).IsLinkOrLinks())
                    Clipboard.SetText((string)currentElement.GetData());
                else
                    Clipboard.SetDataObject(currentElement.GetData());
            }

            _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

            RegisterAll();

            ClipboardDisplayer.CloseClipboard();
            ClipboardDisplayer.ClearBackground();

            if (currentElement != null)
            {
                ExtendedClipboard.Remove(currentElement);
                ExtendedClipboard.AddFirst(currentElement);
            }
            _clipboardIndex = 0;
        }

        void KeyUpKeyHandle(KeyEventArgs e)
        {
            Key key = e.Key;
            switch (key)
            {
                case Key.V:
                    if (_timer.ElapsedMilliseconds < Settings.GeneralSettingsModel.TimeToElapse && IsPastingStarted())
                    {
                        _clipboardEventsHandler.SetPastedBeforeDisplayClipboard(true);
                        _clipboardEventsHandler.SetSelectingFromClipboard(false);
                        PasteStop();
                    }
                    else
                    {
                        _clipboardEventsHandler.SetChangePositionOfSelectedItem(false);
                        _clipboardEventsHandler.SetPastedBeforeDisplayClipboard(false);
                    }
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    _clipboardEventsHandler.SetPastedBeforeDisplayClipboard(false);
                    if (IsPastingStarted())
                    {
                        PasteStop();
                    }
                    break;
            }
        }

        void KeyDownKeyHandle(KeyEventArgs e)
        {
            Key key = e.Key;

            switch (key)
            {
                case Key.V:
                    if (!_clipboardEventsHandler.CanChangePositionOfSelectedItem() && IsPastingStarted())
                    {
                        SetNextElement();
                        _clipboardEventsHandler.SetChangePositionOfSelectedItem(true);
                    }
                    else if (IsPastingStarted())
                    {
                        PasteStart();
                    }
                    break;
            }

            if (e.Key == Settings.GeneralSettingsModel.ShortcutKey
                && Keyboard.Modifiers == (ModifierKeys) Settings.GeneralSettingsModel.ShortcutModifiers)
            {
                bool status = SwitchMode();
                foreach (var observer in _changedStatusObservers)
                    observer.UpdateStatus(status);
            }
        }

        public bool SwitchMode()
        {
            if (!Running)
            {
                AddClipboardFormatListener(_wndHandler);
                RegisterHotKey(_wndHandler, (int)HotKeyId.Paste, CONTROL, VK_V);
                Running = true;
                return true;
            }

            RemoveClipboardFormatListener(_wndHandler);
            UnregisterHotKey(_wndHandler, (int)HotKeyId.Paste);
            Running = false;
            return false;
        }


        public void Copy()
        {
            if (Clipboard.ContainsText())
            {
                ExtendedClipboard.AddFirst(new ClipboardData(Clipboard.GetText(), false));
            }

            else if (Clipboard.ContainsFileDropList())
            {
                string fileNames = string.Empty;
                foreach (var fileName in Clipboard.GetFileDropList())
                    fileNames += fileName + '\n';

                ExtendedClipboard.AddFirst(new ClipboardData(Clipboard.GetDataObject(), true, fileNames));
            }

            while (ExtendedClipboard.Count > Settings.GeneralSettingsModel.MaxSize)
                ExtendedClipboard.RemoveLast();
        }

        public void CloseClipboardView()
        {
            ClipboardDisplayer.Close();
        }

        async void WaitForDisplayExtendedClipboard()
        {
            await Task.Run(() =>
            {
                lock (_timer)
                {
                    while (_timer.ElapsedMilliseconds < Settings.GeneralSettingsModel.TimeToElapse) ;
                }
            });

            if (!IsPastingStarted()) return;
            _clipboardEventsHandler.SetSelectingFromClipboard(true);
            _clipboardEventsHandler.SetChangePositionOfSelectedItem(true);
            ClipboardDisplayer.DisplayClipboard();
        }

        public void AddObserver(IChangedStatusObserver observer) => _changedStatusObservers.Add(observer);

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

        public void RegisterAll()
        {
            AddClipboardFormatListener(_wndHandler);
            RegisterHotKey(_wndHandler, (int)HotKeyId.Paste, CONTROL, VK_V);
            Hook();
        }

        public void UnregisterAll()
        {
            RemoveClipboardFormatListener(_wndHandler);
            UnregisterHotKey(_wndHandler, (int)HotKeyId.Paste);
            UnHook();
        }

        public bool IsPastingStarted()
        {
            return _timer.IsRunning;
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
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
