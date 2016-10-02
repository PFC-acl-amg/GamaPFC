using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
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
        private AtencionesSettings _Settings;

        public DashboardViewModelDTD()
        {
            _Settings = new AtencionesSettings();

            UltimasPersonas = new ObservableCollection<LookupItem>(
                new FakePersonaRepository().GetAll()
                    .OrderBy(a => a.UpdatedAt)
                    .Take(_Settings.DashboardUltimasPersonas)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Nombre,
                        _Settings.DashboardLongitudDeNombres),
                }));

            UltimasCitas = new ObservableCollection<LookupItem>(
                new FakeCitaRepository().GetAll()
                    .OrderBy(c => c.Inicio)
                    .Take(_Settings.DashboardUltimasCitas)
                    .Select(c => new LookupItem
                    {
                        Id = c.Id,
                        DisplayMember1 = c.Inicio.ToString(),
                        DisplayMember2 = c.Sala
                    }));

            UltimasAtenciones = new ObservableCollection<LookupItem>(
                new FakeAtencionRepository().GetAll()
                 .OrderBy(a => a.Fecha)
                 .Take(_Settings.DashboardUltimasAtenciones)
                 .Select(a => new LookupItem
                 {
                     Id = a.Id,
                     DisplayMember1 = a.Fecha.ToString(),
                     DisplayMember2 = LookupItem.ShortenStringForDisplay(
                         a.Seguimiento, _Settings.DashboardLongitudDeSeguimientos)
                 }));
        }

        public ObservableCollection<LookupItem> UltimasAtenciones { get; private set; }
        public ObservableCollection<LookupItem> UltimasCitas { get; private set; }
        public ObservableCollection<LookupItem> UltimasPersonas { get; private set; }
    }
}
