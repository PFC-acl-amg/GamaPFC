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
        List<Persona> _Personas;

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

        public List<Persona> Personas
        {
            get
            {
                if (_Personas != null)
                    return _Personas;

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
                        FechaDeNacimiento = DateTime.Now.AddYears(-20),
                        IdentidadSexual = _IdentidadSexual[_Random.Next(0, 5)],
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
                        CreatedAt = DateTime.Now.AddMonths(createdAt),
                        Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(
                             new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png")),

                    };

                    _Personas.Add(persona);

                    if (i % 5 == 0)
                    {
                        createdAt--;
                    }
                }
                return _Personas;
            }
            set
            {
                _Personas = value;
            }
        }

        public int CountAll()
        {
            return Personas.Count;
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
            return Personas;
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public Persona GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetPersonasNuevasPorMes(int numeroDeMeses)
        {
            var resultado = new List<int>(numeroDeMeses);

            for (int i = 0; i < numeroDeMeses; i++)
                resultado.Add(i + 2);

            return resultado;
        }

        public bool Update(Persona entity)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNifs()
        {
            throw new NotImplementedException();
        }

        public List<Atencion> GetAtenciones()
        {
            throw new NotImplementedException();
        }
    }
}
