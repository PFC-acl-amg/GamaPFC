using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class CooperacionUserConfiguration : ICooperacionUserConfiguration
    {
        public CooperacionUserConfiguration()
        {
            ListadoDeActividadesItemsPerPage = 30;
        }

        public int ListadoDeActividadesItemsPerPage { get; set; }
    }
}
