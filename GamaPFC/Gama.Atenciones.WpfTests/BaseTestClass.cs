using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.WpfTests
{
    public class BaseTestClass
    {
        public BaseTestClass()
        {
            // Aunque sea nulo porque no hay tal aplicación creada, se llamará
            // al constructor estático y se establecerá el "packScheme", para evitar
            // errores en tiempo de ejecución al acceder a código que use ese scheme, como
            // atencion.Imagen = new Uri("pack://aplicattion...")...
            var x = Gama.Atenciones.Wpf.App.Current;
        }
    }
}
