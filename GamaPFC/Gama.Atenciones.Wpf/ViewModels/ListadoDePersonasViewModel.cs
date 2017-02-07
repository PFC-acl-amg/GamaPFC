﻿using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Common.CustomControls;
using NHibernate;
using Prism;
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
    public class ListadoDePersonasViewModel : ViewModelBase, IActiveAware
    {
        private IEventAggregator _EventAggregator;
        private IPersonaRepository _PersonaRepository;
        private List<LookupItem> _Personas;
        private IAtencionesSettings _Settings;

        public ListadoDePersonasViewModel(
            IEventAggregator eventAggregator,
            IPersonaRepository personaRepository,
            IAtencionesSettings settings,
            ISession session)
        {
            Title = "Todas";

            _EventAggregator = eventAggregator;
            _PersonaRepository = personaRepository;
            _PersonaRepository.Session = session;
            _Settings = settings;

            _Personas = _PersonaRepository.GetAll()
                .Select(p => new LookupItem
                {
                    Id = p.Id,
                    DisplayMember1 = LookupItem.ShortenStringForDisplay(p.Nombre, 25),
                    DisplayMember2 = p.Nif,
                    IconSource = p.AvatarPath
                }).ToList();

            Personas = new PaginatedCollectionView(_Personas,
                _Settings.ListadoDePersonasItemsPerPage);

            _EventAggregator.GetEvent<PersonaCreadaEvent>().Subscribe(OnNuevaPersonaEvent);
            _EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(OnPersonaActualizadaEvent);
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(OnPersonaEliminadaEvent);

            PaginaAnteriorCommand = new DelegateCommand(() => Personas.MoveToPreviousPage());
            PaginaSiguienteCommand = new DelegateCommand(() => Personas.MoveToNextPage());
            SeleccionarPersonaCommand = new DelegateCommand<object>(OnSeleccionarPersonaCommandExecute);

        }

        public ICommand SeleccionarPersonaCommand { get; private set; }
        public ICommand PaginaAnteriorCommand { get; private set; }
        public ICommand PaginaSiguienteCommand { get; private set; }

        public PaginatedCollectionView Personas { get; private set; }

        public object ElementosPorPagina
        {
            get { return Personas.ItemsPerPage; }
            set
            {
                if (value.GetType() == typeof(int)) // 30, 50, ...
                {
                    Personas.ItemsPerPage = (int)value;
                    _Settings.ListadoDePersonasItemsPerPage = (int)value;
                }
                else if (value.GetType() == typeof(string)) // "Todas"
                {
                    Personas.ItemsPerPage = int.MaxValue;
                    _Settings.ListadoDePersonasItemsPerPage = int.MaxValue;
                }

                OnPropertyChanged();
            }
        }

        private void OnSeleccionarPersonaCommandExecute(object id)
        {
            _EventAggregator.GetEvent<PersonaSeleccionadaEvent>().Publish((int)id);
        }

        private void OnNuevaPersonaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            _PersonaRepository.Session.Evict(persona);

            var lookupItem = new LookupItem
            {
                Id = persona.Id,
                DisplayMember1 = LookupItem.ShortenStringForDisplay(persona.Nombre, 25),
                DisplayMember2 = persona.Nif,
                IconSource = persona.AvatarPath
            };

            Personas.AddItemAt(0, lookupItem);
        }

        private void OnPersonaEliminadaEvent(int id)
        {
            Personas.Remove(_Personas.Find(x => x.Id == id));
        }

        private void OnPersonaActualizadaEvent(int id)
        {
            var persona = _PersonaRepository.GetById(id);
            _PersonaRepository.Session.Evict(persona);

            var personaDesactualizada = _Personas.Where(x => x.Id == id).FirstOrDefault();

            if (personaDesactualizada != null)
            {
                personaDesactualizada.DisplayMember1 = persona.Nombre;
                personaDesactualizada.DisplayMember2 = persona.Nif;
                personaDesactualizada.IconSource = persona.AvatarPath;
            }

            Personas.Refresh();
        }
        
        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }

            set
            {
                SetProperty(ref _IsActive, value);
                if (_IsActive)
                {
                    // NOTA: Se está usando un 0 (cero) para indicar que ya no hay
                    // persona seleccionada. Se ha convenido así.
                    _EventAggregator.GetEvent<PersonaSeleccionadaChangedEvent>().Publish(0);
                }
            }
        }
    }
}



//var personaActualizada = _PersonaRepository.GetById(id);
//_PersonaRepository.Session.Evict(personaActualizada);

//if (_Personas.Any(a => a.Id == id))
//{
//    var persona = _Personas.Where(a => a.Id == id).Single();
//    var index = _Personas.IndexOf(persona);
//    _Personas[index].DisplayMember1 = personaActualizada.Nombre;
//    _Personas[index].DisplayMember2 = personaActualizada.Nif;
//    _Personas[index].IconSource = personaActualizada.AvatarPath;
//}

