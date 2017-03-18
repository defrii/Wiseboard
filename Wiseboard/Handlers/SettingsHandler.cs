using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Microsoft.Practices.ServiceLocation;
using Wiseboard.Models;
using Wiseboard.ViewModels;

namespace Wiseboard.Handlers 
{
    public class SettingsHandler
    {
        private readonly GeneralSettingsModel _generalSettingsModel =
            ServiceLocator.Current.GetInstance<GeneralSettingsViewModel>().GeneralSettingsModel;

        private readonly AppearanceSettingsModel _appearanceSettingsModel =
            ServiceLocator.Current.GetInstance<AppearanceSettingsViewModel>().AppearanceSettingsModel;

        private XElement _config;

        public SettingsHandler()
        {
            try
            {
                _config = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + "\\config.xml");
                ReadFromXml();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot load config.xml");
                UpdateConfiguration();
            }
        }

        private void ReadFromXml()
        {
            ReadGeneralSettings();
            ReadAppearanceSettings();
        }

        private void ReadGeneralSettings()
        {
            XElement general = _config.Element("general");
            _generalSettingsModel.MaxSize = int.Parse(general.Element("max_size").Value);
            _generalSettingsModel.TimeToElapse = int.Parse(general.Element("time_to_elapse").Value);
            _generalSettingsModel.IsAutostart = bool.Parse(general.Element("is_autostart").Value);
            _generalSettingsModel.IsShortcutActivated = bool.Parse(general.Element("is_shortcut_activated").Value);

            XElement shortcut = general.Element("shortcut");
            _generalSettingsModel.ShortcutKey = (Key)int.Parse(shortcut.Element("key").Value);
            _generalSettingsModel.ShortcutModifiers = int.Parse(shortcut.Element("modifiers").Value);
            _generalSettingsModel.Combination = ConvertCombinationToString();
        }

        private void ReadAppearanceSettings()
        {
            XElement appearance = _config.Element("appearance");
            _appearanceSettingsModel.RectangleSize = int.Parse(appearance.Element("rectangle_size").Value);
            _appearanceSettingsModel.Font = new FontFamily(appearance.Element("font").Value);
            _appearanceSettingsModel.FontSize = int.Parse(appearance.Element("font_size").Value);
        }

        public void UpdateConfiguration()
        {
            SaveConfigToFile();
        }

        private void SaveConfigToFile()
        {
            _config = new XElement("configuration",
            new XElement("general",
                new XElement("max_size", _generalSettingsModel.MaxSize),
                new XElement("time_to_elapse", _generalSettingsModel.TimeToElapse),
                new XElement("is_autostart", _generalSettingsModel.IsAutostart),
                new XElement("is_shortcut_activated", _generalSettingsModel.IsShortcutActivated),
                new XElement("shortcut",
                    new XElement("key", (int)_generalSettingsModel.ShortcutKey),
                    new XElement("modifiers", _generalSettingsModel.ShortcutModifiers)
                )),
            new XElement("appearance",
                new XElement("rectangle_size", _appearanceSettingsModel.RectangleSize),
                new XElement("font", _appearanceSettingsModel.Font),
                new XElement("font_size", _appearanceSettingsModel.FontSize))
            );
            _config.Save(AppDomain.CurrentDomain.BaseDirectory + "\\config.xml");
        }

        private string ConvertCombinationToString()
        {
            string combination = "";
            int modifiers = _generalSettingsModel.ShortcutModifiers;
            for (int i = 1; i <= 8; i*=2)
            {
                int tp = modifiers & i;
                if (tp != 0)
                    combination += (ModifierKeys)i + "+";
            }
            combination += _generalSettingsModel.ShortcutKey;
            return combination;
        }
    }
}
