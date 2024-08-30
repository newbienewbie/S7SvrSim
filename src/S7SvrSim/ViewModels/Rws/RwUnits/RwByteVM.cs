using S7Svr.Simulator.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S7SvrSim.ViewModels.Rw;


/// <summary>
/// 读写字节
/// </summary>
public class RwByteVM : RwVMBase<byte>
{
    public RwByteVM(IS7DataBlockService s7ServerService):base(s7ServerService)
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
