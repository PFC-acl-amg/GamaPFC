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
    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IRevertibleChangeTracking
    {
        private Dictionary<string, object> _originalValues;
        private List<IRevertibleChangeTracking> _trackingObjects;

        public ModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            this.Model = model;
            _originalValues = new Dictionary<string, object>();
            _trackingObjects = new List<IRevertibleChangeTracking>();
        }

        public T Model { get; private set; }

        public virtual bool IsChanged => _originalValues.Count > 0 || _trackingObjects.Any(to => to.IsChanged);

        public bool IsValid => !HasErrors;

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
                ValidateProperty(propertyName, newValue);
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName + "IsChanged");
            }
        }

        private void ValidateProperty(string propertyName, object newValue)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            Validator.TryValidateProperty(newValue, context, results);
            if (results.Any())
            {
                Errors[propertyName] = results.Select(r => r.ErrorMessage).Distinct().ToList();
                OnErrorsChanged(propertyName);
                OnPropertyChanged(nameof(IsValid));
            }
            else if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                OnPropertyChanged(nameof(IsValid));
            }

        }

        //protected void SetComplexValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        //    where TValue : ModelWrapper<TClass>
        //{
        //    var propertyInfo = this.Model.GetType().GetProperty(propertyName);

        //}

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
            wrapperCollection.CollectionChanged += (s, e) => {
                modelCollection.Clear();
                foreach (var wrapper in wrapperCollection)
                {
                    modelCollection.Add(wrapper.Model);
                }
            };
            RegisterTrackingObject(wrapperCollection);
        }

        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        private void RegisterTrackingObject<TTrackingObject>(TTrackingObject trackingObject)
            where TTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
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
        }
    }
}
