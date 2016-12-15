using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PersonaViewModel : ViewModelBase
    {
        private bool _EdicionHabilitada;
        private PersonaWrapper _Persona;

        public PersonaViewModel()
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
        }
    }
}
