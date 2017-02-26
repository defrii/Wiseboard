using Wiseboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Wiseboard.Models;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel.SettingsModel.UpdateConfiguration();
        }
    }
}
