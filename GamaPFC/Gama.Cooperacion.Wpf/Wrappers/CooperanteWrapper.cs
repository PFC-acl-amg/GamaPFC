using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class CooperanteWrapper : ModelWrapper<Cooperante>
    {
        public CooperanteWrapper(Cooperante model) : base (model)
        {

        }

        public string Apellido
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Dni
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Nombre
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        // Propiedad del Wrapper únicamente
        public string NombreCompleto => string.Format("{0} {1}", Nombre, Apellido);
    }
}
