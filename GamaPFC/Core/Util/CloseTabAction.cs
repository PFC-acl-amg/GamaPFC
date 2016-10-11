using Prism.Regions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Core.Util
{
    public class CloseTabAction : TriggerAction<Button>
    {
        private bool _forzarCerrado = false;

        public void Cerrar(TabControl tabControl)
        {
            IRegion region = RegionManager.GetObservableRegion(tabControl).Value;
            if (region == null)
                return;

            _forzarCerrado = true;
            RemoveItemFromRegion(tabControl.SelectedContent, region);
            _forzarCerrado = false;
        }

        protected override void Invoke(object parameter)
        {
            var args = parameter as RoutedEventArgs;
            if (args == null)
                return;

            var tabItem = FindParent<TabItem>(args.OriginalSource as DependencyObject);
            if (tabItem == null)
                return;

            var tabControl = FindParent<TabControl>(tabItem);
            if (tabControl == null)
                return;

            IRegion region = RegionManager.GetObservableRegion(tabControl).Value;
            if (region == null)
                return;

            RemoveItemFromRegion(tabItem.Content, region);
        }

        private void RemoveItemFromRegion(object item, IRegion region)
        {
            var navigationContext = new NavigationContext(region.NavigationService, null);
            if (_forzarCerrado || CanRemove(item, navigationContext))
            {
                region.Remove(item);
            }
        }

        private bool CanRemove(object item, NavigationContext navigationContext)
        {
            bool canRemove = true;

            var frameworkElement = item as FrameworkElement;
            if (frameworkElement != null)
            {
                var confirmRequestDataContext = frameworkElement.DataContext as IConfirmNavigationRequest;

                if (confirmRequestDataContext != null)
                {
                    confirmRequestDataContext.ConfirmNavigationRequest(navigationContext,
                        result => {
                            canRemove = result;
                        });
                }
            }

            return canRemove;
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
