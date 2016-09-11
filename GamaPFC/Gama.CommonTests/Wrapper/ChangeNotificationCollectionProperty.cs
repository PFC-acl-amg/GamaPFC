using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.CommonTests
{
    public class ChangeNotificationCollectionProperty
    {
        private Actividad _Actividad;
        private Cooperante _Cooperante;
        private Tarea _Tarea;

        public ChangeNotificationCollectionProperty()
        {
            _Tarea = new Tarea { Descripcion = "Lorem ipsum endeleble intosqui causa ateanum maravillae" };

            _Cooperante = new Cooperante()
            {
                Nombre = "Agua Rocosa Artificial",
            };

            _Actividad = new Actividad()
            {
                Titulo = "Título del Primer Actividad",
                Tareas = new List<Tarea>()
                {
                    new Tarea { Descripcion = "Tarea número 1 para conseguir algo más o menos concreto" },
                    new Tarea { Descripcion = "A veces los Tareas se escriben de diferentes maneras" },
                    new Tarea { Descripcion = "Las piedras preciosas han llevado a más de un rey a la locura" },
                    _Tarea,
                },
                Cooperantes = new List<Cooperante>()
                {
                    new Cooperante() { Nombre = "Agua Mineral Natural" },
                    _Cooperante,
                },
            };
        }

        [Fact]
        public void ShouldInitializeTareasProperty()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.NotNull(wrapper.Tareas);
            CheckIfModelTareasIsInSync(wrapper);
        }

        [Fact]
        public void ShouldInitializeCooperantesProperty()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.NotNull(wrapper.Cooperantes);
            CheckIfModelCooperantesIsInSync(wrapper);
        }

        [Fact]
        public void ShouldBeInSyncAfterAddingTarea()
        {
            _Actividad.Tareas.Remove(_Tarea);
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Tareas.Add(new TareaWrapper(_Tarea));
            CheckIfModelTareasIsInSync(wrapper);
        }

        [Fact]
        public void ShouldBeInSyncAfterRemovingTarea()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            var tareaToRemove = wrapper.Tareas.Single(o => o.Model == _Tarea);
            wrapper.Tareas.Remove(tareaToRemove);
            CheckIfModelTareasIsInSync(wrapper);
        }

        [Fact]
        public void ShouldBeInSyncAfterClearingTareas()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Tareas.Clear();
            CheckIfModelTareasIsInSync(wrapper);
        }

        [Fact]
        public void ShouldBeInSyncAfterAddingCooperante()
        {
            _Actividad.Cooperantes.Remove(_Cooperante);
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Cooperantes.Add(new CooperanteWrapper(_Cooperante));
            CheckIfModelCooperantesIsInSync(wrapper);
        }

        [Fact]
        public void ShouldBeInSyncAfterRemovingCooperante()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            var cooperanteToRemove = wrapper.Cooperantes.Single(w => w.Model == _Cooperante);
            wrapper.Cooperantes.Remove(cooperanteToRemove);
            CheckIfModelCooperantesIsInSync(wrapper);
        }

        private void CheckIfModelTareasIsInSync(ActividadWrapper wrapper)
        {
            Assert.Equal(_Actividad.Tareas.Count, wrapper.Tareas.Count);
            Assert.True(_Actividad.Tareas.All(t => wrapper.Tareas.Any(wt => wt.Model == t)));
        }

        private void CheckIfModelCooperantesIsInSync(ActividadWrapper wrapper)
        {
            Assert.Equal(_Actividad.Cooperantes.Count, wrapper.Cooperantes.Count);
            Assert.True(_Actividad.Cooperantes.All(
                c => wrapper.Cooperantes.Any(wc => wc.Model == c)));
        }
    }
}
