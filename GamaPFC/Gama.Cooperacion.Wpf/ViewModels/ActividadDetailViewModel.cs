using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;
using Prism.Regions;
using Gama.Cooperacion.Wpf.Services;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ActividadDetailViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;

        public Actividad Actividad { get; set; }

        public ActividadDetailViewModel(IActividadRepository actividadRepository)
        {
            _actividadRepository = actividadRepository;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Actividad = _actividadRepository.GetById((int)navigationContext.Parameters["Id"]);

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
