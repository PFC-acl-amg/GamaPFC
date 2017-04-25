using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.FakeServices
{
    public class FakeRepository
    {
        public List<Persona> Personas { get; set; }
        public List<Cita> Citas { get; set; }
        public List<Atencion> Atenciones { get; set; }

        public FakeRepository()
        {
            var Personas = new FakePersonaRepository().GetAll();
            var Citas = new FakeCitaRepository().GetAll();
            var Atenciones = new FakeAtencionRepository().GetAll();

            var randomNumberGenerator = new Random();

            foreach (var cita in Citas)
            {
                int next = randomNumberGenerator.Next(0, Personas.Count);
                cita.Persona = Personas[next];
                Personas[next].Citas.Add(cita);
            }

            foreach (var atencion in Atenciones)
            {
                int next = randomNumberGenerator.Next(0, Citas.Count);
                atencion.Cita = Citas[next];
                Citas[next].Atencion = atencion;
            }
        }
    }
}
