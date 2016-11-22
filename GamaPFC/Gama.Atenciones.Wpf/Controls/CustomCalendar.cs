using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;

namespace Gama.Atenciones.Wpf.Controls
{
    public class CustomCalendar : Control
    {
        public ObservableCollection<string> DayNames { get; set; }

        public static CustomCalendar _Control;

        public event EventHandler<DayChangedEventArgs> DayChanged;

        private static void OnCurrentDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CustomCalendar;
            if (cc != null)
            {
                if (e.NewValue != null && !IsSameYearMonthDay((DateTime)e.NewValue,(DateTime)e.OldValue))
                {
                    cc.OnCurrentDateChanged((DateTime)e.NewValue);
                }
            }
        }

        public void OnCurrentDateChanged(DateTime newValue)
        {
            //CurrentDate = newValue;
            BuildCalendar(newValue);
        }

        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register(
                "CurrentDate",
                typeof(DateTime),
                typeof(CustomCalendar),
                new PropertyMetadata(DateTime.Now, OnCurrentDateChanged));

        public static readonly DependencyProperty DaysProperty =
            DependencyProperty.Register(
                "Days", 
                typeof(ObservableCollection<Day>), 
                typeof(CustomCalendar),
                new PropertyMetadata(new ObservableCollection<Day>()));


        public static DependencyProperty AppointmentsProperty =
            DependencyProperty.Register
            (
                "Appointments",
                typeof(ObservableCollection<CitaWrapper>),
                typeof(CustomCalendar),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnAppointmentsPropertyChanged))
            );



        public int Refresh
        {
            get { return (int)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Refresh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshProperty =
            DependencyProperty.Register("Refresh", typeof(int), typeof(CustomCalendar), new PropertyMetadata(0,
                new PropertyChangedCallback(OnAppointmentsPropertyChanged)));

        private static void OnAppointmentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CustomCalendar;
            _Control = cc;
            if (cc != null)
            {
                var items = e.NewValue as ObservableCollection<CitaWrapper>;
                items.CollectionChanged += new NotifyCollectionChangedEventHandler(AppointmentsChanged);
            }
        }

        private static void AppointmentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _Control.BuildCalendar(_Control.CurrentDate);
        }

        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        public ObservableCollection<Day> Days
        {
            get { return (ObservableCollection<Day>)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }

        public ObservableCollection<CitaWrapper> Appointments
        {
            get { return (ObservableCollection<CitaWrapper>)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }

        static CustomCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCalendar),
                new FrameworkPropertyMetadata(typeof(CustomCalendar)));
        }

        public CustomCalendar()
        {
            Days = new ObservableCollection<Day>();
            DayNames = new ObservableCollection<string> { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado", "Domingo", };
            //Appointments = new ObservableCollection<CitaWrapper>();
            
            BuildCalendar(DateTime.Today);
            //CurrentDate = DateTime.Today;

            SemanaAnteriorCommand = new DelegateCommand(OnSemanaAnteriorCommand);
            SemanaSiguienteCommand = new DelegateCommand(OnSemanaSiguienteCommand);
        }

        public ICommand SemanaAnteriorCommand { get; private set; }
        public ICommand SemanaSiguienteCommand { get; private set; }

        private void OnSemanaAnteriorCommand()
        {
            CurrentDate = CurrentDate.AddDays(-7);
        }

        private void OnSemanaSiguienteCommand()
        {
            CurrentDate = CurrentDate.AddDays(7);
        }

        public void BuildCalendar(DateTime targetDate)
        {
            Days.Clear();

            //Calculate when the first day of the month is and work out an 
            //offset so we can fill in any boxes before that.
            DateTime startDate = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day);

            while (startDate.DayOfWeek != DayOfWeek.Monday)
            {
                startDate = startDate.AddDays(-1);
            }

            //Show 3 weeks each with 7 days = 21 42
            for (int box = 1; box <= 21; box++)
            {
                Day day = new Day { Date = startDate, Enabled = true, IsTargetMonth = targetDate.Month == startDate.Month };
                day.PropertyChanged += Day_Changed;
                day.IsToday = startDate == DateTime.Today;

                //if (Appointments.Where(a => a.Date == day.Date).Any())
                //{
                //    Days.AddRange(Appointments.Where(a => a.Date == day.Date)
                //        .Select(a => new Day { Date = a.Date }).ToList());
                //}
                
                Days.Add(day);
                startDate = startDate.AddDays(1);
            }
        }

        private void Day_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Notes") return;
            if (DayChanged == null) return;

            DayChanged(this, new DayChangedEventArgs(sender as Day));
        }

        private static int DayOfWeekNumber(DayOfWeek dow)
        {
            return Convert.ToInt32(dow.ToString("D"));
        }

        public static bool IsSameYearMonthDay(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month && a.Day == b.Day);
        }
    }
    
    public class DayChangedEventArgs : EventArgs
    {
        public Day Day { get; private set; }

        public DayChangedEventArgs(Day day)
        {
            this.Day = day;
        }
    }
}
