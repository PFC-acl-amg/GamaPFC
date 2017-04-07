using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using Microsoft.Win32;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class AgregarCooperanteViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;  // para cuando se agrege un nuevo cooperante actualizar las vistas necesarias
        private ICooperanteRepository _CooperanteRepository; // Para acceder a la BBDD, a la tabla "cooperantes"
        private CooperanteWrapper _NuevoCooperante; //Contendrá la informacióndel nuevo cooperante
        private double _TamW;
        private double _TamH;
        private BitmapImage _Fotito;
        private bool? _Cerrar;
        public AgregarCooperanteViewModel(ICooperanteRepository CooperanteRepository,
            IEventAggregator eventAggregator,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _CooperanteRepository = CooperanteRepository;
            _CooperanteRepository.Session = session;

            NuevoCooperante = new CooperanteWrapper(new Cooperante());
            ExaminarFotoCommand = new DelegateCommand(OnExaminarFotoCommandExecute);
            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute, OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute, OnCancelarCommand_CanExecute);

        }
        
        public ICommand AceptarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand ExaminarFotoCommand { get; private set; }
        private void OnExaminarFotoCommandExecute()
        {
            OpenFileDialog Abrir = new OpenFileDialog();
            BitmapImage auxImagen = new BitmapImage();
            Abrir.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (Abrir.ShowDialog() == true)
            {
                auxImagen.BeginInit();
                auxImagen.UriSource = new Uri(Abrir.FileName);
                auxImagen.EndInit();
                //-------
                string path = auxImagen.UriSource.OriginalString;
                FileStream sr = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[sr.Length];
                sr.Read(bytes, 0, bytes.Length);
                NuevoCooperante.Foto = bytes;
                TamH = 105;
                TamW = 200;
            }
        }
        private void OnAceptarCommand_Execute()
        {
            NuevoCooperante.CreatedAt = DateTime.Now;
            _CooperanteRepository.Create(NuevoCooperante.Model);
           _EventAggregator.GetEvent<CooperanteCreadoEvent>().Publish(NuevoCooperante);
            Cerrar = true;
        }
        private bool OnAceptarCommand_CanExecute()
        {
            //var resultado = Actividad.Titulo != null && Actividad.Coordinador.Nombre != null;
            //return resultado;
            var Activar = NuevoCooperante.Nombre != null && NuevoCooperante.Apellido != null &&
                          NuevoCooperante.Dni != null && NuevoCooperante.telefono != null;
            return Activar;

        }
        private void OnCancelarCommand_Execute()
        {
            Cerrar = true;
        }
        private bool OnCancelarCommand_CanExecute()
        {
            return true;
        }
        public BitmapImage Fotito
        {
            get { return _Fotito; }
            set { SetProperty(ref _Fotito, value); }
        }
        public CooperanteWrapper NuevoCooperante
        {
            get { return _NuevoCooperante; }
            set { SetProperty(ref _NuevoCooperante, value); }
        }
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }
        public double TamW
        {
            get { return _TamW; }
            set { SetProperty(ref _TamW, value); }
        }
        public double TamH
        {
            get { return _TamH; }
            set { SetProperty(ref _TamH, value); }
        }
    }
}
