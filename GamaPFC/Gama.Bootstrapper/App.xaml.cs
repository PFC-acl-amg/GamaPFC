using Gama.Bootstrapper.Services;
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

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                type => { return _container.Resolve(type);
            });

            Bootstrapper bootstrapper;

            bool SALTAR_SELECCION_DE_MODULO = true; // Para hacer pruebas más rápido...
            if (SALTAR_SELECCION_DE_MODULO)
            {
                bootstrapper = new Bootstrapper(Modulos.Cooperacion);
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
                    return;

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