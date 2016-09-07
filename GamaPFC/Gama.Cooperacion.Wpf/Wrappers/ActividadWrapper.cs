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
                throw new ArgumentException("La lista de cooperantes no puede ser nula");
            }

            this.Cooperantes = new ChangeTrackingCollection<CooperanteWrapper>
                (model.Cooperantes.Select(c => new CooperanteWrapper(c)));
            this.RegisterCollection(this.Cooperantes, model.Cooperantes.ToList());
        }

        private void InitializeComplexProperties(Actividad model)
        {
            if (model.Coordinador == null)
            {
                throw new ArgumentException("'Coordinador' no puede ser nulo");
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


        /*
        public virtual IList<Cooperante> Cooperantes { get; protected set; }
        public virtual Cooperante Coordinador { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual DateTime FechaDeInicio { get; set; }
        public virtual DateTime FechaDeFin { get; set; }
        public virtual int Id { get; protected set; }
        public virtual string Titulo { get; set; }
        public virtual IList<Tarea> Tareas { get; protected set; }
        */

        public void SetCoordinador(CooperanteWrapper coordinador)
        {
            Model.SetCoordinador(coordinador.Model);
            Coordinador = coordinador;
            OnPropertyChanged("Coordinador");
        }

        public CooperanteWrapper Coordinador { get; private set; }

        public ChangeTrackingCollection<CooperanteWrapper> Cooperantes { get; private set; }

        internal void AddCooperante(CooperanteWrapper cooperanteNuevo)
        {
            Model.AddCooperante(cooperanteNuevo.Model);
        }
    }
}
 