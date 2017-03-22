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
            EliminarPersonaCommand = new DelegateCommand(OnEliminarPersonaCommandExecute, 
                () => _Persona != null);

            _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Subscribe(OnPersonaSeleccionadaChangedEvent);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaSeleccionadaChangedEvent);
        }

        private void OnPersonaSeleccionadaChangedEvent(int id)
        {
            if (id != 0)
            {
                var persona = _PersonaRepository.GetById(id);
                _PersonaRepository.Session.Evict(persona);

                _Persona = persona;
            }
            else
            {
                _Persona = null;
            }

            InvalidateCommands();
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)EliminarPersonaCommand).RaiseCanExecuteChanged();
        }

        private void OnEliminarPersonaCommandExecute()
        {
            var o = new ConfirmarOperacionView();
            o.Mensaje = "¿Está seguro de que desea eliminar esta persona y todos sus registros?";
            o.ShowDialog();

            if (o.EstaConfirmado)
            {
                int id = _Persona.Id;
                // WARNING: Debe hacer antes la publicación del evento porque se recoge
                // la persona para ver sus citas y atenciones desde otros viewmodels
                _EventAggregator.GetEvent<PersonaEliminadaEvent>().Publish(id);
                _PersonaRepository.Delete(_Persona);
            }
        }

    public ICommand NuevaPersonaCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }
        public ICommand EliminarPersonaCommand { get; private set; }

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
