using Gama.Socios.Business;
using Gama.Socios.Wpf.FakeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.DesignTimeData
{
    public class ListadoDeSociosViewModelDTD
    {
        public ListadoDeSociosViewModelDTD()
        {
            Socios = new List<Socio>(new FakeSocioRepository().GetAll());
        }

        public List<Socio> Socios { get; private set; }
    }
}
