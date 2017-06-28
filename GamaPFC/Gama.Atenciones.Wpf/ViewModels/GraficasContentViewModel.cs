using Core;
using Gama.Atenciones.Wpf.Services;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Atenciones.Business;
using System.Collections.ObjectModel;
using Gama.Atenciones.Wpf.UIEvents;
using Prism.Regions;
using Prism.Events;
using Gama.Atenciones.Wpf.Eventos;
using System.Windows.Input;
using Prism.Commands;
using Gama.Common.Eventos;

namespace Gama.Atenciones.Wpf.ViewModels
{

    public class GraficasContentViewModel : ViewModelBase
    {
        private IPersonaRepository _PersonaRepository;
        private IAtencionRepository _AtencionRepository;
        private IEventAggregator _EventAggregator;
        private List<Persona> _Personas;
        private List<Persona> _PersonasFiltradas;

        public GraficasContentViewModel(
            IPersonaRepository personaRepository,
            IAtencionRepository atencionRepository,
            IEventAggregator eventAggregator,
            ISession session)
        {
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _AtencionRepository = atencionRepository;
            _AtencionRepository.Session = session;
            _EventAggregator = eventAggregator;

            FiltroDeIdentidadSexual = new Dictionary<string, bool>()
            {
                {"Hombre Cisexual", true},
                {"Mujer Cisexual", true},
                {"Hombre Transexual", true},
                {"Mujer Transexual", true},
                {"Otra", true},
                {"No Proporcionado", true},
            };

            FiltroDeOrientacionSexual = new Dictionary<string, bool>()
            {
                {"Heterosexual", true},
                {"Bisexual", true},
                {"Lesbiana", true},
                {"Gay", true},
                {"No Proporcionado", true},
            };

            FiltroDeRangoDeEdad = new Dictionary<string, bool>()
            {
                {"19", true},
                {"29", true},
                {"39", true},
                {"40", true},
                {"No Proporcionado", true},
            };

            FiltroDeViaDeAccesoAGama = new Dictionary<string, bool>()
            {
                {"Personal", true},
                {"Telefonica", true},
                {"Email", true},
                {"No Proporcionado", true},
            };

            FiltroDeComoConocioAGama = new Dictionary<string, bool>()
            {
                {"Red Formal", true},
                {"Red Informal", true},
                {"Difusion", true},
                {"No Proporcionado", true},
            };

            FiltroDeEstadoCivil = new Dictionary<string, bool>()
            {
                {"Soltera", true},
                {"Casada", true},
                {"Separada", true},
                {"Divorciada", true},
                {"No Proporcionado", true},
            };

            _Personas = _PersonaRepository.GetAll();
            _PersonasFiltradas = _Personas.ToList();

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe((id) => EstadoSinActualizar());
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe((id) => EstadoSinActualizar());
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe((id) => EstadoSinActualizar());

            RefrescarCommand = new DelegateCommand(() => Refresh(), () => HayCambios);
            FiltrarCommand = new DelegateCommand(() => Filtrar());

            InicializarIdentidadSexual();
            InicializarRangosDeEdad();
            InicializarEstadoCivil();
            InicializarOrientacionSexual();
            InicializarComoConocioAGama();
            InicializarViaDeAccesoAGama();
            InicializarAtencionSolicitada();
            InicializarDerivaciones();
        }

        public ICommand RefrescarCommand { get; private set; }
        public ICommand FiltrarCommand { get; private set; }

        public Dictionary<string, bool> FiltroDeIdentidadSexual { get;  private set; }
        public Dictionary<string, bool> FiltroDeOrientacionSexual { get; private set; }
        public Dictionary<string, bool> FiltroDeRangoDeEdad { get; private set; }
        public Dictionary<string, bool> FiltroDeViaDeAccesoAGama { get; private set; }
        public Dictionary<string, bool> FiltroDeComoConocioAGama { get; private set; }
        public Dictionary<string, bool> FiltroDeEstadoCivil { get; private set; }

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
            return new ChartItem { Key = key, Title = $"{title}: {value}", Value = value};
        }

        private ChartItem _NewChartItem(string title, int value)
        {
            return new ChartItem { Key = title, Title = $"{title}: {value}", Value = value };
        }

        public override void OnActualizarServidor()
        {
            _Personas = _PersonaRepository.GetAll();
            Filtrar();
        }

        private bool _FiltroExclusivo = true;

