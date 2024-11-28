using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;

public class RwUInt64VM : RwVMBase<ulong>
{
    public RwUInt64VM(IS7DataBlockService s7ServerService) : base(s7ServerService)
    {
    }


    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteULong(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.ToBeWritten);
    }

    protected override ulong CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadULong(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
