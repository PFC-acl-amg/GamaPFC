using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.DesignTimeData
{
    public class TareasDeActividadViewModelDTD : ViewModelBase
    {
        private bool _isVisibleForo = false;
        private bool _isVisibleMensaje = false;
        public TareasDeActividadViewModelDTD()
        {
            TareasDisponible = new ObservableCollection<Tarea>();

            TareasDisponible.Add(new Tarea() { HaFinalizado = true, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15), Descripcion = "Tarea 1" });
            TareasDisponible.Add(new Tarea() { HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15), Descripcion = "Tarea 2" });
            TareasDisponible.Add(new Tarea() { HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15), Descripcion = "Tarea 3" });
            MensajesDisponible = new ObservableCollection<Mensaje>();
            MensajesDisponible.Add(new Mensaje() { FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Mensaje => Un poco de WPF" });
            MensajesDisponible.Add(new Mensaje() { FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Mensaje => Y Mvvm", });
            MensajesDisponible.Add(new Mensaje() { FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Mensaje => Aplicacionpara Gama" });
            var eventosDisponibles = new List<Evento>();
            eventosDisponibles.Add(new Evento() { FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Crear las vistas Tarea", Ocurrencia = Ocurrencia.Mensaje_Publicado });
            eventosDisponibles.Add(new Evento() { FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Hola a todos bienvenidos", Ocurrencia = Ocurrencia.Mensaje_Publicado });
            eventosDisponibles.Add(new Evento() { FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Crear los ViewModels", Ocurrencia = Ocurrencia.Mensaje_Publicado });
            IsVisibleMensaje = false;

            EventosDisponibles = new List<EventoWrapper>(eventosDisponibles.Select(e => new EventoWrapper(e)));

            ForosDisponibles = new ObservableCollection<ForoWrapper>();
            ForosDisponibles.Add(new ForoWrapper(new Foro() { Titulo = "Hola caracola", FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15) }) { ForoVisible = false });
            ForosDisponibles.Add(new ForoWrapper(new Foro() { Titulo = "Hola perito" , FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15) }) { ForoVisible = true });
            ForosDisponibles.Add(
                new ForoWrapper(
                    new Foro()
                    {
                        Titulo = "Hola cagao",
                        FechaDePublicacion = new DateTime(2008, 8, 29, 19, 27, 15)
                    })
                {
                    ForoVisible = true,
                    Eventos = new ChangeTrackingCollection<EventoWrapper>(EventosDisponibles)
                });
        }
        public ObservableCollection<Tarea> TareasDisponible { get; private set; }
        public ObservableCollection<Mensaje> MensajesDisponible { get; private set; }
        public List<EventoWrapper> EventosDisponibles { get; private set; }
        public ObservableCollection<ForoWrapper> ForosDisponibles { get; private set; }
        public bool IsVisibleForo
        {
            get
            {
                return _isVisibleForo;
            }

            set
            {
                if (_isVisibleForo != value)
                {
                    _isVisibleForo = value;
                    OnPropertyChanged();
                }

            }
        }
        public bool IsVisibleMensaje
        {
            get
            {
                return _isVisibleMensaje;
            }

            set
            {
                if (_isVisibleMensaje != value)
                {
                    _isVisibleMensaje = value;
                    OnPropertyChanged();
                }

            }
        }
    }
}
