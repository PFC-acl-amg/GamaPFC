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

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ActividadDetailViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;
        private ActividadInformacionBasicaViewModel _ActividadVM;

        public ActividadWrapper Actividad { get; set; }

        public ActividadDetailViewModel(IActividadRepository actividadRepository,
            ActividadInformacionBasicaViewModel actividadVM)
        {
            _actividadRepository = actividadRepository;
            _ActividadVM = actividadVM;
        }

        public ActividadInformacionBasicaViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Actividad = new ActividadWrapper(
                _actividadRepository.GetById((int)navigationContext.Parameters["Id"]));

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
