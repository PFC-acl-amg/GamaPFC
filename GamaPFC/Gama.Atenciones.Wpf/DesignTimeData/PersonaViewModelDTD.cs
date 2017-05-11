using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.FakeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class PersonaViewModelDTD
    {
        public Persona Persona { get; set; }

        public PersonaViewModelDTD()
        {
            Persona = new FakePersonaRepository().GetAll().First();
        }
    }
}
