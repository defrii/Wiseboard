using Wiseboard.Handlers;
using System;
using System.Windows.Input;
using Wiseboard.Models;

namespace Wiseboard.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsModel SettingsModel { get; set; } = PastingHandler.Settings;
    }
}
