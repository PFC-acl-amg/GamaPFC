﻿using Gama.Atenciones.Wpf.Services;
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
    public class FakeCitaRepository : ICitaRepository
    {
        private List<Cita> _Citas;

        public ISession Session { get; set; }

        public FakeCitaRepository()
        {
            _Citas = new List<Cita>();

            int createdAt = 0;
            for (int i = 0; i < 50; i++)
            {
                var cita = new Cita()
                {
                    Id = i + 1,
                    AsistenteEnTexto = Faker.NameFaker.Name(),
                    Atencion = null,
                    Fecha = DateTime.Now.AddDays(i % 7),
                    Fin = DateTime.Now.AddHours(2),
                    Sala = "Sala B",
                    HaTenidoLugar = true,
                    Persona = null,
                    CreatedAt = DateTime.Now.AddMonths(createdAt)
                };

                _Citas.Add(cita);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
            }
        }

        public void Create(Cita entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Cita entity)
        {
            throw new NotImplementedException();
        }

        public List<Cita> GetAll()
        {
            return _Citas;
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public Cita GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Cita entity)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            return _Citas.Count;
        }
    }
}
