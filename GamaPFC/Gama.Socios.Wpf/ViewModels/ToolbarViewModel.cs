using Core;
using Gama.Socios.Wpf.Views;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        public ToolbarViewModel()
        {
            NuevoSocioCommand = new DelegateCommand(OnNuevoSocioCommandExecute);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);
        }

        public ICommand NuevoSocioCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }

        private void OnNuevoSocioCommandExecute()
        {
            var o = new NuevoSocioView();
            o.ShowDialog();
        }

        private void OnExportarCommandExecute()
        {
            //throw new NotImplementedException();
        }
    }
}
