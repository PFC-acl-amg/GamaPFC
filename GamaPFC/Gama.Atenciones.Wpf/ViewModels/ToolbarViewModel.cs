﻿using Core;
using Gama.Atenciones.Wpf.Views;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        public ToolbarViewModel()
        {
            NuevaPersonaCommand = new DelegateCommand(OnNuevaPersonaCommandExecute);
            ExportarCommand = new DelegateCommand(OnExportarCommandExecute);
        }

        public ICommand NuevaPersonaCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }

        private void OnNuevaPersonaCommandExecute()
        {
            var o = new NuevaPersonaView();
            o.ShowDialog();
        }

        private void OnExportarCommandExecute()
        {
            //throw new NotImplementedException();
        }
    }
}