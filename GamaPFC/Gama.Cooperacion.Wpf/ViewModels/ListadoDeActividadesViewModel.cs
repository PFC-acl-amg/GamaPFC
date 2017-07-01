using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
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

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ListadoDeActividadesViewModel : ViewModelBase
    {
        private List<LookupItem> _actividades;
        private IActividadRepository _actividadRepository;
        private IEventAggregator _eventAggregator;
        private ICooperacionSettings _userConfig;

        public ListadoDeActividadesViewModel(IEventAggregator eventAggregator,
            IActividadRepository actividadRepository, 
            ICooperacionSettings userConfig, ISession session)
        {
            Gama.Common.Debug.Debug.StartStopWatch();
            Title = "Todas";

            _eventAggregator = eventAggregator;
            _actividadRepository = actividadRepository;
            _actividadRepository.Session = session;
            _userConfig = userConfig;

            _actividades = _actividadRepository.GetAll()
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = a.Titulo,
                    DisplayMember2 = a.Descripcion
                }).ToList();
            Actividades = new PaginatedCollectionView(_actividades,
                userConfig.ListadoDeActividadesItemsPerPage);

            _eventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnNuevaActividadEvent);
            _eventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);

            PaginaAnteriorCommand = new DelegateCommand(OnPaginaAnterior);
            PaginaSiguienteCommand = new DelegateCommand(OnPaginaSiguiente);
            SeleccionarActividadCommand = new DelegateCommand<object>(OnSeleccionarActividad);
            Gama.Common.Debug.Debug.StopWatch("ListadoDeActividadesViewModel");
        }

        public PaginatedCollectionView Actividades { get; private set; }
        public ICommand PaginaAnteriorCommand { get; private set; }
        public ICommand PaginaSiguienteCommand { get; private set; }
        public ICommand SeleccionarActividadCommand { get; private set; }

        public object ElementosPorPagina
        {
            get { return Actividades.ItemsPerPage; }
            set
            {
                if (value.GetType() == typeof(int)) // 30, 50, ...
                {
                    Actividades.ItemsPerPage = (int)value;
                    _userConfig.ListadoDeActividadesItemsPerPage = (int)value;
                }
                else if (value.GetType() == typeof(string)) // "Todos"
                {
                    Actividades.ItemsPerPage = int.MaxValue;
                    _userConfig.ListadoDeActividadesItemsPerPage = int.MaxValue;
                }

                OnPropertyChanged();
            }
        }

        private void OnPaginaAnterior()
        {
            Actividades.MoveToPreviousPage();
        }

        private void OnPaginaSiguiente()
        {
            Actividades.MoveToNextPage();
        }

        private void OnSeleccionarActividad(object id)
        {
            _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish((int)id);
        }

        private void OnNuevaActividadEvent(int id)
        {
            var actividad = _actividadRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = actividad.Id,
                DisplayMember1 = actividad.Titulo,
                DisplayMember2 = actividad.Descripcion
            };
            _actividades.Insert(0, lookupItem);
            Actividades.Refresh();
        }

        private void OnActividadActualizadaEvent(int id)
        {
            var actividadActualizada = _actividadRepository.GetById(id);
            if (_actividades.Any(a => a.Id == id))
            {
                var actividad = _actividades.Where(a => a.Id == id).Single();
                var index = _actividades.IndexOf(actividad);
                _actividades[index].DisplayMember1 = actividadActualizada.Titulo;
                _actividades[index].DisplayMember2 = actividadActualizada.Descripcion;
            }
        }
    }
}
