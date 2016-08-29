using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Gama.Cooperacion.Wpf.Actions
{
    public class CloseWindowAction : TriggerAction<Button>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as RoutedEventArgs;
            if (args == null)
                return;

            var window = FindParent<Window>(args.OriginalSource as DependencyObject);
            if (window == null)
                return;

            window.Close();
        }

        private static T FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            DependencyObject parentObject =
                VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            var parent = parentObject as T;
            if (parent != null)
                return parent;

            return FindParent<T>(parentObject);
        }
    }
}
