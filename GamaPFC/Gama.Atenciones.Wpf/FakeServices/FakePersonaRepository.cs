using Gama.Atenciones.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using NHibernate;
using Gama.Atenciones.Wpf.Converters;

namespace Gama.Atenciones.Wpf.FakeServices
{
    public class FakePersonaRepository : IPersonaRepository
    {
        public List<Persona> Personas { get; set; }

        private ISession _Session = null;
        public ISession Session
        {
            get { return _Session; }
            set { _Session = value; }
        }

        IdentidadSexual[] _IdentidadSexual = new IdentidadSexual[]
        {
            IdentidadSexual.HombreCisexual,
            IdentidadSexual.HombreTransexual,
            IdentidadSexual.MujerCisexual,
            IdentidadSexual.MujerTransexual,
            IdentidadSexual.NoProporcionado,
            IdentidadSexual.Otra
        };

        Random _Random = new Random();

        public FakePersonaRepository()
        {
            Personas = new List<Persona>();
            int createdAt = 0;
            for (int i = 0; i < 40; i++)
            {
                var persona = new Persona()
                {
                    Id = i + 1,
                    ComoConocioAGama = ComoConocioAGama.Difusion.ToString(),
                    DireccionPostal = Faker.LocationFaker.Street(),
                    Email = Faker.InternetFaker.Email(),
                    EstadoCivil = EstadoCivil.Soltera.ToString(),
                    Facebook = Faker.InternetFaker.Domain(),
                    FechaDeNacimiento = DateTime.Now.AddYears(-20),
                    IdentidadSexual = _IdentidadSexual[_Random.Next(0, 5)].ToString(),
                    LinkedIn = Faker.InternetFaker.Domain(),
                    Nacionalidad = Faker.LocationFaker.Country(),
                    Nif = i.ToString() + Faker.StringFaker.AlphaNumeric(8) + i.ToString(),
                    NivelAcademico = NivelAcademico.EstudioDePostgradoOMaster.ToString(),
                    Nombre = Faker.NameFaker.Name(),
                    Ocupacion = Faker.TextFaker.Sentence(),
                    OrientacionSexual = OrientacionSexual.Heterosexual.ToString(),
                    Telefono = Faker.PhoneFaker.Phone(),
                    TieneTrabajo = true,
                    Twitter = Faker.InternetFaker.Domain(),
                    ViaDeAccesoAGama = ViaDeAccesoAGama.Personal.ToString(),
                    CreatedAt = DateTime.Now.AddMonths(createdAt),
                    //Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(
                    //     new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/persona_dummy.png")),

                };

                Personas.Add(persona);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
            }
        }

        public List<Persona> GetAll()
        {
            return Personas;
        }

        public int CountAll()
        {
            return Personas.Count;
        }

        public IEnumerable<int> GetPersonasNuevasPorMes(int numeroDeMeses)
        {
            var resultado = new List<int>(numeroDeMeses);

            for (int i = 0; i < numeroDeMeses; i++)
                resultado.Add(i + 2);

            return resultado;
        }

        public Persona GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public void Create(Persona entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(Persona entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Persona entity)
        {
            throw new NotImplementedException();
        }

        public List<Atencion> GetAtenciones()
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }
    }
}
