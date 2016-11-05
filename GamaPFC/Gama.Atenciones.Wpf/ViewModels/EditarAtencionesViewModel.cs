using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarAtencionesViewModel : ViewModelBase
    {
        private bool _EdicionHabilitada;
        private PersonaWrapper _Persona;
        private AtencionWrapper _AtencionSeleccionada;

        public EditarAtencionesViewModel()
        {
            _EdicionHabilitada = true;
            Persona = new PersonaWrapper(new Persona());
        }

        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
        }

        public PersonaWrapper Persona
        {
            get { return _Persona; }
            set { SetProperty(ref _Persona, value); }
        }

        public void Load(PersonaWrapper wrapper)
        {
            EdicionHabilitada = false;
            Persona = wrapper;
            Atenciones = new ObservableCollection<AtencionWrapper>(
                Persona.Citas.Select(c => c.Atencion).Where(a => a != null && a.Id != 0).ToList());
            OnPropertyChanged("Atenciones");
        }

        public ObservableCollection<AtencionWrapper> Atenciones { get; private set; }

        public AtencionWrapper AtencionSeleccionada
        {
            get { return _AtencionSeleccionada; }
            set { SetProperty(ref _AtencionSeleccionada, value); }
        }
    }
}
