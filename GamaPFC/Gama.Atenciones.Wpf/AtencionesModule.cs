using Core;
using Gama.Atenciones.DataAccess;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf
{
    public class AtencionesModule : ModuleBase
    {
        public AtencionesModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.UseFaker = false;
        }

        public override void Initialize()
        {
            RegisterViews();
            RegisterViewModels();
            RegisterServices();
            InitializeNavigation();

            try {
                var sessionFactory = new NHibernateSessionFactory();
                var factory = sessionFactory.GetSessionFactory();
            } 
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        private void RegisterViews()
        {
            //throw new NotImplementedException();
        }

        private void RegisterViewModels()
        {
            //throw new NotImplementedException();
        }

        private void RegisterServices()
        {

            //throw new NotImplementedException();
        }

        private void InitializeNavigation()
        {
            //throw new NotImplementedException();
        }
    }
}
