﻿using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using Gama.Cooperacion.Wpf.Wrappers;
using LiveCharts;
using LiveCharts.Wpf;
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

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;
        private IEventAggregator _eventAggregator;
        private ICooperanteRepository _cooperanteRepository;
        private ICooperacionSettings _settings;
        private CooperanteWrapper _NuevoCooperante;
        private Cooperante _CooperanteSeleccionado;
        private int _mesInicialActividades;
        private string[] _Labels;
        private int _mesInicialCooperantes;
        private int _CooperantesMostrados;
        private bool _VisibleListaCooperantes;
        private bool _VisibleListaActividades;
        private bool _VisibleListaActividadesCooperante;
        private bool _VisibleDatosDNI;
        private bool _VisibleContacto;
        private bool _VisibleDireccion;
        private bool _VisibleCooperanteSeleccionado;
        private bool _VisibleListaTodosCooperantes;
        private bool _VisibleImagenSeleccionCooperante;
        private bool _VisibleDatosCooperanteSeleccionado;

        private readonly int itemCount;

        public DashboardViewModel(
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventAggregator eventAggregator, 
            ICooperacionSettings settings,
            ISession session)
        {
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _actividadRepository.Session = session;
            _cooperanteRepository.Session = session;
            _eventAggregator = eventAggregator;
            _settings = settings;
            _CooperantesMostrados = 0;

            _VisibleListaActividades = true;
            _VisibleListaCooperantes = false;
            _VisibleDatosDNI = false;
            _VisibleListaActividadesCooperante = false;
            _VisibleContacto = false;
            _VisibleDireccion = false;
            _VisibleCooperanteSeleccionado = false;
            _VisibleListaTodosCooperantes = true;
            _VisibleImagenSeleccionCooperante = true;
            _VisibleDatosCooperanteSeleccionado = false;
            this.itemCount = 10;
            this.Items = new ObservableCollection<Item>();

            for (var i = 0; i < this.itemCount; i++)
            {
                this.Items.Add(new Item("Thi is item number " + i));
            }
            ListaCompletaActividades = new ObservableCollection<Actividad>(_actividadRepository.GetAll());
            ListaDeActividades = new ObservableCollection<LookupItem>(
                _actividadRepository.GetAll()
                    .OrderBy(a => a.FechaDeFin)
                    .Take(_settings.DashboardActividadesAMostrar)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    //DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
                    //    _settings.DashboardActividadesLongitudDeTitulos),
                    DisplayMember1 = a.Titulo,
                    Id_Coordinador = a.Coordinador.Id,
                }));
            ListaDeActividadesCooperante = new ObservableCollection<LookupItem>();
            ListaDeActividadesCoordina = new ObservableCollection<LookupItem>();
            //ListaDeActividadesCooperante = new ObservableCollection<LookupItem>(
            //    ListaCompletaActividades
            //        .OrderBy(a => a.FechaDeFin)
            //        .Take(_settings.DashboardActividadesAMostrar)
            //    .Select(a => new LookupItem
            //    {
            //        Id = a.Id,
            //        //DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
            //        //    _settings.DashboardActividadesLongitudDeTitulos),
            //        DisplayMember1 = a.Titulo,
            //        Id_Coordinador = a.Coordinador.Id,
            //    }));
            ListaCooperantes = new ObservableCollection<Cooperante>(
                _cooperanteRepository.GetAll()
                .OrderBy(c => c.Id)
                .ToArray());
            ListaParcialCooperantes = new ObservableCollection<Cooperante>(
                _cooperanteRepository.GetAll()
                //.GetRange(_CooperantesMostrados, _CooperantesMostrados + 4)
                //.OrderBy(c => c.Id)
                .Take(4)
                .ToArray());
            _CooperantesMostrados = _CooperantesMostrados + 4;
            InicializarGraficos();

            _eventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnNuevaActividadEvent);
            _eventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);
            _eventAggregator.GetEvent<CooperanteCreadoEvent>().Subscribe(PublicarCooperante);

            SelectActividadCommand = new DelegateCommand<object>(OnSelectActividadCommand);
            SelectCooperanteCommand = new DelegateCommand<Cooperante>(OnSelectCooperanteCommand);
            PruebaTemplateCommand = new DelegateCommand(OnPruebaTemplateCommandExecute);
            NuevaActividadCommand = new DelegateCommand(OnNuevaActividadCommandExecute);
            ListaDeActividadesCommand = new DelegateCommand(OnListaDeActividadesCommandExecute);
            ListaCooperantesCommand = new DelegateCommand(OnListaCooperantesCommandExecute);
            PaginaSiguienteCommand = new DelegateCommand(OnPaginaSiguienteCommandExecute);
            PaginaAnteriorCommand = new DelegateCommand(OnPaginaAnteriorCommandExecute);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperanteCommandExecute);
        }
        private void PublicarCooperante(CooperanteWrapper CooperanteInsertado)
        {
            ListaParcialCooperantes.Clear();
            ListaParcialCooperantes.Add(CooperanteInsertado.Model);
            ListaCooperantes.Add(CooperanteInsertado.Model);
            // En la lista de Socios se muestra solo a este socio
            // En la zona de datos de socio selecionado se muestran los datos del nuevo cooperante creada
            // Se muestar zona de botones para poder editar datos del socio creado
        }
        private void OnListaCooperantesCommandExecute()
        {
            VisibleListaActividades = false;
            VisibleListaCooperantes = true;
            TituloPrincipal = "Listado de Cooperantes";
        }
        private void OnListaDeActividadesCommandExecute()
        {
            TituloPrincipal = "Listado de Actividades Disponibles";
            VisibleListaActividades = true;
            VisibleListaCooperantes = false;
        }
        private void OnNuevaActividadCommandExecute()
        {
            var o = new NuevaActividadView();
            o.ShowDialog();
            
        }
        private void OnPruebaTemplateCommandExecute()
        {
            PruebaTemplate = !PruebaTemplate;
        }

        private bool _PruebaTemplate;
        public bool PruebaTemplate
        {
            get { return _PruebaTemplate; }
            set { _PruebaTemplate = value;  OnPropertyChanged(); }
        }
        private string _TituloPrincipal="Listado de Actividades";
        public string TituloPrincipal
        {
            get { return _TituloPrincipal; }
            set { SetProperty(ref _TituloPrincipal, value); }
        }
        public ObservableCollection<Item> Items { get; private set; }
        public class Item
        {
            public string Text { get; private set; }

            public Item(string text)
            {
                this.Text = text;
            }
        }
        public CooperanteWrapper NuevoCooperante
        {
            get { return _NuevoCooperante; }
            set { SetProperty(ref _NuevoCooperante, value); }
        }
        public Cooperante CooperanteSeleccionado
        {
            get { return _CooperanteSeleccionado; }
            set
            {
                SetProperty(ref _CooperanteSeleccionado, value);
            }
        }
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
        public ObservableCollection<LookupItem> ListaDeActividades { get; private set; }
        public ObservableCollection<LookupItem> ListaDeActividadesCooperante { get; private set; }
        public ObservableCollection<LookupItem> ListaDeActividadesCoordina { get; private set; }
        public ObservableCollection<Cooperante> ListaCooperantes { get; private set; }
        public ObservableCollection<Cooperante> ListaParcialCooperantes { get; private set; }

        public ChartValues<int> ActividadesNuevasPorMes { get; private set; }
        public ChartValues<int> CooperantesNuevosPorMes { get; private set; }
        public ChartValues<int> IncidenciasNuevasPorMes { get; private set; }

        public string[] ActividadesLabels =>
            _Labels.Skip(_mesInicialActividades)
                .Take(_settings.DashboardMesesAMostrarDeActividadesNuevas).ToArray();

        public string[] CooperantesLabels =>
            _Labels.Skip(_mesInicialCooperantes)
                .Take(_settings.DashboardMesesAMostrarDeCooperantesNuevos).ToArray();

        public ICommand SelectActividadCommand { get; set; }
        public ICommand SelectCooperanteCommand { get; set; }
        public ICommand PruebaTemplateCommand { get; set; }
        public ICommand NuevaActividadCommand { get; set; }
        public ICommand PaginaSiguienteCommand { get; set; }
        public ICommand PaginaAnteriorCommand { get; set; }
        public ICommand NuevoCooperanteCommand { get; set; }
        public ICommand ListaDeActividadesCommand { get; set; }
        public ICommand ListaCooperantesCommand { get; set; }
        private void OnNuevoCooperanteCommandExecute()
        {
            var o = new AgregarCooperanteView();    // Esta es la vista

            o.ShowDialog();
        }

        private void OnPaginaSiguienteCommandExecute()
        {
            int indiceSuperior;
            int indiceInferior = _CooperantesMostrados;
            if (indiceInferior <= ListaCooperantes.Count)
            {
                if (_CooperantesMostrados + 4 > ListaCooperantes.Count) indiceSuperior = ListaCooperantes.Count;
                else indiceSuperior = _CooperantesMostrados + 4;
                _CooperantesMostrados = _CooperantesMostrados + 4;
                ListaParcialCooperantes.Clear();
                for (int i = indiceInferior; i < indiceSuperior; i++)
                {
                    ListaParcialCooperantes.Add(ListaCooperantes[i]);
                }
            }
        }
        private void OnPaginaAnteriorCommandExecute()
        {
            int indiceSuperior;
            int indiceInferior = _CooperantesMostrados-8;
            if (indiceInferior >= 0)
            {
                if (_CooperantesMostrados - 4 > 0) indiceSuperior = _CooperantesMostrados-4;
                else indiceSuperior = _CooperantesMostrados - 4;
                _CooperantesMostrados = _CooperantesMostrados - 4;
                ListaParcialCooperantes.Clear();
                for (int i = indiceInferior; i < indiceSuperior; i++)
                {
                    ListaParcialCooperantes.Add(ListaCooperantes[i]);
                }
            }
        }
        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _mesInicialActividades = 12 + (DateTime.Now.Month - 1) - _settings.DashboardMesesAMostrarDeActividadesNuevas + 1;
            _mesInicialCooperantes = 12 + (DateTime.Now.Month - 1) - _settings.DashboardMesesAMostrarDeActividadesNuevas + 1;

            ActividadesNuevasPorMes = new ChartValues<int>(_actividadRepository.GetActividadesNuevasPorMes(
                       _settings.DashboardMesesAMostrarDeActividadesNuevas));

            CooperantesNuevosPorMes = new ChartValues<int>(_cooperanteRepository.GetCooperantesNuevosPorMes(
                       _settings.DashboardMesesAMostrarDeActividadesNuevas));

            // TODO
            IncidenciasNuevasPorMes = new ChartValues<int> { 1, 3, 5, 2, 3 };
        }

        private void OnSelectCooperanteCommand(Cooperante obj)
        {
            //ListaCooperantes.Clear();
            //ListaCooperantes.Add(obj);
            //ListaCooperantes = ListaParcialCooperantes;
            VisibleListaActividadesCooperante = true;
            VisibleDatosCooperanteSeleccionado = true;
            VisibleImagenSeleccionCooperante = false;
            VisibleListaTodosCooperantes = false;
            VisibleCooperanteSeleccionado = true;
            CooperanteSeleccionado = ListaCooperantes.Where(x => x.Id == obj.Id).FirstOrDefault();
            ListaParcialCooperantes.Clear();
            ListaParcialCooperantes.Add(CooperanteSeleccionado);
            ListaDeActividadesCoordina.Clear();
            ListaDeActividadesCooperante.Clear();
            foreach (var actividadCoordina in ListaDeActividades)
            {
                if (actividadCoordina.Id_Coordinador == obj.Id)
                {
                    ListaDeActividadesCoordina.Add(actividadCoordina);
                }
            }
           foreach (var actividadCoopera in ListaCompletaActividades)
            {
                foreach (var CooperanteActividadCoopera in actividadCoopera.Cooperantes)
                {
                    if (CooperanteActividadCoopera.Id == obj.Id)
                    {
                        var ItemCooperante = new LookupItem()
                        {
                            Id = actividadCoopera.Id,
                            DisplayMember1 = actividadCoopera.Titulo,
                            Id_Coordinador = actividadCoopera.Coordinador.Id,
                        };
                        ListaDeActividadesCooperante.Add(ItemCooperante);
                    }
                }
            }
        }

        private void OnSelectActividadCommand(object param)
        {
            var lookup = param as LookupItem;
            if (lookup != null)  _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(lookup.Id);

        }

        private void OnNuevaActividadEvent(int id)
        {
            var actividad = _actividadRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = actividad.Id,
                DisplayMember1 = actividad.Titulo,
            };
            ListaDeActividades.Insert(0, lookupItem);
        }

        private void OnActividadActualizadaEvent(int id)
        {
            var actividadActualizada = _actividadRepository.GetById(id);
            if (ListaDeActividades.Any(a => a.Id == id))
            {
                var indice = ListaDeActividades.IndexOf(ListaDeActividades.Single(a => a.Id == id));
                ListaDeActividades[indice] = new LookupItem
                {
                    Id = actividadActualizada.Id,
                    DisplayMember1=actividadActualizada.Titulo
                };
            }
        }
        public bool VisibleListaActividades
        {
            get { return _VisibleListaActividades; }
            set { SetProperty(ref _VisibleListaActividades, value); }
        }
        public bool VisibleListaActividadesCooperante
        {
            get { return _VisibleListaActividadesCooperante; }
            set { SetProperty(ref _VisibleListaActividadesCooperante, value); }
        }
        public bool VisibleListaCooperantes
        {
            get { return _VisibleListaCooperantes; }
            set { SetProperty(ref _VisibleListaCooperantes, value); }
        }
        public bool VisibleDatosDNI
        {
            get { return _VisibleDatosDNI; }
            set { SetProperty(ref _VisibleDatosDNI, value); }
        }
        public bool VisibleContacto
        {
            get { return _VisibleContacto; }
            set { SetProperty(ref _VisibleContacto, value); }
        }
        public bool VisibleDireccion
        {
            get { return _VisibleDireccion; }
            set { SetProperty(ref _VisibleDireccion, value); }
        }
        public bool VisibleCooperanteSeleccionado
        {
            get { return _VisibleCooperanteSeleccionado; }
            set { SetProperty(ref _VisibleCooperanteSeleccionado, value); }
        }
        public bool VisibleListaTodosCooperantes
        {
            get { return _VisibleListaTodosCooperantes; }
            set { SetProperty(ref _VisibleListaTodosCooperantes, value); }
        }
        public bool VisibleImagenSeleccionCooperante
        {
            get { return _VisibleImagenSeleccionCooperante; }
            set { SetProperty(ref _VisibleImagenSeleccionCooperante, value); }
        }
        public bool VisibleDatosCooperanteSeleccionado
        {
            get { return _VisibleDatosCooperanteSeleccionado; }
            set { SetProperty(ref _VisibleDatosCooperanteSeleccionado, value); }
        }
    }
}
