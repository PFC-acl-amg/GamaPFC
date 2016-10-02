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
            _Atenciones = new List<Atencion>();

            int createdAt = 0;
            for (int i = 0; i< 50; i++)
            {
                var atencion = new Atencion()
                {
                    Id = i + 1,
                    EsDeAcogida = true,
                    EsDeFormacion = true,

                    EsDeParticipacion = true,
                    Fecha = DateTime.Now,
                    Seguimiento = Faker.TextFaker.Sentences(4),
                    CreatedAt = DateTime.Now.AddMonths(createdAt),
                };

                _Atenciones.Add(atencion);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
}

            return _Atenciones;
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
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
