using Core;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class TareasDeActividadViewModel : ViewModelBase
    {
        //private bool _isVisible=true;
        private bool _isVisible = false;
        private Visibility _isVisible2 = Visibility.Hidden;

        public TareasDeActividadViewModel()
        {
            CrearForoCommand = new DelegateCommand(OnCrearForoCommand, OnCrearForoCommand_CanExecute);
        }

        private bool OnCrearForoCommand_CanExecute()
        {
            return true;
        }

        public ICommand CrearForoCommand { get; private set; }
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }

            }
        }


        //public Visibility IsVisible
        //{
        //    get
        //    {
        //        return _isVisible;
        //    }

        //    set
        //    {
        //        _isVisible = value;

        //        OnPropertyChanged("IsVisible");
        //    }
        //}
        //public Visibility IsVisible2
        //{
        //    get
        //    {
        //        return _isVisible2;
        //    }

        //    set
        //    {
        //        _isVisible2 = value;

        //        OnPropertyChanged("IsVisible2");
        //    }
        //}
        private void OnCrearForoCommand()
        {
            IsVisible = true;
        }

        // probando otra cosa para la visibilidad.
        //public class RelayCommand : ICommand
        //{
        //    public event EventHandler CanExecuteChanged
        //    {
        //        add { CommandManager.RequerySuggested += value; }
        //        remove { CommandManager.RequerySuggested -= value; }
        //    }
        //    private Action methodToExecute;
        //    private Func<bool> canExecuteEvaluator;
        //    public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        //    {
        //        this.methodToExecute = methodToExecute;
        //        this.canExecuteEvaluator = canExecuteEvaluator;
        //    }
        //    public RelayCommand(Action methodToExecute)
        //        : this(methodToExecute, null)
        //    {
        //    }
        //    public bool CanExecute(object parameter)
        //    {
        //        if (this.canExecuteEvaluator == null)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            bool result = this.canExecuteEvaluator.Invoke();
        //            return result;
        //        }
        //    }
        //    public void Execute(object parameter)
        //    {
        //        this.methodToExecute.Invoke();
        //    }
        //}
        //public RelayCommand MakeVisibleCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() => IsVisible = Visibility.Collapsed);
        //    }
        //}
    }
}
