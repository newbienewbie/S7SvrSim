using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace S7SvrSim.ViewModels
{
    [DataContract]
    public abstract class ViewModelBase : ObservableObject, IReactiveObject
    {
        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            OnPropertyChanged(args);
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            OnPropertyChanging(args);
        }
    }
}
