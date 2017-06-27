using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;

namespace Gama.Atenciones.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PreloaderView.xaml
    /// </summary>
    public partial class PreloaderView : MetroWindow
    {
        private List<Label> _Labels;
        private DispatcherTimer _Timer;

        public PreloaderView(ObservableCollection<string> coleccion)
        {
            InitializeComponent();
            //_Timer = new DispatcherTimer();
            //_Timer.Tick += _timer_Tick;
            //_Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _Labels = new List<Label>();
            _Labels.AddRange(new[] {_Label1, _Label2, _Label3, _Label4, _Label5 });

            coleccion.CollectionChanged += Coleccion_CollectionChanged;

        }

        private void Coleccion_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //_Timer.Start();
            Next();
        }


        private void _timer_Tick(object sender, EventArgs e)
        {
            Next();
            _Timer.Start();
        }

        public void Next()
        {
            if (_Labels.Count == 0)
            {
                //_Timer.Stop();
                //_Timer = null;
                Dispatcher.Invoke((Action)delegate { Close(); });
            }
            else
            {
                var label = _Labels.First();
                label.Dispatcher.Invoke((Action)delegate { label.Visibility = Visibility.Visible; });
                //_Labels.First().Visibility = Visibility.Visible;
                Dispatcher.Invoke((Action)delegate { _Labels.Remove(_Labels.First()); });

                //var label = _Labels.First();
                //label.Visibility = Visibility.Visible;
                //_Labels.Remove(_Labels.First());
            }
        }
    }
}
