using Gama.Cooperacion.Wpf.Controls;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;


namespace Gama.Cooperacion.Wpf.Converters
{
    [ValueConversion(typeof(ObservableCollection<ActividadWrapper>), typeof(ObservableCollection<ActividadWrapper>))]
    public class AppointmentsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values == null)
                    return new List<ActividadWrapper>();

                if (values[0] == null)
                    return new List<ActividadWrapper>();

                Day day = values[1] as Day;
                DateTime date;

                if (day != null)
                    date = ((Day)values[1]).Date;
                else
                    return new List<ActividadWrapper>();

                ObservableCollection<ActividadWrapper> appointments = new ObservableCollection<ActividadWrapper>();

                if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                {

                    foreach (ActividadWrapper appointment in (ObservableCollection<ActividadWrapper>)values[0])
                    {
                        if (IsSameYearMonthDay(appointment.FechaDeFin, date))
                        {
                            appointments.Add(appointment);
                        }
                    }
                }
                return appointments;
            }
            catch (Exception ex)
            {
                return new List<ActividadWrapper>();
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
