using Wiseboard.Models;
using Wiseboard.Views;
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

namespace Wiseboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GlobalEventsHandler globalEventsHandler;
        System.Windows.Forms.NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();

            Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.Icon.Handle, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            var contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Exit");
            contextMenu.MenuItems[0].Click += (object sender, EventArgs e) => Close();

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = Properties.Resources.Icon;
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;

            notifyIcon.Click += (object sender, EventArgs e) => DisplayFromMinimized();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            PresentationSource source = PresentationSource.FromVisual(this);

            IntPtr wndHandler = new WindowInteropHelper(this).Handle;
            globalEventsHandler = new GlobalEventsHandler(source, wndHandler);

            HwndSource sourceHandler = (HwndSource)source;
            sourceHandler.AddHook(globalEventsHandler.CaptureKeyCombinations);
        }

        void DisplayFromMinimized()
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            globalEventsHandler.CloseClipboardView();
            notifyIcon.Visible = false;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (globalEventsHandler.SwitchMode())
            {
                RunButton.Content = "Running...";
            }
            else
                RunButton.Content = "Click to run";
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
