using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class ActividadWrapper : TimestampedModelWrapper<Actividad>
    {
        public ActividadWrapper(Actividad model) : base(model)
        {
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
        }

        private void InitializeCollectionProperties(Actividad model)
        {
            if (model.Cooperantes == null)
            {
                throw new ArgumentNullException("Cooperantes");
            }
            else if (model.Tareas == null)
            {
                throw new ArgumentNullException("Tareas");
            }

            this.Cooperantes = new ChangeTrackingCollection<CooperanteWrapper>
                (model.Cooperantes.Select(c => new CooperanteWrapper(c)));
            this.RegisterCollection(this.Cooperantes, model.Cooperantes);

            this.Tareas = new ChangeTrackingCollection<TareaWrapper>
                (model.Tareas.Select(t => new TareaWrapper(t)));
            this.RegisterCollection(this.Tareas, model.Tareas);
        }

        private void InitializeComplexProperties(Actividad model)
        {
            if (model.Coordinador == null)
            {
                throw new ArgumentNullException("Coordinador");
            }

            _Coordinador = new CooperanteWrapper(model.Coordinador);
            _CoordinadorOriginalValue = new CooperanteWrapper(model.Coordinador);
            this.CoordinadorIsChanged = false;
        }

        public override bool IsChanged => base.IsChanged || CoordinadorIsChanged;

        public override void AcceptChanges()
        {
            CoordinadorOriginalValue = new CooperanteWrapper(Coordinador.Model);

            base.AcceptChanges();
        }

        public override void RejectChanges()
        {
            Coordinador = new CooperanteWrapper(CoordinadorOriginalValue.Model);

            base.RejectChanges();
        }

        public string Descripcion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DescripcionOriginalValue => GetOriginalValue<string>(nameof(Descripcion));

        public bool DescripcionIsChanged => GetIsChanged(nameof(Descripcion));

        public Estado Estado
        {
            get { return GetValue<Estado>(); }
            set { SetValue(value); }
        }

        public Estado EstadoOriginalValue => GetOriginalValue<Estado>(nameof(Estado));

        public bool EstadoIsChanged => GetIsChanged(nameof(Estado));

        public DateTime FechaDeInicio
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeInicioOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDeInicio));

        public bool FechaDeInicioIsChanged => GetIsChanged(nameof(FechaDeInicio));

        public DateTime FechaDeFin
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeFinOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDeFin));

        public bool FechaDeFinIsChanged => GetIsChanged(nameof(FechaDeFin));

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

        public string TituloOriginalValue => GetOriginalValue<string>(nameof(Titulo));

        public bool TituloIsChanged => GetIsChanged(nameof(Titulo));

        private CooperanteWrapper _Coordinador;
        public CooperanteWrapper Coordinador
        {
            get { return _Coordinador; } 
            set
            {
                _Coordinador = value;

                if (value != null)
                {
                    if (value.Id == CoordinadorOriginalValue.Id)
                    {
                        CoordinadorIsChanged = false;
                        OnPropertyChanged(nameof(IsChanged));
                    }
                    else
                    {
                        CoordinadorIsChanged = true;
                        OnPropertyChanged(nameof(IsChanged));
                    }

                    SetValue(value.Model);
                }
            }
        }

        private CooperanteWrapper _CoordinadorOriginalValue;
        public CooperanteWrapper CoordinadorOriginalValue
        {
            get { return _CoordinadorOriginalValue; }
            set
            {
                _CoordinadorOriginalValue = value;
                if (Coordinador.Id == CoordinadorOriginalValue.Id)
                {
                    CoordinadorIsChanged = false;
                }
                else
                {
                    CoordinadorIsChanged = true;
                }
            }
        }

        public bool CoordinadorIsChanged { get; set; }

        public ChangeTrackingCollection<CooperanteWrapper> Cooperantes { get; private set; }
        public ChangeTrackingCollection<TareaWrapper> Tareas { get; private set; }
    }
}
 