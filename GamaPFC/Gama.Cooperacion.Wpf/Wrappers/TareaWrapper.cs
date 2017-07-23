using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class TareaWrapper : ModelWrapper<Business.Tarea>
    {
        public TareaWrapper(Business.Tarea model) : base(model)
        {
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
        }
        protected override void InitializeCollectionProperties(Business.Tarea model)
        {
            if (model.Seguimiento == null)
            {
                throw new ArgumentNullException("Historial");
            }
            else if (model.Incidencias == null)
            {
                throw new ArgumentNullException("Mensajes");
            }
            
            this.Incidencias = new ChangeTrackingCollection<IncidenciaWrapper>
                (model.Incidencias.Select(t => new IncidenciaWrapper(t)));
            this.RegisterCollection(this.Incidencias, model.Incidencias);

            this.Seguimiento = new ChangeTrackingCollection<SeguimientoWrapper>
                (model.Seguimiento.Select(t => new SeguimientoWrapper(t)));
            this.RegisterCollection(this.Seguimiento, model.Seguimiento);
        }
        protected override void InitializeComplexProperties(Business.Tarea model)
        {
            if (model.Responsable == null)
            {
                throw new ArgumentNullException("Resposable");
            }
            _Responsable = new CooperanteWrapper(model.Responsable);
            _ResponsableOriginalValue = new CooperanteWrapper(model.Responsable);
            this.ResponsableIsChanged = false;
        }
        private CooperanteWrapper _Responsable;
        public CooperanteWrapper Responsable
        {
            get { return _Responsable; }
            set
            {
                _Responsable = value;

                if (value != null)
                {
                    if (value.Id == ResponsableOriginalValue.Id)
                    {
                        ResponsableIsChanged = false;
                        OnPropertyChanged(nameof(IsChanged));
                    }
                    else
                    {
                        ResponsableIsChanged = true;
                        OnPropertyChanged(nameof(IsChanged));
                    }

                    SetValue(value.Model);
                }
            }
        }
        private CooperanteWrapper _ResponsableOriginalValue;
        public CooperanteWrapper ResponsableOriginalValue
        {
            get { return _ResponsableOriginalValue; }
            set
            {
                _ResponsableOriginalValue = value;
                if (Responsable.Id == ResponsableOriginalValue.Id)
                {
                    ResponsableIsChanged = false;
                }
                else
                {
                    ResponsableIsChanged = true;

                }
            }
        }
        public bool ResponsableIsChanged { get; set; }
        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Descripcion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DescripcionOriginalValue => GetOriginalValue<string>(nameof(Descripcion));

        public bool DescripcionIsChanged => GetIsChanged(nameof(Descripcion));

        public bool HaFinalizado
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool HaFinalizadoOriginalValue => GetOriginalValue<bool>(nameof(HaFinalizado));

        public bool HaFinalizadoIsChanged => GetIsChanged(nameof(HaFinalizado));

        public DateTime? FechaDeFinalizacion
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeFinalizacionOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDeFinalizacion));

        public bool FechaDeFinalizacionIsChanged => GetIsChanged(nameof(FechaDeFinalizacion));

        public ChangeTrackingCollection<IncidenciaWrapper> Incidencias { get; set; } // Antes se llamaba Incidencia
        public ChangeTrackingCollection<SeguimientoWrapper> Seguimiento { get; set; }

        private bool _SeguimientoVisible = true;
        public bool SeguimientoVisible
        {
            get { return _SeguimientoVisible; }
            set
            {
                if (_SeguimientoVisible != value)
                {
                    _SeguimientoVisible = value;
                    OnPropertyChanged();
                }

            }
        }
        private bool _IncidenciaVisible = true;
        public bool IncidenciaVisible
        {
            get { return _IncidenciaVisible; }
            set
            {
                if (_IncidenciaVisible != value)
                {
                    _IncidenciaVisible = value;
                    OnPropertyChanged();
                }

            }
        }

        public ActividadWrapper Actividad { get; set; }
        //public CooperanteWrapper Responsable { get;  set; } // Estaba puesto private set pero no me dejaba hacer la asignacion
        // TareasDisponibles[indice].Responsable = ResponsableTarea.Model;
        // para el nuevo evento OnTareaModificadaEvent
    }
}
