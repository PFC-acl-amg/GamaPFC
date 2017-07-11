using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Gama.Socios.Wpf.Wrappers;
using Prism.Events;
using Gama.Socios.Wpf.Services;
using Prism.Commands;
using System.Windows.Input;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarPeriodosDeAltaViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private ISession _Session;
        private PreferenciasDeSocios _Settings;
        private IPeriodoDeAltaRepository _PeriodoDeAltaRepository;
        private ICuotaRepository _CuotaRepository;
        public EditarPeriodosDeAltaViewModel(
            ISocioRepository socioRepository,
            IPeriodoDeAltaRepository periodoDeAltaRepository,
            ICuotaRepository cuotaRepository,
            IEventAggregator eventAggregator,
            PreferenciasDeSocios settings,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _PeriodoDeAltaRepository = periodoDeAltaRepository;
            _PeriodoDeAltaRepository.Session = session;
            _CuotaRepository = cuotaRepository;
            _CuotaRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            ActualizarCommand = new DelegateCommand<PeriodoDeAltaWrapper>(OnActualizarCommandExecute,
                OnActualizarCommandCanExecute);
        }
        //---------------------------
        // Publics principales
        //---------------------------
        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _SocioRepository.Session = value;
                _CuotaRepository.Session = value;
                _PeriodoDeAltaRepository.Session = value;
            }
        }
        public PeriodoDeAltaWrapper _PeriodoDeAltaSeleccionado;
        public PeriodoDeAltaWrapper PeriodoDeAltaSeleccionado
        {
            get { return _PeriodoDeAltaSeleccionado; }
            set
            {
                _PeriodoDeAltaSeleccionado = value;
            }
        }
        public SocioWrapper Socio { get; set; }
        //---------------------------
        // ICommands
        //---------------------------
        public ICommand ActualizarCommand { get; private set; }

        private bool OnActualizarCommandCanExecute(PeriodoDeAltaWrapper wrapper)
        {
            return
                (wrapper.IsChanged && wrapper.IsValid) ||
                (wrapper.MesesAplicables.IsChanged && wrapper.MesesAplicables.IsValid);
        }

        private void OnActualizarCommandExecute(PeriodoDeAltaWrapper wrapper)
        {
            var model = wrapper.Model;

            // Guardamos las cuotas efectivas, esto es, donde se haya introducido algún
            // cambio alguna vez en alguno de sus campos
            var cuotasPreexistentes = new List<Cuota>(wrapper.Cuotas.Select(x => x.Model));
            // Los meses aplicables serán en forma de Cuota, desde el mes de alta hasta
            // el mes de baja o la fecha actual, en caso de no haber mes de baja establecido.
            var mesesAplicables = wrapper.GetMesesAplicables();
            wrapper.Cuotas.Clear();

            foreach (var mesAplicable in mesesAplicables)
            {
                wrapper.Cuotas.Add(new CuotaWrapper(mesAplicable));
            }
            wrapper.MesesAplicables.Clear();
            wrapper.Cuotas.Clear();
            mesesAplicables.Select(x => new CuotaWrapper(x)).ToList().ForEach(c =>
            {
                wrapper.MesesAplicables.Add(c);
                wrapper.Cuotas.Add(c);
            }
            );
            _SocioRepository.Update(Socio.Model);
            wrapper.AcceptChanges();
            InvalidateCommands();

            _EventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Publish(wrapper.Id);
            _EventAggregator.GetEvent<ContabilidadModificadaEvent>().Publish(wrapper.Id);
        }
       
        //------------------------------
        // Funciones/Procedimientos auxiliares
        //-----------------------------
        public void Load(SocioWrapper socio)
        {
            Socio = socio;
            if (socio.PeriodosDeAlta.FirstOrDefault() != null)
                PeriodoDeAltaSeleccionado = socio.PeriodosDeAlta.FirstOrDefault();

            foreach (var periodoDeAlta in Socio.PeriodosDeAlta)
            {
                periodoDeAlta.PropertyChanged += (s, e) => { InvalidateCommands(); };
            }
        }
        public void AddPeriodoDeAlta()
        {
            var nuevoPeriodoDeAlta = new PeriodoDeAltaWrapper(new PeriodoDeAlta()
            {
                FechaDeAlta = DateTime.Now.Date,
                Cuotas = new List<Cuota>()
            });

            nuevoPeriodoDeAlta.AddCuota(
                new Cuota
                {
                    CantidadTotal = _Settings.CuotaMensualPredeterminada,
                    Fecha = DateTime.Now.Date.AddDays(1 - DateTime.Now.Date.Day),
                });

            // siempre será esta cuota porque se acaba de añadir y es la única que hay
            nuevoPeriodoDeAlta.MesesAplicables.Clear();
            nuevoPeriodoDeAlta.MesesAplicables.Add(nuevoPeriodoDeAlta.Cuotas[0]);

            nuevoPeriodoDeAlta.PropertyChanged += (s, e) => { InvalidateCommands(); };

            Socio.AddPeriodoDeAlta(nuevoPeriodoDeAlta);
            _SocioRepository.Update(Socio.Model);
            Socio.AcceptChanges();
            InvalidateCommands();
            
            //_EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(Socio.Model);
            _EventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Publish(Socio.Id);
            _EventAggregator.GetEvent<ContabilidadModificadaEvent>().Publish(Socio.Id);
        }
        //------------------------------
        // InvalidateCommands
        //-----------------------------
        private void InvalidateCommands()
        {
            ((DelegateCommand<PeriodoDeAltaWrapper>)ActualizarCommand).RaiseCanExecuteChanged();
        }
    }
}
