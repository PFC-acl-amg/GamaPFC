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
        string _title;

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }

            set
            {
                SetProperty(ref _IsActive, value);
            }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event EventHandler IsActiveChanged;

        // Virtual porque las viewmodels concretos lo sobreescribirán 
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
