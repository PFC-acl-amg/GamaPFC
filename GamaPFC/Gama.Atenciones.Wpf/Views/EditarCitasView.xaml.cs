using Gama.Common.CustomControls;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Gama.Atenciones.Wpf.Views
{
    public static class DesignTimeSupport
    {
        public static void LoadCommonConvertersForBlend(this ResourceDictionary resourceDictionary,
            DependencyObject ok)
        {
            if (resourceDictionary == null || !DesignerProperties.GetIsInDesignMode(ok)) return;

            var convertersXamlUri = new Uri("pack://application:,,,/Gama.Common;component/Resources/Templates/CircleIconButton.xaml");
            var streamInfo = Application.GetResourceStream(convertersXamlUri);
            using (var reader = new StreamReader(streamInfo.Stream))
            {
                var converters = (ResourceDictionary)XamlReader.Load(GenerateStreamFromString( reader.ReadToEnd()));
                resourceDictionary.MergedDictionaries.Add(converters);
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

    /// <summary>
    /// Interaction logic for EditarCitasView.xaml
    /// </summary>
    public partial class EditarCitasView : UserControl
    {
        private bool _ToggleCalendarStyleCheckboxIsChecked = false;
        public EditarCitasView()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                runtimeConstructor();
            }
            Resources.LoadCommonConvertersForBlend(this);
        }

        private void runtimeConstructor()
        {
            var circleIconButtonStyle = new ResourceDictionary();
            circleIconButtonStyle.Source = new Uri("pack://application:,,,/Gama.Common;component/Resources/Templates/CircleIconButton.xaml");
            CircleIconButton x = new CircleIconButton();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    _Calendario.Style = FindResource("_ListadoCalendarStyle") as Style;
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    _Calendario.Style = FindResource("_EditarCitasCalendarStyle") as Style;
        //}

        private void CircleIconButton_Click(object sender, RoutedEventArgs e)
        {
            _ToggleCalendarStyleCheckboxIsChecked = !_ToggleCalendarStyleCheckboxIsChecked;
            _ToggleCalendarStyleCheckbox.IsChecked = !_ToggleCalendarStyleCheckbox.IsChecked;
        }
    }
}
