using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gama.Common.CustomControls
{
    public class CircleIconButton : Button
    {
        public Visual Icon
        {
            get { return (Visual)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Visual), typeof(CircleIconButton), new PropertyMetadata(default(Visual)));

    }
}
