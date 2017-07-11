using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Regions;
using Gama.Common;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ListarActividadesDataGridViewModel : ViewModelBase
    {
        private bool _EnCursoSeleccionado;
        private bool _PorComenzarSeleccionado;
        private bool _ProximasFechasSeleccionado;
        private bool _FueraPlazoSeleccionado;
        private bool _FinalizadasSeleccionadas;

        private IRegionManager _regionManager;
        private IActividadRepository _actividadRepository;
        private IEventAggregator _eventAggregator;
        private ICooperanteRepository _cooperanteRepository;
        private Preferencias _settings;
        private CooperanteWrapper _NuevoCooperanteWrapper;
        private ActividadWrapper _NuevaActividadWrapper;
        private Actividad _Actividad;
        public ListarActividadesDataGridViewModel(
            IRegionManager regionManager,
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator,
            Preferencias settings,
            ISession session)
        {
            Gama.Common.Debug.Debug.StartWatch();
            _regionManager = regionManager;
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _actividadRepository.Session = session;
            _cooperanteRepository.Session = session;
            _eventAggregator = eventAggregator;
            _settings = settings;

            NombreCoordinadorActividades = new ObservableCollection<string>();
            ListaParcialActividades = new ObservableCollection<Actividad>(_actividadRepository.GetAll());
            ListaCompletaActividades = new ObservableCollection<Actividad>(_actividadRepository.GetAll());
            foreach (var ActividadSeleccionada in ListaCompletaActividades)
            {
                var Coord = _cooperanteRepository.GetById(ActividadSeleccionada.Coordinador.Id);
                string NombreCompleto = Coord.Nombre + Coord.Apellido;
                NombreCoordinadorActividades.Add(NombreCompleto);
            }

            BotonListarTodoCommand = new DelegateCommand<string>(OnBotonListarTodoCommandExecute);
            ListarActividadesCommand = new DelegateCommand(OnListarActividadesCommandExecute);
            Gama.Common.Debug.Debug.StopWatch("ListarActividadDataGridViewModel");
        }
        public ObservableCollection<Actividad> ListaParcialActividades { get; private set; }
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
        public ObservableCollection<string> NombreCoordinadorActividades { get; private set; }
        public ICommand BotonListarTodoCommand { get; set; }
        public ICommand ListarActividadesCommand { get; set; }

        private void OnBotonListarTodoCommandExecute(string AListar)    // Pulsado el boton de listar todo
        {
            //ListaCompletaActividades.Clear();
            //ListaCompletaActividades.Add( _actividadRepository.GetAll());
            ActividadesOpciones.CheckBoxSeleccionados.Clear();
            ListaParcialActividades.Clear();
            NombreCoordinadorActividades.Clear();
            int NumFalse = 0;
            if (EnCursoSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else {
                ActividadesOpciones.CheckBoxSeleccionados.Add(false);
                NumFalse++;
            }
            if (PorComenzarSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else {
                ActividadesOpciones.CheckBoxSeleccionados.Add(false);
                NumFalse++;
            }
            if (ProximasFechasSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else {
                ActividadesOpciones.CheckBoxSeleccionados.Add(false);
                NumFalse++;
            }
            if (FueraPlazoSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else {
                ActividadesOpciones.CheckBoxSeleccionados.Add(false);
                NumFalse++;
            }
            if (FinalizadasSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else {
                ActividadesOpciones.CheckBoxSeleccionados.Add(false);
                NumFalse++;
            }
            if ((NumFalse == 0) || (NumFalse == 5)) // Imprimir Todo
                ActividadesOpciones.SeleccionParaListar = "TODO";
            else ActividadesOpciones.SeleccionParaListar = "CASITODO";
            string EstadosNoSeleccionados = "";
            if (ActividadesOpciones.SeleccionParaListar == "CASITODO")
            {
                for (int i = 0; i < 5; i++)
                {
                    if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 0) EstadosNoSeleccionados = "Comenzado";
                    if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 1) EstadosNoSeleccionados = "NoComenzado";
                    if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 2) EstadosNoSeleccionados = "ProximasFinalizaciones";
                    if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 3) EstadosNoSeleccionados = "FueraPlazo";
                    if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 4) EstadosNoSeleccionados = "Finalizado";
                    if (EstadosNoSeleccionados != "")
                    {
                        foreach (var ActividadSeleccionada in ListaCompletaActividades)
                        {
                            if (ActividadSeleccionada.Estado.ToString() == EstadosNoSeleccionados)  // Si  esta seleccionadose anade de ListaParcialActividades
                            {
                                ListaParcialActividades.Add(ActividadSeleccionada);
                                var Coord = _cooperanteRepository.GetById(ActividadSeleccionada.Coordinador.Id);
                                string NombreCompleto = Coord.Nombre + Coord.Apellido;
                                NombreCoordinadorActividades.Add(NombreCompleto);
                            }
                        }
                        EstadosNoSeleccionados = "";
                    }
                }
            }
            else if (ActividadesOpciones.SeleccionParaListar == "TODO")
            {
                foreach (var ActividadSeleccionada in ListaCompletaActividades)
                {
                    ListaParcialActividades.Add(ActividadSeleccionada);
                    var Coord = _cooperanteRepository.GetById(ActividadSeleccionada.Coordinador.Id);
                    string NombreCompleto = Coord.Nombre + Coord.Apellido;
                    NombreCoordinadorActividades.Add(NombreCompleto);
                }
            }
            //if(ActividadesOpciones.SeleccionParaListar == "CASITODO")
            //{
            //    ListaCompletaActividades.Clear();
            //    foreach (var Act in ListaParcialActividades)
            //    {
            //        ListaCompletaActividades.Add(Act);
            //    }
            //}
        }
        private void OnListarActividadesCommandExecute() // Mostrar el GroupBox de Opciones
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");
        }
        public bool EnCursoSeleccionado
        {
            get { return _EnCursoSeleccionado; }
            set { SetProperty(ref _EnCursoSeleccionado, value); }
        }
        public bool PorComenzarSeleccionado
        {
            get { return _PorComenzarSeleccionado; }
            set { SetProperty(ref _PorComenzarSeleccionado, value); }
        }
        public bool ProximasFechasSeleccionado
        {
            get { return _ProximasFechasSeleccionado; }
            set { SetProperty(ref _ProximasFechasSeleccionado, value); }
        }
        public bool FueraPlazoSeleccionado
        {
            get { return _FueraPlazoSeleccionado; }
            set { SetProperty(ref _FueraPlazoSeleccionado, value); }
        }
        public bool FinalizadasSeleccionado
        {
            get { return _FinalizadasSeleccionadas; }
            set { SetProperty(ref _FinalizadasSeleccionadas, value); }
        }
    }

}
