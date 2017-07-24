using Core;
using Core.Controls;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
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

namespace Gama.Socios.Wpf.ViewModels
{
    public class GraficasContentViewModel : ViewModelBase
    {
        private ISocioRepository _SocioRepository;
        private ICuotaRepository _CuotaRepository;
        private EventAggregator _EventAggregator;
        private List<Socio> _Socios;

        public GraficasContentViewModel(
            ISocioRepository socioRepository,
            ICuotaRepository cuotaRepository,
            EventAggregator eventAggregator,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _CuotaRepository = cuotaRepository;
            _EventAggregator = eventAggregator;

            _SocioRepository.Session = session;
            _CuotaRepository.Session = session;

            _Socios = _SocioRepository.Socios;

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe((id) => EstadoSinActualizar());
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe((id) => EstadoSinActualizar());
            _EventAggregator.GetEvent<SocioEliminadoEvent>().Subscribe((id) => EstadoSinActualizar());

            RefrescarCommand = new DelegateCommand(() => Refresh(), () => HayCambios);

            InicializarSocios();
            InicializarCuotas();
            InicializarMeses();
        }

        public ICommand RefrescarCommand { get; private set; }

        private bool _HayCambios;
        public bool HayCambios
        {
            get { return _HayCambios; }
            set { SetProperty(ref _HayCambios, value); }
        }

        private void EstadoSinActualizar()
        {
            HayCambios = true;
            ((DelegateCommand)RefrescarCommand).RaiseCanExecuteChanged();
        }

        private ChartItem NewChartItem(string key, string title, int value)
        {
            return new ChartItem { Key = key, Title = $"{title}: {value}", Value = value };
        }

        private ChartItem _NewChartItem(string title, int value)
        {
            return new ChartItem { Key = title, Title = $"{title}: {value}", Value = value };
        }

        private bool _Check(bool conditionIsActive, string conditionValue, string expectedValue)
        {
            return conditionIsActive ? conditionValue == expectedValue : false;
        }

        private bool _CheckEdad(bool conditionIsActive, int? conditionValue, int expectedMinValue, int expectedMaxValue)
        {
            if (conditionIsActive && !conditionValue.HasValue)
                return false;

            return conditionIsActive ? conditionValue.Value >= expectedMinValue && conditionValue.Value <= expectedMaxValue : false;
        }

        private void Refresh()
        {
            InicializarSocios();
            InicializarCuotas();
            InicializarMeses();

            HayCambios = false;
            ((DelegateCommand)RefrescarCommand).RaiseCanExecuteChanged();
        }

