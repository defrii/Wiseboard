using System;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Collections;

namespace Wiseboard.Models 
{
    public class SettingsModel : INotifyPropertyChanged
    {
        int _maxSize = 5;
        public int MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                _maxSize = value;
                OnPropertyChanged("MaxSize");
            }
        }

        int _rectangleWidth = 200;
        public int RectangleWidth
        {
            get
            {
                return _rectangleWidth;
            }
            set
            {
                _rectangleWidth = value;
                OnPropertyChanged("RectangleWidth");
            }
        }

        public IOrderedEnumerable<FontFamily> FontNames { get; set; } = Fonts.SystemFontFamilies.OrderBy(n => n.ToString());

        FontFamily _font = new FontFamily("Arial");
        public FontFamily Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                OnPropertyChanged("Font");
            }
        }

        int _fontSize = 12;
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        int _timeToElapse = 600;
        public int TimeToElapse
        {
            get
            {
                return _timeToElapse;
            }
            set
            {
                _timeToElapse = value;
                OnPropertyChanged("TimeToElapse");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
