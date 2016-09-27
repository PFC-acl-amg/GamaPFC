using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using Prism.Commands;
using Remotion.Linq.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class TareasDeActividadViewModel : ViewModelBase
    {
        //private bool _isVisible=true;
        private bool _isVisibleForo = true;
        private bool _isVisibleMensaje = false;
        //private ObservableCollection<Tarea> _tarea = new ObservableCollection<Tarea>();
        //private Visibility _isVisible2 = Visibility.Hidden;
        public ObservableCollection<Tarea> TareasDisponible { get; private set; }
        public ObservableCollection<Mensaje> MensajesDisponible { get; private set; }
        public ObservableCollection<Evento> EventosDisponible { get; private set; }

        public ObservableCollection<ForoWrapper>ForosDisponibles { get; private set; }
        public TareasDeActividadViewModel()
        {
            CrearForoCommand = new DelegateCommand(OnCrearForoCommand, OnCrearForoCommand_CanExecute);
            TareasDisponible = new ObservableCollection<Tarea>();
            TareasDisponible.Add(new Tarea() { HaFinalizado = true, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15), Descripcion = "Tarea 1" });
            TareasDisponible.Add(new Tarea() { HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15), Descripcion = "Tarea 2" });
            TareasDisponible.Add(new Tarea() { HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15), Descripcion = "Tarea 3" });
            MensajesDisponible = new ObservableCollection<Mensaje>();
            MensajesDisponible.Add(new Mensaje() { FechaPublico = new DateTime(2008, 8, 29, 19, 27, 15), TituloMensaje = "Mensaje => Un poco de WPF" });
            MensajesDisponible.Add(new Mensaje() { FechaPublico = new DateTime(2008, 8, 29, 19, 27, 15), TituloMensaje = "Mensaje => Y Mvvm", });
            MensajesDisponible.Add(new Mensaje() { FechaPublico = new DateTime(2008, 8, 29, 19, 27, 15), TituloMensaje = "Mensaje => Aplicacionpara Gama" });
            EventosDisponible = new ObservableCollection<Evento>();
            EventosDisponible.Add(new Evento() { FechaPublicado = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Crear las vistas Tarea", EventoSucedido = Ocurrencia.Mensaje_Publicado });
            EventosDisponible.Add(new Evento() { FechaPublicado = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Hola a todos bienvenidos", EventoSucedido = Ocurrencia.Mensaje_Publicado });
            EventosDisponible.Add(new Evento() { FechaPublicado = new DateTime(2008, 8, 29, 19, 27, 15), Titulo = "Crear los ViewModels", EventoSucedido = Ocurrencia.Mensaje_Publicado });
            IsVisibleMensaje = false;
            ForosDisponibles = new ObservableCollection<ForoWrapper>();
            ForosDisponibles.Add(new ForoWrapper(new Foro() { TituloForo = "Hola caracola", FechaForo = new DateTime(2008, 8, 29, 19, 27, 15) }) { ForoVisible = false });
            ForosDisponibles.Add(new ForoWrapper(new Foro() { TituloForo = "Hola perito" , FechaForo = new DateTime(2008, 8, 29, 19, 27, 15) }) { ForoVisible = true });
            ForosDisponibles.Add(new ForoWrapper(new Foro() { TituloForo = "Hola cagao", FechaForo = new DateTime(2008, 8, 29, 19, 27, 15) }) { ForoVisible = false });
        }

        private bool OnCrearForoCommand_CanExecute()
        {
            return true;
        }

        public ICommand CrearForoCommand { get; private set; }
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
        // Necesario para trabajar con observableCollection.
        //public static Tarea GetTarea()
        //{
        //    var tar = new Tarea() { Id = 01, Descripcion = "Crear las vistas de la clase Tarea", HaFinalizado = false, FechaDeFinalizacion = new DateTime (2008, 8, 29, 19, 27, 15) };
        //    return tar;
        //}
        //public static ObservableCollection<Tarea> GetColeccionTareas()
        //{
        //    var ConjuntoTareas = new ObservableCollection<Tarea>();
        //    ConjuntoTareas.Add(new Tarea() { Id = 01, Descripcion = "Crear las vistas Tarea", HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15) });
        //    ConjuntoTareas.Add(new Tarea() { Id = 01, Descripcion = "Crear la vista modelo Tarea", HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15) });
        //    ConjuntoTareas.Add(new Tarea() { Id = 01, Descripcion = "Crear ObsevableCollection", HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15) });
        //    ConjuntoTareas.Add(new Tarea() { Id = 01, Descripcion = "Crear conjunto de tareas", HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15) });
        //    ConjuntoTareas.Add(new Tarea() { Id = 01, Descripcion = "Comprobar que todo va bien", HaFinalizado = false, FechaDeFinalizacion = new DateTime(2008, 8, 29, 19, 27, 15) });
        //    return ConjuntoTareas;
        //}
        //public Visibility IsVisible
        //{
        //    get
        //    {
        //        return _isVisible;
        //    }

        //    set
        //    {
        //        _isVisible = value;

        //        OnPropertyChanged("IsVisible");
        //    }
        //}
        //public Visibility IsVisible2
        //{
        //    get
        //    {
        //        return _isVisible2;
        //    }

        //    set
        //    {
        //        _isVisible2 = value;

        //        OnPropertyChanged("IsVisible2");
        //    }
        //}
        private void OnCrearForoCommand()
        {
            //if (IsVisibleForo == true)
            //{
            //    IsVisibleForo = false;
            //    IsVisibleMensaje = true;
            //}
            //else
            //{
            //    IsVisibleMensaje = false;
            //    IsVisibleForo = true;
            //}
            //-----Funciona
            //if (IsVisibleMensaje == false)
            //{
            //    IsVisibleMensaje = true;
            //}
            //else
            //{
            //    IsVisibleMensaje = false;
            //}
            //---funciona
            

        }

        // probando otra cosa para la visibilidad.
        //public class RelayCommand : ICommand
        //{
        //    public event EventHandler CanExecuteChanged
        //    {
        //        add { CommandManager.RequerySuggested += value; }
        //        remove { CommandManager.RequerySuggested -= value; }
        //    }
        //    private Action methodToExecute;
        //    private Func<bool> canExecuteEvaluator;
        //    public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        //    {
        //        this.methodToExecute = methodToExecute;
        //        this.canExecuteEvaluator = canExecuteEvaluator;
        //    }
        //    public RelayCommand(Action methodToExecute)
        //        : this(methodToExecute, null)
        //    {
        //    }
        //    public bool CanExecute(object parameter)
        //    {
        //        if (this.canExecuteEvaluator == null)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            bool result = this.canExecuteEvaluator.Invoke();
        //            return result;
        //        }
        //    }
        //    public void Execute(object parameter)
        //    {
        //        this.methodToExecute.Invoke();
        //    }
        //}
        //public RelayCommand MakeVisibleCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() => IsVisible = Visibility.Collapsed);
        //    }
        //}
    }
}
