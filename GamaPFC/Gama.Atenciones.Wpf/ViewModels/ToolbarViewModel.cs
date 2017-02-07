using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Views;
using Gama.Common.Views;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private Persona _Persona;
        private IPersonaRepository _PersonaRepository;
        private IEventAggregator _EventAggregator;

        public ToolbarViewModel(
            IPersonaRepository PersonaRepository,
            //ExportService exportService,
            IEventAggregator eventAggregator,
            ISession session)
        {
            _PersonaRepository = PersonaRepository;
            _PersonaRepository.Session = session;
            //_ExportService = exportService;
            _EventAggregator = eventAggregator;

            NuevaPersonaCommand = new DelegateCommand(OnNuevaPersonaCommandExecute);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);
            //EliminarPersonaCommand = new DelegateCommand(OnEliminarPersonaCommandExecute);

            _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Subscribe(OnPersonaSeleccionadaEvent);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaSeleccionadaEvent);
        }

        private void OnPersonaSeleccionadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            _PersonaRepository.Session.Evict(persona);

            _Persona = persona;
        }

        public ICommand NuevaPersonaCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }

        private void OnNuevaPersonaCommandExecute()
        {
            var o = new NuevaPersonaView();
            o.ShowDialog();
        }

        private void OnExportarCommandExecute()
        {
            //throw new NotImplementedException();
        }
    }
}
