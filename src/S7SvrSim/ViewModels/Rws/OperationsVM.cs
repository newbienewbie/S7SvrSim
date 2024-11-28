using Microsoft.Win32;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services;
using S7SvrSim.UserControls;
using S7SvrSim.ViewModels.Rw;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace S7Server.Simulator.ViewModels
{
    /// <summary>
    /// 读写操作
    /// </summary>
    public class OperationVM : ReactiveObject
    {
        private IS7DataBlockService _s7ServerService;
        private readonly PyScriptRunner _scriptRunner;

        public RwTargetVM RwTargetVM { get; }
        public ObservableCollection<KeyValuePair<string, ReactiveObject>> OpViewModels { get; }

        public OperationVM(IS7DataBlockService s7ServerService, PyScriptRunner scriptRunner)
        {
            this._s7ServerService = s7ServerService;
            this._scriptRunner = scriptRunner;

            RwTargetVM = Locator.Current.GetRequiredService<RwTargetVM>();

 
        }


        public ReactiveObject RwBitVM = Locator.Current.GetRequiredService<RwBitVM>();
        public ReactiveObject RwByteVM = Locator.Current.GetRequiredService<RwByteVM>();
        public ReactiveObject RwShortVM = Locator.Current.GetRequiredService<RwShortVM>();
        public ReactiveObject RwUInt32VM = Locator.Current.GetRequiredService<RwUInt32VM>();
        public ReactiveObject RwUInt64VM = Locator.Current.GetRequiredService<RwUInt64VM>();
        public ReactiveObject RwRealVM = Locator.Current.GetRequiredService<RwRealVM>();
        public ReactiveObject RwStringVM = Locator.Current.GetRequiredService<RwStringVM>();
        



    }
}
