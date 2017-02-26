using System;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Collections;
using System.Xml.Linq;

namespace Wiseboard.Models 
{
    public class SettingsModel : INotifyPropertyChanged
    {
        XElement _config;

        public SettingsModel()
        {
            try
            {
                _config = XElement.Load("config.xml");
                ReadFromXml();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot load config.xml");
                UpdateConfiguration();
            }
        }

        void ReadFromXml()
        {
            MaxSize = int.Parse(_config.Element("max_size").Value);
            RectangleWidth = int.Parse(_config.Element("rectangle_width").Value);
            Font = new FontFamily(_config.Element("font").Value);
            FontSize = int.Parse(_config.Element("font_size").Value);
            TimeToElapse = int.Parse(_config.Element("time_to_elapse").Value);
        }

        public void UpdateConfiguration()
        {
            _config = new XElement("configuration",
                     new XElement("max_size", _maxSize),
                     new XElement("rectangle_width", _rectangleWidth),
                     new XElement("font", _font.ToString()),
                     new XElement("font_size", _fontSize),
                     new XElement("time_to_elapse", _timeToElapse)
                );
            _config.Save("config.xml");
        }

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
