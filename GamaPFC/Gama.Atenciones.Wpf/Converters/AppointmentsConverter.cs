﻿using Gama.Atenciones.Wpf.Controls;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace Gama.Atenciones.Wpf.Converters
{
    [ValueConversion(typeof(ObservableCollection<CitaWrapper>), typeof(ObservableCollection<CitaWrapper>))]
    public class AppointmentsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values == null)
                    return new List<CitaWrapper>();

                if (values[0] == null)
                    return new List<CitaWrapper>();

                Day day = values[1] as Day;
                DateTime date;

                if (day != null)
                    date = ((Day)values[1]).Date;
                else
                    return new List<CitaWrapper>();

                ObservableCollection<CitaWrapper> appointments = new ObservableCollection<CitaWrapper>();

                if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
                {

                    foreach (CitaWrapper appointment in (ObservableCollection<CitaWrapper>)values[0])
                    {
                        if (IsSameYearMonthDay(appointment.Fecha.Value, date))
                        {
                            appointments.Add(appointment);
                        }
                    }
                }
                return appointments;
            }
            catch (Exception ex)
            {
                return new List<CitaWrapper>();
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
