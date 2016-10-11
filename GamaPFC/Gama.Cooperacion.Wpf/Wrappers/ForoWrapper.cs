using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class ForoWrapper : ModelWrapper<Foro>
    {
        public ForoWrapper(Foro model) : base(model)
        {
            InitializeCollectionProperties(model); // Para poder inicilizar la lista mensajes que tiene el foro
        }

        private void InitializeCollectionProperties(Foro model)
        {
            if (model.Mensajes == null)
            {
                throw new ArgumentNullException("Mensajes");
            }

            this.Mensajes = new ChangeTrackingCollection<MensajeWrapper>
                (model.Mensajes.Select(c => new MensajeWrapper(c)));
            this.RegisterCollection(this.Mensajes, model.Mensajes);
        }
        public ChangeTrackingCollection<MensajeWrapper> Mensajes { get; private set; }
        public int Id
        {
            get { return GetValue<int>(); }
            set { } // No se pone nada dentro del set porque los mensajes son para ller solo no se modificaran ni se borraran asi tma read only
        }
        public string Titulo
        {
            get { return GetValue<string>(); }
            set { }
        }
        public DateTime FechaDePublicacion
        {
            get { return GetValue<DateTime>(); }
            set { }
        }

        //-----        
        
        private bool _ForoVisible = true;
        public bool ForoVisible
        {
            get { return _ForoVisible; }
            set
            {
                if (_ForoVisible != value)
                {
                    _ForoVisible = value;
                    OnPropertyChanged();
                }

            }
        }

        public ChangeTrackingCollection<EventoWrapper> Eventos { get; set; }

    }
}
