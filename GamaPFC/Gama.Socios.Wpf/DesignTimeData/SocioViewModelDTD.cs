using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.DesignTimeData
{
    public class SocioViewModelDTD
    {
        public Socio Socio { get; set; }

        public SocioViewModelDTD()
        {
            Socio.AvatarPath = AvatarPath;
        }
       public string AvatarPath { get; set; } = @"C:\Users\acl450\Source\Repos\GamaPFC\GamaPFC\Gama.Socios.Wpf\Resources\Images\icono_nuevo_socio.png";
    }
}
