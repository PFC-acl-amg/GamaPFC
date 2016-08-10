using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
