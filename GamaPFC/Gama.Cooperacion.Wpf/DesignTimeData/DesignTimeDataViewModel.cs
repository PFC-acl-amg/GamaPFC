using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.DesignTimeData
{
    public class DesignTimeDataViewModel : ViewModelBase
    {
        public ActividadWrapper Actividad { get; set; }
        public List<CooperanteWrapper> CooperantesDisponibles { get; set; }

        private ObservableCollection<Cooperante> _cooperantes { get; set; }

        public DesignTimeDataViewModel()
        {
            _cooperantes = new ObservableCollection<Cooperante>();

            for (int i = 0; i < 4; i++)
            {
                Cooperante c = new Cooperante
                {
                    Apellido = Faker.NameFaker.LastName(),
                    Dni = Faker.StringFaker.AlphaNumeric(9),
                    Nombre = Faker.NameFaker.FirstName(),
                };

                _cooperantes.Add(c);
            }

            var actividad = new Actividad
            {
                Titulo = "Taller de actividades multitudinarias de carácter general",
                Descripcion = "Descripción corta sobre el multitudinario taller de general carácter" +
                    " de actividades de interés mayoritariamente general salvo en casos en los que es" +
                    " de carácter individualista. No se está considera si de posición liberal o comunista" +
                    " en cuanto al libre mercado se refiere. Tampoco se aportan detalles."
            };

            actividad.AddCooperantes(_cooperantes);

            Actividad = new ActividadWrapper(actividad);
            Actividad.Coordinador = new CooperanteWrapper(_cooperantes[0]);

            CooperantesDisponibles = new List<CooperanteWrapper>(_cooperantes.Select(c => new CooperanteWrapper(c)));
        }
    }
}
