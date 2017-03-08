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

namespace Gama.Atenciones.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ucDateTimeUpDown.xaml
    /// </summary>
    public partial class TimeControl : UserControl
    {
        public TimeControl()
        {
            InitializeComponent();
        }

        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeControl),
                new UIPropertyMetadata(DateTime.Now.TimeOfDay, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeControl control = obj as TimeControl;
            control.Hora = ((TimeSpan)e.NewValue).Hours;
            control.Minutos = ((TimeSpan)e.NewValue).Minutes;
        }

        public int Hora
        {
            get { return (int)GetValue(HoraProperty); }
            set { SetValue(HoraProperty, value); }
        }
        public static readonly DependencyProperty HoraProperty =
            DependencyProperty.Register("Hora", typeof(int), typeof(TimeControl),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged))
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        public int Minutos
        {
            get { return (int)GetValue(MinutosProperty); }
            set { SetValue(MinutosProperty, value); }
        }
        public static readonly DependencyProperty MinutosProperty =
        DependencyProperty.Register("Minutos", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));       

        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeControl control = obj as TimeControl;
            if (control.Hora < 0)
                control.Hora = 0;
            if (control.Hora > 23)
                control.Hora = 23;
            if (control.Minutos > 59)
                control.Minutos = 59;
            if (control.Minutos < 0)
                control.Minutos = 0;
            control.Value = new TimeSpan(control.Hora, control.Minutos, 0);
        }

        private void Down(object sender, KeyEventArgs args)
        {
            switch (((Grid)sender).Name)
            {
                case "min":
                    if (args.Key == Key.Up)
                        this.Minutos++;
                    if (args.Key == Key.Down)
                        this.Minutos--;
                    break;

                case "hour":
                    if (args.Key == Key.Up)
                        this.Hora++;
                    if (args.Key == Key.Down)
                        this.Hora--;
                    break;
            }
        }

    }
}