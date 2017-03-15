using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Wiseboard.Models.Settings;

namespace Wiseboard.Models 
{
    public class SettingsModel
    {
        public GeneralSettingsModel GeneralSettingsModel { get; set; } = new GeneralSettingsModel();
        public AppearanceSettingsModel AppearanceSettingsModel { get; set; } = new AppearanceSettingsModel();

        private XElement _config;

        public SettingsModel()
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
            GeneralSettingsModel.MaxSize = int.Parse(general.Element("max_size").Value);
            GeneralSettingsModel.TimeToElapse = int.Parse(general.Element("time_to_elapse").Value);
            GeneralSettingsModel.IsAutostart = bool.Parse(general.Element("is_autostart").Value);
            GeneralSettingsModel.IsShortcutActivated = bool.Parse(general.Element("is_shortcut_activated").Value);

            XElement shortcut = general.Element("shortcut");
            GeneralSettingsModel.ShortcutKey = (Key)int.Parse(shortcut.Element("key").Value);
            GeneralSettingsModel.ShortcutModifiers = int.Parse(shortcut.Element("modifiers").Value);
            GeneralSettingsModel.Combination = ConvertCombinationToString();
        }

        private void ReadAppearanceSettings()
        {
            XElement appearance = _config.Element("appearance");
            AppearanceSettingsModel.RectangleSize = int.Parse(appearance.Element("rectangle_size").Value);
            AppearanceSettingsModel.Font = new FontFamily(appearance.Element("font").Value);
            AppearanceSettingsModel.FontSize = int.Parse(appearance.Element("font_size").Value);
        }

        public void UpdateConfiguration()
        {
            SaveConfigToFile();
        }

        private void SaveConfigToFile()
        {
            _config = new XElement("configuration",
            new XElement("general",
                new XElement("max_size", GeneralSettingsModel.MaxSize),
                new XElement("time_to_elapse", GeneralSettingsModel.TimeToElapse),
                new XElement("is_autostart", GeneralSettingsModel.IsAutostart),
                new XElement("is_shortcut_activated", GeneralSettingsModel.IsShortcutActivated),
                new XElement("shortcut",
                    new XElement("key", (int)GeneralSettingsModel.ShortcutKey),
                    new XElement("modifiers", GeneralSettingsModel.ShortcutModifiers)
                )),
            new XElement("appearance",
                new XElement("rectangle_size", AppearanceSettingsModel.RectangleSize),
                new XElement("font", AppearanceSettingsModel.Font),
                new XElement("font_size", AppearanceSettingsModel.FontSize))
            );
            _config.Save(AppDomain.CurrentDomain.BaseDirectory + "\\config.xml");
        }

        private string ConvertCombinationToString()
        {
            string combination = "";
            int modifiers = GeneralSettingsModel.ShortcutModifiers;
            for (int i = 1; i <= 8; i*=2)
            {
                int tp = modifiers & i;
                if (tp != 0)
                    combination += (ModifierKeys)i + "+";
            }
            combination += GeneralSettingsModel.ShortcutKey;
            return combination;
        }
    }
}
