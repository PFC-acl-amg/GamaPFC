using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class ActividadWrapper : ModelWrapper<Actividad>
    {
        public ActividadWrapper(Actividad model) : base(model)
        {
            InitializeCollectionProperties(model);
            InitializeComplexProperties(model);
        }

        private void InitializeCollectionProperties(Actividad model)
        {
            if (model.Cooperantes == null)
            {
                throw new ArgumentNullException("Cooperantes");
            }

            this.Cooperantes = new ChangeTrackingCollection<CooperanteWrapper>
                (model.Cooperantes.Select(c => new CooperanteWrapper(c)));
            this.RegisterCollection(this.Cooperantes, model.Cooperantes.ToList());

            if (model.Tareas == null)
            {
                throw new ArgumentNullException("Tareas");
            }

            this.Tareas = new ChangeTrackingCollection<TareaWrapper>
                (model.Tareas.Select(t => new TareaWrapper(t)));
            this.RegisterCollection(this.Tareas, model.Tareas.ToList());
        }

        private void InitializeComplexProperties(Actividad model)
        {
            if (model.Coordinador == null)
            {
                throw new ArgumentNullException("Coordinador");
            }

            this.Coordinador = new CooperanteWrapper(model.Coordinador);
            RegisterComplex(this.Coordinador);
        }

        public string Descripcion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public Estado Estado
        {
            get { return GetValue<Estado>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeInicio
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeFin
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Titulo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public CooperanteWrapper Coordinador { get; set; }

        public ChangeTrackingCollection<CooperanteWrapper> Cooperantes { get; private set; }
        public ChangeTrackingCollection<TareaWrapper> Tareas { get; private set; }

        public void AddCooperante(CooperanteWrapper cooperanteNuevo)
        {
            Cooperantes.Add(cooperanteNuevo);
        }
    }
}
 