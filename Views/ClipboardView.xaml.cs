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
        LinearGradientBrush brush;
        SettingsModel Settings;
        LinkedList<IClipboardData> extendedClipboard;

        public ClipboardView(SettingsModel settings, LinkedList<IClipboardData> extendedClipboard)
        {
            InitializeComponent();

            Settings = settings;
            this.extendedClipboard = extendedClipboard;

            brush = new LinearGradientBrush(Color.FromRgb(80, 80, 80), Color.FromRgb(120, 120, 120), 0);
            brush.Opacity = 0.8;

            Show();
            Visibility = Visibility.Hidden;
        }

        public void DisplayClipboard()
        {
            clipboardStack.Children.Clear();
            foreach (var clip in extendedClipboard)
            {
                TextBlock block = new TextBlock() { Text = clip.GetVisibleText() };
                block.Width = Settings.RectangleWidth;
                block.Height = Settings.RectangleWidth;
                block.FontFamily = Settings.Font;
                block.FontSize = Settings.FontSize;
                block.Foreground = Brushes.Bisque;
                block.Padding = new Thickness(10, 10, 10, 10);
                block.TextAlignment = TextAlignment.Justify;
                block.TextWrapping = TextWrapping.Wrap;

                block.Background = brush;
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
                element.Background = brush;
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
