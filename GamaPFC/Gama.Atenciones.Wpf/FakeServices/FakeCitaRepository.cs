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
    public class FakeCitaRepository : ICitaRepository
    {
        private List<Cita> Citas { get; set; }

        public ISession Session { get; set; }

        List<Cita> ICitaRepository.Citas
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

        public FakeCitaRepository()
        {
            Citas = new List<Cita>();
            Random random = new Random();

            int createdAt = 0;
            for (int i = 0; i < 50; i++)
            {
                var cita = new Cita()
                {
                    Id = i + 1,
                    AsistenteEnTexto = Faker.NameFaker.Name(),
                    Atencion = null,
                    Fecha = i % 2 == 0 ? DateTime.Now.AddDays(i % 26) : DateTime.Now.AddDays(-(i % 26)),
                    Fin = DateTime.Now.AddHours(2),
                    Hora = random.Next(0, 23),
                    Minutos = random.Next(0, 59),
                    Sala = "Sala B",
                    HaTenidoLugar = true,
                    Persona = new Persona
                    {
                        Id = 0,
                        Nombre = Faker.NameFaker.FirstName(),
                        //Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png")),
                    },
                    CreatedAt = DateTime.Now.AddMonths(createdAt)
                };

                while (Citas.Where(c => c.Fecha.Day == cita.Fecha.Day).Count(c => true) > 3)
                {
                    cita.Fecha = cita.Fecha.AddDays(1);
                }

                Citas.Add(cita);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
            }
        }

        public List<Cita> GetAll()
        {
            return Citas;
        }
        public int CountAll()
        {
            return Citas.Count;
        }

        public Cita GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Create(Cita entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(Cita entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Cita entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }
    }
}
