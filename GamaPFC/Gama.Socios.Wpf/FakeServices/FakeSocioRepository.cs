﻿using Gama.Socios.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Socios.Business;
using NHibernate;

namespace Gama.Socios.Wpf.FakeServices
{
    public class FakeSocioRepository : ISocioRepository
    {
        public ISession Session { get; set; }

        private List<Socio> _Socios;
        private const int SEED_COUNT = 50;

        public FakeSocioRepository()
        {
            _Socios = new List<Socio>();

            for (int i = 0; i < SEED_COUNT; i++)
            {
                var socio = new Socio()
                {
                    CreatedAt = DateTime.Now,
                    DireccionPostal = Faker.Address.StreetAddress(),
                    Email = Faker.Internet.Email(),
                    Facebook = Faker.Internet.DomainName(),
                    FechaDeNacimiento = DateTime.Now.AddYears(-30 + i).AddMonths(i % 4),
                    LinkedIn = Faker.Internet.DomainName(),
                    Nacionalidad = Faker.Address.Country(),
                    Nombre = Faker.Name.FullName(),
                    Nif = Faker.Phone.Number(),
                    Telefono = Faker.Phone.Number(),
                    Twitter = Faker.Internet.DomainName(),
                    UpdatedAt = null
                };

                _Socios.Add(socio);
            }
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

        public IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses)
        {
            throw new NotImplementedException();
        }

        public bool Update(Socio entity)
        {
            throw new NotImplementedException();
        }
    }
}
