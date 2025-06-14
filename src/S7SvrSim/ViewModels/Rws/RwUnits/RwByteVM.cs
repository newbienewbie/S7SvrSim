using MediatR;
using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;


/// <summary>
/// 读写字节
/// </summary>
public class RwByteVM : RwVMBase<byte>
{
    public RwByteVM(IS7DataBlockService s7ServerService, IMediator mediator) : base(s7ServerService, mediator)
    {
    }


    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteByte(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.ToBeWritten);
    }

    protected override byte CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadByte(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
