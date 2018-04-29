using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Wiseboard.Data;
using Wiseboard.Models;
using Wiseboard.ViewModels;

namespace Wiseboard.Views
{
    /// <summary>
    /// Interaction logic for ClipboardDisplayer.xaml
    /// </summary>
    public partial class ClipboardView : Window
    {
        private readonly LinearGradientBrush _brush;

        private readonly AppearanceSettingsModel _settings =
            ServiceLocator.Current.GetInstance<AppearanceSettingsViewModel>().AppearanceSettingsModel;
        private readonly LinkedList<IClipboardData> _extendedClipboard;

        public ClipboardView(LinkedList<IClipboardData> extendedClipboard)
        {
            InitializeComponent();

            _extendedClipboard = extendedClipboard;

            _brush = new LinearGradientBrush(Color.FromRgb(80, 80, 80), Color.FromRgb(120, 120, 120), 0) {Opacity = 0.8};

            Show();
            Visibility = Visibility.Hidden;
        }

        public void DisplayClipboard()
        {
            clipboardStack.Children.Clear();
            if (_extendedClipboard.Count == 0) return;

            foreach (var clip in _extendedClipboard)
            {
                TextBlock block = new TextBlock
                {
                    Text = clip.GetVisibleText(),
                    Width = _settings.RectangleSize,
                    Height = _settings.RectangleSize,
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
            SetNextElement(0);
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

        private void CenterWindow()
        {
            int count = clipboardStack.Children.OfType<TextBlock>().Count();
            if (count > 0)
            {
                double scale;
                try { scale = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11; }
                catch (NullReferenceException) { scale = 1.0; }

                var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
                Left = (screen.WorkingArea.Left + screen.WorkingArea.Width / 2) / (1 * scale) -
                       (clipboardStack.Children.OfType<TextBlock>().First().Width * count) / 2;
                Top = (screen.WorkingArea.Top + screen.WorkingArea.Height / 2) / (1 * scale) -
                      clipboardStack.Children.OfType<TextBlock>().First().Height / 2;
            }
        }
        
        public void CloseClipboard()
        {
            Visibility = Visibility.Hidden;
        }

    }
}
