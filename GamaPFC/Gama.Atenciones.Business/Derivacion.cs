using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Derivacion
    {
        public virtual int Id { get; set; }
        public virtual string Tipo { get; set; } 

        public virtual bool EsSocial { get; set; }
        public virtual bool EsJuridica { get; set; }
        public virtual bool EsPsicologica { get; set; }
        public virtual bool EsDeFormacion { get; set; }
        public virtual bool EsDeOrientacionLaboral { get; set; }
        public virtual bool EsExterna { get; set; }
        public virtual string Externa { get; set; } = "";

        public virtual bool EsSocial_Realizada { get; set; }
        public virtual bool EsJuridica_Realizada { get; set; }
        public virtual bool EsPsicologica_Realizada { get; set; }
        public virtual bool EsDeFormacion_Realizada { get; set; }
        public virtual bool EsDeOrientacionLaboral_Realizada { get; set; }
        public virtual bool EsExterna_Realizada { get; set; }
        public virtual string Externa_Realizada { get; set; } = "";

        public virtual Atencion Atencion { get; set; }

        public Derivacion()
        {
            Tipo = "Tipo base";
        }
    }
}
