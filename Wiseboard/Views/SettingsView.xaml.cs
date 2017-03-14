using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wiseboard.ViewModels;

namespace Wiseboard.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsViewModel ViewModel { get; set; } = new SettingsViewModel();

        public SettingsView()
        {
            InitializeComponent();
            DataContext = ViewModel;
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
            ViewModel.SettingsModel.UpdateConfiguration();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!ViewModel.SettingsModel.GeneralSettingsModel.IsChangingCombination) return;

            string combination = string.Join("+", Keyboard.Modifiers.ToString().Split(',')).Replace(" ", "");
            if ((int) e.Key < 44 || (int) e.Key > 69 || combination == "None") return;

            combination = combination + "+" + e.Key;
            CombinationTextBox.Text = combination;
            ViewModel.SettingsModel.GeneralSettingsModel.ShortcutKey = e.Key;
            ViewModel.SettingsModel.GeneralSettingsModel.ShortcutModifiers = (int)Keyboard.Modifiers;
        }

        private void OnClickChangeCombinationButton(object sender, RoutedEventArgs e)
        {
            ViewModel.SettingsModel.GeneralSettingsModel.IsChangingCombination = true;
        }

        private void OnCombinationChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SettingsModel.GeneralSettingsModel.IsChangingCombination = false;
        }
    }
}