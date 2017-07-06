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
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Cooperante Cooperante;
        private IEventAggregator _EventAggregator;
        private ICooperanteRepository _CooperanteRepository;
        private ExportService _ExportService;
        private string VistaCargada;

        public ToolbarViewModel(CooperanteRepository CooperanteRepository,
            ExportService ExportService,
            IEventAggregator EventAggregator,
            ISession Session)
        {
            _CooperanteRepository = CooperanteRepository;
            _CooperanteRepository.Session = Session;
            _ExportService = ExportService;
            _eventAggregator = EventAggregator;

            _EventAggregator.GetEvent<CooperanteSeleccionadoEvent>().Subscribe(OnCooperanteSeleccionadoEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnListaCooperantesExportarEvent);


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
            //_ExportService.ExportarSocios(ListaSocios);
            //if (VistaCargada != "SociosContentView")
            //{
            //    var ListaSocios = _CooperanteRepository.GetAll();
            //    _ExportService.ExportarSocios(ListaSocios);
            //}
            //else _ExportService.ExportarSocio(Cooperante, Cooperante.Nombre);
        }
        private void OnListaCooperantesExportarEvent(string obj)
        {
            VistaCargada = obj;
        }
        private void OnCooperanteSeleccionadoEvent(int id)
        {
            var _cooperante = _CooperanteRepository.GetById(id);

            Cooperante = _cooperante;
        }
    }
}
