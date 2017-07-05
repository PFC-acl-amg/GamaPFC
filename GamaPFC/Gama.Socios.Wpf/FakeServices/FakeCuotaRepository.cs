using Gama.Socios.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Socios.Business;
using NHibernate;
using Core.Util;

namespace Gama.Socios.Wpf.FakeServices
{
    public class FakeCuotaRepository : ICuotaRepository
    {
        public ISession Session { get; set; }

        public List<Cuota> Cuotas { get; set; }

        private List<Cuota> _Cuotas;

        public FakeCuotaRepository()
        {
            _Cuotas = new List<Cuota>();
            DateTime fechaDePago = DateTime.Now.AddMonths(-7);
            for (int i = 0; i < FakeSocioRepository.SEED_COUNT * 5; i++)
            {
                var cuota = new Cuota
                {
                    Id = i + 1,
                    CantidadPagada = 10,
                    CantidadTotal = 10,
                    Comentarios = Faker.TextFaker.Sentences(3),
                    EstaPagado = true,
                    Fecha = fechaDePago,
                    NoContabilizar = false,
                };

                if (i != 0 && i % (FakeSocioRepository.SEED_COUNT - 1) == 0)
                    fechaDePago = fechaDePago.AddMonths(1);

                _Cuotas.Add(cuota);
            }
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public void Create(Cuota entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Cuota entity)
        {
            throw new NotImplementedException();
        }

        public List<Cuota> GetAll()
        {
            return _Cuotas;
        }

        public Cuota GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Cuota entity)
        {
            throw new NotImplementedException();
        }
    }
}
