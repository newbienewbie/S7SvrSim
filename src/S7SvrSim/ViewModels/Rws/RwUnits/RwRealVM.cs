using MediatR;
using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;

public class RwRealVM : RwVMBase<float>
{
    public RwRealVM(IS7DataBlockService s7ServerService, IMediator mediator) : base(s7ServerService, mediator)
    {
    }


    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteReal(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.ToBeWritten);
    }

    protected override float CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadReal(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
