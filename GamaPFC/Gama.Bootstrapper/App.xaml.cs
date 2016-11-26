using Gama.Bootstrapper.Services;
using Gama.Bootstrapper.Views;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Gama.Bootstrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IUnityContainer _container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                type => { return _container.Resolve(type);
            });

            Bootstrapper bootstrapper;

            bool SALTAR_SELECCION_DE_MODULO = false; // Para hacer pruebas más rápido...
            if (SALTAR_SELECCION_DE_MODULO)
            {
                bootstrapper = new Bootstrapper(Modulos.ServicioDeAtenciones);
                bootstrapper.Run();
            }
            else
            {
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                var selectorDeModulo = new SelectorDeModulo();
                var vm = new SelectorDeModuloViewModel(new LoginService());
                selectorDeModulo.DataContext = vm;
                selectorDeModulo.ShowDialog();

                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                if (!vm.SeHaAccedido)
                {
                    Application.Current.Shutdown();
                }

                switch (vm.ModuloSeleccionado)
                {
                    case Modulos.Cooperacion:
                        bootstrapper = new Bootstrapper(Modulos.Cooperacion);
                        break;
                    case Modulos.ServicioDeAtenciones:
                        bootstrapper = new Bootstrapper(Modulos.ServicioDeAtenciones);
                        break;
                    case Modulos.GestionDeSocios:
                        bootstrapper = new Bootstrapper(Modulos.GestionDeSocios);
                        break;
                    default:
                        throw new Exception("¡No se ha seleccionado ningún módulo!");
                }

                bootstrapper.Run();
            }
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            var o = new ExceptionMessageView();
            o.DataContext = e.Exception.GetBaseException();
            o.ShowDialog();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var o = new ExceptionMessageView();
            o.DataContext =  e.ExceptionObject;
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

//ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(
//    viewType => {
//        var viewName = viewType.FullName;

//        // A veces las vistas terminan con 'View', otras veces no.
//        // si ya termina con 'View', se lo quitaremos para que al
//        // contrsuir el VieWModel NO nos quede 'NombreVistaViewViewModel'
//        // sino 'NombreVistaViewModel'.
//        if (viewName.EndsWith("View"))
//        {
//            viewName = viewName.Substring(0, viewName.Length - "View".Length);
//        }

//        var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
//        var viewModelName = String.Format(CultureInfo.InvariantCulture,
//            "{0}ViewModel, {1}", viewName, viewAssemblyName);
//        return Type.GetType(viewModelName);
//    });