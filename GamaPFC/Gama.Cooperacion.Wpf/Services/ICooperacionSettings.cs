﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface ICooperacionSettings
    {
        int DashboardActividadesLongitudDeTitulos { get; set; }
        int DashboardCooperantesAMostrar { get; set; }
        int DashboardActividadesAMostrar { get; set; }
        int ListadoDeActividadesItemsPerPage { get; set; }
        int DashboardMesesAMostrarDeActividadesNuevas { get; set; }
        int DashboardMesesAMostrarDeCooperantesNuevos { get; set; }
    }
}