using Core;
using Gama.Cooperacion.Business;
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
        public ObservableCollection<Cooperante> Cooperantes { get; private set; }

        public DesignTimeDataViewModel()
        {
            Cooperantes = new ObservableCollection<Cooperante>();

            for (int i = 0; i < 30; i++)
            {
                Cooperante c = new Cooperante
                {
                    Apellido = Faker.NameFaker.LastName(),
                    Dni = Faker.StringFaker.AlphaNumeric(9),
                    Nombre = Faker.NameFaker.FirstName(),
                };

                Cooperantes.Add(c);
            }
        }
    }
}
