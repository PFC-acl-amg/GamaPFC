using Core;
using Gama.Cooperacion.Wpf.Views;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        public ToolbarViewModel()
        {
            NuevaActividadCommand = new DelegateCommand(OnNuevaActividad);
        }

        public ICommand NuevaActividadCommand { get; private set; }

        private void OnNuevaActividad()
        {
            var o = new NuevaActividadView();
            o.Show();
        }
    }
}
