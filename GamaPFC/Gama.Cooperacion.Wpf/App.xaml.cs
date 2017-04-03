using Gama.Common.Views;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Gama.Cooperacion.Wpf
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

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                type => {
                    return _container.Resolve(type);
                });

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var o = new ExceptionMessageView();
            o.DataContext = e.ExceptionObject;
            o.ShowDialog();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var o = new ExceptionMessageView();
            o.DataContext = e.Exception.GetBaseException();
            o.ShowDialog();
            e.Handled = true;

            //string errorMessage = string.Format("¡Oops! Algo ha salido mal: {0}", e.Exception.Message);
            //MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //e.Handled = true;
        }
    }
}
