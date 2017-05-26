using Gama.Cooperacion.Wpf.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Gama.Cooperacion.Wpf.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        Brush _SelectedBrush = Brushes.Black;
        //Brush _AzulPersonal  = (Brush) System.Windows.Media.BrushConverter().
        BrushConverter _ConverterColor;
        
        //var converter = new System.Windows.Media.BrushConverter();
        //var brush = (Brush)converter.ConvertFromString("#0079F7");
        public DashboardView()
        {
            InitializeComponent();
            
        }
        public Brush AzulPersonal
        {
            get
            {
                _ConverterColor = new BrushConverter();
                return (Brush)_ConverterColor.ConvertFromString("#0079F7");
            }
            set { }
        }
        private void Label_ActividadesCooperante(object sender, MouseButtonEventArgs e)
        {
            // Seleccionado Label Actividades del Cooperante seleccinado
            // Se ponen el reto de etiquetas a su forma original => todo azul con letras blancas

            // 1º => Forma original de la etiqueta DatosDNI => _DatosDNILabel
            // Se modifica la etiqueta pulsada => _DatosDNI
            var labelDatosDNI = _DatosDNILabel;
            labelDatosDNI.Foreground = Brushes.White;
            labelDatosDNI.Background = AzulPersonal;
            labelDatosDNI.BorderBrush = Brushes.Black;
            labelDatosDNI.BorderThickness = new Thickness(0, 0, 3, 0);

            // 2º => Forma original dela etiqueta Contacto => _ContactoLabel
            var labelContacto = _ContactoLabel;
            labelContacto.Foreground = Brushes.White;
            labelContacto.Background = AzulPersonal;
            labelContacto.BorderBrush = Brushes.Black;
            labelContacto.BorderThickness = new Thickness(0, 0, 3, 0);

            // 3º => Forma original de la etiqueta Direccion => _DireccionLabel
            var labelDireccion = _DireccionLabel;
            labelDireccion.Foreground = Brushes.White;
            labelDireccion.Background = AzulPersonal;
            labelDireccion.BorderBrush = Brushes.Black;
            labelDireccion.BorderThickness = new Thickness(0, 0, 3, 0);

            // Se modifica la etiqueta pulsada => _DatosDNILabel
            var labelActividades = _ActividadesLabel;
            labelActividades.Foreground = Brushes.Black;
            labelActividades.Background = Brushes.White;
            labelActividades.BorderBrush = Brushes.Black;
            labelActividades.BorderThickness = new Thickness(3, 3, 0, 3);

            // Se pone a visible la zona de datos seleccinada y a hidden el resto
            var vm = this.DataContext as DashboardViewModel;
            vm.VisibleListaActividades = false;
            vm.VisibleListaActividadesCooperante = true;
            vm.VisibleContacto = false;
            vm.VisibleDireccion = false;
            vm.VisibleDatosDNI = true;

        }
        private void Label_DatosDNI(object sender, MouseButtonEventArgs e)
        {
            // Seleccionado Label DatosDNI
            // Se ponen el reto de etiquetas a su forma original => todo azul con letras blancas

            // 1º => Forma original de la etiqueta Actividades => _ActividadesLabel
            var labelActividades = _ActividadesLabel;
            labelActividades.Foreground = Brushes.White;
            labelActividades.Background = AzulPersonal;
            labelActividades.BorderBrush = Brushes.Black;
            labelActividades.BorderThickness = new Thickness(0, 0, 3, 0);

            // 2º => Forma original dela etiqueta Contacto => _ContactoLabel
            var labelContacto = _ContactoLabel;
            labelContacto.Foreground = Brushes.White;
            labelContacto.Background = AzulPersonal;
            labelContacto.BorderBrush = Brushes.Black;
            labelContacto.BorderThickness = new Thickness(0, 0, 3, 0);
            // 3º => Forma original de la etiqueta Direccion => _DireccionLabel
            var labelDireccion = _DireccionLabel;
            labelDireccion.Foreground = Brushes.White;
            labelDireccion.Background = AzulPersonal;
            labelDireccion.BorderBrush = Brushes.Black;
            labelDireccion.BorderThickness = new Thickness(0, 0, 3, 0);
            // Se modifica la etiqueta pulsada => _DatosDNI
            var labelDatosDNI = _DatosDNILabel;
            labelDatosDNI.Foreground = Brushes.Black;
            labelDatosDNI.Background = Brushes.White;
            labelDatosDNI.BorderBrush = Brushes.Black;
            labelDatosDNI.BorderThickness = new Thickness(3, 3, 0, 3);

            //var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#0079F7");

            //< Setter Property = "Foreground" Value = "Black" />

            //   < Setter Property = "Background" Value = "White" />

            //      < Setter Property = "BorderThickness" Value = "1,1,0,1" />

            //         < Setter Property = "BorderBrush" Value = "Black" />

            //            < Setter Property = "Cursor" Value = "Hand    
            //< Setter Property = "Foreground" Value = "White" />

            //                                       < Setter Property = "Background" Value = "#0079F7" />

            //                                          < Setter Property = "BorderBrush" Value = "#0079F7" />

            //                                             < Setter Property = "BorderThickness" Value = "0,0,0,0" />
            //var labelActividades = _ActividadesLabel;
            //labelActividades.Foreground = Brushes.MediumBlue;

            //labelActividades.BorderBrush = Brushes.Blue;
            //labelActividades.BorderThickness = new Thickness(0, 0, 0, 0);
            //var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#0079F7");
            //labelActividades.Background = AzulPersonal;



            // Se pone a visible la zona de datos seleccinada y a hidden el resto
            var vm = this.DataContext as DashboardViewModel;
            //vm.VisibleListaActividades = false; // Comprobar si esto hace falta o es porqueria arrastrada de otras pruebas
            vm.VisibleListaActividadesCooperante = false;
            vm.VisibleContacto = false;
            vm.VisibleDireccion = false;
            vm.VisibleDatosDNI = true;
            
        }
        private void Label_Contacto(object sender, MouseButtonEventArgs e)
        {
            // Seleccionado Label Contacto
            // Se ponen el reto de etiquetas a su forma original => todo azul con letras blancas

            // 1º => Forma original de la etiqueta Actividades => _ActividadesLabel
            var labelActividades = _ActividadesLabel;
            labelActividades.Foreground = Brushes.White;
            labelActividades.Background = AzulPersonal;
            labelActividades.BorderBrush = Brushes.Black;
            labelActividades.BorderThickness = new Thickness(0, 0, 3, 0);

            // 2º => Forma original dela etiqueta DatosDNI => _DatosDNILabel
            var labelDatosDNI = _DatosDNILabel;
            labelDatosDNI.Foreground = Brushes.White;
            labelDatosDNI.Background = AzulPersonal;
            labelDatosDNI.BorderBrush = Brushes.Black;
            labelDatosDNI.BorderThickness = new Thickness(0, 0, 3, 0);
      
            // 3º => Forma original de la etiqueta Direccion => _DireccionLabel
            var labelDireccion = _DireccionLabel;
            labelDireccion.Foreground = Brushes.White;
            labelDireccion.Background = AzulPersonal;
            labelDireccion.BorderBrush = Brushes.Black;
            labelDireccion.BorderThickness = new Thickness(0, 0, 3, 0);

            // Se modifica la etiqueta pulsada => _ContactoLabel
            var labelContacto = _ContactoLabel;
            labelContacto.Foreground = Brushes.Black;
            labelContacto.Background = Brushes.White;
            labelContacto.BorderBrush = Brushes.Black;
            labelContacto.BorderThickness = new Thickness(3, 3, 0, 3);
            

            //var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#0079F7");

            //< Setter Property = "Foreground" Value = "Black" />

            //   < Setter Property = "Background" Value = "White" />

            //      < Setter Property = "BorderThickness" Value = "1,1,0,1" />

            //         < Setter Property = "BorderBrush" Value = "Black" />

            //            < Setter Property = "Cursor" Value = "Hand    
            //< Setter Property = "Foreground" Value = "White" />

            //                                       < Setter Property = "Background" Value = "#0079F7" />

            //                                          < Setter Property = "BorderBrush" Value = "#0079F7" />

            //                                             < Setter Property = "BorderThickness" Value = "0,0,0,0" />
            //var labelActividades = _ActividadesLabel;
            //labelActividades.Foreground = Brushes.MediumBlue;

            //labelActividades.BorderBrush = Brushes.Blue;
            //labelActividades.BorderThickness = new Thickness(0, 0, 0, 0);
            //var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#0079F7");
            //labelActividades.Background = AzulPersonal;



            // Se pone a visible la zona de datos seleccinada y a hidden el resto
            var vm = this.DataContext as DashboardViewModel;
            //vm.VisibleListaActividades = false; // Comprobar si esto hace falta o es porqueria arrastrada de otras pruebas
            vm.VisibleListaActividadesCooperante = false;
            vm.VisibleContacto = true;
            vm.VisibleDireccion = false;
            vm.VisibleDatosDNI = false;

        }
        private void Label_Direccion(object sender, MouseButtonEventArgs e)
        {
            // Seleccionado Label Contacto
            // Se ponen el reto de etiquetas a su forma original => todo azul con letras blancas

            // 1º => Forma original de la etiqueta Actividades => _ActividadesLabel
            var labelActividades = _ActividadesLabel;
            labelActividades.Foreground = Brushes.White;
            labelActividades.Background = AzulPersonal;
            labelActividades.BorderBrush = Brushes.Black;
            labelActividades.BorderThickness = new Thickness(0, 0, 3, 0);

            // 2º => Forma original dela etiqueta DatosDNI => _DatosDNILabel
            var labelDatosDNI = _DatosDNILabel;
            labelDatosDNI.Foreground = Brushes.White;
            labelDatosDNI.Background = AzulPersonal;
            labelDatosDNI.BorderBrush = Brushes.Black;
            labelDatosDNI.BorderThickness = new Thickness(0, 0, 3, 0);

            // 3º => Forma original de la etiqueta Direccion => _ContactoLabel
            var labelContacto = _ContactoLabel;
            labelContacto.Foreground = Brushes.White;
            labelContacto.Background = AzulPersonal;
            labelContacto.BorderBrush = Brushes.Black;
            labelContacto.BorderThickness = new Thickness(0, 0, 3, 0);

            // Se modifica la etiqueta pulsada => _DireccionLabel
            var labelDireccion = _DireccionLabel;
            labelDireccion.Foreground = Brushes.Black;
            labelDireccion.Background = Brushes.White;
            labelDireccion.BorderBrush = Brushes.Black;
            labelDireccion.BorderThickness = new Thickness(3, 3, 0, 3);

            //var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#0079F7");

            //< Setter Property = "Foreground" Value = "Black" />

            //   < Setter Property = "Background" Value = "White" />

            //      < Setter Property = "BorderThickness" Value = "1,1,0,1" />

            //         < Setter Property = "BorderBrush" Value = "Black" />

            //            < Setter Property = "Cursor" Value = "Hand    
            //< Setter Property = "Foreground" Value = "White" />

            //                                       < Setter Property = "Background" Value = "#0079F7" />

            //                                          < Setter Property = "BorderBrush" Value = "#0079F7" />

            //                                             < Setter Property = "BorderThickness" Value = "0,0,0,0" />
            //var labelActividades = _ActividadesLabel;
            //labelActividades.Foreground = Brushes.MediumBlue;

            //labelActividades.BorderBrush = Brushes.Blue;
            //labelActividades.BorderThickness = new Thickness(0, 0, 0, 0);
            //var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#0079F7");
            //labelActividades.Background = AzulPersonal;



            // Se pone a visible la zona de datos seleccinada y a hidden el resto
            var vm = this.DataContext as DashboardViewModel;
            //vm.VisibleListaActividades = false; // Comprobar si esto hace falta o es porqueria arrastrada de otras pruebas
            vm.VisibleListaActividadesCooperante = false;
            vm.VisibleContacto = false;
            vm.VisibleDireccion = true;
            vm.VisibleDatosDNI = false;

        }
        private void Label_EscogerCooperante (object sender, MouseButtonEventArgs e)
        {
            var vm = this.DataContext as DashboardViewModel;
            vm.VisibleImagenSeleccionCooperante = true;
            vm.VisibleCooperanteSeleccionado = false;
            vm.VisibleDatosCooperanteSeleccionado = false;
            vm.VisibleDatosCooperanteSeleccionado = false;
            vm.VisibleListaTodosCooperantes = true;
            vm.VisibleListaCooperantes = true;
        }
        private void Label_SeleccionarActividades(object sender, MouseButtonEventArgs e)
        {
            var labelActividades = _SeleccionarActividadesLabel;
            var vm = this.DataContext as DashboardViewModel;
            labelActividades.Foreground = Brushes.Yellow;

            vm.ListaDeActividadesCommand.Execute(null);
            
        }

    }
}
