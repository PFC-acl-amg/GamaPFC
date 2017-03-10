using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Gama.Atenciones.Wpf.Eventos;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using Prism.Events;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class NuevaCitaViewModel : ViewModelBase
    {
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IPersonaRepository _PersonaRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;

        public NuevaCitaViewModel(IPersonaRepository personaRepository, 
            ICitaRepository citaRepository, IEventAggregator eventAggregator)
        {
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _EventAggregator = eventAggregator;
            
            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);
        }

        private void Cita_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        public CitaWrapper Cita { get; private set; }
        public PersonaWrapper Persona { get; private set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }
        public ISession Session
        {
            get { return null; }
            set
            {
                _PersonaRepository.Session = value;
                _CitaRepository.Session = value;
            }
        }

        public void Load(PersonaWrapper persona)
        {
            Persona = persona;
            Cita = new CitaWrapper(new Cita() { Persona = Persona.Model });
            Cita.PropertyChanged += Cita_PropertyChanged;
            //Cita.Persona = persona;
            OnPropertyChanged(nameof(Cita));
        }

        private void OnAceptarCommand_Execute()
        {
            Persona.Citas.Add(Cita);
            _PersonaRepository.Update(Persona.Model);
            Persona.AcceptChanges();
            _EventAggregator.GetEvent<CitaCreadaEvent>().Publish(Cita.Id);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            if (Cita == null) return false;

            return Cita.IsValid;
        }

        private void OnCancelarCommand_Execute()
        {
            Persona.RejectChanges();
            Cerrar = true;
        }
    }
}