        private void Filtrar()
        {
            //Filtrar
            IEnumerable<Persona> query = _Personas;

            query = query.Where
                (p =>
                    (false
                     || _Check(FiltroDeIdentidadSexual["Hombre Cisexual"], p.IdentidadSexual, IdentidadSexual.HombreCisexual.ToString())
                     || _Check(FiltroDeIdentidadSexual["Mujer Cisexual"], p.IdentidadSexual, IdentidadSexual.MujerCisexual.ToString())
                     || _Check(FiltroDeIdentidadSexual["Hombre Transexual"], p.IdentidadSexual, IdentidadSexual.HombreTransexual.ToString())
                     || _Check(FiltroDeIdentidadSexual["Mujer Transexual"], p.IdentidadSexual, IdentidadSexual.MujerTransexual.ToString())
                     || _Check(FiltroDeIdentidadSexual["Otra"], p.IdentidadSexual, IdentidadSexual.Otra.ToString())
                     || _Check(FiltroDeIdentidadSexual["No Proporcionado"], p.IdentidadSexual, IdentidadSexual.NoProporcionado.ToString())
                    )
                    &&
                    (false
                     || _Check(FiltroDeOrientacionSexual["Heterosexual"], p.OrientacionSexual, OrientacionSexual.Heterosexual.ToString())
                     || _Check(FiltroDeOrientacionSexual["Bisexual"], p.OrientacionSexual, OrientacionSexual.Bisexual.ToString())
                     || _Check(FiltroDeOrientacionSexual["Lesbiana"], p.OrientacionSexual, OrientacionSexual.Lesbiana.ToString())
                     || _Check(FiltroDeOrientacionSexual["Gay"], p.OrientacionSexual, OrientacionSexual.Gay.ToString())
                     || _Check(FiltroDeOrientacionSexual["No Proporcionado"], p.OrientacionSexual, OrientacionSexual.NoProporcionado.ToString())
                    )
                    &&
                    (false
                     || _CheckEdad(FiltroDeRangoDeEdad["19"], p.EdadNumerica, 0, 19)
                     || _CheckEdad(FiltroDeRangoDeEdad["29"], p.EdadNumerica, 20, 29)
                     || _CheckEdad(FiltroDeRangoDeEdad["39"], p.EdadNumerica, 30, 39)
                     || _CheckEdad(FiltroDeRangoDeEdad["40"], p.EdadNumerica, 40, 999)
                     || _Check(FiltroDeRangoDeEdad["No Proporcionado"], p.EdadNumerica.HasValue.ToString(), false.ToString())
                    )
                    &&
                    (false
                     || _Check(FiltroDeEstadoCivil["Soltera"], p.EstadoCivil, EstadoCivil.Soltera.ToString())
                     || _Check(FiltroDeEstadoCivil["Casada"], p.EstadoCivil, EstadoCivil.Casada.ToString())
                     || _Check(FiltroDeEstadoCivil["Separada"], p.EstadoCivil, EstadoCivil.Separada.ToString())
                     || _Check(FiltroDeEstadoCivil["Divorciada"], p.EstadoCivil, EstadoCivil.Divorciada.ToString())
                     || _Check(FiltroDeEstadoCivil["No Proporcionado"], p.EstadoCivil, EstadoCivil.NoProporcionado.ToString())
                    )
                );

            _PersonasFiltradas = query.ToList();

            Refresh();
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
            InicializarIdentidadSexual();
            InicializarRangosDeEdad();
            InicializarEstadoCivil();
            InicializarOrientacionSexual();
            InicializarComoConocioAGama();
            InicializarViaDeAccesoAGama();
            InicializarAtencionSolicitada();
            InicializarDerivaciones();

            HayCambios = false;
            ((DelegateCommand)RefrescarCommand).RaiseCanExecuteChanged();
        }

