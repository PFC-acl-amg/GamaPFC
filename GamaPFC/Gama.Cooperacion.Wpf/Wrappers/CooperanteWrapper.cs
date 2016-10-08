using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class CooperanteWrapper : TimestampedModelWrapper<Cooperante>
    {
        public CooperanteWrapper(Cooperante model) : base (model)
        {
        }

        protected override void InitializeCollectionProperties(Cooperante model)
        {
            if (model.Emails == null)
                throw new ArgumentNullException("Emails");

            this.Emails = new ChangeTrackingCollection<EmailWrapper>
                (model.Emails.Select(e => new EmailWrapper(e)));
            this.RegisterCollection(this.Emails, model.Emails.ToList());

            if (model.Telefonos == null)
                throw new ArgumentNullException("Telefonos");

            this.Telefonos = new ChangeTrackingCollection<TelefonoWrapper>
                (model.Telefonos.Select(t => new TelefonoWrapper(t)));
            this.RegisterCollection(this.Telefonos, model.Telefonos.ToList());
        }

        public string Apellido
        {
            get { return GetValue<string>(); }
            set { SetValue(value); OnPropertyChanged("NombreCompleto"); }
        }

        public string ApellidoOriginalValue => GetOriginalValue<string>(nameof(Apellido));

        public bool ApellidoIsChanged => GetIsChanged(nameof(Apellido));

        public string Dni
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DniOriginalValue => GetOriginalValue<string>(nameof(Dni));

        public bool DniIsChanged => GetIsChanged(nameof(Dni));

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Nombre
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
                OnPropertyChanged("NombreCompleto");
            }
        }

        public string NombreOriginalValue => GetOriginalValue<string>(nameof(Nombre));

        public bool NombreIsChanged => GetIsChanged(nameof(Nombre));

        public string Observaciones
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ObservacionesOriginalValue => GetOriginalValue<string>(nameof(Observaciones));

        public bool ObservacionesIsChanged => GetIsChanged(nameof(Observaciones));

        // Propiedad del Wrapper únicamente
        public string NombreCompleto => string.Format("{0} {1}", Nombre, Apellido);

        public ChangeTrackingCollection<EmailWrapper> Emails { get; private set; }
        public ChangeTrackingCollection<TelefonoWrapper> Telefonos { get; private set; }
    }
}
