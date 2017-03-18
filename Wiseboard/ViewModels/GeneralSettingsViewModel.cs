using GalaSoft.MvvmLight;
using Wiseboard.Models;

namespace Wiseboard.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        public GeneralSettingsModel GeneralSettingsModel { get; set; } = new GeneralSettingsModel();
    }
}