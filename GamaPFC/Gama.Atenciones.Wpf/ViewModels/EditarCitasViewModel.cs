using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Controls;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Views;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarCitasViewModel : ViewModelBase
    {
        private ICitaRepository _CitaRepository;
        private ISession _Session;

        public EditarCitasViewModel(ICitaRepository citaRepository)
        {
            _CitaRepository = citaRepository;

            NuevaCitaCommand = new DelegateCommand<Day>(OnNuevaCitaCommandExecute);
            NuevaAtencionCommand = new DelegateCommand<CitaWrapper>(OnNuevaAtencionExecute);
            Citas = new ObservableCollection<CitaWrapper>();
            Citas.Add(new CitaWrapper(new Cita { Inicio = DateTime.Now, Asistente = "Asistente", Sala = "Sala B" }));
        }

        private void OnNuevaAtencionExecute(CitaWrapper cita)
        {
            var ok = new MahApps.Metro.Controls.MetroWindow();
            ok.Content = new System.Windows.Controls.TextBlock { Text = "Aguachineinao" };
            ok.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ok.ShowDialog();
        }

        public void Load(PersonaWrapper wrapper)
        {
            Persona = wrapper;
            //Citas = new ObservableCollection<Cita>();
            Citas = Persona.Citas;
            //OnPropertyChanged("Atenciones");
        }

        private void OnNuevaCitaCommandExecute(Day fechaSeleccionada)
        {
            var o = new NuevaCitaView();
            var vm = (NuevaCitaViewModel)o.DataContext;
            vm.Cita.Inicio = fechaSeleccionada.Date;
            vm.Session = _Session;
            vm.Load(Persona);
            o.ShowDialog();
        }

        private int _Refresh;
        public int Refresh
        {
            get { return _Refresh; }
            set { SetProperty(ref _Refresh, value); }
        }

        public ICommand NuevaCitaCommand { get; private set; }
        public ICommand NuevaAtencionCommand { get; private set; }

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
        public ObservableCollection<CitaWrapper> _Citas;
        public ObservableCollection<CitaWrapper> Citas
        {
            get { return _Citas; }
            set
            {
                _Citas = value;
                OnPropertyChanged("Citas");
            }
        }
    }
}
