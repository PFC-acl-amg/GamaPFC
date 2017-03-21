using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.ViewModels
{
    public class ContabilidadViewModel
    {
        private GestorDeContabilidad _GestorDeContabilidad;

        public ContabilidadViewModel(
            GestorDeContabilidad gestorDeContabilidad)
        {
            _GestorDeContabilidad = gestorDeContabilidad;
        }
    }
}
