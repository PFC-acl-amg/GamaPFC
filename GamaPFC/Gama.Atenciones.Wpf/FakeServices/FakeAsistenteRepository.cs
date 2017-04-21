using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.FakeServices
{
    public class FakeAsistenteRepository 
    {
        public List<Asistente> Asistentes { get; set; }

        public FakeAsistenteRepository()
        {
            Asistentes = new List<Asistente>();
            for (int i = 0; i < 50; i++)
            {
                var asistente = new Asistente
                {
                    Nombre = Faker.NameFaker.FirstName(),
                    Apellidos = Faker.NameFaker.LastName(),
                    Nif = Faker.StringFaker.AlphaNumeric(8),
                    Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png")),
                    FechaDeNacimiento = DateTime.Now.AddYears(-28),
                    ComoConocioAGama = ComoConocioAGama.Difusion.ToString(),
                    Provincia = Faker.LocationFaker.Street(),
                    Municipio = Faker.LocationFaker.Street(),
                    Localidad = Faker.LocationFaker.Street(),
                    Email = Faker.InternetFaker.Email(),
                    Facebook = Faker.InternetFaker.Domain(),
                    LinkedIn = Faker.InternetFaker.Domain(),
                    NivelAcademico = NivelAcademico.EstudioDePostgradoOMaster.ToString(),
                    Ocupacion = Faker.TextFaker.Sentence(),
                    TelefonoFijo = Faker.PhoneFaker.Phone(),
                    TelefonoMovil = Faker.PhoneFaker.Phone(),
                    Twitter = Faker.InternetFaker.Domain(),
                    CreatedAt = DateTime.Now,
                };

                Asistentes.Add(asistente);
            }
        }
    }
}
