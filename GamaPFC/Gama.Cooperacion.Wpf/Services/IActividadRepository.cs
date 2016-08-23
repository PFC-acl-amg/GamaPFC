﻿using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface IActividadRepository
    {
        Actividad GetById(int id);

        List<Actividad> GetAll();

        //List<LookupItem> GetAllForLookup();

        void Create(Actividad entity);

        bool Update(Actividad entity);

        void Delete(Actividad entity);
    }
}