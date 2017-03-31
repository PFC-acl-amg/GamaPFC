using Core;
using Gama.Common.CustomControls;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Services;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class ContabilidadViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private GestorDeContabilidad _GestorDeContabilidad;
        private ISocioRepository _SocioRepository;
        private ObservableCollection<Socio> _Socios;
        private string _NacionEscogida;

        public ContabilidadViewModel(
            GestorDeContabilidad GestorDeContabilidad,
            ISocioRepository SocioRepository,
            IEventAggregator eventAggregator,
            ISession Session)
        {
            _GestorDeContabilidad = GestorDeContabilidad;
            _SocioRepository = SocioRepository;
            _SocioRepository.Session = Session;
            _EventAggregator = eventAggregator;
            VisibleColumnaDireccion = true;
            VisibleColumnaNacionalidad = true;
            BuscadorVisible = true;
            FiltrosVisibles = false;
            DireccionesPostales = new ObservableCollection<string>();
            Nacionalidades = new ObservableCollection<string>();

            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());
            ListaSociosAux = new ObservableCollection<LookupItemSocio>();
            ListaSocios = new ObservableCollection<LookupItemSocio>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Select(a => new LookupItemSocio
                    {
                        Id = a.Id,
                        Nombre= a.Nombre,
                        NIF = a.Nif,
                        Nacionalidad = a.Nacionalidad,
                        DireccionPostal = a.DireccionPostal
                    }));
            //DireccionesPostales.Add(_Socios.Select(item => item.DireccionPostal);
            foreach (var UnSocio in _Socios)
            {
                var CP = UnSocio.DireccionPostal;
                if (!DireccionesPostales.Contains(CP))
                {
                    DireccionesPostales.Add(CP);
                }
                var Nacion = UnSocio.Nacionalidad;
                if (!Nacionalidades.Contains(Nacion))
                {
                    Nacionalidades.Add(Nacion);
                }

            }
            AplicarFiltroCommand = new DelegateCommand(OnAplicarFiltroCommand_Execute,
                OnAplicarFiltroCommand_CanExecute);
            BotonFiltrarCommand = new DelegateCommand(OnBotonFiltrarCommand_Execute,
                OnBotonFiltrarCommand_CanExecute);
        }
        public ObservableCollection<LookupItemSocio> ListaSocios { get; private set; }
        public ObservableCollection<LookupItemSocio> ListaSociosAux { get; private set; }
        public ObservableCollection<string> DireccionesPostales { get; private set; }
        public ObservableCollection<string> Nacionalidades { get; private set; }
        public ICommand AplicarFiltroCommand { get; private set; }
        public ICommand BotonFiltrarCommand { get; private set; }
        private void OnBotonFiltrarCommand_Execute()
        {
            if (FiltrosVisibles == false)
            {
                FiltrosVisibles = true;
            }
            else FiltrosVisibles = false;
        }
        private bool OnBotonFiltrarCommand_CanExecute()
        {
            //return TituloForo != null && TituloForoMensaje != null;
            return true;
        }
        private void OnAplicarFiltroCommand_Execute()
        {
            var aux = NacionEscogida;
            //collection.Remove(collection.Where(i => i.Id == instance.Id).Single());
            //ListaSocios.Remove(ListaSocios.Where(i => i.Nacionalidad != NacionEscogida));
            foreach(var UnSocio in ListaSocios)
            {
                if (UnSocio.Nacionalidad != NacionEscogida) ListaSociosAux.Add(UnSocio);
            }
            foreach (var UnSocio in ListaSociosAux)
            {
                if (ListaSocios.Contains(UnSocio)) ListaSocios.Remove(UnSocio);
            }
            //ListaSocios = ListaSociosAux;
            VisibleColumnaNacionalidad = false;
            //if (BuscadorVisible == false)
            //{
            //    BuscadorVisible = true;
            //}
            //else BuscadorVisible = false;

        }
        private bool OnAplicarFiltroCommand_CanExecute()
        {
            //return TituloForo != null && TituloForoMensaje != null;
            return true;
        }
        private bool _BuscardorVisible = true;
        public bool BuscadorVisible
        {
            get { return _BuscardorVisible; }
            set
            {
                if (_BuscardorVisible != value)
                {
                    _BuscardorVisible = value;
                    OnPropertyChanged();
                }

            }
        }
        private bool _FiltrosVisibles = true;
        public bool FiltrosVisibles
        {
            get { return _FiltrosVisibles; }
            set
            {
                if (_FiltrosVisibles != value)
                {
                    _FiltrosVisibles = value;
                    OnPropertyChanged();
                }

            }
        }
        private bool _VisibleColumnaDireccion = true;
        public bool VisibleColumnaDireccion
        {
            get { return _VisibleColumnaDireccion; }
            set
            {
                if (_VisibleColumnaDireccion != value)
                {
                    _VisibleColumnaDireccion = value;
                    OnPropertyChanged();
                }

            }
        }
        private bool _VisibleColumnaNacionalidad = true;
        public bool VisibleColumnaNacionalidad
        {
            get { return _VisibleColumnaNacionalidad; }
            set
            {
                if (_VisibleColumnaNacionalidad != value)
                {
                    _VisibleColumnaNacionalidad = value;
                    OnPropertyChanged();
                }

            }
        }
        
        public string NacionEscogida
        {
            get { return _NacionEscogida; }
            set
            {
                if (_NacionEscogida != value)
                {
                    _NacionEscogida = value;
                    OnPropertyChanged();
                }

            }
        }
    }
    
}
