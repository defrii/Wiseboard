using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Wiseboard.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();

            Logo.Source = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.Icon.Handle, Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
            VersionLabel.Content = "Version " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        public new void Show()
        {
            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Left = screen.Bounds.Left;
            Top = screen.Bounds.Top;
            base.Show();
        }
    }
}
