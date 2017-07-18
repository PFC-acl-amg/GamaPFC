using Gama.Socios.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Socios.Business;
using NHibernate;
using Gama.Common.CustomControls;
using Core.Util;

namespace Gama.Socios.Wpf.FakeServices
{
    public class FakeSocioRepository : ISocioRepository
    {
        public ISession Session { get; set; }

        public List<Socio> Socios { get; set; }

        private List<Socio> _Socios;
        public static int SEED_COUNT = 50;

        public FakeSocioRepository()
        {
            string path = @"C:\Users\Alberto\Documents\Profile Photos\Para la demo\";
            _Socios = new List<Socio>();

            int createdAt = 0;
            int imageIndex = 1;
            for (int i = 0; i < SEED_COUNT; i++)
            {
                var socio = new Socio()
                {
                    Id = i + 1,
                    CreatedAt = DateTime.Now.AddMonths(createdAt),
                    DireccionPostal = Faker.LocationFaker.Street(),
                    Email = Faker.InternetFaker.Email(),
                    EstaDadoDeAlta = true,
                    Facebook = Faker.InternetFaker.Domain(),
                    FechaDeNacimiento = DateTime.Now.AddYears(-25),
                    ImagenUpdatedAt = DateTime.Now,
                    LinkedIn = Faker.InternetFaker.Domain(),
                    Nacionalidad = Faker.LocationFaker.Country(),
                    Nif = i.ToString() + Faker.StringFaker.AlphaNumeric(8) + i.ToString(),
                    Nombre = Faker.NameFaker.Name(),
                    Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(new Uri($"{path}{imageIndex}.jpg")),
                    Telefono = Faker.PhoneFaker.Phone(),
                    Twitter = Faker.InternetFaker.Domain(),
                };

                imageIndex++;
                if (imageIndex > 28)
                    imageIndex = 1;
                if (i != 0 && i % 5 == 0)
                {
                    createdAt--;
                }
                //var periododeAlta = new PeriodoDeAlta
                //{
                //    FechaDeAlta = DateTime.Now.AddYears(-3).AddMonths(-4),
                //};

                //periododeAlta.AddCuota(new Cuota() {
                //    Fecha = periododeAlta.FechaDeAlta,
                //    CantidadTotal = 10.0,
                //    CantidadPagada = 0,
                //    Comentarios = "Algún comentario"
                //});

                //socio.AddPeriodoDeAlta(periododeAlta);
                //socio.AddPeriodoDeAlta(periododeAlta);
                //socio.AddPeriodoDeAlta(periododeAlta);

                _Socios.Add(socio);
            }
        }

        public IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses)
        {
            var resultado = new List<int>(numeroDeMeses);

            for (int i = 0; i < numeroDeMeses; i++)
                resultado.Add(i + 2);

            return resultado;
        }

        public List<Socio> GetAll()
        {
            return _Socios;
        }

        public int CountAll()
        {
            return _Socios.Count;
        }

        public void Create(Socio entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Socio entity)
        {
            throw new NotImplementedException();
        }

        public Socio GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Socio entity)
        {
            throw new NotImplementedException();
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public List<string> GetNifs()
        {
            throw new NotImplementedException();
        }

        public void UpdateClient()
        {
            throw new NotImplementedException();
        }
    }
}
