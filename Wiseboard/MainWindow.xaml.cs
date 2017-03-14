using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Wiseboard.Handlers;
using Wiseboard.Observers;
using Wiseboard.Views;

namespace Wiseboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IChangedStatusObserver
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
            _pastingHandler.AddObserver(this);

            HwndSource sourceHandler = (HwndSource) source;
            sourceHandler?.AddHook(_pastingHandler.CaptureKeyCombinations);

            VerifyRunButtonContent();
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

        private void VerifyRunButtonContent()
        {
            RunButton.Content = _pastingHandler.Running ? "Running..." : "Click to run";
        }

        public void UpdateStatus(bool status)
        {
            RunButton.Content = status ? "Running..." : "Click to run";
        }
    }
}