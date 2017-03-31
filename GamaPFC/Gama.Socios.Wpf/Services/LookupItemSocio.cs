using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common.CustomControls
{

    public class LookupItemSocio : BindableBase
    {
        public int Id { get; set; }

        private string _Nombre;
        public string Nombre
        {
            get { return _Nombre; }
            set { SetProperty(ref _Nombre, value); }
        }

        private string _NIF;
        public string NIF
        {
            get { return _NIF; }
            set { SetProperty(ref _NIF, value); }
        }
        private string _DireccionPostal;
        public string DireccionPostal
        {
            get { return _DireccionPostal; }
            set { SetProperty(ref _DireccionPostal, value); }
        }
        private string _Email;
        public string Email
        {
            get { return _Email; }
            set { SetProperty(ref _Email, value); }
        }
        private string _Facebook;
        public string Facebook
        {
            get { return _Facebook; }
            set { SetProperty(ref _Facebook, value); }
        }
        private string _Nacionalidad;
        public string Nacionalidad
        {
            get { return _Nacionalidad; }
            set { SetProperty(ref _Nacionalidad, value); }
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
                result = contenido.Substring(0, Math.Min(contenido.Length, n))
                    + (contenido.Length > n ? "..." : "");
            }

            return result;
        }
    }
}
