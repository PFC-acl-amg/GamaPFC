using Core;
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
        public List<Socio> _Socios { get; private set; }

        public ListadoDeSociosViewModelDTD()
        {
            _Socios = new List<Socio>(new FakeSocioRepository().GetAll());
            Socios = new PaginatedCollectionView(_Socios, 30);
        }

        public PaginatedCollectionView Socios { get; private set; }
    }
}
