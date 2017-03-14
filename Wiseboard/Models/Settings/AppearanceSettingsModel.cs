using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Wiseboard.Annotations;

namespace Wiseboard.Models.Settings
{
    public class AppearanceSettingsModel : INotifyPropertyChanged
    {
        int _rectangleSize = 200;
        public int RectangleSize
        {
            get { return _rectangleSize; }
            set
            {
                _rectangleSize = value;
                OnPropertyChanged(nameof(RectangleSize));
            }
        }

        FontFamily _font = new FontFamily("Arial");
        public FontFamily Font
        {
            get { return _font; }
            set
            {
                _font = value;
                OnPropertyChanged(nameof(Font));
            }
        }

        int _fontSize = 12;
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public IOrderedEnumerable<FontFamily> FontNames { get; set; } = Fonts.SystemFontFamilies.OrderBy(n => n.ToString());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}