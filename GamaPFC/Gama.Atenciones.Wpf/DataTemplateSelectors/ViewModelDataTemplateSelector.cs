using Gama.Atenciones.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gama.Atenciones.Wpf.DataTemplateSelectors
{
    public class ViewModelDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is object)
            {
                DashboardViewModel dashboardViewModel = item as DashboardViewModel;

                if (dashboardViewModel != null)
                    return
                        element.FindResource("_DashboardViewDataTemplate") as DataTemplate;
                else // Es PersonasContentViewModel
                    return
                        element.FindResource("_PersonasContentViewDataTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
