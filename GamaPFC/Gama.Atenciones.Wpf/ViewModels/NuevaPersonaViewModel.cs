using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
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
    public class NuevaPersonaViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PersonaViewModel _PersonaVM;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public NuevaPersonaViewModel(
            IPersonaRepository personaRepository,
            IEventAggregator eventAggregator,
            PersonaViewModel personaViewModel,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _PersonaVM = personaViewModel;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;

            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);

            Persona.PropertyChanged += Persona_PropertyChanged;
        }

        private void Persona_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        public PersonaViewModel PersonaVM
        {
            get { return _PersonaVM; }
        }

        public PersonaWrapper Persona
        {
            get { return _PersonaVM.Persona; }
        }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        private void OnAceptarCommand_Execute()
        {
            Persona.CreatedAt = DateTime.Now;
            _PersonaRepository.Create(Persona.Model);
            _EventAggregator.GetEvent<NuevaPersonaEvent>().Publish(Persona.Id);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return Persona.IsValid;
        }
    }
}