        public ObservableCollection<ChartItem> ValoresDeIdentidadSexual { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeEdad { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeEstadoCivil { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeOrientacionSexual { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeComoConocioAGama { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeViaDeAccesoAGama { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeAtencionSolicitada { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeDerivaciones { get; private set; }

        private int _HombreCisexualCount;
        private int _HombreTransexualCount;
        private int _MujerCisexualCount;
        private int _MujerTransexualCount;
        private int _NoProporcionadoCount;
        private int _OtraIdentidadCount;

        private void InicializarIdentidadSexual()
        {
            ValoresDeIdentidadSexual = new ObservableCollection<ChartItem>();

            _HombreCisexualCount = _PersonasFiltradas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreCisexual.ToString());
            _HombreTransexualCount = _PersonasFiltradas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreTransexual.ToString());
            _MujerCisexualCount = _PersonasFiltradas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerCisexual.ToString());
            _MujerTransexualCount = _PersonasFiltradas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerTransexual.ToString());
            _NoProporcionadoCount = _PersonasFiltradas.Count(p => p.IdentidadSexual == IdentidadSexual.NoProporcionado.ToString());
            _OtraIdentidadCount = _PersonasFiltradas.Count(p => p.IdentidadSexual == IdentidadSexual.Otra.ToString());

            ValoresDeIdentidadSexual.Add(NewChartItem("HombreCisexual", "Hombre Cisexual", _HombreCisexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("HombreTransexual", "Hombre Transexual", _HombreTransexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("MujerCisexual", "Mujer Cisexual", _MujerCisexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("MujerTransexual", "Mujer Transexual",  _MujerTransexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("NoProporcionado", "No Proporcionado", _NoProporcionadoCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("Otra", "Otra", _OtraIdentidadCount));

            OnPropertyChanged(nameof(ValoresDeIdentidadSexual));
        }

        private int _From0To19;
        private int _From20To29;
        private int _From30To39;
        private int _From40;
        private int _EdadNoProporcionada;

        private void InicializarRangosDeEdad()
        {
            ValoresDeEdad = new ObservableCollection<ChartItem>();

            _From0To19 = _PersonasFiltradas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 19);
            _From20To29 = _PersonasFiltradas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value >= 20 && p.EdadNumerica.Value <= 29);
            _From30To39 = _PersonasFiltradas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value >= 30 && p.EdadNumerica.Value <= 39);
            _From40 = _PersonasFiltradas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value >= 40);
            _EdadNoProporcionada = _PersonasFiltradas.Count(p => !p.FechaDeNacimiento.HasValue);

            ValoresDeEdad.Add(NewChartItem("19", "0 a 19", _From0To19));
            ValoresDeEdad.Add(NewChartItem("29", "20 a 29", _From20To29));
            ValoresDeEdad.Add(NewChartItem("39", "30 a 39", _From30To39 ));
            ValoresDeEdad.Add(NewChartItem("40", "40 o más", _From40 ));
            ValoresDeEdad.Add(NewChartItem("NoProporcionado", "No Proporcionado", _EdadNoProporcionada ));

            OnPropertyChanged(nameof(ValoresDeEdad));
        }

        private int _Soltero;
        private int _Casado;
        private int _Separado;
        private int _Divorciado;
        private int _EstadoCivilNoProporcionado;

        private void InicializarEstadoCivil()
        {
            ValoresDeEstadoCivil = new ObservableCollection<ChartItem>();

            _Soltero = _PersonasFiltradas.Count(p => p.EstadoCivil == EstadoCivil.Soltera.ToString());
            _Casado = _PersonasFiltradas.Count(p => p.EstadoCivil == EstadoCivil.Casada.ToString());
            _Separado = _PersonasFiltradas.Count(p => p.EstadoCivil == EstadoCivil.Separada.ToString());
            _Divorciado = _PersonasFiltradas.Count(p => p.EstadoCivil == EstadoCivil.Divorciada.ToString());
            _EstadoCivilNoProporcionado = _PersonasFiltradas.Count(p => p.EstadoCivil == EstadoCivil.NoProporcionado.ToString());

            ValoresDeEstadoCivil.Add(NewChartItem("Soltera/o", "Soltera/o", _Soltero ));
            ValoresDeEstadoCivil.Add(NewChartItem("Casada/o", "Casada/o", _Casado));
            ValoresDeEstadoCivil.Add(NewChartItem("Separada/o", "Separada/o", _Separado));
            ValoresDeEstadoCivil.Add(NewChartItem("Divorciada/o", "Divorciada/o", _Divorciado));
            ValoresDeEstadoCivil.Add(NewChartItem("No Proporcionado", "No Proporcionado", _EstadoCivilNoProporcionado));

            OnPropertyChanged(nameof(ValoresDeEstadoCivil));
        }

        private int _Heterosexual;
        private int _Gay;
        private int _Lesbiana;
        private int _Bisexual;
        private int _OrientacionSexualNoProporcionada;

        private void InicializarOrientacionSexual()
        {
            ValoresDeOrientacionSexual = new ObservableCollection<ChartItem>();
            
            _Heterosexual = _PersonasFiltradas.Count(p => p.OrientacionSexual == OrientacionSexual.Heterosexual.ToString());
            _Gay = _PersonasFiltradas.Count(p => p.OrientacionSexual == OrientacionSexual.Gay.ToString());
            _Lesbiana = _PersonasFiltradas.Count(p => p.OrientacionSexual == OrientacionSexual.Lesbiana.ToString());
            _Bisexual = _PersonasFiltradas.Count(p => p.OrientacionSexual == OrientacionSexual.Bisexual.ToString());
            _OrientacionSexualNoProporcionada = _PersonasFiltradas.Count(p => p.OrientacionSexual == OrientacionSexual.NoProporcionado.ToString());

            ValoresDeOrientacionSexual.Add(NewChartItem("Heterosexual", "Heterosexual", _Heterosexual));
            ValoresDeOrientacionSexual.Add(NewChartItem("Gay", "Gay", _Gay));
            ValoresDeOrientacionSexual.Add(NewChartItem("Lesbiana", "Lesbiana", _Lesbiana));
            ValoresDeOrientacionSexual.Add(NewChartItem("Bisexual", "Bisexual", _Bisexual));
            ValoresDeOrientacionSexual.Add(NewChartItem("No Proporcionado", "No Proporcionado", _OrientacionSexualNoProporcionada));

            OnPropertyChanged(nameof(ValoresDeOrientacionSexual));
        }

        private int _RedFormal;
        private int _RedInformal;
        private int _Difusion;
        private int _ComoConocioAGamaNoProporcionado;

        private void InicializarComoConocioAGama()
        {
            ValoresDeComoConocioAGama = new ObservableCollection<ChartItem>();

            _RedFormal = _PersonasFiltradas.Count(p => p.ComoConocioAGama == ComoConocioAGama.RedFormal.ToString());
            _RedInformal = _PersonasFiltradas.Count(p => p.ComoConocioAGama == ComoConocioAGama.RedInformal.ToString());
            _Difusion = _PersonasFiltradas.Count(p => p.ComoConocioAGama == ComoConocioAGama.Difusion.ToString());
            _ComoConocioAGamaNoProporcionado = _PersonasFiltradas.Count(p => p.ComoConocioAGama == ComoConocioAGama.NoProporcionado.ToString());

            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "Red Formal", Value = _RedFormal });
            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "Red Informal", Value = _RedInformal });
            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "Difusión", Value = _Difusion });
            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "No Proporcionado", Value = _ComoConocioAGamaNoProporcionado });

            OnPropertyChanged(nameof(ValoresDeComoConocioAGama));
        }

        private int _Personal;
        private int _Telefonica;
        private int _Email;
        private int _ViaDeAccesoNoProporcionada;

        private void InicializarViaDeAccesoAGama()
        {
            ValoresDeViaDeAccesoAGama = new ObservableCollection<ChartItem>();

            _Personal = _PersonasFiltradas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.Personal.ToString());
            _Telefonica = _PersonasFiltradas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.Telefonica.ToString());
            _Email = _PersonasFiltradas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.Email.ToString());
            _ViaDeAccesoNoProporcionada = _PersonasFiltradas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.NoProporcionado.ToString());

            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "Personal", Value = _Personal });
            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "Telefónica", Value = _Telefonica });
            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "Email", Value = _Email });
            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "No Proporcionado", Value = _ViaDeAccesoNoProporcionada });

            OnPropertyChanged(nameof(ValoresDeViaDeAccesoAGama));
        }

        private int _Psicologica;
        private int _Juridica;
        private int _Social;
        private int _DeAcogida;
        private int _PrevencionParaLaSalud;
        private int _OrientacionLaboral;
        private int _EducacionFormacion;
        private int _ParticipacionEnGama;
        private int _OtraAtencion;

        private void InicializarAtencionSolicitada()
        {
            ValoresDeAtencionSolicitada = new ObservableCollection<ChartItem>();

            var atenciones = _PersonaRepository.GetAtenciones();

            _Psicologica = atenciones.Count(x => x.EsPsicologica);
            _Juridica = atenciones.Count(x => x.EsJuridica);
            _Social = atenciones.Count(x => x.EsSocial);
            _DeAcogida = atenciones.Count(x => x.EsDeAcogida);
            _PrevencionParaLaSalud = atenciones.Count(x => x.EsDePrevencionParaLaSalud);
            _OrientacionLaboral = atenciones.Count(x => x.EsDeOrientacionLaboral);
            _EducacionFormacion = atenciones.Count(x => x.EsDeFormacion);
            _ParticipacionEnGama = atenciones.Count(x => x.EsDeParticipacion);
            _OtraAtencion = atenciones.Count(x => x.EsOtra);

            ValoresDeAtencionSolicitada.Add(_NewChartItem("Psicológica", _Psicologica ));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Jurídica", _Juridica));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Social", _Social));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Acogida", _DeAcogida));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Prevención para la Salud", _PrevencionParaLaSalud));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Orientación Formativa/Laboral", _OrientacionLaboral));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Educación/Formación", _EducacionFormacion));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Participación en Gamá", _ParticipacionEnGama));
            ValoresDeAtencionSolicitada.Add(_NewChartItem("Otra", _OtraAtencion));

            OnPropertyChanged(nameof(ValoresDeAtencionSolicitada));
        }

        private int _DerivacionSocial;
        private int _DerivacionJuridica;
        private int _DerivacionPsicologica;
        private int _DerivacionDeFormacion;
        private int _DerivacionDeOrientacionLaboral;
        private int _DerivacionExterna;
        private int _DerivacionSocial_Realizada;
        private int _DerivacionJuridica_Realizada;
        private int _DerivacionPsicologica_Realizada;
        private int _DerivacionDeFormacion_Realizada;
        private int _DerivacionDeOrientacionLaboral_Realizada;
        private int _DerivacionExterna_Realizada;

        private void InicializarDerivaciones()
        {
            ValoresDeDerivaciones = new ObservableCollection<ChartItem>();

            List<Derivacion> derivaciones = _AtencionRepository.GetAll().Select(x => x.Derivacion).ToList();

            _DerivacionSocial = derivaciones.Count(x => x.EsSocial);
            _DerivacionJuridica = derivaciones.Count(x => x.EsJuridica);
            _DerivacionPsicologica = derivaciones.Count(x => x.EsPsicologica);
            _DerivacionDeFormacion = derivaciones.Count(x => x.EsDeFormacion);
            _DerivacionDeOrientacionLaboral = derivaciones.Count(x => x.EsDeOrientacionLaboral);
            _DerivacionExterna = derivaciones.Count(x => x.EsExterna);

            _DerivacionSocial_Realizada = derivaciones.Count(x => x.EsSocial_Realizada);
            _DerivacionJuridica_Realizada = derivaciones.Count(x => x.EsJuridica_Realizada);
            _DerivacionPsicologica_Realizada = derivaciones.Count(x => x.EsPsicologica_Realizada);
            _DerivacionDeFormacion_Realizada = derivaciones.Count(x => x.EsDeFormacion_Realizada);
            _DerivacionDeOrientacionLaboral_Realizada = derivaciones.Count(x => x.EsDeOrientacionLaboral_Realizada);
            _DerivacionExterna_Realizada = derivaciones.Count(x => x.EsExterna_Realizada);

            ValoresDeDerivaciones.Add(_NewChartItem("Psicológica", _DerivacionPsicologica ));
            ValoresDeDerivaciones.Add(_NewChartItem("Psicológica Realizada", _DerivacionPsicologica_Realizada));
            ValoresDeDerivaciones.Add(_NewChartItem("Jurídica", _DerivacionJuridica));
            ValoresDeDerivaciones.Add(_NewChartItem("Jurídica Realizada", _DerivacionJuridica_Realizada));
            ValoresDeDerivaciones.Add(_NewChartItem("Social", _DerivacionSocial));
            ValoresDeDerivaciones.Add(_NewChartItem("Social Realizada", _DerivacionSocial_Realizada));
            ValoresDeDerivaciones.Add(_NewChartItem("Orientación Formativa/Laboral", _DerivacionDeOrientacionLaboral));
            ValoresDeDerivaciones.Add(_NewChartItem("Orientación Formativa/Laboral Realizada", _DerivacionDeOrientacionLaboral_Realizada));
            ValoresDeDerivaciones.Add(_NewChartItem("Educación/Formación", _DerivacionDeFormacion));
            ValoresDeDerivaciones.Add(_NewChartItem("Educación/Formación Realizada", _DerivacionDeFormacion_Realizada));
            ValoresDeDerivaciones.Add(_NewChartItem("Externa", _DerivacionExterna));
            ValoresDeDerivaciones.Add(_NewChartItem("Externa Realizada", _DerivacionExterna_Realizada));

            OnPropertyChanged(nameof(ValoresDeDerivaciones));
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("GraficasView");
        }
    }
}