        public ObservableCollection<ChartItem> ValoresDeSocios { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeCuotas { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeMeses { get; private set; }

        private int _From0To25;
        private int _From26To40;
        private int _From41To55;
        private int _From56To65;
        private int _From66;

        private void InicializarSocios()
        {
            ValoresDeSocios = new ObservableCollection<ChartItem>();

            _From0To25 = _Socios.Count(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value <= 25);
            _From26To40 = _Socios.Count(x => x.EdadNumerica.HasValue && x.EdadNumerica >= 26 && x.EdadNumerica.Value <= 40);
            _From41To55 = _Socios.Count(x => x.EdadNumerica.HasValue && x.EdadNumerica >= 41 && x.EdadNumerica.Value <= 55);
            _From56To65 = _Socios.Count(x => x.EdadNumerica.HasValue && x.EdadNumerica >= 56 && x.EdadNumerica.Value <= 65);
            _From66 = _Socios.Count(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value >= 66);

            ValoresDeSocios.Add(NewChartItem("25", "0 a 25", _From0To25));
            ValoresDeSocios.Add(NewChartItem("40", "26 a 40", _From26To40));
            ValoresDeSocios.Add(NewChartItem("55", "41 a 55", _From41To55));
            ValoresDeSocios.Add(NewChartItem("65", "56 a 65", _From56To65));
            ValoresDeSocios.Add(NewChartItem("66", "66 o más", _From66));

            OnPropertyChanged(nameof(ValoresDeSocios));
        }

        private int _From0To25Cuotas;
        private int _From26To40Cuotas;
        private int _From41To55Cuotas;
        private int _From56To65Cuotas;
        private int _From66Cuotas;

        private void InicializarCuotas()
        {
            ValoresDeCuotas = new ObservableCollection<ChartItem>();

            var socios = new List<Socio>(_Socios);
            int valor = 0;

            List<Socio> tempSocios;

            tempSocios =_Socios.Where(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value <= 25).ToList();
            var periodosDeAlta = tempSocios.SelectMany(x => x.PeriodosDeAlta);
            var cuotas = periodosDeAlta.SelectMany(x => x.Cuotas).ToList();
            cuotas.ForEach(c => valor += (int)c.CantidadPagada);
            ValoresDeCuotas.Add(NewChartItem("25", "0 a 25", valor));

            valor = 0;
            tempSocios = _Socios.Where(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value >= 26 && x.EdadNumerica.Value <= 40).ToList();
            periodosDeAlta = tempSocios.SelectMany(x => x.PeriodosDeAlta);
            cuotas = periodosDeAlta.SelectMany(x => x.Cuotas).ToList();
            cuotas.ForEach(c => valor += (int)c.CantidadPagada);
            ValoresDeCuotas.Add(NewChartItem("40", "26 a 40", valor));

            valor = 0;
            tempSocios = _Socios.Where(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value >= 41 && x.EdadNumerica.Value <= 55).ToList();
            periodosDeAlta = tempSocios.SelectMany(x => x.PeriodosDeAlta);
            cuotas = periodosDeAlta.SelectMany(x => x.Cuotas).ToList();
            cuotas.ForEach(c => valor += (int)c.CantidadPagada);
            ValoresDeCuotas.Add(NewChartItem("55", "41 a 55", valor));

            valor = 0;
            tempSocios = _Socios.Where(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value >= 56 && x.EdadNumerica.Value <= 65).ToList();
            periodosDeAlta = tempSocios.SelectMany(x => x.PeriodosDeAlta);
            cuotas = periodosDeAlta.SelectMany(x => x.Cuotas).ToList();
            cuotas.ForEach(c => valor += (int)c.CantidadPagada);
            ValoresDeCuotas.Add(NewChartItem("65", "56 a 65", valor));

            valor = 0;
            tempSocios = _Socios.Where(x => x.EdadNumerica.HasValue && x.EdadNumerica.Value >= 66).ToList();
            periodosDeAlta = tempSocios.SelectMany(x => x.PeriodosDeAlta);
            cuotas = periodosDeAlta.SelectMany(x => x.Cuotas).ToList();
            cuotas.ForEach(c => valor += (int)c.CantidadPagada);
            ValoresDeCuotas.Add(NewChartItem("66", "66 o más", valor));

            OnPropertyChanged(nameof(ValoresDeCuotas));
        }

        private void InicializarMeses()
        {
            ValoresDeMeses = new ObservableCollection<ChartItem>();

            var socios = new List<Socio>(_Socios);
            int valor = 0;

            string[] meses = new string[12]
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre",
                "Octubre", "Noviembre", "Diciembre"
            };

            var periodosDeAlta = _Socios.SelectMany(x => x.PeriodosDeAlta);
            var cuotas = periodosDeAlta.SelectMany(x => x.Cuotas)
                .Where(c => c.Fecha.Year == DateTime.Now.Year && c.Fecha.Month == 1)
                .ToList();
            cuotas.ForEach(c => valor += (int)c.CantidadPagada);
            ValoresDeMeses.Add(NewChartItem("Enero", "Enero", valor));

            for (int i = 1; i <= 11; i++)
            {
                valor = 0;
                cuotas = periodosDeAlta.SelectMany(x => x.Cuotas)
                    .Where(c => c.Fecha.Year == DateTime.Now.Year && c.Fecha.Month == i + 1)
                    .ToList();
                cuotas.ForEach(c => valor += (int)c.CantidadPagada);
                ValoresDeMeses.Add(NewChartItem(meses[i], meses[i], valor));
            }

            OnPropertyChanged(nameof(ValoresDeMeses));
        }
    }
}
