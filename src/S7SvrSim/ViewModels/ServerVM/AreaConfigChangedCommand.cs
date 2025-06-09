using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services;

namespace S7SvrSim.ViewModels.ServerVM
{
    internal enum ChangedType
    {
        Add,
        Remove,
    }

    internal class AreaConfigChangedCommand : ICommand
    {
        private readonly ConfigSnap7ServerVM _vm;
        private readonly ChangedType changedType;
        private readonly AreaConfigVM areaConfig;

        public AreaConfigChangedCommand(ConfigSnap7ServerVM vm, ChangedType changedType, AreaConfigVM areaConfig)
        {
            _vm = vm;
            this.changedType = changedType;
            this.areaConfig = areaConfig;
        }

        public void Execute()
        {
            if (changedType == ChangedType.Add)
            {
                _vm.AreaConfigs.Add(areaConfig);
            }
            else
            {
                _vm.AreaConfigs.Remove(areaConfig);
            }
        }

        public void Undo()
        {
            if (changedType == ChangedType.Add)
            {
                _vm.AreaConfigs.Remove(areaConfig);
            }
            else
            {
                _vm.AreaConfigs.Add(areaConfig);
            }
        }
    }
}
