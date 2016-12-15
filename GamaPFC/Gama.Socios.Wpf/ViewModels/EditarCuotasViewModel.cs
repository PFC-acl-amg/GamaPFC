using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Gama.Socios.Wpf.Wrappers;

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarCuotasViewModel : ViewModelBase
    {
        public ISession Session { get; internal set; }

        public EditarCuotasViewModel()
        {

        }

        public void Load(SocioWrapper socio)
        {

        }
    }
}
