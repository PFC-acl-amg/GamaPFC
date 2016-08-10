using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = _PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
