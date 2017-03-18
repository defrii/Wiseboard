using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Wiseboard.Handlers;
using Wiseboard.Models;
using Wiseboard.ViewModels;

namespace Wiseboard.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        readonly SettingsHandler _settingsHandler = new SettingsHandler();

        private readonly GeneralSettingsModel _generalSettingsModel =
            ServiceLocator.Current.GetInstance<GeneralSettingsViewModel>().GeneralSettingsModel;
        public SettingsView()
        {
            InitializeComponent();
        }

        public new void Show()
        {
            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Left = screen.Bounds.Left;
            Top = screen.Bounds.Top;
            base.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _settingsHandler.UpdateConfiguration();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!_generalSettingsModel.IsChangingCombination) return;

            string combination = string.Join("+", Keyboard.Modifiers.ToString().Split(',')).Replace(" ", "");
            if ((int) e.Key < 44 || (int) e.Key > 69 || combination == "None") return;

            combination = combination + "+" + e.Key;
            CombinationTextBox.Text = combination;
            _generalSettingsModel.ShortcutKey = e.Key;
            _generalSettingsModel.ShortcutModifiers = (int)Keyboard.Modifiers;
        }

        private void OnClickChangeCombinationButton(object sender, RoutedEventArgs e)
        {
            _generalSettingsModel.IsChangingCombination = true;
        }

        private void OnCombinationChanged(object sender, TextChangedEventArgs e)
        {
            _generalSettingsModel.IsChangingCombination = false;
        }
    }
}