using GalaSoft.MvvmLight;
using Wiseboard.Models;

namespace Wiseboard.ViewModels
{
    public class AppearanceSettingsViewModel : ViewModelBase
    {
        public AppearanceSettingsModel AppearanceSettingsModel { get; set; } = new AppearanceSettingsModel();
    }
}