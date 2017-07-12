using Gama.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf
{
    public static class ActividadResources
    {
       
        public static string Guid = new Guid().ToString();

        public static ClientService ClientService { get; set; }
        public static string ClientId { get; set; }
    }
}
