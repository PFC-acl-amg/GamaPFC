using Core;
using Gama.Common.CustomControls;
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
        private List<LookupItem> _Socios;

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
            
            _Socios = _SocioRepository.GetAll()
                .Select(p => new LookupItem
                {
                    Id = p.Id,
                    DisplayMember1 = p.Nombre,
                    DisplayMember2 = p.Nif,
                    IconSource = p.AvatarPath
                }).ToList();

            Socios = new PaginatedCollectionView(_Socios, _Settings.ListadoDeSociosItemsPerPage);

            SeleccionarSocioCommand = new DelegateCommand<object>(OnSeleccionarSocioCommandExecute);
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

        private void OnSeleccionarSocioCommandExecute(object id)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish((int)id);
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
            Socios.AddItemAt(0, new LookupItem
            {
                Id = socio.Id,
                DisplayMember1 = socio.Nombre,
                DisplayMember2 = socio.Nif,
                IconSource = socio.AvatarPath
            });
        }

        private void OnSocioActualizadoEvent(Socio socio)
        {
            if (_Socios.Any(x => x.Id == socio.Id))
            {
                var socioSinActualizar = _Socios.Where(x => x.Id == socio.Id).Single();
                var index = _Socios.IndexOf(socioSinActualizar);
                var socioEncontrado = _Socios[index];
                socioEncontrado.DisplayMember1 = socio.Nombre;
                socioEncontrado.DisplayMember2 = socio.Nif;
                socioEncontrado.Id = socio.Id;
                socioEncontrado.IconSource = socio.AvatarPath;
                //_Socios[index].CopyValuesFrom(socio);
            }
        }
    }
}
