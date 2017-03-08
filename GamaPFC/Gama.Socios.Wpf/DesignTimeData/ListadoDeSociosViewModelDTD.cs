using Core;
using Gama.Common.CustomControls;
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
        public List<LookupItem> _Socios { get; private set; }

        public ListadoDeSociosViewModelDTD()
        {
            _Socios = new List<Socio>(new FakeSocioRepository().GetAll())
                .Select(p => new LookupItem
                {
                    Id = p.Id,
                    DisplayMember1 = p.Nombre,
                    DisplayMember2 = p.Nif,
                    IconSource = p.AvatarPath
                }).ToList();
            Socios = new PaginatedCollectionView(_Socios, 30);
        }

        public PaginatedCollectionView Socios { get; private set; }
    }
}
