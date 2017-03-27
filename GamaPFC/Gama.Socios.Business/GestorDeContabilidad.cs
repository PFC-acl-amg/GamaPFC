using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Gama.Socios.Business
{
    public class GestorDeContabilidad:ObservableObject
    {
        public GestorDeContabilidad()
        {
            
        }

        //public virtual ObservableCollection<LookupItemSocio> ListarSocios()
        // {

        // }

        // Hacer visible el Stack Panel para busrcar por distintos campos en la lista de socios
        private bool _BuscardorVisible = true;
        public bool BuscadorVisible
        {
            get { return _BuscardorVisible; }
            set
            {
                if (_BuscardorVisible != value)
                {
                    _BuscardorVisible = value;
                    OnPropertyChanged();
                }

            }
        }
        //-----------------
    }
}
