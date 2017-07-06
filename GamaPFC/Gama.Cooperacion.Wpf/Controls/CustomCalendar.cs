using Gama.Cooperacion.Wpf.Wrappers;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;

namespace Gama.Cooperacion.Wpf.Controls
{
    public class CustomCalendar : Control
    {
        public ObservableCollection<string> DayNames { get; set; }

        //public static CustomCalendar _Control;

        public event EventHandler<DayChangedEventArgs> DayChanged;

        static CustomCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCalendar),
                new FrameworkPropertyMetadata(typeof(CustomCalendar)));
        }

        public CustomCalendar()
        {
            Days = new ObservableCollection<Day>();
            DayNames = new ObservableCollection<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo", };

            BuildCalendar(DateTime.Today);

            SemanaAnteriorCommand = new DelegateCommand(OnSemanaAnteriorCommandExecute);
            SemanaSiguienteCommand = new DelegateCommand(OnSemanaSiguienteCommandExecute);
            ToggleNavegacionCommand = new DelegateCommand<object>(OnToggleNavegacionCommandExecute);
        }

        private static void OnCurrentDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CustomCalendar;
            if (cc != null)
            {
                if (e.NewValue != null && !IsSameYearMonthDay((DateTime)e.NewValue, (DateTime)e.OldValue))
                {
                    cc.OnCurrentDateChanged((DateTime)e.NewValue);
                }
            }
        }

        public void OnCurrentDateChanged(DateTime newValue)
        {
            BuildCalendar(newValue);
        }

        private static void OnRefreshChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CustomCalendar;
            if (cc != null)
            {
                cc.BuildCalendar(cc.CurrentDate);
            }
        }

        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register(
                "CurrentDate",
                typeof(DateTime),
                typeof(CustomCalendar),
                new PropertyMetadata(DateTime.Now, OnCurrentDateChanged));

        public static readonly DependencyProperty RefreshProperty =
            DependencyProperty.Register(
                "Refresh",
                typeof(int),
                typeof(CustomCalendar),
                new PropertyMetadata(0, OnRefreshChanged));

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
                typeof(ObservableCollection<ActividadWrapper>),
                typeof(CustomCalendar),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnAppointmentsPropertyChanged))
            );

        private static void OnAppointmentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CustomCalendar;
            if (cc != null)
            {
                var items = e.NewValue as ObservableCollection<ActividadWrapper>;
                if (items == null)
                    items = new ObservableCollection<ActividadWrapper>();

                if (items != null)
                    items.CollectionChanged += new NotifyCollectionChangedEventHandler(cc.AppointmentsChanged);
            }
        }

        private void AppointmentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate { BuildCalendar(CurrentDate); });
        }

        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        public int Refresh
        {
            get { return (int)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }

        public ObservableCollection<Day> Days
        {
            get { return (ObservableCollection<Day>)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }

        public ObservableCollection<object> Appointments
        {
            get { return (ObservableCollection<object>)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }

        public ICommand SemanaAnteriorCommand { get; private set; }
        public ICommand SemanaSiguienteCommand { get; private set; }
        public ICommand ToggleNavegacionCommand { get; private set; }

        private void OnToggleNavegacionCommandExecute(object parameters)
        {
            var values = (object[])parameters;
            var toggleButton = values[0] as FrameworkElement;
            var stackPanel = values[1] as FrameworkElement;
            var calendarControl = values[2] as FrameworkElement;
            var otherToggleButton = values[3] as FrameworkElement;

            if (stackPanel.Visibility == Visibility.Visible)
            {
                stackPanel.Visibility = Visibility.Collapsed;
                toggleButton.Visibility = Visibility.Collapsed;
                otherToggleButton.Visibility = Visibility.Visible;
                calendarControl.Margin = new Thickness(0, -40, 0, 0);
            }
            else
            {
                stackPanel.Visibility = Visibility.Visible;
                toggleButton.Visibility = Visibility.Visible;
                otherToggleButton.Visibility = Visibility.Collapsed;

                calendarControl.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        public void ExpandNavigation()
        {

        }

        public void CollapseNavigation()
        {

        }

        private void OnSemanaAnteriorCommandExecute()
        {
            CurrentDate = CurrentDate.AddDays(-7);
        }

        private void OnSemanaSiguienteCommandExecute()
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
                Day day = new Day
                {
                    Date = startDate,
                    Enabled = true,
                    IsToday = startDate.Date == DateTime.Today.Date,
                    IsTargetMonth = targetDate.Month == startDate.Month
                };

                day.PropertyChanged += Day_Changed;

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
