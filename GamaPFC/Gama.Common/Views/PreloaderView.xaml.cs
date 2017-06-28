using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Gama.Common.Views
{
    /// <summary>
    /// Interaction logic for PreloaderView.xaml
    /// </summary>
    public partial class PreloaderView : MetroWindow, INotifyPropertyChanged
    {
        private List<Label> _Labels;
        private int _ContadorDePuntosSuspensivos;
        private DispatcherTimer _Timer;

        public PreloaderView()
        {
            InitializeComponent();

            _ContadorDePuntosSuspensivos = 3;

            _Labels = new List<Label>();
            _Labels.AddRange(new[] { _Label1, _Label2, _Label3, _Label4, _Label5 });

            _Timer = new DispatcherTimer();
            _Timer.Tick += _Timer_Tick;
            _Timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _Timer.Start();
            
            DataContext = this;
        }

        private string _LabelDeCargarBase = "Conectando con el servidor";
        private string _LabelDeCargar = "Conectando con el servidor...";
        public string LabelDeCargar
        {
            get { return _LabelDeCargar; }
            set
            {
                _LabelDeCargar = value;
                OnPropertyChanged(nameof(LabelDeCargar));
            }
        }

        private string _Titulo;
        public string Titulo
        {
            get { return _Titulo; }
            set
            {
                _Titulo = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }

        private void _Timer_Tick(object sender, EventArgs e)
        {
            if (_ContadorDePuntosSuspensivos > 2)
            {
                _ContadorDePuntosSuspensivos = 0;
                LabelDeCargar = _LabelDeCargarBase;
            }

            LabelDeCargar += ".";

            _ContadorDePuntosSuspensivos++;
            _Timer.Start();
        }

        public void Avanzar()
        {
            if (_Labels.Count == 0)
                Dispatcher.Invoke((Action)delegate { Close(); });
            else
            {
                var label = _Labels.First();
                label.Dispatcher.Invoke((Action)delegate { label.Visibility = Visibility.Visible; });
                Dispatcher.Invoke((Action)delegate { _Labels.Remove(_Labels.First()); });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
