﻿using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace Gama.Socios.Wpf
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        public Shell()
        {
            InitializeComponent();

            Closed += Shell_Closed;
        }

        private void Shell_Closed(object sender, EventArgs e)
        {
            var vm = (ShellViewModel)this.DataContext;
            vm.OnCloseApplication();
        }

        private void WebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.colectivogama.com");
        }

        private void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/colectivogama/");
        }

        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/GamaLgtb");
        }
    }
}
