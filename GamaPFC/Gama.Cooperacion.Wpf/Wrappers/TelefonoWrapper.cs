using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class TelefonoWrapper : ModelWrapper<Telefono>
    {
        public TelefonoWrapper(Telefono model) : base(model)
        {
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Numero
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public override string ToString()
        {
            return Numero;
        }
    }
}
