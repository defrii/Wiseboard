using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wiseboard.Handlers;
using Wiseboard.Views;

namespace Wiseboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PastingHandler _pastingHandler;
        readonly System.Windows.Forms.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.Icon.Handle, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            var contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Exit");
            contextMenu.MenuItems[0].Click += (sender, e) => Close();

            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = Properties.Resources.Icon,
                ContextMenu = contextMenu,
                Visible = true
            };

            _notifyIcon.Click += (sender, e) => DisplayFromMinimized();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            PresentationSource source = PresentationSource.FromVisual(this);

            IntPtr wndHandler = new WindowInteropHelper(this).Handle;
            _pastingHandler = new PastingHandler(source, wndHandler);

            HwndSource sourceHandler = (HwndSource)source;
            sourceHandler?.AddHook(_pastingHandler.CaptureKeyCombinations);
        }

        void DisplayFromMinimized()
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _pastingHandler.CloseClipboardView();
            _pastingHandler.UnregisterAll();
            _notifyIcon.Visible = false;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunButton.Content = _pastingHandler.SwitchMode() ? "Running..." : "Click to run";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView settingsView = new SettingsView();
            settingsView.Show();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutView aboutView = new AboutView();
            aboutView.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
