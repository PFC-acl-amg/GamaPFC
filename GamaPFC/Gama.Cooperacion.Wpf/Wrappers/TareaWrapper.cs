using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class TareaWrapper : ModelWrapper<Tarea>
    {
        public TareaWrapper(Tarea model) : base(model)
        {
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
        }
        protected override void InitializeCollectionProperties(Tarea model)
        {
            if (model.Seguimiento == null)
            {
                throw new ArgumentNullException("Historial");
            }
            else if (model.Incidencias == null)
            {
                throw new ArgumentNullException("Mensajes");
            }


            this.Incidencia = new ChangeTrackingCollection<IncidenciaWrapper>
                (model.Incidencias.Select(t => new IncidenciaWrapper(t)));
            this.RegisterCollection(this.Incidencia, model.Incidencias);

            this.Seguimiento = new ChangeTrackingCollection<SeguimientoWrapper>
                (model.Seguimiento.Select(t => new SeguimientoWrapper(t)));
            this.RegisterCollection(this.Seguimiento, model.Seguimiento);
        }
        protected override void InitializeComplexProperties(Tarea model)
        {
            if (model.Responsable == null)
            {
                throw new ArgumentNullException("Resposable");
            }

            this.Responsable = new CooperanteWrapper(model.Responsable);
            //if (model.Actividad == null)
            //{
            //    throw new ArgumentNullException("Actividad");
            //}

            //this.Actividad = new ActividadWrapper(model.Actividad);
            // Hasta aqui lo comento yo.
            //RegisterComplex(this.Responsable);
        }
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

        public DateTime FechaDeFinalizacion
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeFinalizacionOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDeFinalizacion));

        public bool FechaDeFinalizacionIsChanged => GetIsChanged(nameof(FechaDeFinalizacion));

        public ChangeTrackingCollection<IncidenciaWrapper> Incidencia { get; set; }
        public ChangeTrackingCollection<SeguimientoWrapper> Seguimiento { get; set; }

        public ActividadWrapper Actividad { get; private set; }
        public CooperanteWrapper Responsable { get; private set; }
    }
}
