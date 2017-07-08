using Core;
using Core.DataAccess;
using Core.Util;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Views;
using Gama.Atenciones.Wpf.Wrappers;
using Gama.Common.Debug;
using Gama.Common.Eventos;
using Gama.Common.Views;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        private Persona _Persona;
        private AsistenteWrapper _Asistente;
        private IPersonaRepository _PersonaRepository;
        //private IAsistenteRepository _AsistenteRepository;
        private ICitaRepository _CitaRepository;
        private IEventAggregator _EventAggregator;
        private string VistaCargada;
        private ExportService _ExportService;

        public ToolbarViewModel(
            IPersonaRepository PersonaRepository,
            ICitaRepository CitaRepository,
            ExportService exportService,
            IEventAggregator eventAggregator,
            ISession session)
        {
            Debug.StartWatch();
            _PersonaRepository = PersonaRepository;
            _PersonaRepository.Session = session;
            _CitaRepository = CitaRepository;
            _CitaRepository.Session = session;
            _ExportService = exportService;
            _EventAggregator = eventAggregator;

            NuevaPersonaCommand = new DelegateCommand(OnNuevaPersonaCommandExecute);
            NuevoAsistenteCommand = new DelegateCommand(OnNuevoAsistenteCommandExecute);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);
            EliminarPersonaCommand = new DelegateCommand(OnEliminarPersonaCommandExecute, 
                () => _Persona != null);

            HacerBackupCommand = new DelegateCommand(OnMakeBackupCommandExecute);
            HacerRestoreCommand = new DelegateCommand(OnRestoreBackupCommandExecute);

            _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Subscribe(OnPersonaSeleccionadaChangedEvent);
            _EventAggregator.GetEvent<AsistenteSeleccionadoChangedEvent>().Subscribe(OnAsistenteSeleccionadaChangedEvent);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaSeleccionadaChangedEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnContenidoVistaExportarEvent);
            Debug.StopWatch("Toolbar");
        }


        public ICommand NuevaPersonaCommand { get; private set; }
        public ICommand NuevoAsistenteCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }
        public ICommand EliminarPersonaCommand { get; private set; }
        public ICommand HacerBackupCommand { get; private set; }
        public ICommand HacerRestoreCommand { get; private set; }

        public override void OnActualizarServidor()
        {
            if (_Persona != null)
                _Persona = _PersonaRepository.GetById(_Persona.Id);
            
            InvalidateCommands();
        }

        private void OnPersonaSeleccionadaChangedEvent(int id)
        {
            if (id != 0)
            {
                var persona = _PersonaRepository.GetById(id);
                _PersonaRepository.Session.Evict(persona);

                _Persona = persona;
            }
            else
            {
                _Persona = null;
            }

            InvalidateCommands();
        }
        private void OnAsistenteSeleccionadaChangedEvent(int id)
        {
            if (id != 0)
            {
                var persona = _PersonaRepository.GetById(id);
                _PersonaRepository.Session.Evict(persona);

                _Persona = persona;
            }
            else
            {
                _Persona = null;
            }

            InvalidateCommands();
        }
        private void OnAsistenteSeleccionadaChangedEvent(AsistenteWrapper ats)
        {
            _Asistente = ats;
        }


        private void OnContenidoVistaExportarEvent(string obj)
        {
            VistaCargada = obj;
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)EliminarPersonaCommand).RaiseCanExecuteChanged();
        }

        private void OnEliminarPersonaCommandExecute()
        {
            var o = new ConfirmarOperacionView();
            o.Mensaje = "¿Está seguro de que desea eliminar esta persona y todos sus registros?";
            o.ShowDialog();

            if (o.EstaConfirmado)
            {
                int id = _Persona.Id;
                // WARNING: Debe hacerse antes la publicación del evento porque se recoge
                // la persona para ver sus citas y atenciones desde otros viewmodels
                // Alternativamente, se podría hacer que el parámetro que enviará este
                // evento fuera el objeto de la persona entero, de forma que no afecte
                // si se hace antes o después siempre que se guarde una copia 'deep' del modelo
                // primero
                _EventAggregator.GetEvent<PersonaEliminadaEvent>().Publish(id);
                _PersonaRepository.Delete(_Persona);
            }
        }

        private void OnNuevaPersonaCommandExecute()
        {
            var o = new NuevaPersonaView();
            o.ShowDialog();
        }

        private void OnNuevoAsistenteCommandExecute()
        {
            var o = new NuevoAsistenteView();
            o.ShowDialog();
        }

        //private void OnExportarCommandExecute()
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    saveFileDialog.FileName =
        //        $"{DateTime.Now.ToShortDateString().Replace('/', '-')} - {_Persona.Nombre}.docx";

        //    saveFileDialog.Filter = "DocX (*.docx)|*.docx";

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        var exportService = new ExportService();
        //        exportService.ExportarPersona(_Persona, saveFileDialog.FileName);
        //    }
        //}
        private void OnExportarCommandExecute()
        {

            if (VistaCargada == "AsistentesContentView")
            {
                // Exportar datos de una solo Actividad Que ya se copio an Toolbar con el evento ActividadSeleccionadaEvent.
                _ExportService.ExportarAsistente(_Asistente.Model, _Asistente.Nombre);
            }
            else
            {
                if (VistaCargada == "CitasContentView")
                {
                    // Exportar la lista completa de cooperantes
                    var ListaCitas = _CitaRepository.GetAll();
                    _ExportService.ExportarTodasCitas(ListaCitas);
                }
                else
                {
                    //var ListaActividades = _ActividadRepository.GetAll();
                    //_ExportService.ExportarTodasActividades(ListaActividades);
                }
            }
        }

        private void OnMakeBackupCommandExecute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = DateTime.Now.ToShortDateString().Replace('/', '-') + " - atenciones backup.sql";
            saveFileDialog.Filter = "Sql file (*.sql)|*.sql";

            if (saveFileDialog.ShowDialog() == true)
            {
                string connectionString = 
                    ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString;
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
                    ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString;
                DBHelper.Restore(connectionString, openFileDialog.FileName);
                UIServices.RestartApplication();
            }
        }
    }
}
