using Gama.Common.Views;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gama.Common.BaseClasses
{
    public class UnityBootstrapperBase : UnityBootstrapper
    {
        protected bool _CLEAR_DATABASE = false;
        protected bool _SEED_DATABASE = false;
        protected Thread _PreloadThread;
        protected SplashScreenView _PreloaderView;
        private string _Title;

        public UnityBootstrapperBase(string title)
        {
            _Title = title;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            LanzarPreloader();
        }

        protected virtual void LanzarPreloader()
        {
            _PreloadThread = new Thread(PreLoad);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();

            Thread.Sleep(200); // Para dar tiempo al nuevo hilo a crear la vista.

            lock (_PreloaderView)
            {
                _PreloaderView.Next(); InitializeDirectories(); 
                _PreloaderView.Next(); ConfigurePreferences(); 
                _PreloaderView.Next(); RegisterServices();
                _PreloaderView.Next(); GenerateDatabaseConfiguration();
                _PreloaderView.Next();
            }
        }

        protected virtual void PreLoad()
        {
            _PreloaderView = new SplashScreenView();
            _PreloaderView.ProductName = _Title;
            _PreloaderView.ShowDialog();
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        protected virtual void TerminarPreload()
        {
            _PreloaderView.Dispatcher.Invoke((Action)delegate { _PreloaderView.Close(); });
            _PreloadThread.Abort();
        }

        protected virtual void InitializeDirectories() { }
        protected virtual void ConfigurePreferences() { }
        protected virtual void RegisterServices() { }
        protected virtual void GenerateDatabaseConfiguration() { }
    }
}
