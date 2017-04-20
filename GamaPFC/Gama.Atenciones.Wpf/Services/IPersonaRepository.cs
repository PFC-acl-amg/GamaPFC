using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Gama.Atenciones.Wpf.Services
{
    public interface IPersonaRepository
    {
        ISession Session { get; set; }
        List<Persona> Personas { get; set; }

        /// <summary>
        /// Devuelve la persona cuyo Id coincide con el que se ha pasado por parámetro.
        /// </summary>
        Persona GetById(int id);

        /// <summary>
        /// Devuelve una lista con todas las personas de la base de datos. Primero
        /// intentará devolver la lista cacheada. Si no se ha creado aún, accederá a 
        /// la base de datos para crearla.
        /// </summary>
        List<Persona> GetAll();

        /// <summary>
        /// Devuelve una lista con todas las personas de la base de datos en forma
        /// de objetos 'LookupItem'. Primero
        /// intentará devolver la lista cacheada. Si no se ha creado aún, accederá a 
        /// la base de datos para crearla.
        /// </summary>
        List<LookupItem> GetAllForLookup();

        /// <summary>
        /// Encripta y persiste la entidad en la base de datos. Establece el campo 'CreatedAt',
        /// añade la entidad a la lista cacheada del repositorio, añade el Nif a la lista
        /// cacheadas de Nifs, mantiene la entidad desencriptada y lanza el evento de
        /// 'PersonaCreadaEvent'.
        /// </summary>
        /// <param name="entity">La persona nueva a persistir en la base de datos.</param>
        void Create(Persona entity);

        bool Update(Persona entity);

        void Delete(Persona entity);

        IEnumerable<int> GetPersonasNuevasPorMes(int dashboardUltimasPersonas);

        int CountAll();

        List<string> GetNifs();

        List<Atencion> GetAtenciones();
        void DeleteAll();
    }
}
