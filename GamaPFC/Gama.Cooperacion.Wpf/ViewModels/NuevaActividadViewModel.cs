using Core;
using Gama.Cooperacion.Business;
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
        public Actividad Actividad { get; set; }

        public NuevaActividadViewModel()
        {
            Actividad = new Actividad();
            Actividad.Titulo = "Nombre del proyecto...";
            Actividad.Descripcion = "Descripción del proyecto...";

            AceptarCommand = new DelegateCommand(OnAceptar, OnAceptar_CanExecute);
        }

        ICommand AceptarCommand { get; set; }

        private void OnAceptar()
        {
            throw new NotImplementedException();
        }

        private bool OnAceptar_CanExecute()
        {
            return (String.IsNullOrEmpty(Actividad.Titulo) &&
                String.IsNullOrEmpty(Actividad.Descripcion));
        }
    }
}
