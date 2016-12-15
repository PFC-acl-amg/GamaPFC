using Gama.Socios.Business;
using Gama.Socios.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.DesignTimeData
{
    public class EditarPeriodosDeAltaViewModelDTD
    {
        public EditarPeriodosDeAltaViewModelDTD()
        {
            Socio = new SocioWrapper(new FakeServices.FakeSocioRepository().GetAll().FirstOrDefault());
        }

        public SocioWrapper Socio { get; set; }
    }
}
