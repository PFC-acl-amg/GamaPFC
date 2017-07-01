using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using Microsoft.Win32;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class CooperanteViewModel : ViewModelBase
    {
        private CooperanteWrapper _Cooperante;
        public CooperanteViewModel()
        {
            Cooperante = new CooperanteWrapper(new Cooperante());

            // Por defecto lo ponemos a true, para cuando se use al añadir
            // uno nuevo. En ventanas de editar se pondrá a False para obligar
            // a pulsar el botón de 'Habilitar Edición'
            Cooperante.IsInEditionMode = true;

            ExaminarAvatarCommand = new DelegateCommand(OnExaminarAvatarCommandExecute);
        }
        public ICommand ExaminarAvatarCommand { get; private set; }

        public CooperanteWrapper Cooperante
        {
            get { return _Cooperante; }
            set { SetProperty(ref _Cooperante, value); }
        }
        public void Load(CooperanteWrapper wrapper)
        {
            Cooperante = wrapper;
            Cooperante.IsInEditionMode = false;
        }

        private void OnExaminarAvatarCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            BitmapImage imagenAuxiliar = new BitmapImage();
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                imagenAuxiliar.BeginInit();
                imagenAuxiliar.UriSource = new Uri(openFileDialog.FileName);
                imagenAuxiliar.EndInit();

                string imagenPath = imagenAuxiliar.UriSource.OriginalString;
                FileStream imagenFileStream = new FileStream(imagenPath, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[imagenFileStream.Length];
                imagenFileStream.Read(bytes, 0, bytes.Length);

                Cooperante.Foto = bytes;
            }
        }

    }




    //public class CooperanteViewModel : ViewModelBase
    //{
    //    // Atributos privados de la clase
    //    private IActividadRepository _actividadRepository;
    //    private IEventAggregator _eventAggregator;
    //    private ICooperanteRepository _cooperanteRepository;
    //    private ICooperacionSettings _settings;
    //    private CooperanteWrapper _NuevoCooperante;
    //    private Cooperante _CooperanteSeleccionado;

    //    private bool _VisibleListaActividadesCooperante;
    //    private bool _VisibleDatosCooperanteSeleccionado;
    //    private bool _VisibleImagenSeleccionCooperante;
    //    private bool _VisibleListaTodosCooperantes;
    //    private bool _VisibleCooperanteSeleccionado;
    //    // Constructor de la clase.
    //    public CooperanteViewModel(IActividadRepository actividadRepository, 
    //        ICooperanteRepository cooperanteRepository,
    //        IEventAggregator eventAggregator,
    //        ICooperacionSettings settings,
    //        ISession session)
    //    {
    //        _actividadRepository = actividadRepository;
    //        _actividadRepository.Session = session;
    //        _cooperanteRepository = cooperanteRepository;
    //        _cooperanteRepository.Session = session;
    //        _eventAggregator = eventAggregator;
    //        _settings = settings;

    //        VisibleListaTodosCooperantes = true;

    //        ListaCooperantes = new ObservableCollection<Cooperante>(
    //            _cooperanteRepository.GetAll()
    //            .OrderBy(c => c.Id)
    //            .ToArray());
    //        ListaParcialCooperantes = new ObservableCollection<Cooperante>(
    //            _cooperanteRepository.GetAll()
    //            //.GetRange(_CooperantesMostrados, _CooperantesMostrados + 4)
    //            //.OrderBy(c => c.Id)
    //            .Take(4)
    //            .ToArray());
    //        ListaDeActividades = new ObservableCollection<LookupItem>(
    //            _actividadRepository.GetAll()
    //                .Where(a => a.Estado == Estado.Comenzado)
    //                .OrderBy(a => a.FechaDeFin)
    //                .Take(_settings.DashboardActividadesAMostrar)
    //            .Select(a => new LookupItem
    //            {
    //                Id = a.Id,
    //                //DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
    //                //    _settings.DashboardActividadesLongitudDeTitulos),
    //                DisplayMember1 = a.Titulo,
    //                Id_Coordinador = a.Coordinador.Id,
    //                FechaDeInicioActividad = a.FechaDeInicio,
    //            }));
    //        ListaDeActividadesCooperante = new ObservableCollection<LookupItem>();
    //        ListaCompletaActividades = new ObservableCollection<Actividad>(_actividadRepository.GetAll());
    //        ListaDeActividadesCoordina = new ObservableCollection<LookupItem>();

    //        SelectCooperanteCommand = new DelegateCommand<Cooperante>(OnSelectCooperanteCommand);
    //    }
    //    public ObservableCollection<Cooperante> ListaCooperantes { get; private set; }
    //    public ObservableCollection<Cooperante> ListaParcialCooperantes { get; private set; }
    //    public ObservableCollection<LookupItem> ListaDeActividadesCoordina { get; private set; }
    //    public ObservableCollection<LookupItem> ListaDeActividadesCooperante { get; private set; }
    //    public ObservableCollection<LookupItem> ListaDeActividades { get; private set; }
    //    public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }


    //    public ICommand SelectCooperanteCommand { get; set; }

    //    private void OnSelectCooperanteCommand(Cooperante obj)
    //    {
    //        //ListaCooperantes.Clear();
    //        //ListaCooperantes.Add(obj);
    //        //ListaCooperantes = ListaParcialCooperantes;
    //        VisibleListaActividadesCooperante = true;
    //        VisibleDatosCooperanteSeleccionado = true;
    //        VisibleImagenSeleccionCooperante = false;
    //        VisibleListaTodosCooperantes = false;
    //        VisibleCooperanteSeleccionado = true;
    //        CooperanteSeleccionado = ListaCooperantes.Where(x => x.Id == obj.Id).FirstOrDefault();
    //        ListaParcialCooperantes.Clear();
    //        ListaParcialCooperantes.Add(CooperanteSeleccionado);
    //        ListaDeActividadesCoordina.Clear();
    //        ListaDeActividadesCooperante.Clear();
    //        foreach (var actividadCoordina in ListaDeActividades)
    //        {
    //            if (actividadCoordina.Id_Coordinador == obj.Id)
    //            {
    //                ListaDeActividadesCoordina.Add(actividadCoordina);
    //            }
    //        }
    //        foreach (var actividadCoopera in ListaCompletaActividades)
    //        {
    //            foreach (var CooperanteActividadCoopera in actividadCoopera.Cooperantes)
    //            {
    //                if (CooperanteActividadCoopera.Id == obj.Id)
    //                {
    //                    var ItemCooperante = new LookupItem()
    //                    {
    //                        Id = actividadCoopera.Id,
    //                        DisplayMember1 = actividadCoopera.Titulo,
    //                        Id_Coordinador = actividadCoopera.Coordinador.Id,
    //                    };
    //                    ListaDeActividadesCooperante.Add(ItemCooperante);
    //                }
    //            }
    //        }
    //    }


    //    public Cooperante CooperanteSeleccionado
    //    {
    //        get { return _CooperanteSeleccionado; }
    //        set
    //        {
    //            SetProperty(ref _CooperanteSeleccionado, value);
    //        }
    //    }



    //    public bool VisibleListaActividadesCooperante
    //    {
    //        get { return _VisibleListaActividadesCooperante; }
    //        set { SetProperty(ref _VisibleListaActividadesCooperante, value); }
    //    }
    //    public bool VisibleDatosCooperanteSeleccionado
    //    {
    //        get { return _VisibleDatosCooperanteSeleccionado; }
    //        set { SetProperty(ref _VisibleDatosCooperanteSeleccionado, value); }
    //    }
    //    public bool VisibleImagenSeleccionCooperante
    //    {
    //        get { return _VisibleImagenSeleccionCooperante; }
    //        set { SetProperty(ref _VisibleImagenSeleccionCooperante, value); }
    //    }
    //    public bool VisibleListaTodosCooperantes
    //    {
    //        get { return _VisibleListaTodosCooperantes; }
    //        set { SetProperty(ref _VisibleListaTodosCooperantes, value); }
    //    }
    //    public bool VisibleCooperanteSeleccionado
    //    {
    //        get { return _VisibleCooperanteSeleccionado; }
    //        set { SetProperty(ref _VisibleCooperanteSeleccionado, value); }
    //    }
    //}
}
//public class ActividadesContentViewModel : ViewModelBase