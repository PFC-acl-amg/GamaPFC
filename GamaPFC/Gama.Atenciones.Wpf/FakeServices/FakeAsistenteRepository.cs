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
        public List<Asistente> Asistentes;

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
                };

                Asistentes.Add(asistente);
            }
        }
    }
}
