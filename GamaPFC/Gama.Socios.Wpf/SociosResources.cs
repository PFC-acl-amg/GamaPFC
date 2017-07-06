using Gama.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf
{
    public static class SociosResources
    {
        public static List<string> TodosLosNif { get; set; }

        public static void AddNif(string nif)
        {
            if (!TodosLosNif.Contains(nif))
            {
                TodosLosNif.Add(nif);
            }
        }

        public static ClientService ClientService { get; set; }
        public static string ClientId { get; set; } = Guid.NewGuid().ToString();
    }
}
