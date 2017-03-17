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

        public EditarPeriodosDeAltaViewModel(
            ISocioRepository socioRepository,
            IEventAggregator eventAggregator, 
            IPreferenciasDeSocios settings)
        {
            _SocioRepository = socioRepository;
            _EventAggregator = eventAggregator;
            _Settings = settings;

            ActualizarCommand = new DelegateCommand<PeriodoDeAltaWrapper>(OnActualizarCommandExecute,
                OnActualizarCommandCanExecute);
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand<PeriodoDeAltaWrapper>)ActualizarCommand).RaiseCanExecuteChanged();
        }

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

        public PeriodoDeAltaWrapper _PeriodoDeAltaSeleccionado;
        private IPreferenciasDeSocios _Settings;

        public PeriodoDeAltaWrapper PeriodoDeAltaSeleccionado
        {
            get { return _PeriodoDeAltaSeleccionado; }
            set
            {
                _PeriodoDeAltaSeleccionado = value;
            }
        }

        public SocioWrapper Socio { get; set; }

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

            //wrapper.Cuotas.Clear();
            wrapper.MesesAplicables.Clear();
            // Los meses aplicables serán en forma de Cuota, desde el mes de alta hasta
            // el mes de baja o la fecha actual, en caso de no haber mes de baja establecido.
            foreach (var mesAplicable in wrapper.GetMesesAplicables())
            {
                var cuota = cuotasPreexistentes.Where(x => x.Id == mesAplicable.Id).FirstOrDefault();

                // Si la cuota ya existe, la tomamos, actualizamos sus campos en caso de modificación
                // y la añadimos a la lista de cuotas.
                // Si no existe, es que es nueva o no tenía cambio, por lo que añadimos
                // una predeterminada, que es una Cuota con sólo el campo de "CantidadTotal" especificado,
                // que viene de los Settings según la Cuota Mensual Predeterminada
                if (cuota == null)
                {
                    mesAplicable.CantidadTotal = _Settings.CuotaMensualPredeterminada;
                    wrapper.AddCuota(mesAplicable);
                    wrapper.MesesAplicables.Add(new CuotaWrapper(mesAplicable));
                    //model.AddCuota(mesAplicable);
                }
                else // Se ha modificado una cuota existente
                {
                    cuota.CopyValuesFrom(mesAplicable);
                    wrapper.MesesAplicables.Add(new CuotaWrapper(cuota));
                    //wrapper.AddCuota(cuota);
                }

            }

            _SocioRepository.Update(Socio.Model);
            wrapper.AcceptChanges();
            InvalidateCommands();

            _EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(Socio.Model);
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

            _EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(Socio.Model);
        }

        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _SocioRepository.Session = value;
            }
        }
    }
}
