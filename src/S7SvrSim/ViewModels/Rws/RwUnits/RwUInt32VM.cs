using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;

public class RwUInt32VM : RwVMBase<uint>
{
    public RwUInt32VM(IS7DataBlockService s7ServerService) : base(s7ServerService)
    {
    }


    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteUInt32(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.ToBeWritten);
    }

    protected override uint CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadUInt32(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
