using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common.CustomControls
{
    public class LookupItem
    {
        public int Id { get; set; }
        public string DisplayMember1 { get; set; }
        public string DisplayMember2 { get; set; }

        /// <summary>
        /// Selecciona los primeros n caracteres,  y añade "..." si la longitud del título 
        /// es mayor que n.
        /// </summary>
        /// <param name="contenido">String fuente</param>
        /// <param name="n">Número de caracteres a seleccionar</param>
        /// <returns></returns>
        public static string ShortenStringForDisplay(string contenido, int n)
        {
            string result = "";

            if (contenido != null)
            {
                result = contenido.Substring(0,Math.Min(contenido.Length, n))
                    + (contenido.Length > n ? "..." : "");
            }

            return result;
        }
    }
}
