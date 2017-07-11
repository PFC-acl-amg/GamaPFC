using Core;
using Core.DataAccess;
using Core.Util;
using Gama.Common.Eventos;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Views;
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
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private Socio Socio;
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private ExportService _ExportService;
        private string VistaCargada;

        public ToolbarViewModel(
            ISocioRepository socioRepository,
            ExportService exportService,
            IEventAggregator eventAggregator, 
            ISession session)
        {
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _ExportService = exportService;
            _EventAggregator = eventAggregator;

            NuevoSocioCommand = new DelegateCommand(OnNuevoSocioCommandExecute);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);
            HacerBackupCommand = new DelegateCommand(OnHacerBackupCommand);
            HacerRestoreCommand = new DelegateCommand(OnHacerRestoreCommand);

            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Subscribe(OnSocioSeleccionadoEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnListaSociosExportarEvent);
        }

        //----------------------------------------
        // ICommands - Declaraciones
        //----------------------------------------
        public ICommand NuevoSocioCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }
        public ICommand HacerBackupCommand { get; set; }
        public ICommand HacerRestoreCommand { get; set; }

        //----------------------------------------
        // ICommands - Implementaciones
        //----------------------------------------
        private void OnNuevoSocioCommandExecute()
        {
            var o = new NuevoSocioView();
            o.ShowDialog();
        }
        private void OnExportarCommandExecute()
        {
            if (VistaCargada != "SociosContentView")
            {
                var ListaSocios = _SocioRepository.GetAll();
                _ExportService.ExportarSocios(ListaSocios);
            }
            else _ExportService.ExportarSocio(Socio, Socio.Nombre);
        }
        private void OnHacerBackupCommand()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = DateTime.Now.ToShortDateString().Replace('/', '-') + " - socios backup.sql";
            saveFileDialog.Filter = "Sql file (*.sql)|*.sql";

            if (saveFileDialog.ShowDialog() == true)
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["GamaSociosMySql"].ConnectionString;
                DBHelper.Backup(connectionString, saveFileDialog.FileName);
                _EventAggregator.GetEvent<BackupFinalizadoEvent>().Publish();
            }
        }
        private void OnHacerRestoreCommand()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Sql file (*.sql)|*.sql";

            if (openFileDialog.ShowDialog() == true)
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["GamaSociosMySql"].ConnectionString;
                DBHelper.Restore(connectionString, openFileDialog.FileName);
                UIServices.RestartApplication();
            }
        }


        //----------------------------------------
        // Eventos - Implementaciones
        //----------------------------------------
        private void OnListaSociosExportarEvent(string obj)
        {
            VistaCargada = obj;
        }

        private void OnSocioSeleccionadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            Socio = socio;
        }
    }
}
