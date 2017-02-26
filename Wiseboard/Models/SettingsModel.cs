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
        int maxSize = 5;
        public int MaxSize
        {
            get
            {
                return maxSize;
            }
            set
            {
                maxSize = value;
                OnPropertyChanged("MaxSize");
            }
        }

        int rectangleWidth = 200;
        public int RectangleWidth
        {
            get
            {
                return rectangleWidth;
            }
            set
            {
                rectangleWidth = value;
                OnPropertyChanged("RectangleWidth");
            }
        }

        public IOrderedEnumerable<FontFamily> FontNames { get; set; } = Fonts.SystemFontFamilies.OrderBy(n => n.ToString());

        FontFamily font = new FontFamily("Arial");
        public FontFamily Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                OnPropertyChanged("Font");
            }
        }

        int fontSize = 12;
        public int FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        int timeToElapse = 600;
        public int TimeToElapse
        {
            get
            {
                return timeToElapse;
            }
            set
            {
                timeToElapse = value;
                OnPropertyChanged("TimeToElapse");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
