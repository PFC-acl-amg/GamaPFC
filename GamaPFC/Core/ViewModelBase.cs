using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Core
{
    public class ViewModelBase : BindableBase, INavigationAware, IViewModelBase, IDisposable
    {
        private readonly ISession _Session;
        string _title;

        public ViewModelBase()
        {
            //this._Session = session;
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public void Dispose()
        {
            //_Session.Dispose();
        }

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
