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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class GraficasViewModel : ViewModelBase
    {
        private IPersonaRepository _PersonaRepository;
        private List<Persona> _Personas;

        public GraficasViewModel(
            IPersonaRepository personaRepository,
            ISession session)
        {
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;

            _Personas = _PersonaRepository.GetAll();
            ValoresDeIdentidadSexual = new ObservableCollection<ChartItem>();
            ValoresDeEdad = new ObservableCollection<ChartItem>();

            InicializarIdentidadSexual();
            InicializarRangosDeEdad();
        }

        private int _HombreCisexualCount;
        private int _HombreTransexualCount;
        private int _MujerCisexualCount;
        private int _MujerTransexualCount;
        private int _NoProporcionadoCount;
        private int _OtraIdentidadCount;
        public ObservableCollection<ChartItem> ValoresDeIdentidadSexual { get; private set; }

        private void InicializarIdentidadSexual()
        {
            _HombreCisexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreCisexual);
            _HombreTransexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreTransexual);
            _MujerCisexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerCisexual);
            _MujerTransexualCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerTransexual);
            _NoProporcionadoCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.NoProporcionado);
            _OtraIdentidadCount = _Personas.Count(p => p.IdentidadSexual == IdentidadSexual.Otra);
            ValoresDeIdentidadSexual.Add(new ChartItem { Title = "Hombre Cisexual", Value = _HombreCisexualCount });
            ValoresDeIdentidadSexual.Add(new ChartItem { Title = "Hombre Transexual", Value = _HombreTransexualCount });
            ValoresDeIdentidadSexual.Add(new ChartItem { Title = "Mujer Cisexual", Value = _MujerCisexualCount });
            ValoresDeIdentidadSexual.Add(new ChartItem { Title = "Mujer Transexual", Value = _MujerTransexualCount });
            ValoresDeIdentidadSexual.Add(new ChartItem { Title = "No Proporcionado", Value = _NoProporcionadoCount });
            ValoresDeIdentidadSexual.Add(new ChartItem { Title = "Otra Identidad", Value = _OtraIdentidadCount });
        }

        private int _From0To19;
        private int _From20To29;
        private int _From30To39;
        private int _From40;
        private int _EdadNoProporcionada;
        public ObservableCollection<ChartItem> ValoresDeEdad { get; private set; }

        private void InicializarRangosDeEdad()
        {
            _From0To19 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 19);
            _From20To29 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 29);
            _From30To39 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value <= 39);
            _From40 = _Personas.Count(p => p.EdadNumerica.HasValue && p.EdadNumerica.Value >= 40);
            _EdadNoProporcionada = _Personas.Count(p => !p.FechaDeNacimiento.HasValue);

            ValoresDeEdad.Add(new ChartItem { Title = "0 a 19", Value = _From0To19 });
            ValoresDeEdad.Add(new ChartItem { Title = "20 a 29", Value = _From20To29 });
            ValoresDeEdad.Add(new ChartItem { Title = "30 a 39", Value = _From30To39 });
            ValoresDeEdad.Add(new ChartItem { Title = "40 o más", Value = _From40 });
            ValoresDeEdad.Add(new ChartItem { Title = "No Proporcionado", Value = _EdadNoProporcionada });
        }
    }
}
