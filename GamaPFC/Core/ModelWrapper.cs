using Core.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IValidatableTrackingObject, IValidatableObject
    {
        private Dictionary<string, object> _originalValues;
        private List<IValidatableTrackingObject> _trackingObjects;
        private bool _IsInEditionMode;

        public ModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            this.Model = model;
            _originalValues = new Dictionary<string, object>();
            _trackingObjects = new List<IValidatableTrackingObject>();
            InitializeUniqueProperties(model);
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
            
            Validate();
        }

        protected virtual void InitializeUniqueProperties(T model) { }

        protected virtual void InitializeCollectionProperties(T model) { }
  
        protected virtual void InitializeComplexProperties(T model) { }

        public T Model { get; private set; }

        public virtual bool IsChanged =>_originalValues.Count > 0 || _trackingObjects.Any(to => to.IsChanged);

        public bool IsValid => !HasErrors;// && _trackingObjects.All(t => t.IsValid);

        public bool IsInEditionMode
        {
            get { return _IsInEditionMode; }
            set { SetProperty(ref _IsInEditionMode, value); }
        }

        public virtual void AcceptChanges()
        {
            _originalValues.Clear();

            foreach (var trackedObject in _trackingObjects)
            {
                trackedObject.AcceptChanges();
            }

            OnPropertyChanged(string.Empty);
        }

        public virtual void RejectChanges()
        {
            foreach (var propertyName in _originalValues.Keys)
            {
                var propertyInfo = this.Model.GetType().GetProperty(propertyName);
                propertyInfo.SetValue(this.Model, _originalValues[propertyName]);
            }
            _originalValues.Clear();

            foreach (var trackedObject in _trackingObjects)
            {
                trackedObject.RejectChanges();
            }

            Validate();
            OnPropertyChanged(string.Empty);
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = this.Model.GetType().GetProperty(propertyName);
            return (TValue)propertyInfo.GetValue(this.Model);
        }

        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName)
                ? (TValue)_originalValues[propertyName]
                : GetValue<TValue>(propertyName);
        }

        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = this.Model.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);

            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName);
                propertyInfo.SetValue(this.Model, newValue);
                Validate();
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName + "IsChanged");
            }
        }

        protected void SetValue(object newValue, [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = this.Model.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);

            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName);
                propertyInfo.SetValue(this.Model, newValue);
                Validate();
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName + "IsChanged");
            }
        }

        protected void Validate()
        {
            ClearErrors();

            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, true);

            if (results.Any())
            {
                var propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();

                foreach (var propertyName in propertyNames)
                {    
                    Errors[propertyName] = results
                        .Where(r => r.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage)
                        .Distinct()
                        .ToList();
                    OnErrorsChanged(propertyName);
                }
            }

            OnPropertyChanged(nameof(IsValid));
        }

        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged("IsChanged");
            }
            else if (Equals(_originalValues[propertyName], newValue))
            {
                _originalValues.Remove(propertyName);
                OnPropertyChanged("IsChanged");
            }
        }

        protected void RegisterCollection<TWrapper, TModel>(
            ChangeTrackingCollection<TWrapper> wrapperCollection,
            IList<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (s, e) => 
            {
                modelCollection.Clear();
                foreach (var wrapper in wrapperCollection)
                {
                    modelCollection.Add(wrapper.Model);
                }
                Validate();
            };
            RegisterTrackingObject(wrapperCollection);
        }

        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        private void RegisterTrackingObject(IValidatableTrackingObject trackingObject)
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
            else if (e.PropertyName == nameof(IsValid)) 
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
