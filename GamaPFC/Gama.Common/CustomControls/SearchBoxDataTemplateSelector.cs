using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gama.Common.CustomControls
{
    public class SearchBoxDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
                return App.Current.Resources["EsperarResultadoDataTemplate"] as DataTemplate;
            else if (item is LookupItem)
                return App.Current.Resources["ResultadoDataTemplate"] as DataTemplate;
            else
                return null;
        }
    }
}
