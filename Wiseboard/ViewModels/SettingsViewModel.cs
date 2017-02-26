using Wiseboard.Models;
using System;
using System.Windows.Input;

namespace Wiseboard.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsModel SettingsModel { get; set; } = GlobalEventsHandler.Settings;
    }
}
