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
        }

        public ObservableCollection<LookupItem> UltimasPersonas { get; private set; }
    }
}
