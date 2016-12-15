using Gama.Atenciones.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using NHibernate;

namespace Gama.Atenciones.Wpf.FakeServices
{
    public class FakeAtencionRepository : IAtencionRepository
    {
        private List<Atencion> _Atenciones;

        public ISession Session { get; set; }

        public FakeAtencionRepository()
        {
            _Atenciones = new List<Atencion>();

            var opciones = new bool[] { true, false };

            int createdAt = 0;
            var random = new Random();
            for (int i = 0; i < 50; i++)
            {
                var atencion = new Atencion()
                {
                    Id = i + 1,
                    EsDeAcogida = opciones[random.Next(0, 1)],
                    EsDeFormacion = opciones[random.Next(0, 1)],
                    EsDeOrientacionLaboral = opciones[random.Next(0, 1)],
                    EsDePrevencionParaLaSalud = opciones[random.Next(0, 1)],
                     EsJuridica = opciones[random.Next(0, 1)],
                     EsPsicologica = opciones[random.Next(0, 1)],
                     EsSocial = opciones[random.Next(0, 1)],
                     EsOtra = opciones[random.Next(0, 1)],
                     Otra = "Otraaaaa",
                    EsDeParticipacion = opciones[random.Next(0, 1)],
                    Fecha = DateTime.Now,
                    Seguimiento = Faker.TextFaker.Sentences(4),
                    CreatedAt = DateTime.Now.AddMonths(createdAt),
                    Derivacion = new Derivacion
                    {
                         EsDeFormacion = opciones[random.Next(0, 1)],
                          EsSocial = opciones[random.Next(0, 1)],
                           EsPsicologica = opciones[random.Next(0, 1)],
                            EsJuridica = opciones[random.Next(0, 1)],
                             EsDeFormacion_Realizada = opciones[random.Next(0, 1)],
                        EsDeOrientacionLaboral = opciones[random.Next(0, 1)],
                        EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 1)],
                        EsExterna = opciones[random.Next(0, 1)],
                        EsExterna_Realizada = opciones[random.Next(0, 1)],
                        EsJuridica_Realizada = opciones[random.Next(0, 1)],
                        EsPsicologica_Realizada = opciones[random.Next(0, 1)],
                        EsSocial_Realizada = opciones[random.Next(0, 1)],
                        Externa_Realizada = "Externaaaa realizada",
                         Externa = "Externaaaaaa"
                    }
                };

                _Atenciones.Add(atencion);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
            }
        }

        public int CountAll()
        {
            return _Atenciones.Count;
        }

        public void Create(Atencion entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Atencion entity)
        {
            throw new NotImplementedException();
        }

        public List<Atencion> GetAll()
        {
            return _Atenciones;
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetAtencionesNuevasPorMes(int numeroDeMeses)
        {
            var resultado = new List<int>(numeroDeMeses);

            for (int i = 0; i < numeroDeMeses; i++)
                resultado.Add(i + 2);

            return resultado;
        }

        public Atencion GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Atencion entity)
        {
            throw new NotImplementedException();
        }
    }
}
