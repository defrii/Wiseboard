using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Wiseboard.Annotations;

namespace Wiseboard.Models.Settings
{
    public class GeneralSettingsModel : INotifyPropertyChanged
    {
        private int _maxSize = 5;
        public int MaxSize
        {
            get { return _maxSize; }
            set
            {
                _maxSize = value;
                OnPropertyChanged(nameof(MaxSize));
            }
        }

        private int _timeToElapse = 600;
        public int TimeToElapse
        {
            get { return _timeToElapse; }
            set
            {
                _timeToElapse = value;
                OnPropertyChanged(nameof(TimeToElapse));
            }
        }

        private bool _isAutostart = false;
        public bool IsAutostart
        {
            get { return _isAutostart; }
            set
            {
                if (_isAutostart == value) return;

                _isAutostart = value;
                OnPropertyChanged(nameof(IsAutostart));

                if (_isAutostart)
                    RegisterOnStartUp();
                else
                    UnregisterOnStartUp();
            }
        }

        private bool _isShortcutActivated = false;
        public bool IsShortcutActivated
        {
            get { return _isShortcutActivated; }
            set
            {
                _isShortcutActivated = value;
                OnPropertyChanged(nameof(IsShortcutActivated));
            }
        }

        private bool _isChangingCombination = false;
        public bool IsChangingCombination
        {
            get { return _isChangingCombination; }
            set
            {
                _isChangingCombination = value;
                OnPropertyChanged(nameof(IsChangingCombination));
            }
        }

        private string _combination;
        public string Combination
        {
            get { return _combination; }
            set
            {
                _combination = value;
                OnPropertyChanged(nameof(Combination));
            }
        }

        public Key ShortcutKey;
        public int ShortcutModifiers;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RegisterOnStartUp()
        {
            using (RegistryKey key = Registry.CurrentUser
                .OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().Location);
            }
        }

        private void UnregisterOnStartUp()
        {
            using (RegistryKey key = Registry.CurrentUser
                .OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue(Assembly.GetExecutingAssembly().GetName().Name, false);
            }
        }

    }
}