using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class AsistenteViewModelDTD
    {
        public AsistenteViewModelDTD()
        {
            Asistente = new AsistenteWrapper(new FakeAsistenteRepository().Asistentes.First());
        }

        public AsistenteWrapper Asistente { get; private set; }
    }
}
