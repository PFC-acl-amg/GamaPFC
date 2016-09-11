using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class EmailWrapper : ModelWrapper<Email>
    {
        public EmailWrapper(Email model) : base(model)
        {
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Direccion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DireccionOriginalValue => GetOriginalValue<string>(nameof(Direccion));

        public bool DireccionIsChanged => GetIsChanged(nameof(Direccion));

        public override string ToString()
        {
            return Direccion;
        }
    }
}
