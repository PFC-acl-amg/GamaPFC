﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public enum Estado
    {
        NoComenzado,
        Comenzado,
        Finalizado,
        FueraPlazo,
        ProximasFinalizaciones,
        EstadoTest
    }

    public class Actividad : TimestampedModel
    {
        public virtual int CoordinadorId { get; set; }
        public virtual Cooperante Coordinador { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual DateTime FechaDeInicio { get; set; }
        public virtual DateTime FechaDeFin { get; set; }
        public virtual int Id { get; set; }
        public virtual string Titulo { get; set; }
        public virtual IList<Cooperante> Cooperantes { get; set; }
        public virtual IList<Tarea> Tareas { get; set; }
        public virtual IList<Foro> Foros { get; set; }
        public virtual IList<Evento> Eventos { get; set; }

        public Actividad()
        {
            Coordinador = new Cooperante();
            Cooperantes = new List<Cooperante>();
            Tareas = new List<Tarea>();
            Foros = new List<Foro>();
            Eventos = new List<Evento>();
            EncryptedFields = new List<string>();
            EncryptedFields.AddRange(new[] {
                nameof(Cooperantes),
            });
        }

        public virtual string TituloCorto => Titulo.Length > 10 ? Titulo.Substring(0, 10) : Titulo;

        public virtual void AddCooperante(Cooperante cooperante)
        {
            Cooperantes.Add(cooperante);
            cooperante.ActividadesEnQueParticipa.Add(this);
        }

        public virtual void AddCooperantes(IEnumerable<Cooperante> cooperantes)
        {
            foreach (var cooperante in cooperantes)
            {
                AddCooperante(cooperante);
            }
        }

        public virtual void AddTarea(Tarea tarea)
        {
            tarea.Actividad = this;
            Tareas.Add(tarea);
        }

        public virtual void SetCoordinador(Cooperante coordinador)
        {
            Coordinador = coordinador;
            coordinador.ActividadesDeQueEsCoordinador.Add(this);
            coordinador.ActividadesEnQueParticipa.Add(this);
        }

        public virtual void AddEvento(Evento evento)
        {
            evento.Actividad = this;
            Eventos.Add(evento);
        }
        public virtual void AddForo(Foro foro)
        {
            foro.Actividad = this;
            Foros.Add(foro);
        }
    }
}
