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
using System.Windows;

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
                    try
                    {
                        var result = _container.Resolve(type);
                        return _container.Resolve(type);
                    }
                    catch (Exception ex)
                    {
                        var message = ex.Message;
                        throw ex;
                    }
                });

            Bootstrapper bootstrapper;

            //bootstrapper = new Bootstrapper(Modulos.Cooperacion);
            //bootstrapper.Run();
            try
            {
                //System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var selectorDeModulo = new SelectorDeModulo();
                selectorDeModulo.ShowDialog();
                selectorDeModulo.ModuloSeleccionado = selectorDeModulo.ModuloSeleccionado;
                Application.Current.MainWindow = null;
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                switch (selectorDeModulo.ModuloSeleccionado)
                {
                    case Modulos.Cooperacion:
                        bootstrapper = new Bootstrapper(Modulos.Cooperacion);
                        bootstrapper.Run();
                        break;
                    case Modulos.ServicioDeAtenciones:
                        bootstrapper = new Bootstrapper(Modulos.ServicioDeAtenciones);
                        bootstrapper.Run();
                        break;
                    case Modulos.GestionDeSocios:
                        bootstrapper = new Bootstrapper(Modulos.GestionDeSocios);
                        bootstrapper.Run();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //var x = 1;
            //if (x == 1)
            //{
            //bootstrapper = new Bootstrapper(Modulos.ServicioDeAtenciones);
            //bootstrapper.Run();
            //}
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
