using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;

public class RwStringVM : RwVMBase<string>
{
    public RwStringVM(IS7DataBlockService s7ServerService) : base(s7ServerService)
    {
    }

    [Reactive]
    public int StringArrayMaxLength { get; set; } = 256;

    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteString(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.StringArrayMaxLength, this.ToBeWritten);
    }

    protected override string CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadString(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
