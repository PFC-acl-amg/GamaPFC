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
    public class FakePersonaRepository : IPersonaRepository
    {
        List<Persona> _Personas;

        public FakePersonaRepository()
        {
            _Personas = new List<Persona>();
            int createdAt = 0;
            for (int i = 0; i < 50; i++)
            {
                var persona = new Persona()
                {
                    Id = i + 1,
                    ComoConocioAGama = ComoConocioAGama.Difusion,
                    DireccionPostal = Faker.LocationFaker.Street(),
                    Email = Faker.InternetFaker.Email(),
                    EstadoCivil = EstadoCivil.Soltera,
                    Facebook = Faker.InternetFaker.Domain(),
                    FechaDeNacimiento = Faker.DateTimeFaker.BirthDay(),
                    IdentidadSexual = IdentidadSexual.MujerCisexual,
                    LinkedIn = Faker.InternetFaker.Domain(),
                    Nacionalidad = Faker.LocationFaker.Country(),
                    Nif = Faker.StringFaker.AlphaNumeric(8),
                    NivelAcademico = NivelAcademico.EstudioDePostgradoOMaster,
                    Nombre = Faker.NameFaker.Name(),
                    Ocupacion = Faker.TextFaker.Sentence(),
                    OrientacionSexual = OrientacionSexual.Heterosexual,
                    Telefono = Faker.PhoneFaker.Phone(),
                    TieneTrabajo = true,
                    Twitter = Faker.InternetFaker.Domain(),
                    ViaDeAccesoAGama = ViaDeAccesoAGama.Personal,
                    CreatedAt = DateTime.Now.AddMonths(createdAt)
                };

                _Personas.Add(persona);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
            }
        }

        public ISession Session
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Create(Persona entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Persona entity)
        {
            throw new NotImplementedException();
        }

        public List<Persona> GetAll()
        {
            return _Personas;
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public Persona GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Persona entity)
        {
            throw new NotImplementedException();
        }
    }
}
