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
                _Citas = value;
                OnPropertyChanged("Citas");
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
            vm.Load(Persona);
            vm.Cita.Fecha = fechaSeleccionada.Date;
            vm.Session = _Session;
            o.ShowDialog();
        }

        private void OnEditarCitaCommandExecute(CitaWrapper wrapper)
        {
            var o = new NuevaCitaView();
            o.Title = "Editar Cita";
            var vm = (NuevaCitaViewModel)o.DataContext;
            vm.Load(Persona);
            vm.EnEdicionDeCitaExistente = true;
            vm.Cita.CopyValuesFrom(wrapper.Model);
            vm.Session = _Session;
            //CitaWrapper citaActualizada = Citas.Where(x => x.Id == vm.Cita.Id).FirstOrDefault();
            //citaActualizada.CopyValuesFrom(vm.Cita.Model);
            o.ShowDialog();
            Refresh++;
        }
    }
}
