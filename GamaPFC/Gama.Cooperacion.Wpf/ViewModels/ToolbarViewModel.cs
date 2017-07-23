using Core;
using Core.DataAccess;
using Core.Util;
using Gama.Common.Eventos;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using Microsoft.Win32;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private Cooperante Cooperante;
        private Actividad Actividad;
        private EventAggregator _EventAggregator;
        private ICooperanteRepository _CooperanteRepository;
        private IActividadRepository _ActividadRepository;
        private ExportService _ExportService;
        private string VistaCargada;

        public ToolbarViewModel(CooperanteRepository CooperanteRepository, ActividadRepository ActividadRepository,
            ExportService ExportService,
            EventAggregator EventAggregator,
            ISession Session)
        {
            _CooperanteRepository = CooperanteRepository;
            _CooperanteRepository.Session = Session;
            _ActividadRepository = ActividadRepository;
            _ActividadRepository.Session = Session;
            _ExportService = ExportService;
            _EventAggregator = EventAggregator;

            _EventAggregator.GetEvent<CooperanteSeleccionadoEvent>().Subscribe(OnCooperanteSeleccionadoEvent);
            _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnContenidoVistaExportarEvent);


            NuevoActividadCommand = new DelegateCommand(OnNuevoActividad);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperante);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);

            HacerBackupCommand = new DelegateCommand(OnMakeBackupCommandExecute);
            HacerRestoreCommand = new DelegateCommand(OnRestoreBackupCommandExecute);
        }

        public ICommand NuevoCooperanteCommand { get; private set; }
        public ICommand NuevoActividadCommand { get; private set; }
        public ICommand ExportarCommand { get; set; }
        public ICommand HacerBackupCommand { get; private set; }
        public ICommand HacerRestoreCommand { get; private set; }

        private void OnNuevoCooperante()
        {
            var o = new AgregarCooperanteView();
            o.ShowDialog();
        }
        private void OnNuevoActividad()
        {
            var o = new NuevaActividadView();
            o.ShowDialog();
        }
        private void OnExportarCommandExecute()
        {
            if (VistaCargada == "ActividadesContentView")
                _ExportService.ExportarActividad(Actividad, Actividad.Titulo);
            else
            {
                if (VistaCargada == "CooperantesContentView")
                    _ExportService.ExportarTodosCooperantes(_CooperanteRepository.GetAll());
                else _ExportService.ExportarTodasActividades(_ActividadRepository.GetAll());
            }
        }
        private void OnContenidoVistaExportarEvent(string obj)
        {
            VistaCargada = obj;
        }
        private void OnCooperanteSeleccionadoEvent(int id)
        {
            var _cooperante = _CooperanteRepository.GetById(id);
            Cooperante = _cooperante;
        }
        private void OnActividadSeleccionadaEvent(int id)
        {
            var _ActSel = _ActividadRepository.GetById(id);
            Actividad = _ActSel;
        }

        public void LoadCooperante(int id)
        {
            var _cooperante = _CooperanteRepository.GetById(id);
            Cooperante = _cooperante;
        }

        private void OnMakeBackupCommandExecute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = DateTime.Now.ToShortDateString().Replace('/', '-') + " - cooperacion backup.sql";
            saveFileDialog.Filter = "Sql file (*.sql)|*.sql";

            if (saveFileDialog.ShowDialog() == true)
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["GamaCooperacionMySql"].ConnectionString;
                DBHelper.Backup(connectionString, saveFileDialog.FileName);
                _EventAggregator.GetEvent<BackupFinalizadoEvent>().Publish();
            }
        }

        private void OnRestoreBackupCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Sql file (*.sql)|*.sql";

            if (openFileDialog.ShowDialog() == true)
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["GamaCooperacionMySql"].ConnectionString;
                DBHelper.Restore(connectionString, openFileDialog.FileName);
                UIServices.RestartApplication();
            }
        }
    }
}
