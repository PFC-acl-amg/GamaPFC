using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controls
{
    public class ChartItem : ObservableObject
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Key;
        public string Key
        {
            get { return _Key; }
            set
            {
                if (_Key != value)
                    _Key = value;
                OnPropertyChanged();
            }
        }

        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _StringValue;
        public string StringValue
        {
            get { return _StringValue; }
            set
            {
                if (_StringValue != value)
                    _StringValue = value;
                OnPropertyChanged();
            }
        }
    }
}
