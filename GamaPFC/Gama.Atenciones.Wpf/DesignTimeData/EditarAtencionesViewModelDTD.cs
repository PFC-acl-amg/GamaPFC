using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class EditarAtencionesViewModelDTD
    {
        private List<Atencion> _Atenciones;
        private FakeAtencionRepository _AtencionRepository;
        private FakeCitaRepository _CitaRepository;
        private FakePersonaRepository _PersonaRepository;
        private List<Persona> _Personas;

        public EditarAtencionesViewModelDTD()
        {
            _CitaRepository = new FakeCitaRepository();
            _AtencionRepository = new FakeAtencionRepository();
            _PersonaRepository = new FakePersonaRepository();
            _Atenciones = _AtencionRepository.GetAll();
            _Personas = _PersonaRepository.GetAll();
            Persona = new PersonaWrapper(_Personas.First());

            int i = 0;
            foreach (var cita in Persona.Citas)
            {
                i++;
                //cita.Atencion = new AtencionWrapper(_Atenciones[i++]);
            }
        }

        public PersonaWrapper Persona { get; private set; }
    }
}
