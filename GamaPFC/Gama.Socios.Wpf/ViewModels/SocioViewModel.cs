using Core;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Wrappers;

namespace Gama.Socios.Wpf.ViewModels
{
    public class SocioViewModel : ViewModelBase
    {
        private bool _EdicionHabilitada;
        private SocioWrapper _Socio;

        public SocioViewModel()
        {
            _EdicionHabilitada = true;
            Socio = new SocioWrapper(new Socio());
        }

        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
        }

        public SocioWrapper Socio
        {
            get { return _Socio; }
            set { SetProperty(ref _Socio, value); }
        }

        public void Load(SocioWrapper wrapper)
        {
            EdicionHabilitada = false;
            Socio = wrapper;
        }
    }
}
