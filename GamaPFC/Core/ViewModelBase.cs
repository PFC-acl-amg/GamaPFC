using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Prism;

namespace Core
{
    public class ViewModelBase : BindableBase, INavigationAware, IActiveAware
    {
        string _Title;
        private bool _IsActive;

        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value); }
        }

        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        public event EventHandler IsActiveChanged;

        // Virtual porque las viewmodels concretos lo sobreescribirán 
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            IsActive = false;
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsActive = true;
        }
    }
}
