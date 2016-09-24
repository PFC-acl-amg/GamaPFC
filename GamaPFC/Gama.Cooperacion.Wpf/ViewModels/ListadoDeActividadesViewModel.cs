using Core;
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
        private List<Actividad> _actividades;
        private IActividadRepository _actividadRepository;
        private IEventAggregator _eventAggregator;
        private ICooperacionSettings _userConfig;

        public ListadoDeActividadesViewModel(IEventAggregator eventAggregator,
            IActividadRepository actividadRepository, 
            ICooperacionSettings userConfig, ISession session)
        {
            Title = "Todas";

            _eventAggregator = eventAggregator;
            _actividadRepository = actividadRepository;
            _actividadRepository.Session = session;
            _userConfig = userConfig;

            _actividades = _actividadRepository.GetAll();
            Actividades = new PaginatedCollectionView(_actividades,
                userConfig.ListadoDeActividadesItemsPerPage);

            PaginaAnteriorCommand = new DelegateCommand(OnPaginaAnterior);
            PaginaSiguienteCommand = new DelegateCommand(OnPaginaSiguiente);
            SeleccionarActividadCommand = new DelegateCommand<Actividad>(OnSeleccionarActividad);
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

        private void OnSeleccionarActividad(Actividad actividadSeleccionada)
        {
            _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(actividadSeleccionada.Id);
        }
    }
}
