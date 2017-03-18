using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Wiseboard.ViewModels
{
    public class ViewModelLocator
    {
        public static void Configure()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<GeneralSettingsViewModel>();
            SimpleIoc.Default.Register<AppearanceSettingsViewModel>();
        }

        public GeneralSettingsViewModel GeneralSettings
            => ServiceLocator.Current.GetInstance<GeneralSettingsViewModel>();

        public AppearanceSettingsViewModel AppearanceSettings
            => ServiceLocator.Current.GetInstance<AppearanceSettingsViewModel>();

        
        public static void Cleanup()
        {
            
        }
    }
}