using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class DashboardViewModelDTD
    {
        private PreferenciasDeAtenciones _Settings;
        private FakeAtencionRepository _AtencionRepository;
        private FakeCitaRepository _CitaRepository;
        private FakePersonaRepository _PersonaRepository;

        public DashboardViewModelDTD()
        {
            _Settings = new PreferenciasDeAtenciones();
            _PersonaRepository = new FakePersonaRepository();
            _CitaRepository = new FakeCitaRepository();
            _AtencionRepository = new FakeAtencionRepository();

            Personas = new ObservableCollection<LookupItem>(
                new FakePersonaRepository().GetAll()
                    .OrderBy(a => a.Nombre)
                    //.Take(_Settings.DashboardUltimasPersonas)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Nombre,
                        _Settings.DashboardLongitudDeNombres),
                    DisplayMember2 = a.Nif,
                    //IconSource = a.AvatarPath,
                    //Imagen = a.Imagen
                }));

            ProximasCitas = new ObservableCollection<LookupItem>(
                new FakeCitaRepository().GetAll()
                    .OrderBy(c => c.Fecha)
                    .Take(_Settings.DashboardUltimasCitas)
                    .Select(c => new LookupItem
                    {
                        Id = c.Id,
                        DisplayMember1 = c.Fecha.ToString(),
                        DisplayMember2 = c.Sala,
                    }));

            Atenciones = new ObservableCollection<LookupItem>(
                new FakeAtencionRepository().GetAll()
                 .OrderBy(a => a.Fecha)
                 //.Take(_Settings.DashboardUltimasAtenciones)
                 .Select(a => new LookupItem
                 {
                     Id = a.Id,
                     DisplayMember1 = a.Fecha.ToString(),
                     DisplayMember2 = LookupItem.ShortenStringForDisplay(
                         a.Seguimiento, 30),
                     DisplayMember3 = "Nombre de una persona",

                 }));
        }

        public ObservableCollection<LookupItem> Atenciones { get; private set; }
        public ObservableCollection<LookupItem> ProximasCitas { get; private set; }
        public ObservableCollection<LookupItem> Personas { get; private set; }
        
        //private int _MesInicialPersonas;
        //private string[] _Labels;
        //private int _MesInicialAtenciones;
        //private string[] _LabelsTotales;
        //public ChartValues<int> PersonasNuevasPorMes { get; private set; }
        //public ChartValues<int> AtencionesNuevasPorMes { get; private set; }
        //public ChartValues<int> Totales { get; private set; }

        //public string[] PersonasLabels =>
        //    _Labels.Skip(_MesInicialPersonas)
        //        .Take(_Settings.DashboardMesesAMostrarDePersonasNuevas).ToArray();

        //public string[] AtencionesLabels =>
        //    _Labels.Skip(_MesInicialAtenciones)
        //        .Take(_Settings.DashboardMesesAMostrarDeAtencionesNuevas).ToArray();

        //public string[] TotalesLabels => _LabelsTotales;

        //private void InicializarGraficos()
        //{
        //    _Labels = new[] {
        //        "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
        //        "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };
        //    _LabelsTotales = new[] { "Personas", "Citas", "Atenciones" };

        //    _MesInicialPersonas = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDePersonasNuevas + 1;
        //    _MesInicialAtenciones = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeAtencionesNuevas + 1;

        //    PersonasNuevasPorMes = new ChartValues<int>(_PersonaRepository.GetPersonasNuevasPorMes(
        //               _Settings.DashboardMesesAMostrarDePersonasNuevas));

        //    AtencionesNuevasPorMes = new ChartValues<int>(_AtencionRepository.GetAtencionesNuevasPorMes(
        //               _Settings.DashboardMesesAMostrarDeAtencionesNuevas));

        //    Totales = new ChartValues<int>(
        //        new int[] {
        //            _PersonaRepository.CountAll(),
        //            _CitaRepository.CountAll(),
        //            _AtencionRepository.CountAll()
        //        });
        //}
    }
}
