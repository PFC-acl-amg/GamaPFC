using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class CitasContentViewModelDTD
    {
        public CitasContentViewModelDTD()
        {
            var citas = (new FakeCitaRepository().GetAll());
            Citas = new List<CitaWrapper>(citas.Select(c => new CitaWrapper(c)));
        }

        public List<CitaWrapper> Citas { get; private set; }
    }
}
