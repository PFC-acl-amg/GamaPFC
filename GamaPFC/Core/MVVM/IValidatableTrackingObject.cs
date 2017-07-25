using System.ComponentModel;

namespace Core
{
    public interface IValidatableTrackingObject :
            IRevertibleChangeTracking, 
            INotifyPropertyChanged
        {
            bool IsValid { get; }
        }
    }
