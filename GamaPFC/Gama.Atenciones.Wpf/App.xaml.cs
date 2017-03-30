using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Gama.Atenciones.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IUnityContainer _container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                       typeof(FrameworkElement),
                       new FrameworkPropertyMetadata(
                           XmlLanguage.GetLanguage(
               CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                type => {
                    return _container.Resolve(type);
                });

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
