using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common.Eventos
{
    public enum MensajeDeConexion
    {
        Conectado,
        NoConectado,
        Conectando
    }
    
    public class LaConexionConElServidorHaCambiadoEvent : PubSubEvent<MensajeDeConexion>
    {
    }
}
