using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;

public class RwShortVM : RwVMBase<short>
{
    public RwShortVM(IS7DataBlockService s7ServerService) : base(s7ServerService)
    {
    }


    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteShort(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.ToBeWritten);
    }

    protected override short CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadShort(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
