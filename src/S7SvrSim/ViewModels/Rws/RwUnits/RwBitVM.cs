using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using S7Svr.Simulator.ViewModels;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace S7SvrSim.ViewModels.Rw;

public class RwBitVM : RwVMBase<bool>, IValidatableViewModel
{


    public RwBitVM(IS7DataBlockService s7ServerService) :base(s7ServerService)
    {
        var watchTargetBitPos = this.WhenAnyValue(x => x.TargetBitPos)
            .Select(p => p >= 0 && p <= 7);

        this.CmdRead = ReactiveCommand.Create(this.CmdRead_Impl, watchTargetBitPos);
        this.CmdWrite = ReactiveCommand.Create(this.CmdWrite_Impl, watchTargetBitPos);
    }


    [Reactive]
    public byte TargetBitPos { get; set; } = 0;

   
    public IValidationContext ValidationContext => new ValidationContext();

    protected override void CmdWrite_Impl()
    {
         this._s7ServerService.WriteBit(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.TargetBitPos, this.ToBeWritten);
    }

    protected override bool CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadBit(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.TargetBitPos);
        this.ValueRead = val;
        return val;
    }

}
