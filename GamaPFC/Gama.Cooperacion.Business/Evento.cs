using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public enum Ocurrencia
    {
        MENSAJE_PUBLICADO_EN_FORO,
        INCIDENCIA_EN_TAREA,
        SEGUIMIENGO_EN_TAREA,
        TAREA_FINALIZADA,
        NUEVA_TAREA_PUBLICADA,
        Nueva_Actividad,
        FORO_CREADO,
        Mensaje_Publicado,
        TAREA_RECUPERADA
    }
    public class Evento : TimestampedModel
    {
        public virtual int Id { get; set; }
        public virtual string Titulo { get; set; }
        public virtual DateTime FechaDePublicacion { get; set; }
        public virtual string Ocurrencia { get; set; }
        public virtual Actividad Actividad { get; set;}
    }
}
