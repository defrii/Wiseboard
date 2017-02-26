using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wiseboard.Models;
namespace Wiseboard.Views
{
    /// <summary>
    /// Interaction logic for ClipboardDisplayer.xaml
    /// </summary>
    public partial class ClipboardView : Window
    {
        readonly LinearGradientBrush _brush;
        readonly SettingsModel _settings;
        readonly LinkedList<IClipboardData> _extendedClipboard;

        public ClipboardView(SettingsModel settings, LinkedList<IClipboardData> extendedClipboard)
        {
            InitializeComponent();

            _settings = settings;
            _extendedClipboard = extendedClipboard;

            _brush = new LinearGradientBrush(Color.FromRgb(80, 80, 80), Color.FromRgb(120, 120, 120), 0) {Opacity = 0.8};

            Show();
            Visibility = Visibility.Hidden;
        }

        public void DisplayClipboard()
        {
            clipboardStack.Children.Clear();
            foreach (var clip in _extendedClipboard)
            {
                TextBlock block = new TextBlock
                {
                    Text = clip.GetVisibleText(),
                    Width = _settings.RectangleWidth,
                    Height = _settings.RectangleWidth,
                    FontFamily = _settings.Font,
                    FontSize = _settings.FontSize,
                    Foreground = Brushes.Bisque,
                    Padding = new Thickness(10, 10, 10, 10),
                    TextAlignment = TextAlignment.Justify,
                    TextWrapping = TextWrapping.Wrap
                };

                if (clip.IsLinkOrLinks())
                {
                    block.FontStyle = FontStyles.Italic;
                    block.FontWeight = FontWeights.Bold;
                }

                block.Background = _brush;
                clipboardStack.Children.Add(block);
            }

            CenterWindow();
            Visibility = Visibility.Visible;
        }

        public void SetNextElement(int index)
        {
            ClearBackground();

            var element = clipboardStack.Children.OfType<TextBlock>().ToArray()[index];
            element.Background = Brushes.Coral;
        }

        public void ClearBackground()
        {
            var elements = clipboardStack.Children.OfType<TextBlock>().ToArray();
            foreach (var element in elements)
                element.Background = _brush;
        }

        void CenterWindow()
        {
            int count = clipboardStack.Children.OfType<TextBlock>().Count();
            if (count > 0)
            {
                double systemWidth = SystemParameters.PrimaryScreenWidth;
                double systemHeight = SystemParameters.PrimaryScreenHeight;

                Left = systemWidth / 2 - (clipboardStack.Children.OfType<TextBlock>().First().Width * count) / 2;
                Top = systemHeight / 2 - clipboardStack.Children.OfType<TextBlock>().First().Height / 2;
            }
        }
        
        public void CloseClipboard()
        {
            Visibility = Visibility.Hidden;
        }

    }
}
