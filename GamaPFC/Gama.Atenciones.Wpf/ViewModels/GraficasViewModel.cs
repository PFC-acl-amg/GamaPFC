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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class GraficasContentViewModel : ViewModelBase
    {
        private IPersonaRepository _PersonaRepository;
        private IEventAggregator _EventAggregator;
        private List<Persona> _Personas;

        public GraficasContentViewModel(
            IPersonaRepository personaRepository,
            IEventAggregator eventAggregator,
            ISession session)
        {
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _EventAggregator = eventAggregator;

            _Personas = _PersonaRepository.GetAll();

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnPersonaCreadaEvent);

            InicializarIdentidadSexual();
            InicializarRangosDeEdad();
            InicializarEstadoCivil();
            InicializarOrientacionSexual();
            InicializarComoConocioAGama();
            InicializarViaDeAccesoAGama();
            InicializarAtencionSolicitada();
        }

        private void OnPersonaCreadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            ValoresDeIdentidadSexual.First(x => x.Key == persona.IdentidadSexual.ToString()).Value++;

            int? edad = persona.EdadNumerica;
            if (edad.HasValue)
            {
                if (edad.Value < 20) ValoresDeEdad.First(x => x.Key == "19").Value++;
                if (edad.Value >= 20 && edad.Value <= 29) ValoresDeEdad.First(x => x.Key == "29").Value++;
                if (edad.Value >= 30 && edad.Value <= 39) ValoresDeEdad.First(x => x.Key == "39").Value++;
                if (edad.Value >= 40) ValoresDeEdad.First(x => x.Key == "40").Value++;
            }
            else
            {
                ValoresDeEdad.First(x => x.Key == "NoProporcionado").Value++;
            }
        }

        public ObservableCollection<ChartItem> ValoresDeIdentidadSexual { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeEdad { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeEstadoCivil { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeOrientacionSexual { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeComoConocioAGama { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeViaDeAccesoAGama { get; private set; }
        public ObservableCollection<ChartItem> ValoresDeAtencionSolicitada { get; private set; }

        private int _HombreCisexualCount;
        private int _HombreTransexualCount;
        private int _MujerCisexualCount;
        private int _MujerTransexualCount;
        private int _NoProporcionadoCount;
        private int _OtraIdentidadCount;

        private ChartItem NewChartItem(string key, string title, int value)
        {
            return new ChartItem { Key = key, Title = title, Value = value };
        }

        private void InicializarIdentidadSexual()
        {
            ValoresDeIdentidadSexual = new ObservableCollection<ChartItem>();

            _HombreCisexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreCisexual);
            _HombreTransexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreTransexual);
            _MujerCisexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerCisexual);
            _MujerTransexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerTransexual);
            _NoProporcionadoCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.NoProporcionado);
            _OtraIdentidadCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.Otra);
            ValoresDeIdentidadSexual.Add(NewChartItem("HombreCisexual", "Hombre Cisexual", _HombreCisexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("HombreTransexual", "Hombre Transexual", _HombreTransexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("MujerCisexual", "Mujer Cisexual", _MujerCisexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("MujerTransexual", "Mujer Transexual",  _MujerTransexualCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("NoProporcionado", "No Proporcionado", _NoProporcionadoCount));
            ValoresDeIdentidadSexual.Add(NewChartItem("Otra", "Otra", _OtraIdentidadCount));
        }

        private int _From0To19;
        private int _From20To29;
        private int _From30To39;
        private int _From40;
        private int _EdadNoProporcionada;

        private void InicializarRangosDeEdad()
        {
            ValoresDeEdad = new ObservableCollection<ChartItem>();

            _From0To19 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 19);
            _From20To29 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 29);
            _From30To39 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 39);
            _From40 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value >= 40);
            _EdadNoProporcionada = _Personas.Count(p => !p.FechaDeNacimiento.HasValue);

            ValoresDeEdad.Add(NewChartItem("19", "0 a 19", _From0To19));
            ValoresDeEdad.Add(NewChartItem("29", "20 a 29", _From20To29));
            ValoresDeEdad.Add(NewChartItem("39", "30 a 39", _From30To39 ));
            ValoresDeEdad.Add(NewChartItem("40", "40 o más", _From40 ));
            ValoresDeEdad.Add(NewChartItem("NoProporcionado", "No Proporcionado", _EdadNoProporcionada ));
        }

        private int _Soltero;
        private int _Casado;
        private int _Separado;
        private int _Divorciado;
        private int _EstadoCivilNoProporcionado;

        private void InicializarEstadoCivil()
        {
            ValoresDeEstadoCivil = new ObservableCollection<ChartItem>();

            _Soltero = _Personas.Count(p => p.EstadoCivil == EstadoCivil.Soltera);
            _Casado = _Personas.Count(p => p.EstadoCivil == EstadoCivil.Casada);
            _Separado = _Personas.Count(p => p.EstadoCivil == EstadoCivil.Separada);
            _Divorciado = _Personas.Count(p => p.EstadoCivil == EstadoCivil.Divorciada);
            _EstadoCivilNoProporcionado = _Personas.Count(p => p.EstadoCivil == EstadoCivil.NoProporcionado);

            ValoresDeEstadoCivil.Add(new ChartItem { Title = "Soltera/o", Value = _Soltero });
            ValoresDeEstadoCivil.Add(new ChartItem { Title = "Casada/o", Value = _Casado });
            ValoresDeEstadoCivil.Add(new ChartItem { Title = "Separada/o", Value = _Separado });
            ValoresDeEstadoCivil.Add(new ChartItem { Title = "Divorciada/o", Value = _Divorciado });
            ValoresDeEstadoCivil.Add(new ChartItem { Title = "No Proporcionado", Value = _EstadoCivilNoProporcionado });
        }

        private int _Heterosexual;
        private int _Gay;
        private int _Lesbiana;
        private int _Bisexual;
        private int _OrientacionSexualNoProporcionada;

        private void InicializarOrientacionSexual()
        {
            ValoresDeOrientacionSexual = new ObservableCollection<ChartItem>();

            _Heterosexual = _Personas.Count(p => p.OrientacionSexual == OrientacionSexual.Heterosexual);
            _Gay = _Personas.Count(p => p.OrientacionSexual == OrientacionSexual.Gay);
            _Lesbiana = _Personas.Count(p => p.OrientacionSexual == OrientacionSexual.Lesbiana);
            _Bisexual = _Personas.Count(p => p.OrientacionSexual == OrientacionSexual.Bisexual);
            _OrientacionSexualNoProporcionada = _Personas.Count(p => p.OrientacionSexual == OrientacionSexual.NoProporcionado);

            ValoresDeOrientacionSexual.Add(new ChartItem { Title = "Heterosexual", Value = _Heterosexual });
            ValoresDeOrientacionSexual.Add(new ChartItem { Title = "Gay", Value = _Gay });
            ValoresDeOrientacionSexual.Add(new ChartItem { Title = "Lesbiana", Value = _Lesbiana });
            ValoresDeOrientacionSexual.Add(new ChartItem { Title = "Bisexual", Value = _Bisexual });
            ValoresDeOrientacionSexual.Add(new ChartItem { Title = "No Proporcionado", Value = _OrientacionSexualNoProporcionada });
        }

        private int _RedFormal;
        private int _RedInformal;
        private int _Difusion;
        private int _ComoConocioAGamaNoProporcionado;

        private void InicializarComoConocioAGama()
        {
            ValoresDeComoConocioAGama = new ObservableCollection<ChartItem>();

            _RedFormal = _Personas.Count(p => p.ComoConocioAGama == ComoConocioAGama.RedFormal);
            _RedInformal = _Personas.Count(p => p.ComoConocioAGama == ComoConocioAGama.RedInformal);
            _Difusion = _Personas.Count(p => p.ComoConocioAGama == ComoConocioAGama.Difusion);
            _ComoConocioAGamaNoProporcionado = _Personas.Count(p => p.ComoConocioAGama == ComoConocioAGama.NoProporcionado);

            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "Red Formal", Value = _RedFormal });
            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "Red Informal", Value = _RedInformal });
            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "Difusión", Value = _Difusion });
            ValoresDeComoConocioAGama.Add(new ChartItem { Title = "No Proporcionado", Value = _ComoConocioAGamaNoProporcionado });
        }

        private int _Personal;
        private int _Telefonica;
        private int _Email;
        private int _ViaDeAccesoNoProporcionada;

        private void InicializarViaDeAccesoAGama()
        {
            ValoresDeViaDeAccesoAGama = new ObservableCollection<ChartItem>();

            _Personal = _Personas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.Personal);
            _Telefonica = _Personas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.Telefonica);
            _Email = _Personas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.Email);
            _ViaDeAccesoNoProporcionada = _Personas.Count(p => p.ViaDeAccesoAGama == ViaDeAccesoAGama.NoProporcionado);

            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "Personal", Value = _Personal });
            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "Telefónica", Value = _Telefonica });
            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "Email", Value = _Email });
            ValoresDeViaDeAccesoAGama.Add(new ChartItem { Title = "No Proporcionado", Value = _ViaDeAccesoNoProporcionada });
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

            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Psicológica", Value = _Psicologica });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Jurídica", Value = _Juridica });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Social", Value = _Social });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Acogida", Value = _DeAcogida });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Prevención para la Salud", Value = _PrevencionParaLaSalud });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Orientación Formativa/Laboral", Value = _OrientacionLaboral });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Educación/Formación", Value = _EducacionFormacion });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Participación en Gamá", Value = _ParticipacionEnGama });
            ValoresDeAtencionSolicitada.Add(new ChartItem { Title = "Otra", Value = _OtraAtencion });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish("GraficasView");
        }
    }
}
