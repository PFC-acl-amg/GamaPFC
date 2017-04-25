using Core;
using Gama.Atenciones.Wpf.Controls;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Views;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarCitasViewModel : ViewModelBase
    {
        private ICitaRepository _CitaRepository;
        private ISession _Session;
        public ObservableCollection<CitaWrapper> _Citas;
        private IEventAggregator _EventAggregator;

        public EditarCitasViewModel(ICitaRepository citaRepository, IEventAggregator eventAggregator)
        {
            _CitaRepository = citaRepository;
            _EventAggregator = eventAggregator;

            NuevaCitaCommand = new DelegateCommand<Day>(OnNuevaCitaCommandExecute);
            NuevaAtencionCommand = new DelegateCommand<CitaWrapper>(OnNuevaAtencionCommandExecute);
            EditarCitaCommand = new DelegateCommand<CitaWrapper>(OnEditarCitaCommandExecute);

            _EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
        }

        public ICommand NuevaCitaCommand { get; private set; }
        public ICommand NuevaAtencionCommand { get; private set; }
        public ICommand EditarCitaCommand { get; private set; }

        private int _Refresh;
        public int Refresh
        {
            get { return _Refresh; }
            set { SetProperty(ref _Refresh, value); }
        }

        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _CitaRepository.Session = _Session;
            }
        }

        public PersonaWrapper Persona { get; set; }

        public ObservableCollection<CitaWrapper> Citas
        {
            get { return _Citas; }
            set
            {
                if (_Citas != value)
                {
                    _Citas = value;
                    OnPropertyChanged("Citas");
                }
            }
        }

        public void Load(PersonaWrapper wrapper)
        {
            Persona = wrapper;
            Citas = Persona.Citas;
        }

        private void OnNuevaAtencionCommandExecute(CitaWrapper cita)
        {
            _EventAggregator.GetEvent<NuevaAtencionEvent>().Publish(cita);
        }

        private void OnNuevaCitaCommandExecute(Day fechaSeleccionada)
        {
            var o = new NuevaCitaView();
            var vm = (NuevaCitaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.LoadForCreation(Persona, fechaSeleccionada.Date);
            o.ShowDialog();
            Persona.AcceptChanges();
        }

        private void OnEditarCitaCommandExecute(CitaWrapper wrapper)
        {
            var o = new NuevaCitaView() { Title = "Editar Cita" };
            var vm = (NuevaCitaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.EnEdicionDeCitaExistente = true;
            vm.LoadForEdition(wrapper);
            o.ShowDialog();
            Persona.AcceptChanges();
            Refresh++;
        }

        private void OnCitaCreadaEvent(int id)
        {
            var cita = _CitaRepository.GetById(id);
            if (cita.Persona.Id == Persona.Id && !Persona.Citas.Any(c => c.Id == id))
            {
                Persona.Citas.Add(new CitaWrapper(cita));
            }
        }
    }
}
