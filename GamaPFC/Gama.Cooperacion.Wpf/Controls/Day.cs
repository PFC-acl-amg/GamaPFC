using System;
using System.ComponentModel;

namespace Gama.Cooperacion.Wpf.Controls
{
    public class Day : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime date;
        private string notes;
        private bool enabled;
        private bool isTargetMonth;
        private bool isToday;

        public bool IsToday
        {
            get { return isToday; }
            set
            {
                isToday = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsToday"));
            }
        }

        public bool IsTargetMonth
        {
            get { return isTargetMonth; }
            set
            {
                isTargetMonth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsTargetMonth"));
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notes"));
            }
        }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Date"));
            }
        }
    }
}
