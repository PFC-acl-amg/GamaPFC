using Gama.Atenciones.Wpf.Converters;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class AsistenteViewModelDTD
    {
        private List<Asistente> _Asistentes;

        public AsistenteViewModelDTD()
        {
            _Asistentes = new FakeAsistenteRepository().Asistentes;
            AsistenteSeleccionado = new AsistenteWrapper(_Asistentes.First());
            AsistenteSeleccionado.Imagen = 
                BinaryImageConverter.GetBitmapImageFromUriSource(
                             new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png"));
            var persona = new Persona() { Nombre = Faker.NameFaker.Name() };
            AsistenteSeleccionado.Citas.Add(new Cita { Fecha = DateTime.Now, Sala = "Sala B", Persona = persona });
            AsistenteSeleccionado.Citas.Add(new Cita { Fecha = DateTime.Now.AddDays(2), Sala = "Sala A", Persona = persona });
            AsistenteSeleccionado.Citas.Add(new Cita { Fecha = DateTime.Now.AddDays(3), Sala = "Sala B", Persona = persona });

            Asistentes = new ObservableCollection<AsistenteWrapper>(
                _Asistentes
                .Select(x => new AsistenteWrapper(x)));

            AsistenteViewModel = new AsistenteViewModel();
            AsistenteViewModel.Load(Asistentes.First());

        }
        
        public ObservableCollection<AsistenteWrapper> Asistentes { get; private set; }
        public AsistenteWrapper AsistenteSeleccionado { get; set; }
        public AsistenteViewModel AsistenteViewModel { get; set; }
    }
}
