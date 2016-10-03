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
        //SelectorDeModulo _SelectorDeModulo;
        DispatcherTimer _Timer = new DispatcherTimer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                type => {
                        return _container.Resolve(type);
                });

            Bootstrapper bootstrapper;

            // Para poder hacer pruebas más rápido...
            bool SALTAR_SELECCION_DE_MODULO = true;

            //System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (SALTAR_SELECCION_DE_MODULO)
            {
                bootstrapper = new Bootstrapper(Modulos.ServicioDeAtenciones);
                //bootstrapper.Run();
                //_Timer.Tick += Timer_Tick;
                //_Timer.Interval = new TimeSpan(2000);
                //_SelectorDeModulo.Show();
                //_Timer.Start();
                //var task = new Task(new Action(() => bootstrapper.Run()));
                //task.Wait(2000);
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

                if (vm.ModuloSeleccionado == null || !vm.SeHaAccedido)
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
                //selectorDeModulo.Show();
                //var task = new Task(new Action(() => bootstrapper.Run()));
                //task.Wait();
                //selectorDeModulo.Close();
                bootstrapper.Run();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //_SelectorDeModulo.Close();
            //_Timer.Stop();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
