using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf
{
    public static class AtencionesResources
    {
        public static List<string> TodosLosNifDeAsistentes { get; set; }

        public static void AddNifAAsistente(string nif)
        {
            if (!TodosLosNifDeAsistentes.Contains(nif))
            {
                TodosLosNifDeAsistentes.Add(nif);
            }
        }

        public static List<Persona> Personas { get; set; }
        public static ClientService ClientService { get; set; }
        public static string ClientId { get; set; } = Guid.NewGuid().ToString();
    }
}
