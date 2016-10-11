using Core;
using System.Windows.Media;

namespace Gama.Bootstrapper
{
    public class ShellViewModel : ViewModelBase
    {
        private ImageSource _IconSource;

        public ShellViewModel()
        {
            Title = "Módulo no cargado";
        }

        public ImageSource IconSource
        {
            get { return _IconSource; }
            set { SetProperty(ref _IconSource, value); }
        }
    }
}