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
    public partial class SplashScreenView : Window, INotifyPropertyChanged
    {
        private int _DotCounter = 0;
        private const int MAX_NUMBER_OF_DOTS = 3;
        private const double TIMER_INTERVAL = 250;
        private DispatcherTimer _DotWritterTimer;
        private Label _CurrentLabel;
        private string _CurrentText;
        private List<Label> _Labels;
        private List<string> _LabelsBaseTexts;

        public SplashScreenView()
        {
            InitializeComponent();

            ProductName = "Insert your product name here...";

            _Labels = new List<Label> {
                _Label1, _Label2, _Label3, _Label4
            };

            _LabelsBaseTexts = new List<string> {
                "Inicializando directorios",
                "Cargando preferencias",
                "Configurando servicios",
                "Cargando",
            };

            _CurrentLabel = _Label1;
            _CurrentText = _LabelsBaseTexts[0];

            _DotWritterTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_INTERVAL),
            };

            _DotWritterTimer.Tick += _OnDotWritterTimerTickEvent;

            DataContext = this;

            _DotWritterTimer.Start();
        }


        private string _ProductName;
        public string ProductName
        {
            get { return _ProductName; }
            set
            {
                _ProductName = value;
                OnPropertyChanged();
            }
        }

        private void _OnDotWritterTimerTickEvent(object sender, EventArgs e)
        {
            if (_DotCounter > MAX_NUMBER_OF_DOTS)
                RestartCurrentLabel();

            _CurrentLabel.Content += ".";
            OnPropertyChanged(nameof(_CurrentLabel.Content));
            _DotCounter++;
            _DotWritterTimer.Start();
        }

        public void Next()
        {
            if (_Labels.Count > 0)
            {
                RestartCurrentLabel();

                _CurrentLabel = _Labels.First();
                _CurrentText = _LabelsBaseTexts.First();

                _CurrentLabel.Dispatcher.Invoke(delegate
                {
                    _CurrentLabel.Visibility = Visibility.Visible;
                    _CurrentLabel.Content = _CurrentText;
                    OnPropertyChanged(nameof(_CurrentLabel.Content));
                });

                Dispatcher.Invoke(delegate
                {
                    _Labels.Remove(_CurrentLabel);
                    _LabelsBaseTexts.Remove(_CurrentText);
                });
            }
        }

        private void RestartCurrentLabel()
        {
            _DotCounter = 0;

            _CurrentLabel.Dispatcher.Invoke(delegate
            {
                _CurrentLabel.Content = _CurrentText;
                OnPropertyChanged(nameof(_CurrentLabel.Content));
            });
        }

        #region PropertyChanged Event Handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}