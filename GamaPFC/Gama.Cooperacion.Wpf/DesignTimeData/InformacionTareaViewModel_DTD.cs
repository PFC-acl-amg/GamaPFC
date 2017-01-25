using Core;
using Gama.Cooperacion.Business;
using Remotion.Linq.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.DesignTimeData
{
    public class InformacionTareaViewModel_DTD : ViewModelBase
    {
        public InformacionTareaViewModel_DTD()
        {
            Incidencia = new ObservableCollection<Mensaje>();
            Incidencia.Add(new Mensaje() { Titulo = "Incidencia 1", FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15) });
            Incidencia.Add(new Mensaje() { Titulo = "Incidencia 2", FechaDePublicacion = new DateTime(2009, 8, 29, 19, 27, 15) });
            Incidencia.Add(new Mensaje() { Titulo = "Incidencia 3", FechaDePublicacion = new DateTime(2010, 8, 29, 19, 27, 15) });
            Incidencia.Add(new Mensaje() { Titulo = "Incidencia 4", FechaDePublicacion = new DateTime(2011, 8, 29, 19, 27, 15) });

        }
        
        public ObservableCollection<Mensaje> Incidencia { get; private set; }
    }
}
