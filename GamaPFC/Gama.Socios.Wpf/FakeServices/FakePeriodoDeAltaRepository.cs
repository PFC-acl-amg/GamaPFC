using Gama.Socios.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Socios.Business;
using NHibernate;

namespace Gama.Socios.Wpf.FakeServices
{
    public class FakePeriodoDeAltaRepository : IPeriodoDeAltaRepository
    {
        public ISession Session { get; set; }

        public List<PeriodoDeAlta> PeriodosDeAlta { get; set; }

        private List<PeriodoDeAlta> _PeriodosDeAlta;

        public FakePeriodoDeAltaRepository()
        {
            _PeriodosDeAlta = new List<PeriodoDeAlta>();

            for (int i = 0; i < FakeSocioRepository.SEED_COUNT; i++)
            {
                var periodoDeAlta = new PeriodoDeAlta
                {
                    Id = i + 1,
                    FechaDeAlta = DateTime.Now.AddMonths(-7),
                };

                _PeriodosDeAlta.Add(periodoDeAlta);
            }
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public void Create(PeriodoDeAlta entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(PeriodoDeAlta entity)
        {
            throw new NotImplementedException();
        }

        public List<PeriodoDeAlta> GetAll()
        {
            return _PeriodosDeAlta;
        }

        public PeriodoDeAlta GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(PeriodoDeAlta entity)
        {
            throw new NotImplementedException();
        }
    }
}
