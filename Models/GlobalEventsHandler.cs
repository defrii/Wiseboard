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

        bool isRunning = false;
        bool keyNextHandled = false;

        enum HotKeyId { PASTE };
        IntPtr WndHandler;
        PresentationSource Source;

        InputSimulator inputSimulator = new InputSimulator();

        Stopwatch timer = new Stopwatch();

        ClipboardView clipboardDisplayer;

        LinkedList<IClipboardData> extendedClipboard = new LinkedList<IClipboardData>();
        int clipboardIndex = -1;
        
        KeySinglePressInterceptor singleInterceptor;

        public static SettingsModel Settings { get; set; } = new SettingsModel();

        public GlobalEventsHandler(PresentationSource source, IntPtr wndHandler)
        {
            clipboardDisplayer = new ClipboardView(Settings, extendedClipboard);

            WndHandler = wndHandler;
            Source = source;
            singleInterceptor = new KeySinglePressInterceptor(source);

            singleInterceptor.HookedKeys.Add(Key.V);
            singleInterceptor.HookedKeys.Add(Key.LeftCtrl);
            singleInterceptor.HookedKeys.Add(Key.RightCtrl);

            singleInterceptor.KeyUp += KeyUpKeyHandle;
            singleInterceptor.KeyDown += KeyDownKeyHandle;

            Copy();
        }

        public bool SwitchMode()
        {
            if (!isRunning)
            {
                AddClipboardFormatListener(WndHandler);
                RegisterHotKey(WndHandler, (int)HotKeyId.PASTE, CONTROL, VK_V);
                isRunning = true;
                return true;
            }

            else
            {
                RemoveClipboardFormatListener(WndHandler);
                UnregisterHotKey(WndHandler, (int)HotKeyId.PASTE);
                isRunning = false;
                return false;
            }
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
            clipboardDisplayer.Close();
        }

        void Copy()
        {
            if (Clipboard.ContainsText())
            {
                extendedClipboard.AddFirst(new ClipboardData(Clipboard.GetText(), false));
            }

            else if (Clipboard.ContainsFileDropList())
            {
                string fileNames = string.Empty;
                foreach (var fileName in Clipboard.GetFileDropList())
                    fileNames += fileName + '\n';

                extendedClipboard.AddFirst(new ClipboardData(Clipboard.GetDataObject(), true, fileNames));
            }

            while (extendedClipboard.Count > Settings.MaxSize)
                extendedClipboard.RemoveLast();
        }

        void PasteStart()
        {
            if (!timer.IsRunning)
            {
                timer.Start();
                keyNextHandled = false;
                WaitForDisplayExtendedClipboard();
            }
        }

        async void WaitForDisplayExtendedClipboard()
        {
            await Task.Run(() =>
            {
                lock (timer)
                {
                    while (timer.ElapsedMilliseconds < Settings.TimeToElapse) ;
                }
            });

            if (timer.IsRunning)
                clipboardDisplayer.DisplayClipboard();
        }

        void SetNextElement()
        {
            if (extendedClipboard.Count > 0 && !keyNextHandled
                && timer.ElapsedMilliseconds >= Settings.TimeToElapse)
            {
                if (++clipboardIndex >= extendedClipboard.Count)
                    clipboardIndex = 0;

                clipboardDisplayer.SetNextElement(clipboardIndex);

                keyNextHandled = true;
            }
        }

        void PasteStop()
        {
            if (timer.IsRunning)
            {
                timer.Reset();

                UnregisterHotKey(WndHandler, (int)HotKeyId.PASTE);
                singleInterceptor.UnHook();
                RemoveClipboardFormatListener(WndHandler);

                IClipboardData currentElement = null;

                if (clipboardIndex >= 0)
                {
                    currentElement = extendedClipboard.ElementAt(clipboardIndex);
                    if (!extendedClipboard.ElementAt(clipboardIndex).IsLinkOrLinks())
                        Clipboard.SetText((string)currentElement.GetData());
                    else
                        Clipboard.SetDataObject(currentElement.GetData());
                }
                inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

                AddClipboardFormatListener(WndHandler);
                singleInterceptor.Hook();
                RegisterHotKey(WndHandler, (int)HotKeyId.PASTE, CONTROL, VK_V);

                clipboardDisplayer.CloseClipboard();
                clipboardDisplayer.ClearBackground();

                if (currentElement != null)
                {
                    extendedClipboard.Remove(currentElement);
                    extendedClipboard.AddFirst(currentElement);
                }
                clipboardIndex = -1;
            }
        }

        void KeyUpKeyHandle(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            switch (key)
            {
                case Key.V:
                    if (timer.ElapsedMilliseconds < Settings.TimeToElapse)
                        PasteStop();
                    keyNextHandled = false;
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
