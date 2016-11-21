using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class NuevaCitaViewModel : ViewModelBase
    {
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public NuevaCitaViewModel()
        {
            Cita = new CitaWrapper(new Cita());

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

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public void Load(PersonaWrapper persona)
        {
            Cita.Persona = persona;
        }

        private void OnAceptarCommand_Execute()
        {
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return Cita.Inicio != null && Cita.Asistente != null;
        }

        private void OnCancelarCommand_Execute()
        {
            Cerrar = true;
        }
    }
}
