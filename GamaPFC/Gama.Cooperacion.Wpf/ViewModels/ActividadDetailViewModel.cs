using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;
using Prism.Regions;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ActividadDetailViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;
        private InformacionDeActividadViewModel _ActividadVM;

        public ActividadWrapper Actividad { get; set; }

        public ActividadDetailViewModel(IActividadRepository actividadRepository,
            InformacionDeActividadViewModel actividadVM)
        {
            _actividadRepository = actividadRepository;
            _ActividadVM = actividadVM;
        }

        public InformacionDeActividadViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var id = (int)navigationContext.Parameters["Id"];

            if (Actividad != null && Actividad.Id == id)
                return true;

            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            try {
                Actividad = new ActividadWrapper(
                    _actividadRepository.GetById((int)navigationContext.Parameters["Id"]));
            } catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            _ActividadVM.Setup(Actividad);

            if (Actividad.Titulo.Length > 20)
            {
                Title = Actividad.Titulo.Substring(0, 20);
            } 
            else
            {
                Title = Actividad.Titulo;
            }
        }
    }
}
