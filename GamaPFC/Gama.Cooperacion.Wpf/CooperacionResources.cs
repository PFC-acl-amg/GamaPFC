using Gama.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf
{
    public static class CooperacionResources
    {
        public static List<string> TodosLosNifDeCooperantes { get; set; }

        public static void AddNifCooperante(string nif)
        {
            if (!TodosLosNifDeCooperantes.Contains(nif))
            {
                TodosLosNifDeCooperantes.Add(nif);
            }
        }
        public static string Guid = new Guid().ToString();

        public static ClientService ClientService { get; set; }
        public static string ClientId { get; set; }
    }
}
