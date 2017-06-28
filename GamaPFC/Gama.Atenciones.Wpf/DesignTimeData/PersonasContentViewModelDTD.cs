using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class PersonasContentViewModelDTD
    {
        public PersonasContentViewModelDTD()
        {
            EditarPersonaViewModels = new ObservableCollection<object>();

            var vm = new EditarPersonaViewModel(
                new EventAggregator(),
                new FakePersonaRepository(),
                new PersonaViewModel(),
                new EditarAtencionesViewModel
                    (new FakeAtencionRepository(), new EventAggregator(), new FakePersonaRepository(), 
                    new FakeCitaRepository(), null, null),
                new EditarCitasViewModel(new FakeCitaRepository(), new EventAggregator()),
                null, null);
            vm.PersonaVM.Persona = new Wrappers.PersonaWrapper(new Business.Persona
            {
                Nombre =
                Faker.NameFaker.Name()
            });

            EditarPersonaViewModels.Add(vm);
    
        }

        public ObservableCollection<object> EditarPersonaViewModels { get; private set; }
    }
}
