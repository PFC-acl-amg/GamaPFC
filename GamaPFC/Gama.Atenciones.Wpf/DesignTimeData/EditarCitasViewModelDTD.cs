using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class EditarCitasViewModelDTD : ViewModelBase
    {
        public EditarCitasViewModelDTD()
        {
            //Citas = new ObservableCollection<CitaWrapper>(
            //    new FakeServices.FakeCitaRepository().GetAll().Select(c => new CitaWrapper(c)));
            Citas = new ObservableCollection<CitaWrapper>(new FakeServices.FakeCitaRepository().GetAll().Select(x => new CitaWrapper(x)));
            Appointments = new ObservableCollection<Appointment>();

            Appointments.Add(new Appointment() { Sala = "Sala A", Date = DateTime.Now, Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(), Faker.NameFaker.LastName()) });
            Appointments.Add(new Appointment() { Sala = "Sala B", Date = DateTime.Now, Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(),Faker.NameFaker.LastName()) });
            Appointments.Add(new Appointment() { Sala = "Sala A", Date = DateTime.Now, Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(),Faker.NameFaker.LastName()) });
            Appointments.Add(new Appointment() { Sala = "Sala B", Date = DateTime.Now.AddDays(7), Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(),Faker.NameFaker.LastName()) });
            Appointments.Add(new Appointment() { Sala = "Sala A", Date = DateTime.Now.AddDays(14), Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(), Faker.NameFaker.LastName()) });
                                                                                                                                      
            Appointments.Add(new Appointment() { Sala = "Sala A", Date = DateTime.Now.AddDays(21), Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(), Faker.NameFaker.LastName()) });
            Appointments.Add(new Appointment() { Sala = "Sala A", Date = DateTime.Now.AddDays(28), Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(), Faker.NameFaker.LastName()) });
            Appointments.Add(new Appointment() { Sala = "Sala A", Date = DateTime.Now.AddDays(4), Subject = string.Format("{0} {1}", Faker.NameFaker.FirstName(), Faker.NameFaker.LastName()) });

            CurrentDate = DateTime.Now;

            NuevaCitaCommand = new DelegateCommand(OnNuevaCitaCommandExecute);
        }

        private void OnNuevaCitaCommandExecute()
        {
            CurrentDate = CurrentDate.AddDays(7);
        }

        public ObservableCollection<Appointment> Appointments { get; private set; }
        public ObservableCollection<CitaWrapper> Citas { get; private set; }
        public ICommand NuevaCitaCommand { get; private set; }

        private DateTime _CurrentDate;
        public DateTime CurrentDate
        {
            get { return _CurrentDate; }
            set { SetProperty(ref _CurrentDate, value); }
        }
    }
}
