using Core;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class ListadoDeSociosViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private ISociosSettings _Settings;
        private List<Socio> _Socios;

        public ListadoDeSociosViewModel(
            ISocioRepository socioRepository,
            IEventAggregator eventAggregator,
            ISociosSettings settings,
            ISession session)
        {
            Title = "Todos";

            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            _Socios = _SocioRepository.GetAll();
            Socios = new PaginatedCollectionView(_Socios, _Settings.ListadoDeSociosItemsPerPage);

            SeleccionarSocioCommand = new DelegateCommand<Socio>(OnSeleccionarSocioCommandExecute);
            PaginaSiguienteCommand = new DelegateCommand(OnPaginaSiguienteCommandExecute);
            PaginaAnteriorCommand = new DelegateCommand(OnPaginaAnteriorCommandExecute);

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe(OnSocioActualizadoEvent);
        }

        public PaginatedCollectionView Socios { get; private set; }

        public object ElementosPorPagina
        {
            get { return Socios.ItemsPerPage; }
            set
            {
                if (value.GetType() == typeof(int)) // 30, 50, ...
                {
                    Socios.ItemsPerPage = (int)value;
                    _Settings.ListadoDeSociosItemsPerPage = (int)value;
                }
                else if (value.GetType() == typeof(string)) // "Todos"
                {
                    Socios.ItemsPerPage = int.MaxValue;
                    _Settings.ListadoDeSociosItemsPerPage = int.MaxValue;
                }

                OnPropertyChanged();
            }
        }

        public ICommand SeleccionarSocioCommand { get; private set; }
        public ICommand PaginaSiguienteCommand { get; private set; }
        public ICommand PaginaAnteriorCommand { get; private set; }

        private void OnSeleccionarSocioCommandExecute(Socio socio)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(socio.Id);
        }

        private void OnPaginaSiguienteCommandExecute()
        {
            Socios.MoveToNextPage();
        }

        private void OnPaginaAnteriorCommandExecute()
        {
            Socios.MoveToPreviousPage();
        }

        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);
            _Socios.Insert(0, socio);
            Socios.Refresh();
        }

        private void OnSocioActualizadoEvent(Socio socio)
        {
            if (_Socios.Any(x => x.Id == socio.Id))
            {
                var socioSinActualizar = _Socios.Where(x => x.Id == socio.Id).Single();
                var index = _Socios.IndexOf(socioSinActualizar);
                _Socios[index].CopyValuesFrom(socio);
            }
        }
    }
}
