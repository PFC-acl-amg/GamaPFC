﻿using Gama.Common.CustomControls;
using MahApps.Metro.Controls;
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

namespace Gama.Atenciones.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PersonasContentView.xaml
    /// </summary>
    public partial class PersonasContentView : UserControl
    {
        public PersonasContentView()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                runtimeConstructor();
            }

            Loaded += ActividadesContentView_Loaded;
        }

        private void ActividadesContentView_Loaded(object sender, RoutedEventArgs e)
        {
            ((MetroTabItem)_TabControl.Items[0]).IsSelected = true;
        }

        private void runtimeConstructor()
        {
            CircleIconButton x = new CircleIconButton();
        }
    }
}
