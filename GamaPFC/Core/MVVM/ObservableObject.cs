using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler _PropertyChanged;

        // Se usa para que si en algún lugar se escribe una sentencia
        // como esta:
        //   obj.PropertyChanged -= obj_PropertyChanged;
        //   obj.PropertyChanged += obj_PropertyChanged;
        // , el evento no se añadirá más de una vez, lo que provocaría
        // que el delegado 'obj_PropertyChanged' se ejecutara más de
        // una vez.
        List<PropertyChangedEventHandler> _PropertyChangedSuscribers
            = new List<PropertyChangedEventHandler>();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_PropertyChangedSuscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSuscribers.Add(value);
                }
            }
            remove
            {
                _PropertyChanged -= value;
                _PropertyChangedSuscribers.Remove(value);
            }
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
