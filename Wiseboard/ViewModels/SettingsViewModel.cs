using Wiseboard.Models;
using System;

namespace Wiseboard.ViewModels
{
    class SettingsViewModel
    {
        public SettingsModel SettingsModel { get; set; } = GlobalEventsHandler.Settings;
    }
}
