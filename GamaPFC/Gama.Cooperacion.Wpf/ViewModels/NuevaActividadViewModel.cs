using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class NuevaActividadViewModel : ObservableObject
    {
        private IActividadRepository _actividadRepository;
        public Actividad Actividad { get; set; }

        public NuevaActividadViewModel(IActividadRepository actividadRepository)
        {
            _actividadRepository = actividadRepository;

            Actividad = new Actividad();
            Actividad.Titulo = "Nombre del proyecto...";
            Actividad.Descripcion = "Descripción del proyecto...";

            AceptarCommand = new DelegateCommand(OnAceptar, OnAceptar_CanExecute);
        }

        ICommand AceptarCommand { get; set; }

        private void OnAceptar()
        {
            _actividadRepository.Create(Actividad);
        }

        private bool OnAceptar_CanExecute()
        {
            return (String.IsNullOrEmpty(Actividad.Titulo) &&
                String.IsNullOrEmpty(Actividad.Descripcion));
        }
    }
}
