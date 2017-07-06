using Core;
using Gama.Common.Eventos;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Views;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private Socio Socio;
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private ExportService _ExportService;
        private string VistaCargada;

        public ToolbarViewModel(
            ISocioRepository socioRepository,
            ExportService exportService,
            IEventAggregator eventAggregator, 
            ISession session)
        {
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _ExportService = exportService;
            _EventAggregator = eventAggregator;

            NuevoSocioCommand = new DelegateCommand(OnNuevoSocioCommandExecute);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);

            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Subscribe(OnSocioSeleccionadoEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnListaSociosExportarEvent);
        }

        private void OnListaSociosExportarEvent(string obj)
        {
            VistaCargada = obj;
        }

        private void OnSocioSeleccionadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            Socio = socio;
        }

        public ICommand NuevoSocioCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }

        private void OnNuevoSocioCommandExecute()
        {
            var o = new NuevoSocioView();
            o.ShowDialog();
        }

        private void OnExportarCommandExecute()
        {
            //var ListaSocios = _SocioRepository.GetAll();
            //_ExportService.ExportarSocios(ListaSocios);
            if (VistaCargada != "SociosContentView")
            {
                var ListaSocios = _SocioRepository.GetAll();
                _ExportService.ExportarSocios(ListaSocios);
            }
            else _ExportService.ExportarSocio(Socio, Socio.Nombre);
            //if (Socio == null)
            //{
            //    var ListaSocios = _SocioRepository.GetAll();
            //    _ExportService.ExportarSocios(ListaSocios);
            //}
            //else _ExportService.ExportarSocio(Socio, Socio.Nombre);
        }
    }
}
