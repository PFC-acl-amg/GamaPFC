﻿using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Controls;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.Views;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class CitasContentViewModel : ViewModelBase
    {
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private ISession _Session;

        public CitasContentViewModel(
            IEventAggregator eventAggregator,
            ICitaRepository citaRepository,
            IPersonaRepository personaRepository,
            IAsistenteRepository asistenteRepository,
            Preferencias preferencias,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _CitaRepository = citaRepository;
            _CitaRepository.Session = session;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _AsistenteRepository = asistenteRepository;
            _AsistenteRepository.Session = session;
            _Session = session;
            Preferencias = preferencias;

            Citas = new ObservableCollection<CitaWrapper>(
                _CitaRepository.GetAll()
                .Select(x => new CitaWrapper(x)));

            NuevaCitaCommand = new DelegateCommand<Day>(OnNuevaCitaCommandExecute);
            NuevaAtencionCommand = new DelegateCommand<CitaWrapper>(OnNuevaAtencionCommandExecute);
            EditarCitaCommand = new DelegateCommand<CitaWrapper>(OnEditarCitaCommandExecute);
            SeleccionarPersonaCommand = new DelegateCommand<CitaWrapper>(OnSeleccionarPersonaCommand);

            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            _EventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
            _EventAggregator.GetEvent<CitaEliminadaEvent>().Subscribe(OnCitaEliminadaEvent);

            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);

            _EventAggregator.GetEvent<AsistenteActualizadoEvent>().Subscribe(OnAsistenteActualizadoEvent);
        }

        public Preferencias Preferencias { get; private set; }

        public ICommand NuevaCitaCommand { get; private set; }
        public ICommand NuevaAtencionCommand { get; private set; }
        public ICommand EditarCitaCommand { get; private set; }
        public ICommand SeleccionarPersonaCommand { get; private set; }

        private int _Refresh;
        private IPersonaRepository _PersonaRepository;
        private IAsistenteRepository _AsistenteRepository;

        public int Refresh
        {
            get { return _Refresh; }
            set { SetProperty(ref _Refresh, value); }
        }

        public ObservableCollection<CitaWrapper> Citas { get; private set; }

        private void OnSeleccionarPersonaCommand(CitaWrapper wrapper)
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish(wrapper.Persona.Id);
        }

        private void OnNuevaAtencionCommandExecute(CitaWrapper cita)
        {
            _EventAggregator.GetEvent<NuevaAtencionEvent>().Publish(cita);
        }

        private void OnNuevaCitaCommandExecute(Day fechaSeleccionada)
        {
            var o = new NuevaCitaView();
            var vm = (NuevaCitaViewModel)o.DataContext;
            //vm.Session = _Session;
            vm.LoadWithDefaultPerson();
            vm.Cita.Fecha = fechaSeleccionada.Date;
            o.ShowDialog();
            Refresh++;
        }

        private void OnEditarCitaCommandExecute(CitaWrapper wrapper)
        {
            var o = new NuevaCitaView();
            o.Title = "Editar Cita";
            var vm = (NuevaCitaViewModel)o.DataContext;
            //vm.Session = _Session;
            vm.EnEdicionDeCitaExistente = true;
            vm.LoadForEdition(wrapper);
            o.ShowDialog();
            Refresh++;
        }

        private void OnCitaCreadaEvent(int id)
        {
            Cita cita = _CitaRepository.GetById(id);
            Citas.Add(new CitaWrapper(cita));
        }

        private void OnCitaActualizadaEvent(int citaId)
        {
            Cita cita = _CitaRepository.GetById(citaId);

            Cita citaDesactualizada = Citas.Select(x => x.Model).First(x => x.Id == citaId);
            citaDesactualizada.CopyValuesFrom(cita);
            OnPropertyChanged(nameof(Citas));
        }

        private void OnCitaEliminadaEvent(int citaId)
        {
            CitaWrapper cita = Citas.Where(x => x.Id == citaId).First();
            Citas.Remove(cita);
            Refresh++;
        }

        private void OnPersonaActualizadaEvent(int personaId)
        {
            Persona persona = _PersonaRepository.GetById(personaId);

            var citas = Citas.Where(x => x.Persona.Id == personaId).ToList();

            foreach (var cita in citas)
            {
                cita.Persona.CopyValuesFrom(persona);
            }

            Refresh++;
        }

        private void OnPersonaEliminadaEvent(int personaId)
        {
            var citas = Citas.Where(x => x.Persona.Id == personaId).ToList();

            foreach (var wrapper in citas)
                Citas.Remove(wrapper);

            Refresh++;
        }

        private void OnAsistenteActualizadoEvent(int asistenteId)
        {
            Asistente asistente = _AsistenteRepository.GetById(asistenteId);

            var citas = Citas.Where(x => x.Asistente.Id == asistenteId).ToList();

            foreach (var wrapper in citas)
                wrapper.Asistente.CopyValuesFrom(asistente);

            Refresh++;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("CitasContentView");
        }
    }
}
