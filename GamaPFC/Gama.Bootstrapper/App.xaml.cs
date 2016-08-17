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
                    return _container.Resolve(type);
                });

            var bootstrapper = new Bootstrapper(Modulos.ServicioDeAtenciones);
            bootstrapper.Run();
        }
    }
}
