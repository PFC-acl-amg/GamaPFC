using Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common.CustomControls
{
    public class LookupItem : BindableBase
    {
        public int Id { get; set; }

        private string _DisplayMember1;
        public string DisplayMember1
        {
            get { return _DisplayMember1; }
            set { SetProperty(ref _DisplayMember1, value); }
        }

        private string _DisplayMember2;
        public string DisplayMember2
        {
            get { return _DisplayMember2; }
            set { SetProperty(ref _DisplayMember2, value);  }
        }

        private string _DisplayMember3;
        public string DisplayMember3
        {
            get { return _DisplayMember3; }
            set { SetProperty(ref _DisplayMember3, value); }
        }


        private string _IconSource;
        public string IconSource
        {
            get { return _IconSource; }
            set { SetProperty(ref _IconSource, value); }
        }
        private byte[] _Imagen;
        public byte[] Imagen
        {
            get { return _Imagen; }
            set { SetProperty(ref _Imagen, value); }
        }

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
