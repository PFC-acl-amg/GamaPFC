using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.DesignTimeData
{
    public class ListadoDeActividadesVM_DTD
    {
        public PaginatedCollectionView Actividades { get; private set; }

        public ListadoDeActividadesVM_DTD()
        {
            var actividades = new FakeActividadRepository().GetAll()
                .Select(a =>
                new LookupItem
                {
                    DisplayMember1 = a.Titulo,
                    DisplayMember2 = a.Descripcion
                }).ToList();
            Actividades = new PaginatedCollectionView(actividades, 30);
        }
    }
}
