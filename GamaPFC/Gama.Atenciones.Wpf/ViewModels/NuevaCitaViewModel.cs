using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class NuevaCitaViewModel : ViewModelBase
    {
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IPersonaRepository _PersonaRepository;
        private ICitaRepository _CitaRepository;

        public NuevaCitaViewModel(IPersonaRepository personaRepository, 
            ICitaRepository citaRepository)
        {
            Cita = new CitaWrapper(new Cita());
            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;

            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);

            Cita.PropertyChanged += Cita_PropertyChanged;
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
            Cita.Persona = persona;
            Cita.Asistente = "ogkertet";
            Cita.Sala = "Sala W";
        }

        private void OnAceptarCommand_Execute()
        {
            Persona.Citas.Add(Cita);
            _PersonaRepository.Update(Persona.Model);
            Persona.AcceptChanges();
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return Cita.Inicio != null && Cita.Asistente != null;
        }

        private void OnCancelarCommand_Execute()
        {
            Persona.RejectChanges();
            Cerrar = true;
        }
    }
}
