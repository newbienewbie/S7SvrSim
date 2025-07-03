using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using S7SvrSim.S7Signal;
using S7SvrSim.Services.Settings;
using System;

namespace S7SvrSim.ViewModels.Siganls
{
    public class UpdateAddressOptionsVM : ReactiveObject
    {
        [Reactive]
        public bool UpdateAddressByDbIndex { get; set; }
        [Reactive]
        public bool ForbidIndexHasOddNumber { get; set; }
        [Reactive]
        public bool AllowBoolIndexHasOddNumber { get; set; }
        [Reactive]
        public bool AllowByteIndexHAsOddNumber { get; set; }
        [Reactive]
        public bool StringUseTenCeiling { get; set; }
        public UpdateAddressOptionsVM(ISetting<UpdateAddressOptions> setting)
        {
            setting.Value.Subscribe(options =>
            {
                UpdateAddressByDbIndex = options.UpdateAddressByDbIndex;
                ForbidIndexHasOddNumber = options.ForbidIndexHasOddNumber;
                AllowBoolIndexHasOddNumber = options.AllowBoolIndexHasOddNumber;
                AllowByteIndexHAsOddNumber = options.AllowByteIndexHAsOddNumber;
                StringUseTenCeiling = options.StringUseTenCeiling;
            });

            this.WhenAnyPropertyChanged()
                .Subscribe(vm =>
                {
                    setting.Write(new UpdateAddressOptions(UpdateAddressByDbIndex, ForbidIndexHasOddNumber, AllowBoolIndexHasOddNumber, AllowByteIndexHAsOddNumber, StringUseTenCeiling));
                });
        }
    }
}
