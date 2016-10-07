using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Gama.Common.Views;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class EditarPersonaViewModel : ViewModelBase, IConfirmNavigationRequest
    {
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PersonaViewModel _PersonaVM;

        public EditarPersonaViewModel(
            IEventAggregator eventAggregator,
            IPersonaRepository personaRepository,
            PersonaViewModel personaVM,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _PersonaVM = personaVM;

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_PersonaVM.EdicionHabilitada);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => _PersonaVM.EdicionHabilitada 
                   && Persona.IsChanged
                   && Persona.IsValid);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _PersonaVM.EdicionHabilitada);

            _PersonaVM.PropertyChanged += _PersonaVM_PropertyChanged;
        }

        public PersonaViewModel PersonaVM
        {
            get { return _PersonaVM; }
        }

        public PersonaWrapper Persona
        {
            get { return _PersonaVM.Persona; }
        }

        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }

        private void OnActualizarCommand()
        {
            Persona.UpdatedAt = DateTime.Now;
            _PersonaRepository.Update(Persona.Model);
            _PersonaVM.Persona.AcceptChanges();
            _PersonaVM.EdicionHabilitada = false;
            RefrescarTitulo(Persona.Nombre);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Publish(Persona.Id);
        }

        private void OnHabilitarEdicionCommand()
        {
            _PersonaVM.EdicionHabilitada = true;
        }

        private void OnCancelarEdicionCommand()
        {
            Persona.RejectChanges();
            _PersonaVM.EdicionHabilitada = false;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var id = (int)navigationContext.Parameters["Id"];

            if (Persona.Id == id)
                return true;

            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (Persona.Nombre != null)
                return;

            var persona = new PersonaWrapper(
                _PersonaRepository.GetById((int)navigationContext.Parameters["Id"]));

            _PersonaVM.Load(persona);
            RefrescarTitulo(persona.Nombre);
        }

        private void RefrescarTitulo(string nombre)
        {
            if (nombre.Length > 20)
            {
                Title = nombre.Substring(0, 20) + "...";
            }
            else
            {
                Title = nombre;
            }
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        private void _PersonaVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_PersonaVM.EdicionHabilitada))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(Persona))
            {
                Persona.PropertyChanged += (s, ea) => {
                    if (ea.PropertyName == nameof(Persona.IsChanged)
                        || ea.PropertyName == nameof(Persona.IsValid))
                    {
                        InvalidateCommands();
                    }
                };
            }
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext,
            Action<bool> continuationCallback)
        {
            if (Persona.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                continuationCallback.Invoke(o.EstaConfirmado);
            }
        }
    }
}
