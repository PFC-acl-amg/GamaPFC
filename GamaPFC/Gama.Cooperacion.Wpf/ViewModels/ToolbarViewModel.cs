using Core;
using Gama.Common.Eventos;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private Cooperante Cooperante;
        private Actividad Actividad;
        private EventAggregator _EventAggregator;
        private ICooperanteRepository _CooperanteRepository;
        private IActividadRepository _ActividadRepository;
        private ExportService _ExportService;
        private string VistaCargada;

        public ToolbarViewModel(CooperanteRepository CooperanteRepository, ActividadRepository ActividadRepository,
            ExportService ExportService,
            EventAggregator EventAggregator,
            ISession Session)
        {
            _CooperanteRepository = CooperanteRepository;
            _CooperanteRepository.Session = Session;
            _ActividadRepository = ActividadRepository;
            _ActividadRepository.Session = Session;
            _ExportService = ExportService;
            _EventAggregator = EventAggregator;

            _EventAggregator.GetEvent<CooperanteSeleccionadoEvent>().Subscribe(OnCooperanteSeleccionadoEvent);
            _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnContenidoVistaExportarEvent);


            NuevoActividadCommand = new DelegateCommand(OnNuevoActividad);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperante);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);
        }

        public ICommand NuevoCooperanteCommand { get; private set; }
        public ICommand NuevoActividadCommand { get; private set; }
        public ICommand ExportarCommand { get; set; }

        private void OnNuevoCooperante()
        {
            var o = new AgregarCooperanteView();
            o.ShowDialog();
        }
        private void OnNuevoActividad()
        {
            var o = new NuevaActividadView();
            o.ShowDialog();
        }
        private void OnExportarCommandExecute()
        {

            if (VistaCargada == "ActividadesContentView")
            {
                // Exportar datos de una solo Actividad Que ya se copio an Toolbar con el evento ActividadSeleccionadaEvent.
                _ExportService.ExportarActividad(Actividad, Actividad.Titulo);
            }
            else
            {
                if (VistaCargada == "CooperantesContentView")
                {
                    // Exportar la lista completa de cooperantes
                    var ListaCooperantes = _CooperanteRepository.GetAll();
                    _ExportService.ExportarTodosCooperantes(ListaCooperantes);
                }
                else
                {
                    var ListaActividades = _ActividadRepository.GetAll();
                    _ExportService.ExportarTodasActividades(ListaActividades);
                }
            }
           
            //if (VistaCargada == "DashboardView")
            //{
                
            //}

            //_ExportService.ExportarSocios(ListaSocios);
            //if (VistaCargada != "SociosContentView")
            //{
            //    var ListaSocios = _CooperanteRepository.GetAll();
            //    _ExportService.ExportarSocios(ListaSocios);
            //}
            //else _ExportService.ExportarSocio(Cooperante, Cooperante.Nombre);
        }
        private void OnContenidoVistaExportarEvent(string obj)
        {
            VistaCargada = obj;
        }
        private void OnCooperanteSeleccionadoEvent(int id)
        {
            var _cooperante = _CooperanteRepository.GetById(id);
            Cooperante = _cooperante;
        }
        private void OnActividadSeleccionadaEvent(int id)
        {
            var _ActSel = _ActividadRepository.GetById(id);

            Actividad = _ActSel;
        }
        public void LoadCooperante(int id)
        {
            var _cooperante = _CooperanteRepository.GetById(id);

            Cooperante = _cooperante;
        }
    }
}
