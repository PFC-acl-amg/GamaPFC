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

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarPeriodosDeAltaViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private ISession _Session;

        public EditarPeriodosDeAltaViewModel(
            ISocioRepository socioRepository,
            IEventAggregator eventAggregator)
        {
            _SocioRepository = socioRepository;
            _EventAggregator = eventAggregator;

            ActualizarCommand = new DelegateCommand<PeriodoDeAltaWrapper>(OnActualizarCommandExecute,
                OnActualizarCommandCanExecute);
        }

        private void PeriodoDeAltaSeleccionado_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateCommand();
        }

        private void InvalidateCommand()
        {
            ((DelegateCommand<PeriodoDeAltaWrapper>)ActualizarCommand).RaiseCanExecuteChanged();
        }

        public void Load(SocioWrapper socio)
        {
            Socio = socio;

            PeriodoDeAltaSeleccionado = Socio.PeriodosDeAlta.FirstOrDefault();
            PeriodoDeAltaSeleccionado.PropertyChanged += PeriodoDeAltaSeleccionado_PropertyChanged;
        }

        public SocioWrapper Socio { get; set; }
        
        private PeriodoDeAltaWrapper _PeriodoDeAltaSeleccionado;
        public PeriodoDeAltaWrapper PeriodoDeAltaSeleccionado
        {
            get { return _PeriodoDeAltaSeleccionado; }
            set { SetProperty(ref _PeriodoDeAltaSeleccionado, value);}
        }

        public ICommand ActualizarCommand { get; private set; }

        private bool OnActualizarCommandCanExecute(PeriodoDeAltaWrapper wrapper)
        {
            return wrapper.MesesAplicables.IsChanged;
        }

        private void OnActualizarCommandExecute(PeriodoDeAltaWrapper wrapper)
        {
            var model = wrapper.Model;

            var cuotasPreexistentes = new List<Cuota>(wrapper.Cuotas.Select(x => x.Model));

            wrapper.Cuotas.Clear();
            foreach (var mesAplicable in wrapper.MesesAplicables.Select(x => x.Model))
            {
                var cuota = cuotasPreexistentes.Where(x => x.Id == mesAplicable.Id).FirstOrDefault();

                if (cuota == null)
                {
                    model.AddCuota(mesAplicable);
                }
                else // Se ha modificado una cuota existente
                {
                    cuota.CopyValuesFrom(mesAplicable);
                    model.AddCuota(cuota);
                }
            }

            wrapper.AcceptChanges();
            _SocioRepository.Update(Socio.Model);
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
