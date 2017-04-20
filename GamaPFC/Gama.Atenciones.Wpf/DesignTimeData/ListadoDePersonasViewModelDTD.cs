using Core;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Common.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class ListadoDePersonasViewModelDTD
    {
        public PaginatedCollectionView Personas { get; private set; }

        public ListadoDePersonasViewModelDTD()
        {
            var personas = new FakePersonaRepository().GetAll()
                .Select(p =>
                new LookupItem
                {
                    DisplayMember1 = p.Nombre,
                    DisplayMember2 = p.Nif,
                    Imagen = p.Imagen
                }).ToList();
            Personas = new PaginatedCollectionView(personas, 30);
        }
    }
}
