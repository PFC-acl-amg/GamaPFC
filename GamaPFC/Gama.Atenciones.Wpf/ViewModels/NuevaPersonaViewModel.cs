﻿using Core;
using Gama.Atenciones.Wpf.Converters;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class NuevaPersonaViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private PersonaViewModel _PersonaVM;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public NuevaPersonaViewModel(
            IPersonaRepository personaRepository,
            IEventAggregator eventAggregator,
            PersonaViewModel personaViewModel,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _PersonaVM = personaViewModel;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;

            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);

            Persona.PropertyChanged += Persona_PropertyChanged;
            Persona.Imagen = BinaryImageConverter.GetBitmapImageFromUriSource(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png"));
        }

        private void Persona_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        public PersonaViewModel PersonaVM
        {
            get { return _PersonaVM; }
        }

        public PersonaWrapper Persona
        {
            get { return _PersonaVM.Persona; }
        }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        private void OnAceptarCommand_Execute()
        {
            _PersonaRepository.Create(Persona.Model);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return Persona.IsValid;
        }

        private void OnCancelarCommand_Execute()
        {
            Cerrar = true;
        }
    }
}
