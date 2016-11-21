using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Gama.Atenciones.Wpf.Converters
{
    /// <summary>
    /// Gets the appointments for the specified date.
    /// </summary>
    //[ValueConversion(typeof(ObservableCollection<Cita>), typeof(ObservableCollection<Cita>))]
    //public class AppointmentsConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        DateTime date = ((Day)values[1]).Date;

    //        ObservableCollection<Cita> appointments = new ObservableCollection<Cita>();

    //        if (values[0] != DependencyProperty.UnsetValue)
    //        {

    //            foreach (Cita appointment in (ObservableCollection<Cita>)values[0])
    //            {
    //                if (appointment.Inicio == date)
    //                {
    //                    appointments.Add(appointment);
    //                }
    //            }
    //        }
    //        return appointments;
    //    }

    [ValueConversion(typeof(ObservableCollection<Appointment>), typeof(ObservableCollection<Appointment>))]
    public class AppointmentsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Day day = values[1] as Day;
                DateTime date;

                if (day != null)
                    date = ((Day)values[1]).Date;
                else
                    return new List<Appointment>();

                ObservableCollection<Appointment> appointments = new ObservableCollection<Appointment>();

                if (values[0] != DependencyProperty.UnsetValue)
                {

                    foreach (Appointment appointment in (ObservableCollection<Appointment>)values[0])
                    {
                        if (IsSameYearMonthDay(appointment.Date, date))
                        {
                            appointments.Add(appointment);
                        }
                    }
                }
                return appointments;
            }
            catch (InvalidCastException ex)
            {
                return new List<Appointment>();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static bool IsSameYearMonthDay(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month && a.Day == b.Day);
        }
    }
}
