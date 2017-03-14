using Wiseboard.Handlers;
using Wiseboard.Models;

namespace Wiseboard.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsModel SettingsModel { get; set; } = PastingHandler.Settings;
    }
}
