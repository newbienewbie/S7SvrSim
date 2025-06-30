using CommunityToolkit.Mvvm.ComponentModel;

namespace S7SvrSim.S7Signal
{
    public partial class UpdateAddressOptions : ObservableObject
    {
        [ObservableProperty]
        private bool forbidIndexHasOddNumber = true;
        [ObservableProperty]
        private bool allowBoolIndexHasOddNumber = true;
        [ObservableProperty]
        private bool allowByteIndexHAsOddNumber = true;
    }
}
