using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.FakeServices
{
    public class FakeDerivacionRepository
    {
        public static Derivacion Next(Atencion atencion = null)
        {
            var random = new Random();
            var opciones = new bool[] { true, false, true, false, true, true, false, true, false };

            return new Derivacion
            {
                Id = 0,
                Atencion = atencion,
                EsDeFormacion = opciones[random.Next(0, 8)],
                EsDeFormacion_Realizada = opciones[random.Next(0, 8)],
                EsDeOrientacionLaboral = opciones[random.Next(0, 8)],
                EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 8)],
                EsExterna = opciones[random.Next(0, 8)],
                EsExterna_Realizada = opciones[random.Next(0, 8)],
                EsJuridica = opciones[random.Next(0, 8)],
                EsJuridica_Realizada = opciones[random.Next(0, 8)],
                EsPsicologica = opciones[random.Next(0, 8)],
                EsPsicologica_Realizada = opciones[random.Next(0, 8)],
                EsSocial = opciones[random.Next(0, 8)],
                EsSocial_Realizada = opciones[random.Next(0, 8)],
                Externa = "Externa",
                Externa_Realizada = "Externa realizada",
                Tipo = "",
            };
        }
    }
}
